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
    public class MatchDetail
    {
        private static MatchDetail _current;
        public static MatchDetail Current
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

        public Dictionary<string,Team> Teams { get; set; }
        public Team Home { get; set; }
        public Team Away { get; set; }

        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public int Milliseconds { get; set; }

        public int? MatchId { get; set; }

        public MatchDetail()
        {
            Teams = new Dictionary<string,Team>();
            Home = new Team();
            Away = new Team();
        }

        public static MatchDetail Load()
        {
            if (File.Exists(JsonFileName))
            {
                var json = File.ReadAllText(JsonFileName);
                return JsonSerializer.Deserialize<MatchDetail>(json);
            }

            return new MatchDetail();
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
