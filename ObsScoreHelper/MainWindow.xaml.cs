using ObsClassLibrary.Code;
using ObsClassLibrary.Code.Match;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ObsScoreHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CountdownHelper _cowntdownHelper => CountdownHelper.Current;
        FullPage FullPage;

        /// <summary>Main Window.</summary>
        public MainWindow()
        {
            InitializeComponent();

            BringToForeground();
            //cowntdownHelper.TimerTick += CowntdownHelper_TimerTick;
            _cowntdownHelper.SetTime(
                MatchDetail.Current.Minutes,
                MatchDetail.Current.Seconds,
                MatchDetail.Current.Milliseconds);

            matchConfigurationControl.MatchUpdate += MatchConfigurationControl_MatchUpdate;
        }

        private void MatchConfigurationControl_MatchUpdate(object? sender, EventArgs e)
        {
            FullPage.AlignData((List<EventDetail>)sender);
        }

        /// <summary>Brings main window to foreground.</summary>
        public void BringToForeground()
        {
            if (this.WindowState == WindowState.Minimized || this.Visibility == Visibility.Hidden)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            }

            // According to some sources these steps gurantee that an app will be brought to foreground.
            Activate();
            Topmost = true;
            Topmost = false;
            Focus();

            (FullPage ??= new FullPage()).Show();
            //risultatoPiccolo.Show();
            //risultatoGrande.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FullPage.CowntdownHelper = _cowntdownHelper;
            FullPage.AlignData(null);
        }
    }
}
