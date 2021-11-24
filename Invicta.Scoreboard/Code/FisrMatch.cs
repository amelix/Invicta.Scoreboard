using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Invicta.Scoreboard.Code
{
    public class FisrMatch
    {
        private DispatcherTimer _timer;
        public int? Id { get; set; }

        public FisrMatch()
        {
            //var tick = new TimeSpan(0, 0, 0, 1, 100);
            //_timer = new DispatcherTimer(tick, DispatcherPriority.Normal, OnTimerTick, Application.Current.Dispatcher);
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

        private async Task<string> GetPageAsync()
        {
            if (!Id.HasValue)
                return null;

            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"http://www.server2.sidgad.es/fisr/fisr_gr_{Id}_2.php"),
            };
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

                return body;
            }

        }
    }
}
