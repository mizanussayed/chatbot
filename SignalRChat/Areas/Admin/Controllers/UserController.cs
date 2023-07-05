using SignarlRChat.Areas.Admin.Models;
using SignarlRChat.Extensions;
using SignarlRChat.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using System.Text.Encodings.Web;
using System.Text;
using SignarlRChat.DTOs;
using SignarlRChat.Constant;

namespace SignarlRChat.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IToastNotification _toast;
        private readonly IWebHostEnvironment _webHost;
        private readonly IMailService _mailService;
        private readonly ILogger<UserController> _logger;

        public UserController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IToastNotification toast,
            IWebHostEnvironment webHost,
            IMailService mailService,
            ILogger<UserController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _toast = toast;
            _webHost = webHost;
            _mailService = mailService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Permissions.ApplicationUsers.View)]
        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> LoadUser(string roleId = "")
        {
            try
            {
                IList<ApplicationUser> users = new List<ApplicationUser>();
                List<ApplicationUserViewModel> userList = new();
                if (string.IsNullOrEmpty(roleId))
                    users = await _userManager.Users.ToListAsync();
                else
                {
                    var role = await _roleManager.FindByIdAsync(roleId);
                    users = await _userManager.GetUsersInRoleAsync(role?.Name ?? "");
                }
                foreach (var user in users)
                {
                    userList.Add(new ApplicationUserViewModel
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        UserName = user.UserName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        ProfilePhotoUrl = user.ProfilePhotoUrl,
                        Roles = string.Join(", ", await _userManager.GetRolesAsync(user))
                    });
                }
                return PartialView("_ViewAll", userList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        [HttpGet]
        [Authorize(Permissions.ApplicationUsers.Create)]
        [Authorize(Permissions.ApplicationUsers.Edit)]
        public async Task<IActionResult> OnCreateOrEdit(string userId = "")
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return View("CreateOrEdit", new ApplicationUserViewModel());
                }
                else
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        await _toast.Error("User Not Found");
                        return RedirectToAction(nameof(Index));
                    }
                    ApplicationUserViewModel userViewModel = new ApplicationUserViewModel
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        UserName = user.UserName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        ProfilePhotoUrl = user.ProfilePhotoUrl,
                        Password = user.PasswordHash,
                    };
                    return View("CreateOrEdit", userViewModel);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Permissions.ApplicationUsers.Create)]
        public async Task<IActionResult> OnPostCreateOrEdit(ApplicationUserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(model.Id))
                    {
                        ApplicationUser user = new();
                        user.FirstName = model.FirstName.Trim();
                        user.LastName = model.LastName.Trim();
                        user.UserName = model.UserName.Trim();
                        user.Email = model.Email.Trim();
                        user.PhoneNumber = model.PhoneNumber.Trim();
                        user.ProfilePhotoUrl = await SaveImageAsync(model.ProfilePhoto, user.UserName, 200, 200);
                        await _userManager.AddPasswordAsync(user, model.Password);
                        var result = await _userManager.CreateAsync(user, model.Password);
                        if (result.Succeeded)
                        {
                            await _toast.Success("User Create Successfully");
                            return RedirectToAction(nameof(Index));
                        }
                        await _toast.Error($"{result.Errors}");
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var user = await _userManager.FindByIdAsync(model.Id);
                        if (user == null)
                        {
                            await _toast.Error("User Not Found");
                            return RedirectToAction(nameof(Index));
                        }
                        user.FirstName = model.FirstName.Trim();
                        user.LastName = model.LastName.Trim();
                        user.Email = model?.Email.Trim();
                        user.PhoneNumber = model?.PhoneNumber.Trim();
                        user.ProfilePhotoUrl = await SaveImageAsync(model.ProfilePhoto, model.UserName, 200, 200, model.ProfilePhotoUrl);
                        var result = await _userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            await _toast.Success($"{user.UserName} Update Successfully");
                            return RedirectToAction(nameof(Index));
                        }
                        await _toast.Error($"{result.Errors}");
                        return RedirectToAction(nameof(Index));
                    }

                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        [HttpPost]
        [Authorize(Permissions.ApplicationUsers.Delete)]
        public async Task<IActionResult> OnPostUserDelete(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    await _toast.Error("User Not Found");
                    return RedirectToAction(nameof(Index));
                }
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    await _toast.Success($"{user.UserName} Deleted Successfully");
                    return RedirectToAction(nameof(Index));
                }
                await _toast.Error($"{result.Errors}");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region User Role Manage

        [HttpGet]
        [Authorize(Permissions.ManageUserRoles.View)]
        public async Task<IActionResult> OnGetUserRoles(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    await _toast.Error("User Not Found");
                    return RedirectToAction(nameof(Index));
                }
                ApplicationUserViewModel userModel = new()
                {
                    Id = user.Id,
                    UserName = user.UserName
                };
                var roles = await _roleManager.Roles.ToListAsync();
                foreach (var role in roles)
                {
                    userModel.UserRoles.Add(new UserRolesViewModel
                    {
                        RoleId = role.Id,
                        RoleName = role.Name,
                        IsSelected = await _userManager.IsInRoleAsync(user, role.Name)
                    });
                }
                return View("UserRoles", userModel);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Permissions.ManageUserRoles.Create)]
        public async Task<IActionResult> OnPostUserRoles(ApplicationUserViewModel model, string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    await _toast.Error("User Not Found");
                    return RedirectToAction(nameof(Index));
                }
                var userRoles = await _userManager.GetRolesAsync(user);
                var result = await _userManager.RemoveFromRolesAsync(user, userRoles);
                if (!result.Succeeded)
                {
                    await _toast.Error($"{result.Errors}");
                    return RedirectToAction(nameof(Index));
                }
                result = await _userManager
                    .AddToRolesAsync(user, model.UserRoles
                        .Where(r => r.IsSelected).Select(r => r.RoleName)
                        .ToList());
                if (result.Succeeded)
                {
                    await _toast.Success("User Roles Update Successfully");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    await _toast.Error($"{result.Errors}");
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> IsEmailUsed(string email, string id)
        {
            var userById = await _userManager.FindByIdAsync(id);
            var userByEmail = await _userManager.FindByEmailAsync(email ?? "");
            if (userById is null && userByEmail is null)
                return Json(true);
            else if (userById?.Email == userByEmail.Email)
                return Json(true);
            else
                return Json($"Email {email} is already used");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> IsUsernameUsed(string userName, string id)
        {
            var userById = await _userManager.FindByIdAsync(id);
            var userByUsername = await _userManager.FindByNameAsync(userName);
            if (userById is null && userByUsername is null)
                return Json(true);
            else if (userById?.UserName == userByUsername.UserName)
                return Json(true);
            else
                return Json($"Username {userName} is already used");
        }

        private async Task<string> SaveImageAsync(IFormFile file, string username, int maxWidth, int maxHeight, string? fileUrl = null)
        {
            try
            {
                if (file is null && String.IsNullOrEmpty(fileUrl))                   // during create when no file uploaded
                {
                    return string.Empty;
                }
                else if (!string.IsNullOrEmpty(fileUrl) && file is null)             // during edit while file was created and no changes
                {
                    return fileUrl;
                }

                if (file is not null && file.Length > 0)                        // during create/changes file
                {
                    if (!string.IsNullOrEmpty(fileUrl))
                    {
                        string existPath = _webHost.WebRootPath + fileUrl.Replace('/', '\\');
                        if (System.IO.File.Exists(existPath))
                        {
                            System.IO.File.Delete(Path.Combine(existPath));
                        }
                    }
                    string uploadFolder = Path.Combine(_webHost.WebRootPath, "images", "users");
                    //string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                    string extension = Path.GetExtension(file.FileName);
                    string fileName = $"{username}_{DateTime.Now.ToString("yyyyMMdd")}{extension}";
                    //string fileName = $"{fileNameWithoutExtension}_{DateTime.Now.ToString("yyyyMMdd")}{extension}";
                    //string fileName = $"{fileNameWithoutExtension}_{Guid.NewGuid().ToString()}{extension}";
                    string path = Path.Combine(uploadFolder, fileName);

                    using (var stream = new MemoryStream())
                    {
                        // Load the image from the IFormFile into an Image object
                        await file.CopyToAsync(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        var image = Image.Load(stream);

                        // Resize the image to the desired dimensions
                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new Size(maxWidth, maxHeight),
                            //Mode = ResizeMode.Max  // it maintain orignal aspect ratio
                            Mode = ResizeMode.Stretch // it maintain orignal aspect ratio
                        }));

                        // Save the resized image to a file in the wwwroot folder
                        using FileStream fileStream = new(path, FileMode.Create);
                        await image.SaveAsPngAsync(fileStream);
                        //await file.CopyToAsync(fileStream);
                        fileStream.Position = 0;
                    }
                    return $"/images/users/{fileName}";
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
