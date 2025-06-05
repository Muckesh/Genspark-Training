using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
        if (role == "Staff")
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Staffs");

        }
        await base.OnConnectedAsync();
    }
    
}
