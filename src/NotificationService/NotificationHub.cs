using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace NotificationService;

public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var subject = Context.User.FindFirst("sub");

        if (subject == null)
        {
            subject = new Claim("sub", Guid.Empty.ToString());
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, subject.Value);
    }
}
