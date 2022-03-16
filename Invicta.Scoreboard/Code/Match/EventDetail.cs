using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invicta.Scoreboard.Code.Match
{
    public class EventDetail
    {
        public enum EventTypes : byte
        {
            Gol,
            Timeout,
            Fallo,
            Goalkeeper
        }

        public enum TimeType : byte
        {
            PrimoTempo,
            SecondoTempo,
            TempoSupplementare,
            Rigori
        }

        /*
Tempo: P2
Minuto: 00:11
Team: Fal
Player: 14
Player name: Roccaforte, Matteo
Player name: Bellinaso, Antonio
Action: Gol
Team Full 1: 
Team Full 2:  Hc Falcons 
        */
        public TimeType Tempo { get; set; }

        public string Minuto { get; set; }

        public string TeamCode { get; set; }

        public EventTypes EventType { get; set; }

        public int? PlayerId { get; set; }
        public string PlayerName { get; set; }

        public int? AssistPlayerId { get; set; }
        public string AssistPlayerName { get; set; }

        public override string ToString()
        {
            var result = new StringBuilder();
            //result.Append('[');
            //result.Append(TeamCode);
            //result.Append(']');
            if (EventType == EventTypes.Gol || EventType == EventTypes.Timeout || EventType == EventTypes.Fallo)
            {
                switch (Tempo)
                {
                    case TimeType.PrimoTempo:
                        result.Append("1° tempo");
                        result.Append(' ');
                        result.Append(Minuto);
                        result.Append(' ');
                        break;
                    case TimeType.SecondoTempo:
                        result.Append("2° tempo");
                        result.Append(' ');
                        result.Append(Minuto);
                        result.Append(' ');
                        break;
                    case TimeType.TempoSupplementare:
                        result.Append("SUP");
                        result.Append(' ');
                        result.Append(Minuto);
                        result.Append(' ');
                        break;
                    case TimeType.Rigori:
                        result.Append("Rigori");
                        result.Append(' ');
                        break;
                    default:
                        break;
                }

                switch (EventType)
                {
                    case EventTypes.Gol:
                        result.Append("Gol di");
                        result.Append(' ');
                        result.Append(GetPlayerName(PlayerName, PlayerId));
                        if (!string.IsNullOrWhiteSpace(AssistPlayerName))
                        {
                            result.Append('\n');
                            result.Append("     su assist di");
                            result.Append(' ');
                            result.Append(GetPlayerName(AssistPlayerName, AssistPlayerId));
                        }
                        break;
                    case EventTypes.Timeout:
                        result.Append("Timeout");
                        break;
                    case EventTypes.Fallo:
                        result.Append("Fallo");
                        result.Append(' ');
                        result.Append(GetPlayerName(PlayerName, PlayerId));
                        break;
                }
            }

            return result.ToString();
        }

        private string GetPlayerName(string name, int? playerId)
        {
            int i = name.IndexOf(',');
            var result = new StringBuilder();
            if (i > 0)
                result.Append(name.Substring(0, i));
            else
                result.Append(name);

            if (playerId.HasValue)
            {
                result.Append(' ');
                result.Append('(');
                result.Append('#');
                result.Append(playerId);
                result.Append(')');
            }
            result.Append(' ');

            return result.ToString();
        }
    }
}
