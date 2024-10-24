using API.HubServices;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalRClass
{
    public class SignalRConnectionHub(HubService hubService) : Hub
    {
        public static List<string> ConnectedClients = [];
        public static List<string> PendingMessages = [];
        private static bool RetrieveOldMessageCalled = false;

        public override async Task OnConnectedAsync()
        {
            ConnectedClients.Add(Context.ConnectionId);
            await Clients.All.SendAsync("AllClientsNotification", Context.ConnectionId, ConnectedClients);
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            ConnectedClients.Remove(Context.ConnectionId);
            hubService.ConnectionGroups.Remove(Context.ConnectionId, out string? groupName);
            var groupMembers = hubService.GetMembers(groupName!);
            //await Clients.Group(groupName!).SendAsync("NotifyGroupOfNewJoin", groupMembers);
            await Clients.All.SendAsync("AllClientsNotification", $"{Context.ConnectionId} just left", ConnectedClients);
        }

        public async Task SendMessageToGroup(string groupName, string message)
        {
            //gruba mesaj gönderen metot
            await Clients.Groups(groupName).SendAsync("Chat", Context.ConnectionId, message);
        }

        public async Task SendMessageToIndividual(string receiverConnectionId, string message)
        {
            if (!RetrieveOldMessageCalled)
            {
                PendingMessages.Add($"{Context.ConnectionId}:{message}");
            }
            else
            {
                PendingMessages.Clear();
            }
            await Clients.Client(receiverConnectionId).SendAsync("Chat", Context.ConnectionId, message);
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

        public async Task RetrieveOldMessages()
        {
            List<string> messages = [];
            if (PendingMessages.Count == 0)
            {
                return;
            }
            foreach (var item in PendingMessages)
            {
                messages.Add(item);
            }

            RetrieveOldMessageCalled = true;
            await Clients.Clients(Context.ConnectionId).SendAsync("RetrieveOldMessages", messages);
        }
    }
}
