using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Invicta.Scoreboard.Code
{
    public class CountdownHelper
    {
        private DateTime _endTime;
        private TimeSpan _timeLeft;
        private readonly DispatcherTimer _timer;

        public bool Running { get; private set; }
        public int Minutes { get; private set; }
        public int Seconds { get; private set; }
        public int Milliseconds { get; private set; }

        public CountdownHelper()
        {
            var tick = new TimeSpan(0, 0, 0, 0, 100);
            _timer = new DispatcherTimer(tick, DispatcherPriority.Normal, OnTimerTick, Application.Current.Dispatcher);

            Running = false;
        }

        public event EventHandler<CowntdownHelperEventArgs> TimerTick;

        public void Start(int minutes, int seconds, int milliseconds = 0)
        {
            SetTime(minutes, seconds, milliseconds);
            //_endTime = DateTime.Now.AddMinutes(minutes).AddSeconds(seconds).AddMilliseconds(milliseconds);

            //_timer.Start();
            Running = true;
        }

        public void Stop()
        {
            //_timer.Stop();
            Running = false;
        }

        public void AddSeconds(double value)
        {
            _endTime = _endTime.AddSeconds(value);
        }
        private void OnTimerTick(int minutes, int seconds, int milliseconds = 0)
        {
            TimerTick?.Invoke(this, new CowntdownHelperEventArgs() { Minutes = minutes, Seconds = seconds, Milliseconds = milliseconds });
        }

        void OnTimerTick(object sender, EventArgs e)
        {
            _timeLeft = _endTime - DateTime.Now;
            var tickE = new CowntdownHelperEventArgs();
            if (Running)
                if (_timeLeft.TotalMilliseconds > 0)
                {
                    if (Running)
                    {
                        Minutes = _timeLeft.Minutes;
                        Seconds = _timeLeft.Seconds;
                        Milliseconds = _timeLeft.Milliseconds;
                    }
                }
                else
                {
                    Minutes = 0;
                    Seconds = 0;
                    Milliseconds = 0;
                }

            tickE.Running = Running;
            tickE.Minutes = Minutes;
            tickE.Seconds = Seconds;
            tickE.Milliseconds = Milliseconds;


            TimerTick?.Invoke(this, tickE);

            //var labelText = "";

            //if (_timeLeft.TotalMilliseconds > 0)
            //{
            //    if (_timeLeft.TotalMinutes > 1)
            //        labelText = _timeLeft.ToString(@"mm\:ss");
            //    else
            //        labelText = _timeLeft.ToString(@"ss\:f");
            //}
            //else
            //{
            //    if (zero % 10 <= 2)
            //        labelText = "";
            //    else
            //        labelText = "00:0";
            //    //_timer.Stop();
            //    zero++;
            //} //_timeLeft = _timeLeft.Add(-tick);

            //if (zero % 10 == 0)
            //    using (StreamWriter writer = File.CreateText(@"C:\Hockey\Testi\01_Timer\Timer.txt"))
            //    {
            //        _ = writer.WriteAsync(labelText);
            //    }

            //if (_label != null)
            //    _label.Content = labelText;
        }

        public void SetTime(int minutes, int seconds, int milliseconds = 0)
        {
            Minutes = minutes;
            Seconds = seconds;
            Milliseconds = milliseconds > 0 ? milliseconds : milliseconds + 500;
            _endTime = DateTime.Now
                .AddMinutes(minutes)
                .AddSeconds(seconds)
                .AddMilliseconds(milliseconds);
        }
    }

    public class CowntdownHelperEventArgs : EventArgs
    {
        public bool Running { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public int Milliseconds { get; set; }
    }
}
