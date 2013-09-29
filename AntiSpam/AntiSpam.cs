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
		private Config Config = new Config();
		public override string Description
		{
			get { return "Prevents spamming."; }
		}
		public override string Name
		{
			get { return "AntiSpam"; }
		}
		private DateTime[] Time = new DateTime[256];
		private double[] Spam = new double[256];
		public override Version Version
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version; }
		}

		public AntiSpam(Main game)
			: base(game)
		{
			Order = -1000000;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				GameHooks.Initialize -= OnInitialize;
				NetHooks.SendData -= OnSendData;
				ServerHooks.Chat -= OnChat;
				ServerHooks.Leave -= OnLeave;
			}
		}
		public override void Initialize()
		{
			GameHooks.Initialize += OnInitialize;
			NetHooks.SendData += OnSendData;
			ServerHooks.Chat += OnChat;
			ServerHooks.Leave += OnLeave;
		}

		void OnChat(messageBuffer msg, int plr, string text, HandledEventArgs e)
		{
			if (!e.Handled)
			{
				if (text.StartsWith("/"))
				{
					string[] arr = text.Split(' ');
					if (text.StartsWith("/me ") && TShock.Players[plr].Group.HasPermission(Permissions.cantalkinthird))
					{
						text = text.Substring(4);
					}
					else if ((text.StartsWith("/tell") || text.StartsWith("/w ") || text.StartsWith("/whisper")) &&
						TShock.Players[plr].Group.HasPermission(Permissions.whisper))
					{
						text = text.Substring(arr[0].Length + arr[1].Length + 2);
					}
					else if ((text.StartsWith("/r ") || text.StartsWith("/reply ")) &&
						TShock.Players[plr].Group.HasPermission(Permissions.whisper))
					{
						text = text.Substring(arr[0].Length + 1);
					}
                    else if (text.Trim().Length == 1)
                    {
                        text = "/"; // Eliminates spamming with just "/"
                    }
                    else
                    {
                        return;
                    }
				}
				if ((DateTime.Now - Time[plr]).TotalSeconds > Config.Time)
				{
					Spam[plr] = 0.0;
					Time[plr] = DateTime.Now;
				}

				Spam[plr]++;
				double uniqueRatio = (double)text.GetUnique() / text.Length;
				if (text.Trim().Length <= Config.ShortLength)
				{
					Spam[plr] += 0.5;
				}
				else if (uniqueRatio <= 0.20 || uniqueRatio >= 0.80)
				{
					Spam[plr] += 0.5;
				}
				if (text.UpperCount() >= Config.CapsRatio)
				{
					Spam[plr] += 0.5;
				}

				if (Spam[plr] > Config.Threshold && !TShock.Players[plr].Group.HasPermission("antispam.ignore"))
				{
					switch (Config.Action)
					{
						case "ignore":
						default:
							Time[plr] = DateTime.Now;
							TShock.Players[plr].SendErrorMessage("You have been ignored for spamming.");
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
		void OnInitialize()
		{
			Commands.ChatCommands.Add(new Command("antispam.reload", Reload, "asreload"));

			string path = Path.Combine(TShock.SavePath, "antispamconfig.json");
			if (File.Exists(path))
			{
				Config = Config.Read(path);
			}
			Config.Write(path);
		}
		void OnLeave(int plr)
		{
			Spam[plr] = 0.0;
			Time[plr] = DateTime.Now;
		}
		void OnSendData(SendDataEventArgs e)
		{
			if (e.MsgID == PacketTypes.ChatText && !e.Handled)
			{
				if (Config.DisableBossMessages && e.number2 == 175 && e.number3 == 75 && e.number4 == 255)
				{
					if (e.text.StartsWith("Eye of Cthulhu") || e.text.StartsWith("Eater of Worlds") ||
						e.text.StartsWith("Skeletron") || e.text.StartsWith("King Slime") ||
						e.text.StartsWith("The Destroyer") || e.text.StartsWith("The Twins") ||
						e.text.StartsWith("Skeletron Prime") || e.text.StartsWith("Wall of Flesh"))
					{
						e.Handled = true;
					}
				}
				if (Config.DisableOrbMessages && e.number2 == 50 && e.number3 == 255 && e.number4 == 130)
				{
					if (e.text == "A horrible chill goes down your spine..." ||
						e.text == "Screams echo around you...")
					{
						e.Handled = true;
					}
				}
			}
		}

		void Reload(CommandArgs e)
		{
			string path = Path.Combine(TShock.SavePath, "antispamconfig.json");
			if (File.Exists(path))
			{
				Config = Config.Read(path);
			}
			Config.Write(path);
			e.Player.SendMessage("Reloaded antispam config.", Color.Green);
		}
	}
}