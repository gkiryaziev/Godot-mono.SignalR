
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalR.Server.Hubs
{
    public class GameHub : Hub
    {
        public async Task ServerChatMessage(ChatMessage message)
        {
            await Clients.All.SendAsync("ClientChatMessage", message);
        }

        // [HubMethodName("PlayerPosition")]
        public async Task ServerPlayerPosition(PlayerPosition position)
        {
            await Clients.Others.SendAsync("ClientPlayerPosition", position);
        }
    }

    public class ChatMessage
    {
        public string GUID { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
    }

    public class PlayerPosition
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }
}