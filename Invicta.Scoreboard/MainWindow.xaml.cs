using Invicta.Scoreboard.Code;
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

        public MainWindow()
        {
            InitializeComponent();
            
            cowntdownHelper = new CountdownHelper();
            //cowntdownHelper.Start(10, 0, 0);
            cowntdownHelper.TimerTick += CowntdownHelper_TimerTick;
            cowntdownHelper.SetTime(Match.Current.Minutes,
                Match.Current.Seconds,
                Match.Current.Milliseconds);

            txtMinutes.Text = Match.Current.Minutes.ToString();
            txtSeconds.Text = Match.Current.Seconds.ToString();
            txtMilliseconds.Text = Match.Current.Milliseconds.ToString();


            risultatoPiccolo = new TimerWindow();
            risultatoPiccolo.CowntdownHelper = cowntdownHelper;
            risultatoPiccolo.Show();

            risultatoGrande = new RisultatoGrande();
            risultatoGrande.CowntdownHelper = cowntdownHelper;
            risultatoGrande.Show();

            //btnAlignTeams_Click(btnAlignTeams, new RoutedEventArgs());

            AlignData();
        }

        private void CowntdownHelper_TimerTick(object sender, CowntdownHelperEventArgs e)
        {
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
            if (cowntdownHelper.Running)
            {
                cowntdownHelper.Stop();
            }
            else
            {
                cowntdownHelper.Start(cowntdownHelper.Minutes, cowntdownHelper.Seconds, cowntdownHelper.Milliseconds);
            }

            Match.Current.Minutes = Convert.ToInt32(txtMinutes.Text);
            Match.Current.Seconds = Convert.ToInt32(txtSeconds.Text);
            Match.Current.Milliseconds = Convert.ToInt32(txtMilliseconds.Text);
            
            Match.Current.Save();
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
            Match.Current.Minutes = Convert.ToInt32(txtMinutes.Text);
            Match.Current.Seconds = Convert.ToInt32(txtSeconds.Text);
            Match.Current.Milliseconds = Convert.ToInt32(txtMilliseconds.Text);

            Match.Current.Save();
        }


        private void btnAlignTeams_Click(object sender, RoutedEventArgs e)
        {
            Match.Current.Home.NameShort = txtHomeShort.Text;
            Match.Current.Home.Name = txtHomeLong.Text;
            Match.Current.Home.Score = Convert.ToInt32(txtHomeScore.Text);
            Match.Current.Home.PowerPlay = chkHomePowerPlay.IsChecked.Value;
            Match.Current.Home.History = txtHomeDesc.Text;

            Match.Current.Away.NameShort = txtAwayShort.Text;
            Match.Current.Away.Name = txtAwayLong.Text;
            Match.Current.Away.Score = Convert.ToInt32(txtAwayScore.Text);
            Match.Current.Away.PowerPlay = chkAwayPowerPlay.IsChecked.Value;
            Match.Current.Away.History = txtAwayDesc.Text;

            Match.Current.Minutes = Convert.ToInt32(txtMinutes.Text);
            Match.Current.Seconds = Convert.ToInt32(txtSeconds.Text);
            Match.Current.Milliseconds = Convert.ToInt32(txtMilliseconds.Text);
            
            AlignData();

            Match.Current.Save();
        }

        private void AlignData()
        {
            txtHomeShort.Text = Match.Current.Home.NameShort;
            txtHomeLong.Text = Match.Current.Home.Name;
            txtHomeScore.Text = Match.Current.Home.Score.ToString();
            chkHomePowerPlay.IsChecked = Match.Current.Home.PowerPlay;
            txtHomeDesc.Text = Match.Current.Home.History;

            txtAwayShort.Text = Match.Current.Away.NameShort;
            txtAwayLong.Text = Match.Current.Away.Name;
            txtAwayScore.Text = Match.Current.Away.Score.ToString();
            chkAwayPowerPlay.IsChecked = Match.Current.Away.PowerPlay;
            txtAwayDesc.Text = Match.Current.Away.History;

            risultatoGrande.HomeName = Match.Current.Home.Name;
            risultatoGrande.HomeScore = Match.Current.Home.Score;
            risultatoGrande.HomePowerPlay = Match.Current.Home.PowerPlay;
            risultatoGrande.HomeDesc = Match.Current.Home.History;

            risultatoGrande.AwayName = Match.Current.Away.Name;
            risultatoGrande.AwayScore = Match.Current.Away.Score;
            risultatoGrande.AwayPowerPlay = Match.Current.Away.PowerPlay;
            risultatoGrande.AwayDesc = Match.Current.Away.History;

            risultatoPiccolo.HomeName = Match.Current.Home.NameShort;
            risultatoPiccolo.HomeScore = Match.Current.Home.Score;
            risultatoPiccolo.HomePowerPlay = Match.Current.Home.PowerPlay;

            risultatoPiccolo.AwayName = Match.Current.Away.NameShort;
            risultatoPiccolo.AwayScore = Match.Current.Away.Score;
            risultatoPiccolo.AwayPowerPlay = Match.Current.Away.PowerPlay;
        }
    }
}
