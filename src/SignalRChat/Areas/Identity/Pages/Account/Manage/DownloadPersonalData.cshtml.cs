// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FastReport;
using FastReport.Data;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace SignalRChat.Areas.Identity.Pages.Account.Manage
{
    public class DownloadPersonalDataModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DownloadPersonalDataModel> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public DownloadPersonalDataModel(
            UserManager<ApplicationUser> userManager,
            ILogger<DownloadPersonalDataModel> logger,
            IWebHostEnvironment env, 
            IConfiguration config)
        {
            _userManager = userManager;
            _logger = logger;
            _env = env;
            _config = config;
        }

        public IActionResult OnGet()
        {
            return NotFound();
        }

        //public async Task<IActionResult> OnPostAsync()
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null)
        //    {
        //        return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        //    }

        //    _logger.LogInformation("User with ID '{UserId}' asked for their personal data.", _userManager.GetUserId(User));

        //    // Only include personal data for download
        //    var personalData = new Dictionary<string, string>();
        //    var personalDataProps = typeof(ApplicationUser).GetProperties().Where(
        //                    prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
        //    foreach (var p in personalDataProps)
        //    {
        //        personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
        //    }

        //    var logins = await _userManager.GetLoginsAsync(user);
        //    foreach (var l in logins)
        //    {
        //        personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
        //    }

        //    personalData.Add($"Authenticator Key", await _userManager.GetAuthenticatorKeyAsync(user));

        //    Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
        //    return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData), "application/json");
        //}

        //public async Task<IActionResult> OnPostAsync()
        //{
        //    try
        //    {
        //        await Task.Delay(0);
        //        FastReport.Utils.Config.WebMode = true;
        //        Report report = new();
        //        var mssqlConnection = new MsSqlDataConnection();
        //        mssqlConnection.ConnectionString = _config.GetConnectionString("DefaultConnection");
        //        report.Dictionary.Connections.Add(mssqlConnection);
        //        string path = $@"{_env.WebRootPath}\reports\personalData.frx";
        //        report.Report.Load(path);
        //        var loginUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        report.Report.SetParameterValue("id", loginUserId);
        //        if (report.Report.Prepare())
        //        {
        //            PDFSimpleExport pdfExport = new()
        //            {
        //                ShowProgress = true,
        //                Subject = "User Personal Data",
        //                Title = "User Personal Data"
        //            };
        //            using MemoryStream ms = new();
        //            report.Report.Export(pdfExport, ms);
        //            report.Report.Dispose();
        //            pdfExport.Dispose();
        //            ms.Flush();
        //            return File(ms.ToArray(), "application/pdf", "personal-data.pdf");
        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await Task.Delay(0);
                FastReport.Utils.Config.WebMode = true;
                Report report = new();
                //var mssqlConnection = new MsSqlDataConnection();
               // mssqlConnection.ConnectionString = _config.GetConnectionString("DefaultConnection");
                //report.Dictionary.Connections.Add(mssqlConnection);
                //string path = $@"{_env.WebRootPath}\reports\usersUsingSp.frx";
                //string path = $@"{_env.WebRootPath}\reports\usersUsingQuery.frx";
                //string path = $@"{_env.WebRootPath}/reports/usersByQueryWithParameter.frx";
                string path = System.IO.Path.Combine(_env.WebRootPath, "reports", "usersByQueryWithParameter.frx");
                report.Report.Load(path);
                string userName = HttpContext.User.Identity.Name;
                string loginUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                //report.Report.SetParameterValue("userName", userName);
                report.Report.SetParameterValue("id", loginUserId);
                if (report.Report.Prepare())
                {
                    PDFSimpleExport pdfExport = new()
                    {
                        ShowProgress = true,
                        Subject = "User Personal Data",
                        Title = "User Personal Data"
                    };
                    using MemoryStream ms = new();
                    report.Report.Export(pdfExport, ms);
                    report.Report.Dispose();
                    pdfExport.Dispose();
                    ms.Flush();
                    return File(ms.ToArray(), "application/pdf", "personal-data.pdf");
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
