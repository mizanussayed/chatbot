using SignalRChat.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SignalRChat.Views.Shared.Components.Notification
{
    public class NotificationViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public NotificationViewComponent(ApplicationDbContext context) => _context = context;
        public async Task<IViewComponentResult> InvokeAsync(string loginUserId)
        {
            var notifications = await _context.TransectionHistories
                .AsNoTracking()
                .Where(x => x.NotifyUserId == loginUserId)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
            notifications
                .ForEach(x => x.WhenAgo = GetWhenAgo(x.CreatedDate));
            ViewBag.UnreadMessageCount = notifications
                .Where(x => !x.IsSeen)
                .Count();
            return View(notifications);
        }
        private string GetWhenAgo(DateTime createdDate)
        {
            DateTime today = DateTime.Now;
            var dateDiff = today - createdDate;
            string ago;
            if (dateDiff.TotalSeconds > 0 && dateDiff.TotalSeconds < 60)
                ago = $"{(int)dateDiff.TotalSeconds} seconds ago";
            else if (dateDiff.TotalMinutes > 0 && dateDiff.TotalMinutes < 60)
                ago = string.Format("{0} {1} ago", (int)dateDiff.TotalMinutes, (int)dateDiff.TotalMinutes > 1 ? "minutes" : "minute");
            else if (dateDiff.TotalHours > 0 && dateDiff.TotalHours < 24)
                ago = string.Format("{0} {1} ago", (int)dateDiff.TotalHours, (int)dateDiff.TotalHours > 1 ? "hours" : "hour");
            else
                ago = string.Format("{0} {1} ago", (int)dateDiff.TotalDays, (int)dateDiff.TotalDays > 1 ? "days" : "day");
            return ago;
        }
    }
}
