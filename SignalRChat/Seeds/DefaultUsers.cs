using SignalRChat.Constant;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace SignalRChat.Seeds;

public static class DefaultUsers
{
    public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        var defaultUser = new ApplicationUser
        {
            UserName = "superadmin",
            Email = "superadmin@gmail.com",
            EmailConfirmed = true,
            FirstName = "Super Admin",
            LastName = "",
            PhoneNumber = "01555555555"
        };
        if ( userManager.Users.All(u => u.Id != defaultUser.Id))
        {
            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "123Pa$$word!");
                await userManager.AddToRoleAsync(defaultUser, Roles.SuperAdmin.ToString());
                await userManager.AddToRoleAsync(defaultUser, Roles.ChatUser.ToString());
            }
            await roleManager.SeedClaimsForSuperAdmin();
        }
    }

    private static async Task SeedClaimsForSuperAdmin(this RoleManager<IdentityRole> roleManager)
    {
        var adminRole = await roleManager.FindByNameAsync("SuperAdmin");
        await roleManager.AddPermissionClaim(adminRole, "ApplicationUsers");
        await roleManager.AddPermissionClaim(adminRole, "IdentityRoles");
        await roleManager.AddPermissionClaim(adminRole, "ManageUserRoles");
        await roleManager.AddPermissionClaim(adminRole, "ManageRoleClaims");
    }

    private static async Task AddPermissionClaim(this RoleManager<IdentityRole> roleManager, IdentityRole role, string module)
    {
        var allClaims = await roleManager.GetClaimsAsync(role);
        var allPermissions = Permissions.GeneratePermissionsForModule(module);
        foreach (var permission in allPermissions)
        {
            if (!allClaims.Any(c => c.Type == "Permission" && c.Value == permission))
            {
                await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            }
        }

    }
}
