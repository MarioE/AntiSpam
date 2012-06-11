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
        public int ChatSpamThreshold = 5;
        public int ChatSpamTime = 5;
        public static Action<Config> ConfigRead;

        public void Write(string path)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                Write(fs);
            }
        }
        public void Write(Stream stream)
        {
            var str = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (var sw = new StreamWriter(stream))
            {
                sw.Write(str);
            }
        }

        public static Config Read(string path)
        {
            if (!File.Exists(path))
            {
                return new Config();
            }
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Read(fs);
            }
        }
        public static Config Read(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                var cf = JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
                if (ConfigRead != null)
                {
                    ConfigRead(cf);
                }
                return cf;
            }
        }
    }
}
