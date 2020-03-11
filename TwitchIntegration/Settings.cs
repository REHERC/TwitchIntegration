using System.Collections.Generic;
using Configuration = Spectrum.API.Configuration;

namespace TwitchIntegration
{
    public static class Settings
    {
        public static Configuration.Settings Config;

        public static void Initialize()
        {
            Config = new Configuration.Settings("Settings");
            foreach (KeyValuePair<string, object> item in new Dictionary<string, object>() {
                {"ChatFontSize", 24},
                {"CharsPerLine", 22},
                {"CockpitChatFontSize", 18},
                {"CockpitCharsPerLine", 20},
                {"CockpitChatMargin", 0},
            })
            {
                if (!Config.ContainsKey(item.Key))
                {
                    Config[item.Key] = item.Value;
                }
            }
            Config.Save();
        }

        public static int ChatFontSize
        {
            get
            {
                return Config.GetItem<int>("ChatFontSize");
            }
            set
            {
                Config["ChatFontSize"] = value;
                Config.Save();
            }
        }

        public static int CharsPerLine
        {
            get
            {
                return Config.GetItem<int>("CharsPerLine");
            }
            set
            {
                Config["CharsPerLine"] = value;
                Config.Save();
            }
        }
        
        public static int CockpitChatFontSize
        {
            get
            {
                return Config.GetItem<int>("CockpitChatFontSize");
            }
            set
            {
                Config["CockpitChatFontSize"] = value;
                Config.Save();
            }
        }

        public static int CockpitCharsPerLine
        {
            get
            {
                return Config.GetItem<int>("CockpitCharsPerLine");
            }
            set
            {
                Config["CockpitCharsPerLine"] = value;
                Config.Save();
            }
        }

        public static int CockpitChatMargin
        {
            get
            {
                return Config.GetItem<int>("CockpitChatMargin");
            }
            set
            {
                Config["CockpitChatMargin"] = value;
                Config.Save();
            }
        }
    }
}
