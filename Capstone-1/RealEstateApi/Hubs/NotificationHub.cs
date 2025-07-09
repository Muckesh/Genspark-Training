using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RealEstateApi.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            // var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
            // if (role == "Buyer")
            // {
            //     await Groups.AddToGroupAsync(Context.ConnectionId, "Buyers");
            // }

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
