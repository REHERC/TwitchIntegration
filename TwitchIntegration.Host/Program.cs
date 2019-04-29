using System;
using System.Threading.Tasks;

#pragma warning disable RCS1102, IDE0060, RCS1163, RCS1075
namespace TwitchIntegration.Host
{
    public class Program
    {
        private static TwitchChatBot Bot;

        public async static Task Main(string[] args)
        {
            Console.Title = "Distance Twitch Integration - API";

            try
            {
                Bot = new TwitchChatBot(args[1], args[0]);
                Bot.Connect();
            }
            catch (Exception) { }
            await Task.Delay(-1);
        }
    }
}
