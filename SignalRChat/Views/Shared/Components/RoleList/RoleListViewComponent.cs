using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SignalRChat.Views.Shared.Components.RoleList
{
    public class RoleListViewComponent : ViewComponent
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleListViewComponent(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<IViewComponentResult> InvokeAsync(string name, string id, string roleId, string labelName = "", string labelClass="", string classList = "", string functionName="false", string isRequired="false" )
        {
            ViewBag.Name = name;
            ViewBag.Id = id;
            ViewBag.RoleId = roleId;
            ViewBag.LabelName = labelName;
            ViewBag.LabelClass = labelClass;
            ViewBag.ClassList = classList;
            ViewBag.FunctionName = functionName;
            ViewBag.Required = isRequired;  
            return View(await _roleManager.Roles.ToListAsync());
        }
    }
}
