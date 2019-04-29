using TwitchIntegration.Shared;
using System.Linq;
using System;

namespace TwitchIntegration
{
    public static class Chat
    {
        public static string Messages = "";

        public static void OnMessageReceived(ChatMessage m)
        {
            MessageQueue.Queue.Enqueue(m);
            string message = $"{m.DisplayName}: {m.Message}";
            Plugin.Log.Info(message);
            SetMessages();
        }

        public static void SetMessages()
        {
            string msg = "";
            for (int i = 0; i < Math.Min(15, MessageQueue.Queue.Count); i++)
            {
                if (i > 0)
                    msg += "\n";
                msg += MessageQueue.Queue.ElementAt(i).ToString();
            }
            Messages = msg;
        }

        public static readonly string[] FormattingTags = new string[] {
            "i",
            "b",
            "size",
            "color"
        };

        public static string WordWrap(string input, int lineWidth)
        {
            int index=0; //char index in imput
            int position=0; //position on the line
            string output=""; //output value
            string line=""; //current line
            int lineCount=0; //number of lines created
            bool accountForPos=true; //does the current chr will increment the position value
            foreach (char chr in input.ToLower())
            {
                if($"{chr}"=="<")
                    foreach(string tag in FormattingTags)
                        if(input.Substring(index).StartsWith(OpenTag(tag))||input.Substring(index).StartsWith(CloseTag(tag)))
                            accountForPos=false;
                if($"{chr}"==">")
                    accountForPos = true;
                if ($"{chr}" == "\n")
                    position = 0;
                if(accountForPos)
                    position++;
                line+=input[index];
                if((position>=lineWidth&&accountForPos)||index>=input.Length-1)
                {
                    output+=(lineCount>0?"\n":"")+line;
                    line="";
                    position=0;
                    lineCount++;
                }
                index++;
            }
            return output;
        }

        private static string OpenTag(string input) => $"<{input}";
        private static string CloseTag(string input) => $"</{input}>";
    }
}
