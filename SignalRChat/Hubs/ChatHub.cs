using SignarlRChat.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace SignarlRChat.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SendToUserAsync(Conversation conversation)
        {
            await Clients.User(conversation.ToUserId).SendAsync("ReceiveMessages", conversation);
        }

        //public async Task SendToGroupUsersAsync(Conversation conversation, string groupName)
        //{
        //    await Clients.Group(groupName).SendAsync("ReceiveGroupMessages", conversation);
        //}
        public async Task SendToGroupUsersAsync(Conversation conversation)
        {
            var chatGroup = await _context.ChatGroups.FindAsync(conversation.GroupId);
            await Clients.Group(chatGroup.Name).SendAsync("ReceiveGroupMessages", conversation);
        }

        public override async Task OnConnectedAsync()
        {
            //var loginUserId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            //var userInGroups = await _context.UserGroups.Where(g => g.UserId == loginUserId).Include(g => g.ChatGroup).ToListAsync();
            //// Add to each assigned group.
            //foreach (var userGroup in userInGroups)
            //{
            //  await  Groups.AddToGroupAsync(Context.ConnectionId, userGroup.ChatGroup.Name);
            //}

            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserHandler.ConnectedUserIdList.Add(userId);
            await Clients.All.SendAsync("ReceiveConNotify", UserHandler.ConnectedUserIdList);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserHandler.ConnectedUserIdList.Remove(userId);
            await Clients.All.SendAsync("ReceiveConNotify", UserHandler.ConnectedUserIdList);
            await base.OnDisconnectedAsync(exception);
        }

        public static class UserHandler
        {
            public static List<string> ConnectedUserIdList = new List<string>();
        }
    }
}
