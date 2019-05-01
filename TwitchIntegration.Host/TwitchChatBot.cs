using Newtonsoft.Json;
using System;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace TwitchIntegration.Host
{
    public class TwitchChatBot : IDisposable
    {
        readonly ConnectionCredentials credentials_;
        private TwitchClient client_;

        private readonly string oauth_;
        private readonly string channel_;

        public TwitchChatBot(string oauth, string channel)
        {
            oauth_ = oauth;
            channel_ = channel;
            credentials_ = new ConnectionCredentials(
                "DistanceTwitchIntegration",
                oauth_
            );
        }

        public void Connect()
        {
            client_ = new TwitchClient();
            client_.Initialize(credentials_);

            client_.OnDisconnected += Client__OnDisconnected;
            client_.OnConnectionError += Client__OnConnectionError;

            client_.OnMessageReceived += Client__OnMessageReceived;
            client_.OnConnected += Client__OnConnected;
            client_.OnLog += Client__OnLog;

            client_.Connect();
        }

        private void Client__OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            client_.Reconnect();
        }

        private void Client__OnDisconnected(object sender, OnDisconnectedArgs e)
        {
            client_.Connect();
        }

        private void Client__OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine("LOG:" + JsonConvert.SerializeObject(e.Data));
        }

        private void Client__OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
        {
            client_.JoinChannel(channel_);
        }

        private void Client__OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            Console.WriteLine("DATA:" + JsonConvert.SerializeObject(e.ChatMessage));
        }

        public void Dispose()
        {
            client_.Disconnect();
        }
    }
}
