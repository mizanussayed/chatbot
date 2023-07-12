global using SignalRChat.Entity;
using SignalRChat.Data;
using SignalRChat.DTOs;
using SignalRChat.Hubs;
using SignalRChat.Interface;
using SignalRChat.Permission;
using SignalRChat.Service;
using FastReport.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Http.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

//builder.WebHost.UseUrls("http://192.168.20.214:4040");
// Add services to the container.
var connectionString = configuration.GetConnectionString("DefaultConnection");
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
services.AddDatabaseDeveloperPageExceptionFilter();
services.Configure<ForwardedHeadersOptions>(option => {
option.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});


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
app.UseForwardedHeaders();
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
    await SignalRChat.Seeds.DefaultRoles.SeedAsync(userManager, roleManager);
    await SignalRChat.Seeds.DefaultUsers.SeedSuperAdminAsync(userManager, roleManager);
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
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
    
    endpoints.MapGet("/log", async context =>
    {
        context.Response.ContentType = "text/plain";

        // Host info
        var name = Dns.GetHostName(); // get container id
        var ip = Dns.GetHostEntry(name).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
        Console.WriteLine($"Host Name: { Environment.MachineName} \t {name}\t {ip}");
        await context.Response.WriteAsync($"Host Name: {Environment.MachineName}{Environment.NewLine}");
        await context.Response.WriteAsync(Environment.NewLine);

        // Request method, scheme, and path
        await context.Response.WriteAsync($"Request Method: {context.Request.Method}{Environment.NewLine}");
        await context.Response.WriteAsync($"Request Scheme: {context.Request.Scheme}{Environment.NewLine}");
        await context.Response.WriteAsync($"Request URL: {context.Request.GetDisplayUrl()}{Environment.NewLine}");
        await context.Response.WriteAsync($"Request Path: {context.Request.Path}{Environment.NewLine}");

        // Headers
        await context.Response.WriteAsync($"Request Headers:{Environment.NewLine}");
        foreach (var (key, value) in context.Request.Headers)
        {
            await context.Response.WriteAsync($"\t {key}: {value}{Environment.NewLine}");
        }
        await context.Response.WriteAsync(Environment.NewLine);

        // Connection: RemoteIp
        await context.Response.WriteAsync($"Request Remote IP: {context.Connection.RemoteIpAddress}");
    });
});

app.MapHub<ChatHub>("/Chat");
app.Run();
