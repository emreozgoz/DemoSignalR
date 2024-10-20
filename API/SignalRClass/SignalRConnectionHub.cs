using API.HubServices;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalRClass
{
    public class SignalRConnectionHub(HubService hubService) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("AllClientsNotification", Context.ConnectionId);
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            hubService.ConnectionGroups.Remove(Context.ConnectionId, out string? groupName);
            var groupMembers = hubService.GetMembers(groupName!);
            await Clients.Group(groupName!).SendAsync("NotifyGroupOfNewJoin", groupMembers);
            await Clients.All.SendAsync("AllClientsNotification", $"{Context.ConnectionId} just left");
        }

        public async Task SendMessageToGroup(string groupName, string message)
        {
            //gruba mesaj gönderen metot
            await Clients.Groups(groupName).SendAsync("Chat", Context.ConnectionId, message);
        }

        public async Task JoinGroup(string groupName)
        {
            //Gruba katıldığında diğer kullanıcılara mesaj veren bir metot
            var findGroup = hubService.FindGroupName(groupName);
            if (findGroup)
            {
                bool isUserInGroup = hubService.IsUserInGroup(Context.ConnectionId);
                if (!isUserInGroup)
                {
                    hubService.ConnectionGroups[Context.ConnectionId] = groupName;
                    await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                }
                var groupMembers = hubService.GetMembers(groupName);
                await Clients.Groups(groupName).SendAsync("NotifyGroupOfNewJoin", groupMembers);
            }
        }
    }
}
