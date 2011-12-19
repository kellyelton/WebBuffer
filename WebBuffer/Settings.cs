using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WebBuffer
{
    public static class Settings
    {
        public static Dictionary<string, string> ThisSettings = new Dictionary<string, string>();
        static Settings()
        {
            string[] temp = File.ReadAllLines("settings.ini");
            foreach(String line in temp)
            {
                if(line.Length == 0)
                    continue;
                if(line[0] == '#')
                    continue;
                string[] lol = line.Split(new string[1]{":"},StringSplitOptions.RemoveEmptyEntries);
                if(lol.Length == 2)
                    ThisSettings.Add(lol[0],lol[1]);
            }
        }
    }
}
