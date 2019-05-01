using Harmony;
using Newtonsoft.Json;
using Spectrum.API.Configuration;
using Spectrum.API.Interfaces.Plugins;
using Spectrum.API.Interfaces.Systems;
using Spectrum.API.IPC;
using Spectrum.API.Logging;
using Spectrum.API.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using TwitchIntegration.Shared;

#pragma warning disable RCS1001
namespace TwitchIntegration
{
    public class Entry : IPlugin, IIPCEnabled
    {
        #region Singleton
        public static Entry Instance { get; private set; }
        #endregion
        #region Fields
        private string Channel;
        private string Token;
        public ProcessManager Application = new ProcessManager();
        public Process TwitchAPI;
        #endregion

        public void Initialize(IManager manager, string ipcIdentifier)
        {
            Plugin.Manager = manager;
            Plugin.IPCIdentifier = ipcIdentifier;
            Instance = this;
            AutoBehaviour.CreateInstance();
            #region Harmony
            HarmonyInstance Harmony = HarmonyInstance.Create("com.REHERC.TwitchIntegration");
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
            #endregion
            #region Files
            Plugin.Files = new FileSystem();
            #endregion
            #region Config
            Plugin.Config = new Settings("Twitch");
            foreach (KeyValuePair<string, object> item in new Dictionary<string, object>() {
                {"Channel", ""},
                {"Token", ""}
            })
                if (!Plugin.Config.ContainsKey(item.Key))
                    Plugin.Config[item.Key] = item.Value;
            Plugin.Config.Save();
            #endregion
            #region Log
            Plugin.Log = new Logger("Twitch.log")
            {
                WriteToConsole = true,
                ColorizeLines = true
            };
            #endregion
            #region Process Initialize
            Channel = Plugin.Config.GetItem<string>("Channel").ToLower();
            Token = Plugin.Config.GetItem<string>("Token").ToLower();

            ProcessStartInfo info = new ProcessStartInfo
            {
                WorkingDirectory = Plugin.Files.RootDirectory,
                FileName = Path.Combine(Plugin.Files.RootDirectory, @"Data\TwitchIntegration.Host.exe"),
                Arguments = $"\"{Channel}\" \"{Token}\"",
                UseShellExecute = false,

                CreateNoWindow = true,
                RedirectStandardOutput = true
            };

            Plugin.Log.Success(info.FileName);

            TwitchAPI = Application.Start(info);

            new Thread((data) =>
            {
                string line;
                while (Plugin.AppRunning)
                    try
                    {
                        if (!string.IsNullOrEmpty(line = TwitchAPI.StandardOutput.ReadLine()))
                            ProcessLine(line);
                    }
                    catch (Exception e) { Plugin.Log.Exception(e); }
            })
            { IsBackground = false }.Start();
            #endregion
        }

        public void CloseTwitchAPI()
        {
            try
            {
                if (!TwitchAPI.HasExited)
                {
                    TwitchAPI.Kill();
                    TwitchAPI.WaitForExit();
                    while (!TwitchAPI.HasExited)
                        continue;
                }
            }
            catch (Exception) { }
        }

        public void ProcessLine(string line)
        {
            if (line.StartsWith("DATA:"))
            {
                string data = line.Substring(5);
                ChatMessage msg = JsonConvert.DeserializeObject<ChatMessage>(data);
                Chat.OnMessageReceived(msg);
            }
        }

        public void HandleIPCData(IPCData data)
        {
            if (data.TryGetValue("content/header", out object header))
            {
                if ((string)header == "register/request")
                {
                    Plugin.IPCPluginList.Add(data.SourceIdentifier);
                    Plugin.Manager.SendIPC((string)header, new IPCData(Plugin.IPCIdentifier) {
                        { "content/header", "register/success" }
                    });
                }
            }
        }
    }

    public static class Plugin
    {
        public static IManager Manager;
        public static string IPCIdentifier = "";
        public static List<string> IPCPluginList = new List<string>();
        public static bool AppRunning = true;
        public static FileSystem Files;
        public static Settings Config;
        public static Logger Log;
    }
}
