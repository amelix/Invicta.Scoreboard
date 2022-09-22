using ObsClassLibrary.Code;
using ObsClassLibrary.Code.Match;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ObsClassLibrary.Controls
{
    /// <summary>
    /// Interaction logic for MatchConfigurationControl.xaml
    /// </summary>
    public partial class MatchConfigurationControl : UserControl
    {
        CountdownHelper _cowntdownHelper => CountdownHelper.Current;
        FisrMatchGrabber FisrMatch;

        public event EventHandler<EventArgs> MatchUpdate;

        public MatchConfigurationControl()
        {
            InitializeComponent();

            _cowntdownHelper.TimerTick += CowntdownHelper_TimerTick;
            _cowntdownHelper.SetTime(
                MatchDetail.Current.Minutes,
                MatchDetail.Current.Seconds,
                MatchDetail.Current.Milliseconds);

            txtMinutes.Text = MatchDetail.Current.Minutes.ToString();
            txtSeconds.Text = MatchDetail.Current.Seconds.ToString();
            txtMilliseconds.Text = MatchDetail.Current.Milliseconds.ToString();

            //btnAlignTeams_Click(btnAlignTeams, new RoutedEventArgs());
            //File.WriteAllText(@"C:\Hockey\Testi\TestoScorrevole.txt", "");

            AlignData(null);

            FisrMatch = new FisrMatchGrabber();
            FisrMatch.MatchUpdate += (sender, eventArgs) => FisrMatch_MatchUpdate(sender!, eventArgs);
        }

        //bool _history = false;
        //DateTime _dateTimeHistory = DateTime.Now.AddDays(-1);
        List<EventDetail> _eventDetailListHistory = new List<EventDetail>();

        private void FisrMatch_MatchUpdate(object sender, EventArgs eventArgs)
        {
            if (string.IsNullOrWhiteSpace(txtMatchId.Text) && FisrMatch.Id.HasValue)
                txtMatchId.Text = FisrMatch.Id.Value.ToString();

            var eventDetailList = (List<EventDetail>)sender;
            //if (!_history && eventDetailList.Count != _eventDetailListHistory.Count)
            //{
            //    try
            //    {
            //        var text = $"{MatchDetail.Current.Teams[eventDetailList[0].TeamCode.ToUpper()].Name} - {eventDetailList[0]}";
            //        text = text.Replace('\n', ' ');

            //        File.WriteAllText(@"C:\Hockey\Testi\TestoScorrevole.txt", text.PadLeft(100 - text.Length, ' '));
            //        _dateTimeHistory = DateTime.Now.AddSeconds(20);
            //        _history = true;
            //        _eventDetailListHistory = eventDetailList;
            //    }
            //    catch { }
            //}
            MatchDetail.Current.Home.Name = FisrMatch.HomeTeamName;
            MatchDetail.Current.Away.Name = FisrMatch.AwayTeamName;

            var homeEvents = eventDetailList
                .Where(m => string.Compare(m.TeamCode, txtHomeShort.Text, true) == 0)
                .ToList();
            var awayEvents = eventDetailList
                .Where(m => string.Compare(m.TeamCode, txtAwayShort.Text, true) == 0)
                .ToList();

            if (homeEvents.Count > 0)
            {
                int gol = 0;
                var stringBuilder = new StringBuilder();
                foreach (var ev in homeEvents.OrderByDescending(e => e.Tempo).ThenBy(e => e.Minuto))
                {
                    if (ev.EventType == EventDetail.EventTypes.Gol)
                        gol++;

                    stringBuilder.AppendLine(ev.ToString());
                }

                if (MatchDetail.Current.Home.Score < gol)
                    MatchDetail.Current.Home.Score = gol;

                MatchDetail.Current.Home.History = stringBuilder.ToString();
            }

            if (awayEvents.Count > 0)
            {
                int gol = 0;
                var stringBuilder = new StringBuilder();
                foreach (var ev in awayEvents.OrderByDescending(e => e.Tempo).ThenBy(e => e.Minuto))
                {
                    if (ev.EventType == EventDetail.EventTypes.Goalkeeper)
                        continue;

                    if (ev.EventType == EventDetail.EventTypes.Gol)
                        gol++;

                    stringBuilder.AppendLine(ev.ToString());
                }

                if (MatchDetail.Current.Away.Score < gol)
                    MatchDetail.Current.Away.Score = gol;

                MatchDetail.Current.Away.History = stringBuilder.ToString();
            }

            AlignData(eventDetailList);
        }

        private void CowntdownHelper_TimerTick(object sender, CowntdownHelperEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMatchId.Text) && FisrMatch.Id.HasValue)
                txtMatchId.Text = FisrMatch.Id.Value.ToString();

            //if (_history && _dateTimeHistory < DateTime.Now)
            //{
            //    _history = false;
            //    File.WriteAllText(@"C:\Hockey\Testi\TestoScorrevole.txt", "");
            //}
            // Uses the Keyboard.GetKeyStates to determine if a key is down.
            // A bitwise AND operation is used in the comparison.
            // e is an instance of KeyEventArgs.
            if ((Keyboard.GetKeyStates(Key.P) & Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) > 0)
            {
                Pause();
            }

            if (e.Minutes > 0)
            {
                lblTime.Content = $"{e.Minutes:00}:{e.Seconds:00}";

                if (e.Running)
                {
                    txtMinutes.Text = e.Minutes.ToString();
                    txtSeconds.Text = e.Seconds.ToString();
                    txtMilliseconds.Text = "0";
                }
            }
            else
            {
                if (e.Minutes == 0 && e.Seconds == 0 && e.Milliseconds == 0)
                {
                    if (lblTime.Content.ToString() != "")
                    {
                        lblTime.Content = "";

                        if (e.Running)
                        {
                            txtMinutes.Text = "0";
                            txtSeconds.Text = "0";
                            txtMilliseconds.Text = "0";
                        }
                    }
                }
                else
                {
                    lblTime.Content = $"{e.Seconds:00}:{e.Milliseconds / 100:0}";

                    if (e.Running)
                    {
                        txtMinutes.Text = "0";
                        txtSeconds.Text = e.Seconds.ToString();
                        txtMilliseconds.Text = e.Milliseconds.ToString();
                    }
                }
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void lblTime_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Pause();
        }

        private void Pause()
        {
            if (_cowntdownHelper.Running)
            {
                _cowntdownHelper.Stop();
            }
            else
            {
                _cowntdownHelper.Start(_cowntdownHelper.Minutes, _cowntdownHelper.Seconds, _cowntdownHelper.Milliseconds);
            }

            MatchDetail.Current.Minutes = Convert.ToInt32(txtMinutes.Text);
            MatchDetail.Current.Seconds = Convert.ToInt32(txtSeconds.Text);
            MatchDetail.Current.Milliseconds = Convert.ToInt32(txtMilliseconds.Text);

            MatchDetail.Current.Save();
            if (string.IsNullOrWhiteSpace(txtMatchId.Text))
                FisrMatch.Id = null;
            else
                FisrMatch.Id = Convert.ToInt32(txtMatchId.Text);

            FisrMatch.HomeTeamCode = txtHomeShort.Text;
            FisrMatch.AwayTeamCode = txtAwayShort.Text;
        }

        private void btnAlign_Click(object sender, RoutedEventArgs e)
        {
            if (!_cowntdownHelper.Running)
            {
                _cowntdownHelper.Start(
                    Convert.ToInt32(txtMinutes.Text),
                    Convert.ToInt32(txtSeconds.Text),
                    Convert.ToInt32(txtMilliseconds.Text));

                _cowntdownHelper.Stop();
            }
            else
            {
                _cowntdownHelper.Start(
                    Convert.ToInt32(txtMinutes.Text),
                    Convert.ToInt32(txtSeconds.Text),
                    Convert.ToInt32(txtMilliseconds.Text));
            }
            MatchDetail.Current.Minutes = Convert.ToInt32(txtMinutes.Text);
            MatchDetail.Current.Seconds = Convert.ToInt32(txtSeconds.Text);
            MatchDetail.Current.Milliseconds = Convert.ToInt32(txtMilliseconds.Text);

            if (string.IsNullOrWhiteSpace(txtMatchId.Text))
            {
                MatchDetail.Current.MatchId = null;
                FisrMatch.Id = null;
            }
            else
            {
                MatchDetail.Current.MatchId = Convert.ToInt32(txtMatchId.Text);
                FisrMatch.Id = MatchDetail.Current.MatchId;
            }

            AlignData(null);

            MatchDetail.Current.Save();

            FisrMatch.HomeTeamCode = MatchDetail.Current.Home.NameShort;
            FisrMatch.AwayTeamCode = MatchDetail.Current.Away.NameShort;
        }


        private void btnAlignTeams_Click(object sender, RoutedEventArgs e)
        {
            MatchDetail.Current.Teams[txtHomeShort.Text] = MatchDetail.Current.Home;
            MatchDetail.Current.Home.NameShort = txtHomeShort.Text;
            MatchDetail.Current.Home.Name = txtHomeLong.Text;
            MatchDetail.Current.Home.Score = Convert.ToInt32(txtHomeScore.Text);
            MatchDetail.Current.Home.PowerPlay = chkHomePowerPlay.IsChecked.Value;
            MatchDetail.Current.Home.History = txtHomeDesc.Text;
            MatchDetail.Current.Home.LogoTeamUrl = FisrMatch.HomeTeamUrl;


            MatchDetail.Current.Teams[txtAwayShort.Text] = MatchDetail.Current.Away;
            MatchDetail.Current.Away.NameShort = txtAwayShort.Text;
            MatchDetail.Current.Away.Name = txtAwayLong.Text;
            MatchDetail.Current.Away.Score = Convert.ToInt32(txtAwayScore.Text);
            MatchDetail.Current.Away.PowerPlay = chkAwayPowerPlay.IsChecked.Value;
            MatchDetail.Current.Away.History = txtAwayDesc.Text;
            MatchDetail.Current.Away.LogoTeamUrl = FisrMatch.AwayTeamUrl;

            MatchDetail.Current.Minutes = Convert.ToInt32(txtMinutes.Text);
            MatchDetail.Current.Seconds = Convert.ToInt32(txtSeconds.Text);
            MatchDetail.Current.Milliseconds = Convert.ToInt32(txtMilliseconds.Text);

            if (string.IsNullOrWhiteSpace(txtMatchId.Text))
            {
                MatchDetail.Current.MatchId = null;
                FisrMatch.Id = null;
            }
            else
            {
                MatchDetail.Current.MatchId = Convert.ToInt32(txtMatchId.Text);
                FisrMatch.Id = MatchDetail.Current.MatchId;
            }

            AlignData(null);

            MatchDetail.Current.Save();

            FisrMatch.HomeTeamCode = MatchDetail.Current.Home.NameShort;
            FisrMatch.AwayTeamCode = MatchDetail.Current.Away.NameShort;

            FisrMatch.Reset();

            if (!string.IsNullOrWhiteSpace(MatchDetail.Current.Home.LogoTeamUrl))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(MatchDetail.Current.Home.LogoTeamUrl, UriKind.Absolute);
                bitmap.DecodePixelHeight = 40;
                bitmap.DecodePixelWidth = 40;
                bitmap.EndInit();

                imgHomeUrl.Source = bitmap;
            }

            if (!string.IsNullOrWhiteSpace(MatchDetail.Current.Away.LogoTeamUrl))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(MatchDetail.Current.Away.LogoTeamUrl, UriKind.Absolute);
                bitmap.DecodePixelHeight = 40;
                bitmap.DecodePixelWidth = 40;
                bitmap.EndInit();

                imgAwayUrl.Source = bitmap;
            }
        }

        private void AlignData(List<EventDetail> eventDetailList)
        {
            txtHomeShort.Text = MatchDetail.Current.Home.NameShort;
            txtHomeLong.Text = MatchDetail.Current.Home.Name;
            txtHomeScore.Text = MatchDetail.Current.Home.Score.ToString();
            chkHomePowerPlay.IsChecked = MatchDetail.Current.Home.PowerPlay;
            txtHomeDesc.Text = MatchDetail.Current.Home.History;

            txtAwayShort.Text = MatchDetail.Current.Away.NameShort;
            txtAwayLong.Text = MatchDetail.Current.Away.Name;
            txtAwayScore.Text = MatchDetail.Current.Away.Score.ToString();
            chkAwayPowerPlay.IsChecked = MatchDetail.Current.Away.PowerPlay;
            txtAwayDesc.Text = MatchDetail.Current.Away.History;

            if (FisrMatch != null && FisrMatch.Id.HasValue)
                txtMatchId.Text = FisrMatch.Id.ToString();
            else
                txtMatchId.Text = "";

            MatchUpdate?.Invoke(eventDetailList, null);
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            _cowntdownHelper.AddSeconds(1);
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            _cowntdownHelper.AddSeconds(-1);
        }

        private void txtMatchId_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _cowntdownHelper.Stop();
            _cowntdownHelper.SetTime(20, 0, 0);

            txtMatchId.Text = null;
            FisrMatch.Id = null;

            txtHomeScore.Text = "0";
            txtHomeDesc.Text = "";
            txtAwayScore.Text = "0";
            txtAwayDesc.Text = "";

            txtMinutes.Text = "20";
            txtSeconds.Text = "0";
            txtMilliseconds.Text = "0";
        }
    }
}
