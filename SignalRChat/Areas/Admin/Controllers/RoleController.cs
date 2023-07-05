using SignalRChat.Areas.Admin.Models;
using SignalRChat.Constant;
using SignalRChat.Helper;
using SignalRChat.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SignalRChat.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class RoleController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IToastNotification _toast;

    public RoleController(RoleManager<IdentityRole> roleManager, IToastNotification toast)
    {
        _roleManager = roleManager;
        _toast = toast;
    }

    [Authorize(Permissions.IdentityRoles.View)]
    public async Task<IActionResult> Index() => View(await _roleManager.Roles.ToListAsync());

    [HttpPost]
    [Authorize(Permissions.IdentityRoles.Create)]
    public async Task<IActionResult> CreateOrEdit(string roleId, string roleName)
    {
        try
        {
            if (string.IsNullOrEmpty(roleId))
            {
                var role = new IdentityRole(roleName);
                await _roleManager.CreateAsync(role);
                await _toast.Success("Role Created Successfully");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    await _toast.Error("Role Not Found");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    role.Name = roleName;
                    await _roleManager.UpdateAsync(role);
                    await _toast.Success($"Role Update Successfully");
                    return RedirectToAction(nameof(Index));
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    [HttpPost]
    [Authorize(Permissions.IdentityRoles.Delete)]

    public async Task<IActionResult> Delete(string roleId)
    {
        try
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                await _toast.Error("Role Not Found");
                return RedirectToAction(nameof(Index));
            }
            await _roleManager.DeleteAsync(role);
            await _toast.Success($"Role ({role.Name}) Delete Successfully");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    #region Manage Role Permissions/Claims

    [HttpGet]
    [Authorize(Permissions.ManageRoleClaims.View)]
    public async Task<IActionResult> OnGetRoleClaims(string roleId)
    {
        try
        {
            PermissionViewModel model = new PermissionViewModel { RoleId = roleId };

            //Get All Nested Module/Class Type
            var moduleTypeList = Permissions.GetAllNestedModuleType();

            // Get All Permission belongs to a nested module/class
            foreach (var module in moduleTypeList)
            {
                model.RoleClaims.GetPermissions(module, roleId);
            }

            // Get the role by roleId
            var role = await _roleManager.FindByIdAsync(roleId);

            // Get All Claims/Permission of a particular role
            var roleClaims = await _roleManager.GetClaimsAsync(role);

            // Selected all the permission that are already assign in this role.
            foreach (var claim in roleClaims)
            {
                var viewResult = model.RoleClaims.Where(x => x.ViewValue == claim.Value).FirstOrDefault();
                if (viewResult is not null)
                {
                    viewResult.IsViewSelected = true;
                }

                var createResult = model.RoleClaims.Where(x => x.CreateValue == claim.Value).FirstOrDefault();
                if (createResult is not null)
                {
                    createResult.IsCreateSelected = true;
                }

                var editResult = model.RoleClaims.Where(x => x.EditValue == claim.Value).FirstOrDefault();
                if (editResult is not null)
                {
                    editResult.IsEditSelected = true;
                }

                var deleteResult = model.RoleClaims.Where(x => x.DeleteValue == claim.Value).FirstOrDefault();
                if (deleteResult is not null)
                {
                    deleteResult.IsDeleteSelected = true;
                }
            }
            return View("RoleClaims", model);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Permissions.ManageRoleClaims.Create)]
    public async Task<IActionResult> OnPostRoleClaims(PermissionViewModel model)
    {
        try
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                await _toast.Error("Role Not Found");
                return RedirectToAction(nameof(Index));
            }

            // Remove all claim of this role
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            foreach (var claim in roleClaims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            // Get all selected permission
            var selectedClaims = model.RoleClaims.Where(c => c.IsViewSelected || c.IsCreateSelected || c.IsEditSelected || c.IsDeleteSelected).ToList();
            IdentityResult result = new();
            foreach (var claim in selectedClaims)
            {
                if (claim.IsViewSelected)
                {
                    result = await _roleManager.AddPermissionClaim(role, claim.ViewValue);
                }

                if (claim.IsCreateSelected)
                {
                    result = await _roleManager.AddPermissionClaim(role, claim.CreateValue);
                }

                if (claim.IsEditSelected)
                {
                    await _roleManager.AddPermissionClaim(role, claim.EditValue);
                }

                if (claim.IsDeleteSelected)
                {
                    await _roleManager.AddPermissionClaim(role, claim.DeleteValue);
                }
            }
            if (result.Succeeded)
            {
                await _toast.Success($"Permissions Updated Successfully");
                return View("RoleClaims", model);
            }
            await _toast.Error("Role Not Found");
            return View("RoleClaims", model);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    #endregion
}
