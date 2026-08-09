using Amazon.AspNetCore.DataProtection.SSM;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.AspNetCoreServer.Hosting;
using Amazon.SimpleEmailV2;
using GuitarStore.Web.Data;
using GuitarStore.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// The store prices in US dollars, so don't let the host's default culture decide how money
// renders — on Lambda the invariant culture would print "¤599.00" instead of "$599.00".
var storeCulture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = storeCulture;
CultureInfo.DefaultThreadCurrentUICulture = storeCulture;

builder.Services.AddControllersWithViews(options =>
{
    // Every POST gets antiforgery validation by default. The MVC 5 version relied on
    // remembering [ValidateAntiForgeryToken] per action, and several actions didn't.
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

// Cookie auth holds the session. The cookie is issued by Cognito through OIDC when it's
// configured, and by the Development sign-in stub when it isn't — so a fresh clone runs
// with no AWS setup at all.
var cognito = builder.Configuration.GetSection(CognitoOptions.SectionName).Get<CognitoOptions>() ?? new CognitoOptions();
builder.Services.AddSingleton(cognito);

var authBuilder = builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        if (cognito.IsConfigured)
        {
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        }
    })
    .AddCookie(options =>
    {
        options.LoginPath = builder.Environment.IsDevelopment() && !cognito.IsConfigured ? "/DevAuth" : "/Account/SignIn";
        options.AccessDeniedPath = "/Account/Denied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
    });

if (cognito.IsConfigured)
{
    authBuilder.AddOpenIdConnect(options =>
    {
        options.Authority = cognito.Authority;
        options.ClientId = cognito.ClientId;
        options.ClientSecret = cognito.ClientSecret;
        options.ResponseType = "code";
        options.UsePkce = true;
        options.SaveTokens = false;
        options.GetClaimsFromUserInfoEndpoint = true;

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("email");
        options.Scope.Add("profile");

        options.TokenValidationParameters.NameClaimType = "email";
        options.TokenValidationParameters.RoleClaimType = "cognito:groups";

        // Behind API Gateway the app doesn't know its own public host, so the redirect
        // would otherwise be built from the internal one.
        options.Events.OnRedirectToIdentityProvider = context =>
        {
            var publicOrigin = builder.Configuration["App:PublicOrigin"];
            if (!string.IsNullOrEmpty(publicOrigin))
            {
                context.ProtocolMessage.RedirectUri = $"{publicOrigin.TrimEnd('/')}/signin-oidc";
            }

            return Task.CompletedTask;
        };
    });
}

builder.Services.AddAuthorization();

// Data Protection encrypts antiforgery tokens and the auth cookie. Lambda has no writable
// persistent storage, so the default provider falls back to an in-memory key ring that
// dies with each execution environment — a form rendered by one instance then posted to
// another fails to validate, and sessions drop at random. Persisting the keys to Parameter
// Store gives every instance the same key ring.
//
// Left unset locally, where the default file-based store works fine.
var keyPath = builder.Configuration["AWS:DataProtectionParameterPath"];
if (!string.IsNullOrWhiteSpace(keyPath))
{
    builder.Services
        .AddDataProtection()
        .SetApplicationName("GuitarStore")
        .PersistKeysToAWSSystemsManager(keyPath);
}

// DynamoDB. Locally this points at DynamoDB Local; deployed, the AWS SDK picks up
// credentials and region from the Lambda execution role.
builder.Services.AddSingleton<IAmazonDynamoDB>(_ =>
{
    var serviceUrl = builder.Configuration["AWS:DynamoDbServiceUrl"];
    if (!string.IsNullOrEmpty(serviceUrl))
    {
        return new AmazonDynamoDBClient(
            new Amazon.Runtime.BasicAWSCredentials("local", "local"),
            new AmazonDynamoDBConfig
            {
                ServiceURL = serviceUrl,
                AuthenticationRegion = builder.Configuration["AWS:Region"] ?? "us-east-1",
                // Fail fast against a local endpoint. The default retry policy spends
                // minutes backing off, which just looks like the app hanging.
                MaxErrorRetry = 1,
                Timeout = TimeSpan.FromSeconds(5)
            });
    }

    return new AmazonDynamoDBClient();
});

builder.Services.AddSingleton<IDynamoDBContext>(sp =>
    new DynamoDBContextBuilder()
        .WithDynamoDBClient(sp.GetRequiredService<IAmazonDynamoDB>)
        .Build());

builder.Services.AddSingleton<DynamoDbInitializer>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<CartService>();

// No real processor is wired up: checkout simulates authorization so the flow is demoable
// without a merchant account. Swap this registration to plug in a real provider.
builder.Services.AddSingleton<IPaymentService, SimulatedPaymentService>();

// Order receipts over SES. Without a configured sender the app logs instead of sending,
// so local runs need no email setup.
if (!string.IsNullOrWhiteSpace(builder.Configuration["Email:FromAddress"]))
{
    builder.Services.AddSingleton<IAmazonSimpleEmailServiceV2>(_ => new AmazonSimpleEmailServiceV2Client());
    builder.Services.AddScoped<IEmailSender, SesEmailSender>();
}
else
{
    builder.Services.AddScoped<IEmailSender, NullEmailSender>();
}

// Running on Lambda behind an API Gateway HTTP API. This is a no-op when running locally
// with dotnet run, so the same build works both places.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

// API Gateway terminates TLS and forwards plain HTTP, so the original scheme arrives in
// X-Forwarded-Proto. Without this, UseHttpsRedirection would redirect forever.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor;
    // The proxy is AWS-managed and not on a known address; the hop is inside the platform.
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

app.UseForwardedHeaders();

// Lambda can hand us a principal describing the caller of the function itself (the SigV4
// signer, or an API Gateway authorizer). It's flagged authenticated but carries no name,
// which breaks antiforgery token generation and would otherwise read as a signed-in user.
// Reset to anonymous so the auth cookie is the only thing that can sign a visitor in.
app.Use(async (context, next) =>
{
    context.User = new ClaimsPrincipal(new ClaimsIdentity());
    await next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Create tables and seed the catalog on startup so a fresh clone just works. Off in
// production: the deployed function's role is scoped to reading and writing items, not
// creating tables, and this would otherwise run on every cold start.
if (builder.Configuration.GetValue("AWS:AutoProvision", app.Environment.IsDevelopment()))
{
    using var scope = app.Services.CreateScope();
    try
    {
        await scope.ServiceProvider.GetRequiredService<DynamoDbInitializer>().EnsureTablesAsync();
        await SeedData.SeedProductsAsync(scope.ServiceProvider.GetRequiredService<IProductRepository>());
    }
    catch (Exception ex)
    {
        // Say what's wrong in a way that points at the fix. Otherwise this surfaces as an
        // opaque SDK timeout and looks like the app is hanging for no reason.
        var endpoint = builder.Configuration["AWS:DynamoDbServiceUrl"] ?? "AWS";
        app.Services.GetRequiredService<ILogger<Program>>().LogCritical(
            ex,
            "Could not reach DynamoDB at {Endpoint}. If you're running locally, start DynamoDB " +
            "Local first:\n\n    cd ~/.dynamodb-local && java -Djava.library.path=./DynamoDBLocal_lib " +
            "-jar DynamoDBLocal.jar -sharedDb -port 8000\n\nSee the README for first-time setup.",
            endpoint);

        return 1;
    }
}

app.Run();

return 0;
