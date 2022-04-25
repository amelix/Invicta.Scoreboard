using ObsClassLibrary.Code;
using ObsClassLibrary.Code.Match;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ObsScoreHelper
{

    /// <summary>
    /// Interaction logic for FullPage.xaml
    /// </summary>
    public partial class FullPage : Window
    {
        private DispatcherTimer _timer;
        private CountdownHelper cowntdownHelper;

        public CountdownHelper CowntdownHelper
        {
            private get
            {
                return cowntdownHelper;
            }
            set
            {
                cowntdownHelper = value;
                cowntdownHelper.TimerTick += CowntdownHelper_TimerTick;
            }
        }

        public FullPage()
        {
            InitializeComponent();

            var tick = new TimeSpan(0, 0, 0, 34, 567);
            _timer = new DispatcherTimer(tick, DispatcherPriority.Normal, OnTimerTick, Application.Current.Dispatcher);

            //_timer.Start();
        }

        public bool SmallSize
        {
            get { return (bool)GetValue(SmallSizeProperty); }
            set { SetValue(SmallSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Visibily.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SmallSizeProperty =
            DependencyProperty.Register("SmallSize", typeof(bool), typeof(FullPage), new PropertyMetadata(true));

        private void Visibility_Lbl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SmallSize = true;
            _timer.Start();
        }

        private void InVisibility_Lbl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SmallSize = false;
            _timer.Stop();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            InVisibility_Lbl_MouseUp(null, null);
        }


        int round = 0;
        private void CowntdownHelper_TimerTick(object sender, CowntdownHelperEventArgs e)
        {
            if (cowntdownHelper.Running)
            {
                lblTimeH.Foreground = new SolidColorBrush(Colors.White);
                lblTimeDB.Foreground = new SolidColorBrush(Colors.White);
                lblTimeL.Foreground = new SolidColorBrush(Colors.White);

                lblTimeShortH.Foreground = new SolidColorBrush(Colors.White);
                lblTimeShortDB.Foreground = new SolidColorBrush(Colors.White);
                lblTimeShortL.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                lblTimeH.Foreground = new SolidColorBrush(Colors.Black);
                lblTimeDB.Foreground = new SolidColorBrush(Colors.Black);
                lblTimeL.Foreground = new SolidColorBrush(Colors.Black);

                lblTimeShortH.Foreground = new SolidColorBrush(Colors.Black);
                lblTimeShortDB.Foreground = new SolidColorBrush(Colors.Black);
                lblTimeShortL.Foreground = new SolidColorBrush(Colors.Black);
            }

            if (e.Minutes == 0 && e.Seconds == 0 && e.Milliseconds == 0)
            {
                lblTimeDB.Content = ":";
                lblTimeShortDB.Content = ":";
            }
            else
            {
                if (round % 10 > 4 || !cowntdownHelper.Running)
                {
                    lblTimeDB.Content = ":";
                    lblTimeShortDB.Content = ":";
                }
                else
                {
                    lblTimeDB.Content = ".";
                    lblTimeShortDB.Content = ".";
                }

                if (e.Minutes > 0)
                {
                    lblTimeH.Content = $"{e.Minutes:00}";
                    lblTimeL.Content = $"{e.Seconds:00}";

                    lblTimeShortH.Content = $"{e.Minutes:00}";
                    lblTimeShortL.Content = $"{e.Seconds:00}";
                }
                else
                {
                    lblTimeH.Content = $"{e.Seconds:00}";
                    lblTimeL.Content = $"{e.Milliseconds / 100:0}";

                    lblTimeShortH.Content = $"{e.Seconds:00}";
                    lblTimeShortL.Content = $"{e.Milliseconds / 100:0}";
                }
            }

            round++;
        }

        List<EventDetail> _eventDetailListHistory = new List<EventDetail>();

        public void AlignData(List<EventDetail> eventDetailList)
        {
            Title = $"OBSLiveScore";
            //Title = $"OBSLiveScore - {MatchDetail.Current.Home.Name} [{MatchDetail.Current.Home.Score}:{MatchDetail.Current.Away.Score}] {MatchDetail.Current.Away.Name}";
            lblNameShortH.Content = MatchDetail.Current.Home.NameShort;
            lblNameH.Content = MatchDetail.Current.Home.Name;
            lblResultShortH.Content = MatchDetail.Current.Home.Score.ToString();
            lblResultH.Content = MatchDetail.Current.Home.Score.ToString();
            imgHomePowerPlay.Visibility = MatchDetail.Current.Home.PowerPlay ? Visibility.Visible : Visibility.Collapsed;
            imgPowerPlayShortH.Visibility = MatchDetail.Current.Home.PowerPlay ? Visibility.Visible : Visibility.Collapsed;
            //txtHomeDesc.Text = MatchDetail.Current.Home.History;

            lblNameShortA.Content = MatchDetail.Current.Away.NameShort;
            lblNameA.Content = MatchDetail.Current.Away.Name;
            lblResultShortA.Content = MatchDetail.Current.Away.Score.ToString();
            lblResultA.Content = MatchDetail.Current.Away.Score.ToString();
            imgAwayPowerPlay.Visibility = MatchDetail.Current.Away.PowerPlay ? Visibility.Visible : Visibility.Collapsed;
            imgPowerPlayShortA.Visibility = MatchDetail.Current.Away.PowerPlay ? Visibility.Visible : Visibility.Collapsed;
            //txtAwayDesc.Text = MatchDetail.Current.Away.History;

            //if (FisrMatch != null && FisrMatch.Id.HasValue)
            //    txtMatchId.Text = FisrMatch.Id.ToString();
            //else
            //    txtMatchId.Text = "";

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

            if (eventDetailList != null && eventDetailList.Count != _eventDetailListHistory.Count)
            {
                txtDescriptionBorder.Visibility = Visibility.Visible;
                txtDescription.Visibility = Visibility.Visible;
                try
                {
                    var text = $"{MatchDetail.Current.Teams[eventDetailList[0].TeamCode.ToUpper()].Name} - {eventDetailList[0]}";
                    text = text.Replace('\n', ' ');
                    text = text.Replace("     ", " ");

                    _eventDetailListHistory = eventDetailList;
                    txtDescription.Text = text;
                }
                catch { }
            }
            else
            {
                txtDescriptionBorder.Visibility = Visibility.Collapsed;
                txtDescription.Visibility = Visibility.Collapsed;
                txtDescription.Text = "";
            }

            Visibility_Lbl_MouseUp(null, null);
        }
    }
}