// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SignarlRChat.Areas.Identity.Pages.Account.Manage
{
    public class ProfilePhotoModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _webHost;

        public ProfilePhotoModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment webHost)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHost = webHost;
        }
        public string ProfilePhotoUrl { get; set; } = string.Empty;

        [TempData]
        public string StatusMessage { get; set; }


        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string ProfilePhotoUrl { get; set; } = string.Empty;
            public IFormFile ProfilePhoto { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var userInfo = await _userManager.GetUserAsync(User);
            ProfilePhotoUrl = userInfo.ProfilePhotoUrl;
            Input = new InputModel
            {
                ProfilePhotoUrl = userInfo.ProfilePhotoUrl
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }
            if (string.IsNullOrEmpty(Input.ProfilePhotoUrl) && Input.ProfilePhoto is  null)
            {
                return RedirectToPage();
            }
            user.ProfilePhotoUrl = await SaveImageAsync(Input.ProfilePhoto, user.UserName, 220, 220, user.ProfilePhotoUrl);
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Your profile Photo has been updated";
                return RedirectToPage();
            }
            return RedirectToPage();
        }

        private async Task<string> SaveImageAsync(IFormFile file, string username, int maxWidth, int maxHeight, string? existiongFileUrl = null)
        {
            try
            {
                if (file is null && String.IsNullOrEmpty(existiongFileUrl))                   // during create when no file uploaded
                {
                    return string.Empty;
                }
                else if (!string.IsNullOrEmpty(existiongFileUrl) && file is null)             // during edit while file was created but no changes
                {
                    return existiongFileUrl;
                }

                if (file is not null && file.Length > 0)                        // during create/changes file
                {
                    if (!string.IsNullOrEmpty(existiongFileUrl))
                    {
                        string existPath = _webHost.WebRootPath + existiongFileUrl.Replace('/', '\\');
                        if (System.IO.File.Exists(existPath))
                        {
                            System.IO.File.Delete(Path.Combine(existPath));
                        }
                    }
                    string uploadFolder = Path.Combine(_webHost.WebRootPath, "images", "users");
                    string extension = Path.GetExtension(file.FileName);
                    string fileName = $"{username}_{DateTime.Now.ToString("yyyyMMdd")}{extension}";
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
                            //Mode = ResizeMode.Min  // it maintain orignal aspect ratio
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
