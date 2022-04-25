
using Microsoft.AspNetCore.SignalR;

namespace NotificationService;

public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var subject = Context.User.FindFirst("sub");
        if (subject != null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, subject.Value);
        }
    }
}
