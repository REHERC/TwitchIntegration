#pragma warning disable CS0169, RCS1075, RCS1102, IDE0044, SecurityIntelliSenseCS
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using TwitchIntegration.Shared;

namespace TwitchIntegration.Sandbox
{
    public class Program
    {
        public static ProcessManager Application = new ProcessManager();
        public static Process Host;

        public static void Main(string[] args)
        {
            if (args.Length != 2) Environment.Exit(0);

            SetConsoleCtrlHandler((sig) => {
                KillHost();
                return true;
            }, true);

            Console.Title = "Distance Twitch Integration - Sandbox";
            string pipe = Guid.NewGuid().ToString();
            string path = Environment.CurrentDirectory;

            ProcessStartInfo info = new ProcessStartInfo
            {
                WorkingDirectory = path,
                FileName = Path.Combine(path, @"Data\TwitchIntegration.Host.exe"),
                Arguments = $"\"{args[0]}\" \"{args[1]}\"",
                UseShellExecute = false,

                CreateNoWindow = true,
                RedirectStandardOutput = true
            };

            Host = Application.Start(info);

            new Thread((data) =>
            {
                string line;
                while (true)
                    if (!string.IsNullOrEmpty(line = Host.StandardOutput.ReadLine()))
                        ProcessLine(line);
            })
            { IsBackground = true }.Start();

            while (true)
                continue;
        }

        public static void KillHost()
        {
            try
            {
                if (!Host.HasExited)
                {
                    Host.Kill();
                    Host.WaitForExit();
                    while (!Host.HasExited)
                        continue;
                }
            }
            catch (Exception) { }
        }

        public static void ProcessLine(string line)
        {
            if (line.StartsWith("DATA:"))
            {
                string data = line.Substring(5);
                ChatMessage msg = JsonConvert.DeserializeObject<ChatMessage>(data);
                string message = $"{msg.DisplayName}: {msg.Message}";

                Console.WriteLine(message);
            }
        }

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        private static EventHandler _handler;

        public enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }
    }
}
