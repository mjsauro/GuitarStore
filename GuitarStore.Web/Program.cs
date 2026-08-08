using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using GuitarStore.Web.Data;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    // Every POST gets antiforgery validation by default. The MVC 5 version relied on
    // remembering [ValidateAntiForgeryToken] per action, and several actions didn't.
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

// DynamoDB. Locally this points at DynamoDB Local; deployed, the AWS SDK picks up
// credentials and region from the App Runner instance role.
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

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Create tables and seed the catalog on startup so a fresh clone just works.
using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<DynamoDbInitializer>().EnsureTablesAsync();
    await SeedData.SeedProductsAsync(scope.ServiceProvider.GetRequiredService<IProductRepository>());
}

app.Run();
