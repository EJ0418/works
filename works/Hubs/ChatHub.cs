using Microsoft.AspNetCore.SignalR;

namespace works.Hubs
{
    public class ChatHub : Hub
    {
        private readonly string _serverid = Environment.GetEnvironmentVariable("SERVER_ID");

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message, DateTime.Now);
        }

        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("UserJoined", $"{Context.ConnectionId} joined {roomName}");
        }

        public async Task LeaveRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("UserLeft", $"{Context.ConnectionId} left {roomName}");
        }

        public async Task SendMessageToRoom(string roomName, string user, string message)
        {
            await Clients.Group(roomName).SendAsync("ReceiveMessage", user, message, DateTime.Now);
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("UserConnected", $"Server:{_serverid}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.All.SendAsync("UserDisconnected", $"Server:{_serverid}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}