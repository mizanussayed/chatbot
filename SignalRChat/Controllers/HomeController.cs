﻿using SignarlRChat.Interface;
using SignarlRChat.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace SignarlRChat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IToastNotification _toast;

        public HomeController(ILogger<HomeController> logger, IToastNotification toast)
        {
            _logger = logger;
            _toast = toast;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}