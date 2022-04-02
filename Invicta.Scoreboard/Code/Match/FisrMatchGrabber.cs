using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Invicta.Scoreboard.Code.Match
{
    public class FisrMatchGrabber
    {
        private int _events = 0;
        public event EventHandler<EventArgs> MatchUpdate;

        //this line create a new TextInfo based on en-US culture
        TextInfo textInfo = new CultureInfo("it-IT", false).TextInfo;

        private DispatcherTimer _timer;
        public int? Id { get; set; }
        public string HomeTeamCode { get; set; }
        public string AwayTeamCode { get; set; }

        public FisrMatchGrabber()
        {
            var tick = new TimeSpan(0, 0, 0, 5, 333);
            _timer = new DispatcherTimer(tick, DispatcherPriority.Normal, OnTimerTick, Application.Current.Dispatcher);

            _timer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            var pageTask = GetPageAsync();
            //pageTask.Wait();
            //if (!string.IsNullOrWhiteSpace(pageTask.Result))
            //{
            //    var page = pageTask.Result;
            //}
        }

        private async Task GetMatchId()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://www.server2.sidgad.es/fisr/fisr_mc_2.php"),
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var i = body.IndexOf($",\"{HomeTeamCode}\",\"{AwayTeamCode}\"");
                if (i > 20)
                {
                    i -= 20;
                    i = body.IndexOf("writeData", i);
                    if (i > 0)
                    {
                        i += 11;
                        var f = body.IndexOf("\"", i);
                        if (f > 0)
                        {
                            var matchId = body.Substring(i, f - i);
                            Id = Convert.ToInt32(matchId);
                        }
                    }
                }
            }
        }

        private async Task<string> GetPageAsync()
        {
            if (!Id.HasValue)
            {
                _ = GetMatchId();
                return null;
            }

            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"http://www.server2.sidgad.es/fisr/fisr_gr_{Id}_2.php"),
            };
            var details = new List<EventDetail>();
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine(body);

                var i = body.IndexOf("tabla_standard");
                var j = body.Substring(0, i).LastIndexOf("<table");
                body = body.Substring(j);
                i = body.IndexOf("</table>") + "</table>".Length;
                body = body.Substring(0, i);

                //System.IO.File.WriteAllText(@"C:\Temp\test.txt", body);
                string rows = "";
                while (i > 0)
                {
                    var tagStart = "<tr>";
                    var tagEnd = "</tr>";
                    var value = "";
                    i = body.IndexOf(tagStart);
                    if (i > 0)
                    {
                        j = body.IndexOf(tagEnd, i);
                        string row = "#x" + body.Substring(i + 4, j - i - 4) + "x#";
                        row = row
                            .Replace("\n", "")
                            .Replace("\t", "");
                        var detail = new EventDetail();
                        details.Add(detail);

                        #region Tempo
                        {
                            tagStart = "<div class=\"game_view_indcidencias_period\">";
                            tagEnd = "</div>";
                            value = GetTagValue(ref row, tagStart, tagEnd);
                            //rows += $"Tempo: {value}\n";
                            //row = row.Replace($"{tagStart}{value}{tagEnd}", "");
                            if (string.Compare(value, "p1", true) == 0)
                            {
                                detail.Tempo = EventDetail.TimeType.PrimoPeriodo;
                            }
                            else if (string.Compare(value, "p2", true) == 0)
                            {
                                detail.Tempo = EventDetail.TimeType.SecondoPeriodo;
                            }
                            else if (string.Compare(value, "p3", true) == 0)
                            {
                                detail.Tempo = EventDetail.TimeType.ExtraTime;
                            }
                            else if (string.Compare(value, "p4", true) == 0)
                            {
                                detail.Tempo = EventDetail.TimeType.Rigori;
                            }
                        }
                        #endregion
                        #region Minuto
                        {
                            tagStart = "<div class=\"game_view_incidencias_time\">";
                            tagEnd = "</div>";
                            value = GetTagValue(ref row, tagStart, tagEnd);
                            //rows += $"Minuto: {value}\n";
                            //row = row.Replace($"{tagStart}{value}{tagEnd}", "");
                            detail.Minuto = value;
                        }
                        #endregion
                        #region Team
                        {
                            tagStart = "<div class=\"texto_gris_11\" style=\"padding-top: 4px;\">";
                            tagEnd = "</div>";
                            value = GetTagValue(ref row, tagStart, tagEnd);
                            //rows += $"Team: {value}\n";
                            //row = row.Replace($"{tagStart}{value}{tagEnd}", "");
                            detail.TeamCode = value;
                        }
                        #endregion
                        #region Tipo Evento
                        {
                            tagStart = "<span class='lang_label lang_it'>";
                            tagEnd = "</span>";
                            value = GetTagValue(ref row, tagStart, tagEnd);
                            //rows += $"Action: {value}\n";
                            //row = row.Replace($"{tagStart}{value}{tagEnd}", "");
                            if (string.Compare(value, "Sostituzione Portiere", true) == 0)
                            {
                                detail.EventType = EventDetail.EventTypes.Goalkeeper;
                            }
                            else if (string.Compare(value, "gol", true) == 0)
                            {
                                detail.EventType = EventDetail.EventTypes.Gol;
                            }
                            else if (string.Compare(value, "Timeout", true) == 0)
                            {
                                detail.EventType = EventDetail.EventTypes.Timeout;
                            }
                            else if (string.Compare(value, "Falli", true) == 0)
                            {
                                detail.EventType = EventDetail.EventTypes.Fallo;
                            }
                            else
                            {

                            }
                        }
                        #endregion
                        #region Player Number
                        {
                            tagStart = "<div class=\"game_view_incidencias_dorsal\">";
                            tagEnd = "</div>";
                            value = GetTagValue(ref row, tagStart, tagEnd);
                            //rows += $"Player Number: {value}\n";
                            //row = row.Replace($"{tagStart}{value}{tagEnd}", "");
                            int playerId;
                            if (Int32.TryParse(value, out playerId))
                            {
                                detail.PlayerId = playerId;
                            }
                            //detail.PlayerId = Convert.ToInt32(value);
                        }
                        #endregion
                        #region Player Name
                        {
                            tagStart = "player_name = \"";
                            tagEnd = "\"";
                            value = GetTagValue(ref row, tagStart, tagEnd);
                            //rows += $"Player name: {value}\n";
                            //row = row.Replace($"{tagStart}{value}{tagEnd}", "");
                            detail.PlayerName = value;
                        }
                        #endregion
                        #region Assist Player Number
                        {
                            tagStart = "<span class='texto_gris_11'>#";
                            tagEnd = "</span>";
                            value = GetTagValue(ref row, tagStart, tagEnd);
                            rows += $"Assist Player Number: {value}\n";
                            //row = row.Replace($"{tagStart}{value}{tagEnd}", "");
                            int playerId;
                            if (Int32.TryParse(value, out playerId))
                            {
                                detail.AssistPlayerId = playerId;
                            }
                            //detail.PlayerId = Convert.ToInt32(value);
                        }
                        #endregion
                        #region Assist Player
                        {
                            tagStart = "player_name = \"";
                            tagEnd = "\"";
                            value = GetTagValue(ref row, tagStart, tagEnd);
                            rows += $"Assist Player name: {value}\n";
                            //row = row.Replace($"{tagStart}{value}{tagEnd}", "");
                            detail.AssistPlayerName = value;
                        }
                        #endregion

                        /*
                        tagStart = "<br> ";
                        tagEnd = "</div>";
                        value = GetTagValue(ref row, tagStart, tagEnd);
                        rows += $"Team Full 1: {value}\n";
                        //row = row.Replace($"{tagStart}{value}{tagEnd}", "");

                        tagStart = "</strong>";
                        tagEnd = "</div>";
                        value = GetTagValue(ref row, tagStart, tagEnd);
                        rows += $"Team Full 2: {value}\n";
                        //row = row.Replace($"{tagStart}{value}{tagEnd}", "");

                        //rows += row + "\n";
                        rows += "\n<< " + detail.ToString() + " >>\n";
                        */
                        body = body.Substring(j + tagEnd.Length);
                    }
                }

                if (_events != details.Count)
                {
                    _events = details.Count;
                    //System.IO.File.WriteAllText(@"C:\Temp\testClean.txt", rows);
                    MatchUpdate?.Invoke(details, new EventArgs());
                }

                return body;
            }
        }

        private string GetTagValue(ref string text, string tagStart, string tagEnd)
        {
            var result = "";
            var i = text.IndexOf(tagStart);
            if (i > 0)
            {
                var f = text.IndexOf(tagEnd, i + tagStart.Length);
                if (f > 0)
                {
                    result = text.Substring(i + tagStart.Length, f - i - tagStart.Length);

                    result = result.ToLower();
                    result = textInfo.ToTitleCase(result);
                    text = text.Substring(0, i) + text.Substring(f + tagEnd.Length);
                }
            }

            return result;
        }
    }
}
