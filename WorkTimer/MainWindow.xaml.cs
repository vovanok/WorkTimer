using System;
using System.Timers;
using System.Windows;

namespace WorkTimer
{
	public partial class MainWindow : Window
	{
		private Timer timer = new Timer();
		private DateTime currentTime = new DateTime();
		private object timeLocker = new object();

		private DateTime CurrentTime
		{
			get
			{
				return currentTime;
			}
			set
			{
				lock(timeLocker)
				{
					currentTime = value;
					UpdateControls();
				}
			}
		}

		private bool IsTextBoxesUnderEdit
		{
			get
			{
				return tbHours.IsKeyboardFocused || tbMinutes.IsKeyboardFocused || tbSeconds.IsKeyboardFocused;
			}
		}

		public MainWindow()
		{
			InitializeComponent();
			timer.Interval = 1000;
			timer.Elapsed += (sender, args) =>
			{
				Dispatcher.Invoke(() =>
				{
					if (IsTextBoxesUnderEdit)
					{
						return;
					}

					CurrentTime = CurrentTime.AddMilliseconds((int)timer.Interval);
				});
			};
		}

		private void btnStart_Click(object sender, RoutedEventArgs e)
		{
			if(!timer.Enabled)
			{
				timer.Start();
			}
		}

		private void btnStop_Click(object sender, RoutedEventArgs e)
		{
			if(timer.Enabled)
			{
				timer.Stop();
			}
		}

		private void btnClear_Click(object sender, RoutedEventArgs e)
		{
			CurrentTime = new DateTime();
		}

		private void SetCurrentTimeComponents(int hour, int minute, int second)
		{
			hour = (0 <= hour) ? hour : CurrentTime.Hour;
			minute = (0 <= minute && minute < 60) ? minute : CurrentTime.Minute;
			second = (0 <= second && second < 60) ? second : CurrentTime.Second;

			CurrentTime = new DateTime(1, 1, 1, hour, minute, second);
		}

		private void UpdateControls()
		{
			tbHours.Text = CurrentTime.Hour.ToString("00");
			tbMinutes.Text = CurrentTime.Minute.ToString("00");
			tbSeconds.Text = CurrentTime.Second.ToString("00");
		}

		private void tb_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Enter)
			{
				int newHour;
				int newMinute;
				int newSecond;

				if (int.TryParse(tbHours.Text, out newHour) &&
					int.TryParse(tbMinutes.Text, out newMinute) &&
					int.TryParse(tbSeconds.Text, out newSecond))
				{
					SetCurrentTimeComponents(newHour, newMinute, newSecond);
				}

				btnStart.Focus();
            }
		}
	}
}
