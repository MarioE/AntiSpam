using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Hooks;
using Terraria;
using TShockAPI;

namespace AntiSpam
{
    [APIVersion(1, 12)]
    public class AntiSpam : TerrariaPlugin
    {
        public override string Author
        {
            get { return "MarioE"; }
        }
        public Config Config;
        public override string Description
        {
            get { return "Prevents spamming."; }
        }
        public string[] LastChat = new string[256];
        public DateTime LastCheck = DateTime.Now;
        public override string Name
        {
            get { return "AntiSpam"; }
        }
        public int[] Seconds = new int[256];
        public int[] SpamPoints = new int[256];
        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public AntiSpam(Main game)
            : base(game)
        {
            Config = new Config();
            Order = 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                NetHooks.SendData -= OnSendData;
                GameHooks.Update -= OnUpdate;
                ServerHooks.Chat -= OnChat;
                ServerHooks.Leave -= OnLeave;
            }
        }
        public override void Initialize()
        {
            NetHooks.SendData += OnSendData;
            GameHooks.Update += OnUpdate;
            ServerHooks.Chat += OnChat;
            ServerHooks.Leave += OnLeave;

            if (File.Exists(Path.Combine(TShock.SavePath, "antispamconfig.json")))
            {
                Config = Config.Read(Path.Combine(TShock.SavePath, "antispamconfig.json"));
            }
            Config.Write(Path.Combine(TShock.SavePath, "antispamconfig.json"));
        }

        void OnChat(messageBuffer msg, int plr, string text, HandledEventArgs e)
        {
            if (!e.Handled && !text.StartsWith("/"))
            {
                SpamPoints[plr]++;
                if (text.IsUpper())
                {
                    SpamPoints[plr]++;
                }
                else if (text.Length <= 3)
                {
                    SpamPoints[plr]++;
                }
                else if (LastChat[plr] == text)
                {
                    SpamPoints[plr]++;
                }
                if (SpamPoints[plr] > Config.ChatSpamThreshold && !TShock.Players[plr].Group.HasPermission("ignorechatspam"))
                {
                    TShock.Players[plr].SendMessage("You have been ignored for spamming.", Color.Red);
                    e.Handled = true;
                }
                else
                {
                    Seconds[plr] = 0;
                }
                LastChat[plr] = text;
            }
        }
        void OnLeave(int plr)
        {
            LastChat[plr] = "";
            Seconds[plr] = 0;
            SpamPoints[plr] = 0;
        }
        void OnSendData(SendDataEventArgs e)
        {
            if (e.MsgID == PacketTypes.ChatText && !e.Handled && e.number == 255 && e.number2 == 175 && e.number3 == 75 && Config.DisableBossMessages)
            {
                e.Handled = true;
            }
        }
        void OnUpdate()
        {
            if ((DateTime.Now - LastCheck).TotalSeconds >= 1)
            {
                LastCheck = DateTime.Now;
                for (int i = 0; i < 256; i++)
                {
                    Seconds[i]++;
                    if (Seconds[i] > Config.ChatSpamTime)
                    {
                        SpamPoints[i] = 0;
                        Seconds[i] = 0;
                    }
                }
            }
        }
    }
}
