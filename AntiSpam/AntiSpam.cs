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
        public override string Name
        {
            get { return "AntiSpam"; }
        }
        public DateTime[] Time = new DateTime[256];
        public int[] Spam = new int[256];
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
                ServerHooks.Chat -= OnChat;
                ServerHooks.Leave -= OnLeave;
            }
        }
        public override void Initialize()
        {
            NetHooks.SendData += OnSendData;
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
                if ((DateTime.Now - Time[plr]).TotalSeconds > Config.Time)
                {
                    Spam[plr] = 0;
                    Time[plr] = DateTime.Now;
                }

                Spam[plr]++;
                if (text.UpperCount() > Config.CapsRatio)
                {
                    Spam[plr]++;
                }
                if (text.Trim().Length <= 3)
                {
                    Spam[plr]++;
                }
                else if (text.GetCount('!') > 4 || text.GetCount('?') > 4)
                {
                    Spam[plr]++;
                }
                if ((double)text.GetUnique() / text.Length < 5)
                {
                    Spam[plr]++;
                }

                if (Spam[plr] > Config.Threshold && !TShock.Players[plr].Group.HasPermission("ignorechatspam"))
                {
                    switch (Config.Action)
                    {
                        case "ignore":
                        default:
                            Time[plr] = DateTime.Now;
                            TShock.Players[plr].SendMessage("You have been ignored for spamming.", Color.Red);
                            e.Handled = true;
                            break;
                        case "kick":
                            TShock.Utils.ForceKick(TShock.Players[plr], "Spamming");
                            e.Handled = true;
                            break;
                    }
                }
            }
        }
        void OnLeave(int plr)
        {
            Spam[plr] = 0;
            Time[plr] = DateTime.Now;
        }
        void OnSendData(SendDataEventArgs e)
        {
            if (e.MsgID == PacketTypes.ChatText && !e.Handled)
            {
                if (e.number == 255 && e.number2 == 175 && e.number3 == 75 && Config.DisableBossMessages)
                {
                    e.Handled = true;
                }
            }
        }
    }
}
