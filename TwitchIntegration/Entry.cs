using Harmony;
using Newtonsoft.Json;
using Configuration = Spectrum.API.Configuration;
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
using Spectrum.API.GUI.Data;
using Spectrum.API.GUI.Controls;

#pragma warning disable RCS1001, SecurityIntelliSenseCS
namespace TwitchIntegration
{
    public class Entry : IPlugin, IIPCEnabled
    {
        #region Singleton
        public static Entry Instance { get; private set; }
        public ProcessManager Application { get => application; set => application = value; }
        #endregion
        #region Fields
        private string Channel;
        private string Token;
        private ProcessManager application = new ProcessManager();
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
            #region Plugin Settings
            Settings.Initialize();
            #endregion
            #region Twitch Config
            Plugin.Config = new Configuration.Settings("Twitch");
            foreach (KeyValuePair<string, object> item in new Dictionary<string, object>() {
                {"Channel", ""},
                {"Token", ""}
            })
            {
                if (!Plugin.Config.ContainsKey(item.Key))
                {
                    Plugin.Config[item.Key] = item.Value;
                }
            }
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
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(line = TwitchAPI.StandardOutput.ReadLine()))
                        {
                            ProcessLine(line);
                        }
                    }
                    catch (Exception e) { Plugin.Log.Exception(e); }
                }
            })
            { IsBackground = false }.Start();
            #endregion
            #region Create Settings Menu
            manager.Menus.AddMenu(MenuDisplayMode.Both, new MenuTree("twitchintegration.main", "TWITCH CHAT SETTINGS")
            {
                new IntegerSlider(MenuDisplayMode.Both, "twitchintegration.main.chatfontsize", "CHAT TEXT FONT SIZE")
                .LimitedByRange(1, 96)
                .WithGetter(() => Settings.ChatFontSize)
                .WithSetter((value) => Settings.ChatFontSize = value)
                .WithDefaultValue(24)
                .WithDescription("Sets the font size of the twitch chat on teh car screen."),
                
                new IntegerSlider(MenuDisplayMode.Both, "twitchintegration.main.charsperline", "CHARACTERS PER LINE")
                .LimitedByRange(10, 200)
                .WithGetter(() => Settings.CharsPerLine)
                .WithSetter((value) => Settings.CharsPerLine = value)
                .WithDefaultValue(22)
                .WithDescription("Sets the maximum number of characters that can be displayed on a single line."),

                new IntegerSlider(MenuDisplayMode.Both, "twitchintegration.main.cockpitchatfontsize", "CHAT TEXT FONT SIZE (COCKPIT VIEW)")
                .LimitedByRange(1, 48)
                .WithGetter(() => Settings.CockpitChatFontSize)
                .WithSetter((value) => Settings.CockpitChatFontSize = value)
                .WithDefaultValue(18)
                .WithDescription("Sets the font size of the twitch chat on teh car screen when in cockpit view."),
                
                new IntegerSlider(MenuDisplayMode.Both, "twitchintegration.main.cockpitcharsperline", "CHARACTERS PER LINE (COCKPIT VIEW)")
                .LimitedByRange(10, 200)
                .WithGetter(() => Settings.CockpitCharsPerLine)
                .WithSetter((value) => Settings.CockpitCharsPerLine = value)
                .WithDefaultValue(20)
                .WithDescription("Sets the maximum number of characters that can be displayed on a single line when in cockpit view."),

                new IntegerSlider(MenuDisplayMode.Both, "twitchintegration.main.cockpitchatmargin", "CHAT TEXT MARGIN (COCKPIT VIEW)")
                .LimitedByRange(0, 150)
                .WithGetter(() => Settings.CockpitChatMargin)
                .WithSetter((value) => Settings.CockpitChatMargin = value)
                .WithDefaultValue(0)
                .WithDescription("Sets space added to the left side of the chat to avoid being hidden when in cockpit view.")
            });
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
                    while (!TwitchAPI.HasExited);
                }
            }
            catch (Exception e) { 
                Plugin.Log.Exception(e); 
            }
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
        public static Configuration.Settings Config;
        public static Logger Log;
    }
}
