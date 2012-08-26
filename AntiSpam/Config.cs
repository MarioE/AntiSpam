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
		public string Action = "ignore";
		public double CapsRatio = 0.66;
		public bool DisableBossMessages = false;
		public bool DisableOrbMessages = false;
		public int ShortLength = 6;
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