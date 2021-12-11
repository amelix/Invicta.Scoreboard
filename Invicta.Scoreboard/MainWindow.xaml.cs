using Invicta.Scoreboard.Code;
using Invicta.Scoreboard.Code.Match;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Invicta.Scoreboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RisultatoGrande risultatoGrande;
        TimerWindow risultatoPiccolo;
        CountdownHelper cowntdownHelper;
        FisrMatchGrabber FisrMatch;

        public MainWindow()
        {
            InitializeComponent();

            cowntdownHelper = new CountdownHelper();
            //cowntdownHelper.Start(10, 0, 0);
            cowntdownHelper.TimerTick += CowntdownHelper_TimerTick;
            cowntdownHelper.SetTime(MatchDetail.Current.Minutes,
                MatchDetail.Current.Seconds,
                MatchDetail.Current.Milliseconds);

            txtMinutes.Text = MatchDetail.Current.Minutes.ToString();
            txtSeconds.Text = MatchDetail.Current.Seconds.ToString();
            txtMilliseconds.Text = MatchDetail.Current.Milliseconds.ToString();


            risultatoPiccolo = new TimerWindow();
            risultatoPiccolo.CowntdownHelper = cowntdownHelper;
            risultatoPiccolo.Show();

            risultatoGrande = new RisultatoGrande();
            risultatoGrande.CowntdownHelper = cowntdownHelper;
            risultatoGrande.Show();

            //btnAlignTeams_Click(btnAlignTeams, new RoutedEventArgs());

            AlignData();

            FisrMatch = new FisrMatchGrabber();
            FisrMatch.MatchUpdate += FisrMatch_MatchUpdate;
        }

        private void FisrMatch_MatchUpdate(object sender, EventArgs ea)
        {
            var homeEvents = ((List<EventDetail>)sender).Where(m => string.Compare(m.TeamCode, txtHomeShort.Text, true) == 0).ToList();
            var awayEvents = ((List<EventDetail>)sender).Where(m => string.Compare(m.TeamCode, txtAwayShort.Text, true) == 0).ToList();

            if (homeEvents.Count > 0)
            {
                int gol = 0;
                var s = new StringBuilder();
                foreach (var ev in homeEvents.OrderByDescending(e => e.Tempo).ThenBy(e => e.Minuto))
                {
                    if (ev.EventType == EventDetail.EventTypes.Gol)
                        gol++;

                    s.AppendLine(ev.ToString());
                }

                if (MatchDetail.Current.Home.Score < gol)
                    MatchDetail.Current.Home.Score = gol;

                MatchDetail.Current.Home.History = s.ToString();
            }

            if (awayEvents.Count > 0)
            {
                int gol = 0;
                var s = new StringBuilder();
                foreach (var ev in awayEvents.OrderByDescending(e => e.Tempo).ThenBy(e => e.Minuto))
                {
                    if (ev.EventType == EventDetail.EventTypes.Gol)
                        gol++;

                    s.AppendLine(ev.ToString());
                }

                if (MatchDetail.Current.Away.Score < gol)
                    MatchDetail.Current.Away.Score = gol;

                MatchDetail.Current.Away.History = s.ToString();
            }

            AlignData();
        }

        private void CowntdownHelper_TimerTick(object sender, CowntdownHelperEventArgs e)
        {
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
        //MessageBox.Show("Prova");
        //for (var i = 0; i < 10; i++)
        //{
        //    BitmapImage image = new BitmapImage();
        //    image.BeginInit();
        //    image.UriSource = new Uri($"pack://application:,,,/Style/Verdana/{i}.png");
        //    image.EndInit();

        //    imgHH2.Source = image; //Image.from ImageSource.( $"Style\\Verdana\\{i}.png";
        //    Thread.Sleep(1000);
        //}
        private void Pause()
        {
            if (cowntdownHelper.Running)
            {
                cowntdownHelper.Stop();
            }
            else
            {
                cowntdownHelper.Start(cowntdownHelper.Minutes, cowntdownHelper.Seconds, cowntdownHelper.Milliseconds);
            }

            MatchDetail.Current.Minutes = Convert.ToInt32(txtMinutes.Text);
            MatchDetail.Current.Seconds = Convert.ToInt32(txtSeconds.Text);
            MatchDetail.Current.Milliseconds = Convert.ToInt32(txtMilliseconds.Text);

            MatchDetail.Current.Save();

            FisrMatch.Id = Convert.ToInt32(txtMatchId.Text);
        }

        private void btnAlign_Click(object sender, RoutedEventArgs e)
        {
            if (!cowntdownHelper.Running)
            {
                cowntdownHelper.Start(
                    Convert.ToInt32(txtMinutes.Text),
                    Convert.ToInt32(txtSeconds.Text),
                    Convert.ToInt32(txtMilliseconds.Text));

                cowntdownHelper.Stop();
            }
            else
            {
                cowntdownHelper.Start(
                    Convert.ToInt32(txtMinutes.Text),
                    Convert.ToInt32(txtSeconds.Text),
                    Convert.ToInt32(txtMilliseconds.Text));
            }
            MatchDetail.Current.Minutes = Convert.ToInt32(txtMinutes.Text);
            MatchDetail.Current.Seconds = Convert.ToInt32(txtSeconds.Text);
            MatchDetail.Current.Milliseconds = Convert.ToInt32(txtMilliseconds.Text);

            MatchDetail.Current.MatchId = Convert.ToInt32(txtMatchId.Text);

            MatchDetail.Current.Save();

            FisrMatch.Id = MatchDetail.Current.MatchId;
        }


        private void btnAlignTeams_Click(object sender, RoutedEventArgs e)
        {
            MatchDetail.Current.Home.NameShort = txtHomeShort.Text;
            MatchDetail.Current.Home.Name = txtHomeLong.Text;
            MatchDetail.Current.Home.Score = Convert.ToInt32(txtHomeScore.Text);
            MatchDetail.Current.Home.PowerPlay = chkHomePowerPlay.IsChecked.Value;
            MatchDetail.Current.Home.History = txtHomeDesc.Text;

            MatchDetail.Current.Away.NameShort = txtAwayShort.Text;
            MatchDetail.Current.Away.Name = txtAwayLong.Text;
            MatchDetail.Current.Away.Score = Convert.ToInt32(txtAwayScore.Text);
            MatchDetail.Current.Away.PowerPlay = chkAwayPowerPlay.IsChecked.Value;
            MatchDetail.Current.Away.History = txtAwayDesc.Text;

            MatchDetail.Current.Minutes = Convert.ToInt32(txtMinutes.Text);
            MatchDetail.Current.Seconds = Convert.ToInt32(txtSeconds.Text);
            MatchDetail.Current.Milliseconds = Convert.ToInt32(txtMilliseconds.Text);

            MatchDetail.Current.MatchId = Convert.ToInt32(txtMatchId.Text);

            AlignData();

            MatchDetail.Current.Save();

            FisrMatch.Id = MatchDetail.Current.MatchId;
        }

        private void AlignData()
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

            risultatoGrande.HomeName = MatchDetail.Current.Home.Name;
            risultatoGrande.HomeScore = MatchDetail.Current.Home.Score;
            risultatoGrande.HomePowerPlay = MatchDetail.Current.Home.PowerPlay;
            risultatoGrande.HomeDesc = MatchDetail.Current.Home.History;

            risultatoGrande.AwayName = MatchDetail.Current.Away.Name;
            risultatoGrande.AwayScore = MatchDetail.Current.Away.Score;
            risultatoGrande.AwayPowerPlay = MatchDetail.Current.Away.PowerPlay;
            risultatoGrande.AwayDesc = MatchDetail.Current.Away.History;

            risultatoPiccolo.HomeName = MatchDetail.Current.Home.NameShort;
            risultatoPiccolo.HomeScore = MatchDetail.Current.Home.Score;
            risultatoPiccolo.HomePowerPlay = MatchDetail.Current.Home.PowerPlay;

            risultatoPiccolo.AwayName = MatchDetail.Current.Away.NameShort;
            risultatoPiccolo.AwayScore = MatchDetail.Current.Away.Score;
            risultatoPiccolo.AwayPowerPlay = MatchDetail.Current.Away.PowerPlay;

            txtMatchId.Text = MatchDetail.Current.MatchId.ToString();
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            cowntdownHelper.AddSeconds(1);
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            cowntdownHelper.AddSeconds(-1);
        }
    }
}
