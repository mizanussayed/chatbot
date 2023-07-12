using SignalRChat.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using SignalRChat.Interface;
using SignalRChat.DTOs;
using SignalRChat.Extensions;
using SignalRChat.Constant;

namespace SignalRChat.Areas.Admin.Controllers;

    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IToastNotification _toast;
        private readonly IMailService _mailService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager, IToastNotification toast, IMailService mailService, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _toast = toast;
            _mailService = mailService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public PartialViewResult OnGetUserRegister()
        {
            return PartialView("_RegisterUser", new RegisterViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> OnPostUserRegister(RegisterViewModel model)
        {
            try
            {
                var returnUrl = Url.Content("~/");
                if (ModelState.IsValid)
                {

                    ApplicationUser user = new();
                    user.FirstName = model.FirstName.Trim();
                    user.LastName = model.LastName.Trim();
                    user.UserName = model.UserName.Trim();
                    user.Email = model.Email.Trim();
                    user.PhoneNumber = model.PhoneNumber?.Trim();
                    await _userManager.AddPasswordAsync(user, model.Password);
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        result = await _userManager.AddToRoleAsync(user, Roles.ChatUser.ToString());
                        _logger.LogInformation("User created a new account with password.");
                        if (result.Succeeded)
                        {
                            var userId = await _userManager.GetUserIdAsync(user);
                            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                            var callbackUrl = Url.Page(
                                "/Account/ConfirmEmail",
                                pageHandler: null,
                                values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                                protocol: Request.Scheme);

                            List<string> to = new List<string> { model.Email.Trim() };
                            string subject = "Confirm Your Mail";
                            string body = $"<h2>Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.</h2>";
                            MailRequest mail = new(to, subject, body);

                            bool isSend = await _mailService.SendAsync(mail, new CancellationToken());
                            //if (isSend)
                            //{
                            //    await _toast.ToastSuccess("User Registered Successfully");
                            //    return new JsonResult(new { IsSuccess = true, Username = user.UserName, Password = model.Password });
                            //}
                            //else
                            //{
                            //    await _toast.ToastSuccess("User Registered Successfully");
                            //    return new JsonResult(new { IsSuccess = true, Username = user.UserName, Password = model.Password });
                            //}
                            await _toast.Success("User Registered Successfully");
                            return new JsonResult(new { IsSuccess = true, Username = user.UserName, Password = model.Password });
                        }
                    }
                    await _toast.Error($"{result.Errors}");
                    return new JsonResult(new { IsSuccess = false });
                }
                else
                {
                    await _toast.Error(ModelState.GetModelStateError());
                    return new JsonResult(new { IsSuccess = false });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        [AllowAnonymous]
        [HttpGet]
        public PartialViewResult OnGetForgotPassword()
        {
            return PartialView("_ForgotPassword", new ForgotPasswordViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> OnPostForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                //if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                //{
                //    // Don't reveal that the user does not exist or is not confirmed
                //    return new JsonResult(new {IsSuccess = true, Message = "Please check your email to reset your password." });
                //}

                // For more information on how to enable account confirmation and password reset please
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                List<string> to = new List<string> { model.Email.Trim() };
                string subject = "Reset Password";
                string body = $"<h2>Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.</h2>";
                MailRequest mail = new(to, subject, body);

                bool isSend = await _mailService.SendAsync(mail, new CancellationToken());
                if (isSend)
                    return new JsonResult(new { IsSuccess = true, Message = "Please check your email to reset your password." });

                 return new JsonResult(new { IsSuccess = false, Message = "Something went wrong" });
            }
            return new JsonResult(new { IsSuccess = false, Message = "Please write valid Email Address" });
        }
    }
