#if API_CLIENT
namespace TwitchIntegration.Shared
{
    public class ChatMessage
    {
        public int Bits;

        public double BitsInDollars;

        public string Channel;

        public object CheerBadge;

        public string EmoteReplacedMessage;

        public string Id;

        public bool IsBroadcaster;

        public bool IsMe;

        public bool IsModerator;

        public bool IsSubscriber;

        public string Message;

        public int Noisy;

        public string RawIrcMessage;

        public int SubscribedMonthCount;

        public object Badges;

        public string BotUsername;

        public object Color;

        public string ColorHex;

        public string DisplayName;

        public object EmoteSet;

        public bool IsTurbo;

        public string UserId;

        public string Username;

        public int UserType;

        public override string ToString()
        {
            if (DisplayName.Length > 0)
            {
                return $"<color=\"{ColorHex}\">{DisplayName}</color>: {Message}";
            } 
            else
            {
                return $"<color=\"{ColorHex}\">{Message}</color>";
            }
        }
    }
}
#endif