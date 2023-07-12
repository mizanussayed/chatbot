using Microsoft.AspNetCore.Identity;

namespace SignalRChat.Extensions
{
    public static class ApplicationUserExtension
    {
        public static async Task<string> GetUserFullNameById(UserManager<ApplicationUser> userManager, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return string.Empty;
            }
            return string.Concat(user.FirstName, " ", user.LastName);
        }
        public static async Task<string> GetUserFullNameByName(UserManager<ApplicationUser> userManager, string userName)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return string.Empty;
            }
            return string.Concat(user.FirstName, " ", user.LastName);
        }
    }
}
