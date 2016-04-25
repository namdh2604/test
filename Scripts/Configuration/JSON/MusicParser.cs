using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Voltage.Witches.Configuration.JSON
{
    public class MusicParser
    {
        public MusicParser()
        {
        }

        private const string PREFIX = "media/music/";
        private const string SUFFIX = ".mp3";
        public Dictionary<string, string> Parse(string data)
        {
            JObject root = JObject.Parse(data);
            Dictionary<string, string> entries = JsonConvert.DeserializeObject<Dictionary<string, string>>(root["Music"].ToString());
            Dictionary<string, string> mappedEntries = new Dictionary<string, string>();
            foreach (var entry in entries)
            {
                mappedEntries[entry.Key] = entry.Value.Replace(PREFIX, string.Empty).Replace(SUFFIX, string.Empty);
            }
            return mappedEntries;
        }
    }
}

