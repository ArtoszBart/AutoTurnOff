using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace AutoTurnOff
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Author: Bartosz Art
    /// Date: 2019
    /// </summary>
    public partial class MainWindow : Window
    {
        private int Hours;
        private int Minutes;
        private int Seconds;

        public MainWindow()
        {
            InitializeComponent();
            Button.Content = "Start";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int time = GetTime();
            if (time >= 0)
            {
                EnterTimeGrid.Visibility = Visibility.Collapsed;
                lblTime.Visibility = Visibility.Visible;

                Button.Content = "Cancel";
                Button.Click += (s, f) => Close();

                ChangeTimerLabel();
                SetTimer();

                Task.Factory.StartNew(() =>
                {
                    System.Threading.Thread.Sleep(time);
                    Process.Start("shutdown", "/s /t 0");
                });
            }
        }

        private void SetTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Seconds > 0)
            {
                Seconds--;
            }
            else
            {
                if (Minutes > 0)
                {
                    Minutes--;
                }
                else
                {
                    Hours--;
                    Minutes = 59;
                }
                Seconds = 59;
            }

            ChangeTimerLabel();
        }

        private void ChangeTimerLabel()
        {
            lblTime.Content = $"{Hours:00}:{Minutes:00}:{Seconds:00}";
        }

        private int GetTime()
        {
            if (!int.TryParse(HoursTextBox.Text, out Hours))
            {
                ShowError("hours");
                return -1;
            }
            if (!int.TryParse(MinutesTextBox.Text, out Minutes))
            {
                ShowError("minutes");
                return -1;
            }
            if (!int.TryParse(SecondsTextBox.Text, out Seconds))
            {
                ShowError("seconds");
                return -1;
            }

            return (Hours * 3600000) + (Minutes * 60000) + (Seconds * 1000);
        }

        private void ShowError(string WrongData)
        {
            MessageBox.Show("Invalid " + WrongData, "Error", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SelectAddress(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                tb.SelectAll();
            }
        }

        private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                if (!tb.IsKeyboardFocusWithin)
                {
                    e.Handled = true;
                    tb.Focus();
                }
            }
        }
    }
}
