using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.AspNetCoreServer.Hosting;
using GuitarStore.Web.Data;
using GuitarStore.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    // Every POST gets antiforgery validation by default. The MVC 5 version relied on
    // remembering [ValidateAntiForgeryToken] per action, and several actions didn't.
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

// Cookie auth holds the session. In Development the cookie is issued by DevAuthController;
// deployed, Cognito issues it through OIDC.
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = builder.Environment.IsDevelopment() ? "/DevAuth" : "/Account/SignIn";
        options.AccessDeniedPath = "/Account/Denied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
    });

builder.Services.AddAuthorization();

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
                AuthenticationRegion = builder.Configuration["AWS:Region"] ?? "us-east-1"
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

// Running on Lambda behind a Function URL. This is a no-op when running locally with
// dotnet run, so the same build works both places.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

// The Function URL terminates TLS and forwards plain HTTP, so the original scheme arrives
// in X-Forwarded-Proto. Without this, UseHttpsRedirection would redirect forever.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor;
    // The proxy is AWS-managed and not on a known address; the hop is inside the platform.
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

app.UseForwardedHeaders();

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

// The local sign-in stub is only routable in Development; DevAuthController also refuses
// to act outside it.
if (!app.Environment.IsDevelopment())
{
    app.MapControllerRoute(name: "blockDevAuth", pattern: "DevAuth/{*rest}", defaults: new { controller = "Home", action = "Error" });
}

// Create tables and seed the catalog on startup so a fresh clone just works. Off in
// production: the deployed function's role is scoped to reading and writing items, not
// creating tables, and this would otherwise run on every cold start.
if (builder.Configuration.GetValue("AWS:AutoProvision", app.Environment.IsDevelopment()))
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<DynamoDbInitializer>().EnsureTablesAsync();
    await SeedData.SeedProductsAsync(scope.ServiceProvider.GetRequiredService<IProductRepository>());
}

app.Run();
