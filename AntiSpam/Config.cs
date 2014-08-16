using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace AntiSpam
{
	public class Config
	{
		public bool DisableBossMessages = false;
        public bool DisableMobMessages = false;
        public bool DisableNPCMessages = false;
        public bool DisablePVPMessages = false;
        public bool DisableOrbMessages = false;

		public string Action = "ignore";
		public double CapsRatio = 0.66;
		public double CapsWeight = 2.0;
		public double NormalWeight = 1.0;
		public int ShortLength = 4;
		public double ShortWeight = 2.0;

		public double Threshold = 5.0;
		public int Time = 5;

		public void Write(string path)
		{
			File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
		}

		public static Config Read(string path)
		{
			if (!File.Exists(path))
			{
				return new Config();
			}
			return JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
		}
	}
}