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

            risultatoPiccolo = new TimerWindow();
            risultatoPiccolo.CowntdownHelper = cowntdownHelper;
            risultatoPiccolo.Show();

            risultatoGrande = new RisultatoGrande();
            risultatoGrande.CowntdownHelper = cowntdownHelper;
            risultatoGrande.Show();

            btnAlignTeams_Click(btnAlignTeams, new RoutedEventArgs());
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
                cowntdownHelper.Stop();
            else
                cowntdownHelper.Start(cowntdownHelper.Minutes, cowntdownHelper.Seconds, cowntdownHelper.Milliseconds);
        }

        private void btnAlign_Click(object sender, RoutedEventArgs e)
        {
            cowntdownHelper.SetTime(
                Convert.ToInt32(txtMinutes.Text),
                Convert.ToInt32(txtSeconds.Text),
                Convert.ToInt32(txtMilliseconds.Text));
        }


        private void btnAlignTeams_Click(object sender, RoutedEventArgs e)
        {
            risultatoGrande.HomeName = txtHomeLong.Text;
            risultatoGrande.HomeScore = Convert.ToInt32(txtHomeScore.Text);
            risultatoGrande.HomePowerPlay = chkHomePowerPlay.IsChecked.Value;
            risultatoGrande.HomeDesc = txtHomeDesc.Text;

            risultatoGrande.AwayScore = Convert.ToInt32(txtAwayScore.Text);
            risultatoGrande.AwayName = txtAwayLong.Text;
            risultatoGrande.AwayPowerPlay = chkAwayPowerPlay.IsChecked.Value;
            risultatoGrande.AwayDesc = txtAwayDesc.Text;

            risultatoPiccolo.HomeName = txtHomeShort.Text;
            risultatoPiccolo.HomeScore = Convert.ToInt32(txtHomeScore.Text);
            risultatoPiccolo.HomePowerPlay = chkHomePowerPlay.IsChecked.Value;

            risultatoPiccolo.AwayScore = Convert.ToInt32(txtAwayScore.Text);
            risultatoPiccolo.AwayName = txtAwayShort.Text;
            risultatoPiccolo.AwayPowerPlay = chkAwayPowerPlay.IsChecked.Value;
        }
    }
}
