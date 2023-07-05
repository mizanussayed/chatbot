using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalRChat.Entity;

namespace SignalRChat.Views.Shared.Components.Users
{
    public class UsersViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersViewComponent(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync(string name, string id, string userId, string labelName = "", string labelClass="", string classList = "", string functionName="false", string isRequired="false" )
        {
            ViewBag.Name = name;
            ViewBag.Id = id;
            ViewBag.UserId = userId;
            ViewBag.LabelName = labelName;
            ViewBag.LabelClass = labelClass;
            ViewBag.ClassList = classList;
            ViewBag.FunctionName = functionName;
            ViewBag.Required = isRequired;  
            return View(await _userManager.Users.ToListAsync());
        }
    }
}
