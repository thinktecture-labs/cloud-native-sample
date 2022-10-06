using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace NotificationService;

public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var subject = Context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (subject == null)
        {
            subject = new Claim(ClaimTypes.NameIdentifier, string.Empty);
        }
        await Groups.AddToGroupAsync(Context.ConnectionId, subject.Value);
    }
}
