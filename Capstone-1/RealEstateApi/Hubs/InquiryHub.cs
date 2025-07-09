using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RealEstateApi.Misc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RealEstateApi.Hubs
{
    // InquiryHub.cs
    [Authorize]
    public class InquiryHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InquiryHub(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task OnConnectedAsync()
        {
            // var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"Client connected: {Context.ConnectionId}, UserId: {userId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}, UserId: {userId}");
            await base.OnDisconnectedAsync(exception);
        }

        // public async Task JoinInquiryGroup(string inquiryId)
        // {
        //     await Groups.AddToGroupAsync(Context.ConnectionId, $"inquiry-{inquiryId}");
        //     Console.WriteLine($"Client {Context.ConnectionId} joined inquiry group {inquiryId}");
        // }

        public async Task JoinInquiryGroup(string inquiryId)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
            {
                throw new HubException("Unauthorized - missing user claims");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"inquiry-{inquiryId}");
            Console.WriteLine($"Client {Context.ConnectionId} joined group {inquiryId}");
        }

        public async Task SendReply(string inquiryId, string message, string authorType)
        {
            // var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
            // var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

            // if (userId == null || userRole == null)
            // {
            //     throw new HubException("Unauthorized");
            // }
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
            {
                throw new HubException("Unauthorized - missing user claims");
            }

            // Verify the sender's role matches the claimed role
            if (authorType != userRole)
            {
                throw new HubException("Unauthorized - role mismatch");
            }

            // Broadcast the reply to all clients in the group
            await Clients.Group($"inquiry-{inquiryId}").SendAsync("ReceiveReply", new
            {
                InquiryId = inquiryId,
                Message = message,
                AuthorType = authorType,
                AuthorId = userId,
                CreatedAt = DateTime.UtcNow
            });
        }
        
        public async Task RemoveFromGroup(string inquiryId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"inquiry-{inquiryId}");
            Console.WriteLine($"Client {Context.ConnectionId} left group {inquiryId}");
        }


    }
}