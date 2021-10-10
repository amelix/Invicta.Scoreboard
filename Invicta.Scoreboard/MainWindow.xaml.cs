﻿using Invicta.Scoreboard.Code;
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
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        int i = 0;
        CowntdownHelper cowntdownHelper;
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            i++;
            i = i % 10;

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri($"pack://application:,,,/Style/Verdana/{i}.png");
            image.EndInit();

            imgHH2.Source = image; //Image.from ImageSource.( $"Style\\Verdana\\{i}.png";
            //Thread.Sleep(1000);
        }
        public MainWindow()
        {
            InitializeComponent();

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            dispatcherTimer.Start();

            cowntdownHelper = new CowntdownHelper();
            cowntdownHelper.Start(1, 5, 10);
            cowntdownHelper.TimerTick += CowntdownHelper_TimerTick;
            var w = new TimerWindow();
            w.CowntdownHelper = cowntdownHelper;
            w.Show();
        }

        private void CowntdownHelper_TimerTick(object sender, CowntdownHelperEventArgs e)
        {
            if (e.Minutes > 0)
                lblTime.Content = $"{e.Minutes:00}:{e.Seconds:00}";
            else
            {
                if (e.Minutes == 0 && e.Seconds == 0 && e.Milliseconds == 0)
                {
                    lblTime.Content = "";
                }
                else
                {
                    lblTime.Content = $"{e.Seconds:00}:{e.Milliseconds / 100:0}";
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
    }
}
