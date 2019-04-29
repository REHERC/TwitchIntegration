using System.Collections.Generic;
using TwitchIntegration.Shared;

namespace TwitchIntegration
{
    public static class MessageQueue
    {
        public static Queue<ChatMessage> Queue = new Queue<ChatMessage>();
    }
}
