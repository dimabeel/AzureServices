using AzureServices.JsonProperties;
using AzureServices.Services.CarService;
using AzureServices.Services.KeyVault.Keys;
using AzureServices.Services.KeyVault.Secrets;
using AzureServices.Services.Storage.Blobs;
using AzureServices.Services.Storage.Queues;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDistributedMemoryCache();
builder.Services.Configure<CookiePolicyOptions>(options =>
    {
        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
        // Handling SameSite cookie according to https://docs.microsoft.com/en-us/aspnet/core/security/samesite?view=aspnetcore-3.1
        options.HandleSameSiteCookieCompatibility();
    });
builder.Services.AddOptions();

CarServiceProperties carServiceProperties =  builder.Configuration
    .GetSection(nameof(CarServiceProperties))
    .Get<CarServiceProperties>();
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration)
    .EnableTokenAcquisitionToCallDownstreamApi(new string[] { carServiceProperties.Scope })
    .AddInMemoryTokenCaches();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddRazorPages().AddMicrosoftIdentityUI();

builder.Services.AddScoped<IKeyService, KeyService>();
builder.Services.AddScoped<ISecretService, SecretService>();
builder.Services.AddScoped<IQueueService, QueueService>();
builder.Services.AddScoped<IBlobService, BlobService>();

builder.Services.AddCarService(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCookiePolicy();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();