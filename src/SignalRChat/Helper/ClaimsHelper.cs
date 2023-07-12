using SignalRChat.Areas.Admin.Models;
using SignalRChat.Permission;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using System.Security.Claims;

namespace SignalRChat.Helper
{
    public static class ClaimsHelper
    {
        // Get All Permission and assigned to RoleClaimsViewModel object and object added to List
        public static void GetPermissions(this List<RoleClaimsViewModel> allPermissions, Type policy, string roleId)
        {
            FieldInfo[] fields = policy.GetFields(BindingFlags.Static | BindingFlags.Public);
            RoleClaimsViewModel roleClaimsViewModel = new();
            roleClaimsViewModel.ClaimType = "Permissions";
            roleClaimsViewModel.ModuleName = policy.Name;
            foreach (FieldInfo field in fields)
            {
                if (field.Name == "View")
                {
                    roleClaimsViewModel.ViewValue = field.GetValue(null).ToString();
                }
                else if (field.Name == "Create")
                {
                    roleClaimsViewModel.CreateValue = field.GetValue(null).ToString();
                }
                else if (field.Name == "Edit")
                {
                    roleClaimsViewModel.EditValue = field.GetValue(null).ToString();
                }
                else if (field.Name == "Delete")
                {
                    roleClaimsViewModel.DeleteValue = field.GetValue(null).ToString();
                }
            }
            allPermissions.Add(roleClaimsViewModel);
        }


        // Assign permision in particular role that is not exist in that role.
        public static async Task<IdentityResult> AddPermissionClaim(this RoleManager<IdentityRole> roleManager, IdentityRole role, string permission)
        {
            var roleClaims = await roleManager.GetClaimsAsync(role);
            IdentityResult result = new();
            if (!roleClaims.Any(c => c.Type == "Permission" && c.Value == permission))
            {
                result = await roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, permission));
            }
            return result;
        }

    }
}
