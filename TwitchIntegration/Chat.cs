﻿#pragma warning disable RCS1196, RCS1001
using TwitchIntegration.Shared;
using System.Linq;
using System;
using Spectrum.API.IPC;
using System.Text;

namespace TwitchIntegration
{
    public static class Chat
    {
        public static void OnMessageReceived(ChatMessage m)
        {
            MessageQueue.Queue.Enqueue(m);
            if (MessageQueue.Queue.Count > 100)
            {
                MessageQueue.Queue.Dequeue();
            }
            string message = $"{m.DisplayName}: {m.Message}";
            Plugin.Log.Info(message);
            foreach (string plugin in Plugin.IPCPluginList)
            {
                IPCData data = new IPCData(Plugin.IPCIdentifier)
                {
                    { "content/header", "data/message" },
                    { "Bits", m.Bits },
                    { "BitsInDollars", m.BitsInDollars },
                    { "Badges", m.Badges },
                    { "BotUsername", m.BotUsername },
                    { "Channel", m.Channel },
                    { "CheerBadge", m.CheerBadge },
                    { "Color", m.Color },
                    { "ColorHex", m.ColorHex },
                    { "DisplayName", m.DisplayName },
                    { "EmoteReplacedMessage", m.EmoteReplacedMessage },
                    { "EmoteSet", m.EmoteSet },
                    { "Id", m.Id },
                    { "IsBroadcaster", m.IsBroadcaster },
                    { "IsMe", m.IsMe },
                    { "IsModerator", m.IsModerator },
                    { "IsSubscriber", m.IsSubscriber },
                    { "IsTurbo", m.IsTurbo },
                    { "Message", m.Message },
                    { "Noisy", m.Noisy },
                    { "RawIrcMessage", m.RawIrcMessage },
                    { "SubscribedMonthCount", m.SubscribedMonthCount },
                    { "UserId", m.UserId },
                    { "Username", m.Username },
                    { "UserType", m.UserType }
                };

                Plugin.Manager.SendIPC(plugin, data);
            }
        }

        public static string GetMessages(int max, int margin)
        {
            string messages = GetMessages(max);
            string result;

            StringBuilder sb = new StringBuilder();

            foreach (string line in messages.Lines())
            {
                sb.Append(' ', margin);
                sb.Append(line);
                sb.Append('\n');
            }

            result = sb.ToString();
            result = result.Substring(0, result.Length - 1);

            return result;
        }

        public static string GetMessages(int max)
        {
            string msg = "";
            for (int i = Math.Max(0, MessageQueue.Queue.Count - Settings.MaxdDisplayedMessages); i < MessageQueue.Queue.Count; i++)
            {
                if (i > 0)
                {
                    msg += msg.EndsWith("\n") ? string.Empty : "\n";
                }
                msg += StringExtensions.WordWrap(MessageQueue.Queue.ElementAt(i).ToString(), max);
            }

            while (msg.Contains("\n\n"))
            {
                msg = msg.Replace("\n\n", "\n");
            }

            return !string.IsNullOrEmpty(msg) ? msg : "";
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
