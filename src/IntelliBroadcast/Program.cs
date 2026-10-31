using IntelliBroadcast.Data;
using IntelliBroadcast.Identity;
using IntelliBroadcast.Models.Entities;
using IntelliBroadcast.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required. See appsettings.Example.json and README.md.");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddSingleton<IProjectIdentity, ProjectIdentity>();
builder.Services.AddScoped<IPasswordHasher<UserExtended>, PasswordHasher<UserExtended>>();
builder.Services.AddScoped<IUserAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();
builder.Services.AddTransient<ProjectIdentityHeaderHandler>();
builder.Services.AddHttpClient<IProductApiClient, ProductApiClient>((services, client) =>
{
    var url = services.GetRequiredService<IConfiguration>()["ProductApi:BaseUrl"];
    if (Uri.TryCreate(url, UriKind.Absolute, out var uri)) client.BaseAddress = uri;
}).AddHttpMessageHandler<ProjectIdentityHeaderHandler>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "__Host-IntelliBroadcast.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
    options.Cookie.Name = "__Host-IntelliBroadcast.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.Path = "/";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.Events.OnValidatePrincipal = context =>
    {
        var claimTenant = context.Principal?.FindFirst(AppClaimTypes.TenantId)?.Value;
        var sessionTenant = context.HttpContext.Session.GetString(SessionKeys.TenantId);
        if (string.IsNullOrEmpty(claimTenant) || !string.Equals(claimTenant, sessionTenant, StringComparison.Ordinal))
            context.RejectPrincipal();
        return Task.CompletedTask;
    };
});
builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews(options => options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute()));
builder.Services.AddHealthChecks().AddDbContextCheck<ApplicationDbContext>();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllerRoute("default", "{controller=Announcements}/{action=Index}/{id?}");
app.Run();

public partial class Program;
