using Invicta.Scoreboard.Code;
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

namespace Invicta.Scoreboard
{
    /// <summary>
    /// Interaction logic for TimerWindow.xaml
    /// </summary>
    public partial class TimerWindow : Window
    {
        public int HomeScore { set { lblHomeScore.Text = value.ToString(); } }
        public int AwayScore { set { lblAwayScore.Text = value.ToString(); } }
        public string HomeName { set { lblHomeName.Text = value; } }
        public string AwayName { set { lblAwayName.Text = value; } }

        public bool HomePowerPlay { set { imgHomePowerPlay.Visibility = value ? Visibility.Visible : Visibility.Collapsed; } }
        public bool AwayPowerPlay { set { imgAwayPowerPlay.Visibility = value ? Visibility.Visible : Visibility.Collapsed; } }

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

        int round = 0;

        private void CowntdownHelper_TimerTick(object sender, CowntdownHelperEventArgs e)
        {

            if (cowntdownHelper.Running)
            {
                lblTimeUP.Foreground = new SolidColorBrush(Colors.White);
                lblTimeDB.Foreground = new SolidColorBrush(Colors.White);
                lblTimeDOWN.Foreground = new SolidColorBrush(Colors.White);
                if (e.Minutes == 0 && e.Seconds == 0 && e.Milliseconds == 0)
                {
                    lblTimeDB.Text = ":";
                }
                else
                {
                    lblTimeDB.Text = (round % 10 > 4) ? ":" : "";

                    if (e.Minutes > 0)
                    {
                        lblTimeUP.Text = $"{e.Minutes:00}";
                        lblTimeDOWN.Text = $"{e.Seconds:00}";
                    }
                    else
                    {
                        lblTimeUP.Text = $"{e.Seconds:00}";
                        lblTimeDOWN.Text = $"{e.Milliseconds / 100:0}";
                    }
                }
            }
            else
            {
                lblTimeDB.Text = ":";
                lblTimeUP.Foreground = new SolidColorBrush(Colors.Red);
                lblTimeDB.Foreground = new SolidColorBrush(Colors.Red);
                lblTimeDOWN.Foreground = new SolidColorBrush(Colors.Red);

            }

            round++;
        }

        public TimerWindow()
        {
            InitializeComponent();

            //class2 = new CowntdownHelper(lblTime);

        }
    }
}
