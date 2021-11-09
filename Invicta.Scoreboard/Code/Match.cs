using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace Invicta.Scoreboard.Code
{
    public class Match
    {
        private static Match _current;
        public static Match Current
        {
            get
            {
                if (_current == null)
                {
                    _current = Load();
                }

                return _current;
            }
        }

        private static string JsonFileName { get { return @"C:\Hockey\Match.json"; } }

        public Team Home { get; set; }
        public Team Away { get; set; }

        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public int Milliseconds { get; set; }

        public Match()
        {
            Home = new Team();
            Away = new Team();
        }

        public static Match Load()
        {
            if (File.Exists(JsonFileName))
            {
                var json = File.ReadAllText(JsonFileName);
                return JsonSerializer.Deserialize<Match>(json);
            }

            return new Match();
        }
        public void Save()
        {
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(JsonFileName, json);
        }
    }

}
