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
        private CowntdownHelper cowntdownHelper;

        public CowntdownHelper CowntdownHelper
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
                if (e.Minutes == 0 && e.Seconds == 0 && e.Milliseconds == 0)
                {
                    lblTime.Content = "";
                    lblTimeDB.Content = ":";
                }
                else
                {
                    if (round % 10 > 4)
                        lblTimeDB.Content = ":";
                    else
                        lblTimeDB.Content = "";

                    if (e.Minutes > 0)
                    {
                        lblTime.Content = $"{e.Minutes:00}:{e.Seconds:00}";

                        lblTimeUP.Content = $"{e.Minutes:00}";
                        lblTimeDOWN.Content = $"{e.Seconds:00}";
                    }
                    else
                    {
                        lblTime.Content = $"{e.Seconds:00}:{e.Milliseconds / 100:0}";
                        lblTimeUP.Content = $"{e.Seconds:00}";
                        lblTimeDOWN.Content = $"{e.Milliseconds / 100:0}";
                    }
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
