using Microsoft.AspNetCore.SignalR;

namespace API.SignalRClass
{
    public class SignalRConnectionHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("AllClientsNotification", $"{Context.ConnectionId} has joined {Environment.NewLine} Say 'Hey' to him");
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.All.SendAsync("AllClientsNotification", $"{Context.ConnectionId} just left");
        }
    }
}
