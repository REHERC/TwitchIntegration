using System.Collections.Generic;
using TwitchIntegration.Shared;

namespace TwitchIntegration
{
    public static class MessageQueue
    {
        public static Queue<ChatMessage> Queue { get; set; } = new Queue<ChatMessage>();

        static MessageQueue()
        {
            Reset();
        }

        public static void Reset()
        {
            Queue.Clear();
            /*Queue.Enqueue(new ChatMessage()
            {
                Message = $"Welcome to {Plugin.Config.GetItem<string>("Channel")}'s twitch chat:",
                ColorHex = "#6441A4",
                Color = "#6441A4",
                DisplayName = "",
                Username = ""
            });*/
        }
    }
}
