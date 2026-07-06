using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Radzen;
using Serilog;
using StowellCoApp.Common;
using StowellCoApp.Services;
using Syncfusion.Blazor;


var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);
//  Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Optional: File logging via Serilog


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("Logs/stowellco-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();
builder.Configuration.AddJsonFile(
    Path.Combine(AppContext.BaseDirectory, "appsettings.json"),
    optional: false,
    reloadOnChange: true
);
//builder.Services.AddAuthorization();
// Add services
builder.Services.AddRazorPages();
//Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjGyl/VkV+XU9AclRDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS3hTckRqWHtcdnVTT2BcU091XA==");
//Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjGyl/VkV+XU9AclRDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS3hTcEdgWXhedHZTQWdaWE91XA==");
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JHaF1cXmhPYVFxWmFZfVhgdVRMZFpbRH9PMyBoS35RcEVmW3dfcXBTR2hdV0BxVEFe");
builder.Services.AddSyncfusionBlazor();
builder.Services.AddServerSideBlazor()
    .AddHubOptions(options =>
    {
        options.ClientTimeoutInterval = TimeSpan.FromMinutes(5);
        options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        options.HandshakeTimeout = TimeSpan.FromSeconds(30);
        //options.ClientTimeoutInterval = TimeSpan.FromMinutes(5);
        //options.KeepAliveInterval = TimeSpan.FromSeconds(30);
        options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB
    })
    .AddMicrosoftIdentityConsentHandler();

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi(
        builder.Configuration["MicrosoftGraph:Scopes"].Split(' '))
    .AddMicrosoftGraph()
    .AddInMemoryTokenCaches();
// Authorization
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});
builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();
// Required for <CascadingAuthenticationState>
builder.Services.AddHttpContextAccessor();

// Add Radzen services
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();
//builder.Services.AddScoped<ApiAuthorizationMessageHandler>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

var apiBaseUrl = builder.Configuration["APIBaseUrl"] ?? "/";

builder.Services.AddHttpClient<IAccountingService, AccountingService>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["APIBaseUrl"]);
});
builder.Services.AddHttpClient<IAdminPanelService, AdminPanelService>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["APIBaseUrl"]);
});
builder.Services.AddHttpClient<ICostCodeService, CostCodeService>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["APIBaseUrl"]);
});
builder.Services.AddHttpClient<IEstimationService, EstimationService>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["APIBaseUrl"]);
});
builder.Services.AddHttpClient<CalendarApiClient>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["APIBaseUrl"]);
});
//builder.Services.AddHttpClient<IJobService, JobService>();
builder.Services.AddHttpClient<IJobService, JobService>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["APIBaseUrl"]);
});

//.AddHttpMessageHandler<ApiAuthorizationMessageHandler>();
builder.Services.AddHttpClient<IJobSummaryService, JobSummaryService>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["APIBaseUrl"]);
});
builder.Services.AddHttpClient<IProjectBudget, ProjectBudget>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["APIBaseUrl"]);
});
builder.Services.AddHttpClient<IProjectOverviewService, ProjectOverviewService>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["APIBaseUrl"]);
});
builder.Services.AddHttpClient<IStowellAdminService, StowellAdminService>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["APIBaseUrl"]);
});
builder.Services.AddHttpClient<IContactService, ContactService>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["APIBaseUrl"]);
});
builder.Services.AddHttpClient<IBidService, BidService>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["APIBaseUrl"]);
});
builder.Services.AddHttpClient<IReportingService, ReportingServices>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["APIBaseUrl"]);
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
});
var app = builder.Build();
// Middleware
app.UseHttpsRedirection();
// Middleware
//app.UseStaticFiles();
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    var request = context.Request;
    var url = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
    Console.WriteLine($"Incoming request path: {context.Request.Path}");
    logger.LogInformation("Incoming request: {Url}", url);

    await next.Invoke(); // call next middleware
});

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = new FileExtensionContentTypeProvider
    {
        Mappings = { [".rdp"] = "application/x-rdp" }
    }
});
//app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();
// Map endpoints
app.MapControllers();        // API controllers
app.MapBlazorHub();          // SignalR for Blazor
app.MapFallbackToPage("/_Host");  // Blazor fallback page

app.Run();
