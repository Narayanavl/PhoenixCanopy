using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using StowellCoAPI.Models;
using StowellCoAPI.Services;
using Syncfusion.Licensing;
//SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjGyl/VkV+XU9AclRDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS3hTckRqWHtcdnVTT2BcU091XA==");
//SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjGyl/VkV+XU9AclRDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS3hTcEdgWXhedHZTQWdaWE91XA==");
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JHaF1cXmhPYVFxWmFZfVhgdVRMZFpbRH9PMyBoS35RcEVmW3dfcXBTR2hdV0BxVEFe");

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(
    Path.Combine(AppContext.BaseDirectory, "appsettings.json"),
    optional: false,
    reloadOnChange: true
);

//builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddSingleton<GraphServiceClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var clientId = config["MicrosoftGraph:ClientId"];
    var tenantId = config["MicrosoftGraph:TenantId"];
    var clientSecret = config["MicrosoftGraph:ClientSecret"];
    var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
    var scopes = new[] { "https://graph.microsoft.com/.default" };
    return new GraphServiceClient(clientSecretCredential, scopes);
});
builder.Services.AddScoped<GraphCalendarService>();
builder.Services.AddScoped<UserInfoService>();
// HttpClient to call PnPWorker Web API
//builder.Services.AddHttpClient("PnPWorker", client =>
//{
//    client.BaseAddress = new Uri(builder.Configuration["PnPWorkerBaseUrl"]
//        ?? "https://localhost:5005/");
//});

// FolderProxyService wraps the HttpClient for convenient calls
//builder.Services.AddScoped<FolderProxyService>();

//// Add authentication & authorization
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

//builder.Services.AddAuthorization();

builder.Services.AddControllers();
// CORS (allow Blazor UI domain)
//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(policy =>
//    {
//        policy.WithOrigins("https://localhost:5555")
//              .AllowAnyHeader()
//              .AllowAnyMethod();
//    });
//});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// ---Database Context-- -
//builder.Services.AddDbContext<SageSbqContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("SageSBQConnection")));
builder.Services.AddDbContext<SageSbqContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SageSBQConnection")),
    ServiceLifetime.Scoped);
// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseDeveloperExceptionPage(); // ?? add this line
    app.UseSwagger();
    app.UseSwaggerUI();
//}
app.UseRouting();
// Middleware
app.UseHttpsRedirection();
app.UseCors();               // allow cross-domain requests
//app.UseAuthentication();
//app.UseAuthorization();

app.MapControllers();

// Default page (home page) routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
