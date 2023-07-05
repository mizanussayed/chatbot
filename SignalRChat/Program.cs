global using SignarlRChat.Entity;
using SignarlRChat.Data;
using SignarlRChat.DTOs;
using SignarlRChat.Hubs;
using SignarlRChat.Interface;
using SignarlRChat.Permission;
using SignarlRChat.Service;
using FastReport.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;
string defaultConnection = configuration.GetConnectionString("DefaultConnection");
//builder.WebHost.UserUrls("http://192.168.20.214:4040");
// Add services to the container.
var connectionString = configuration.GetConnectionString("DefaultConnection");
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
services.AddDatabaseDeveloperPageExceptionFilter();


services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = configuration["Authentication:Google:ClientId"];
        options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
    })
    .AddTwitter(options =>
    {
        options.ConsumerKey = configuration["Authentication:Twitter:ClientId"];
        options.ConsumerSecret = configuration["Authentication:Twitter:ClientSecret"];
        options.RetrieveUserDetails = true;
    }).AddFacebook(options =>
    {
        options.ClientId = configuration["Authentication:Facebook:ClientId"];
        options.ClientSecret = configuration["Authentication:Facebook:ClientSecret"];
    });

services.AddControllersWithViews();

services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders()
    .AddSignInManager<CustomSignInManager<ApplicationUser>>();



services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();


// When manully create policy for every permission.
//services.AddAuthorization(options =>
//{
//    options.AddPolicy(Permissions.ApplicationUsers.Delete, builder =>
//    {
//        builder.AddRequirements(new PermissionRequirement(Permissions.ApplicationUsers.Delete));
//    });
//    options.AddPolicy(Permissions.IdentityRoles.Delete, builder =>
//    {
//        builder.AddRequirements(new PermissionRequirement(Permissions.IdentityRoles.Delete));
//    });
//    // These goes on for every permission
//});
services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));
services.AddTransient<IToastNotification, ToastNotification>();
services.AddTransient<IMailService, MailService>();
services.AddSignalR();
//services.AddSession(options => {
//    options.IdleTimeout = TimeSpan.FromMinutes(30);
//});
services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

// Here seeding data of Superadmin user, role 
using (var scope = app.Services.CreateScope())
{

    var seedingSevice = scope.ServiceProvider;

    // Ensured Create database when run application
    var dbcontext = seedingSevice.GetRequiredService<ApplicationDbContext>();
    dbcontext.Database.EnsureCreated();

    // Here using UserManager<ApplicaitonUser> as service
    var userManager = seedingSevice.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = seedingSevice.GetRequiredService<RoleManager<IdentityRole>>();
    await SignarlRChat.Seeds.DefaultRoles.SeedAsync(userManager, roleManager);
    await SignarlRChat.Seeds.DefaultUsers.SeedSuperAdminAsync(userManager, roleManager);
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
FastReport.Utils.RegisteredObjects.AddConnection(typeof(MsSqlDataConnection));
//app.UseFastReport();  // It may be comes from FastReport.Web

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
//app.UseSession();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{area=Chat}/{controller=Chat}/{action=Index}/{id?}");
});

app.MapHub<ChatHub>("/Chat");
app.Run();
