using System;
using System.Windows;
using System.Windows.Controls;
using MoonLib.Contexts;
using MoonLib.Services;

namespace Moon
{
	public partial class SettingsControl : UserControl
	{
		public event EventHandler Closed;

		public SettingsControl()
		{
			InitializeComponent();
		}

		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			double newValue = 1d;

			for (double i = 0; i <= 1d; i += 0.25d)
			{
				if (e.NewValue <= i)
				{
					newValue = i;
					break;
				}
			}

			(sender as Slider).Value = newValue;
		}

		private void ButtonOk_Click(object sender, RoutedEventArgs e)
		{
			var settings = ServiceLocator.Get<GameContext>().Settings;
			settings.MusicVolume = SliderMusicVolume.Value;
			settings.SoundVolume = SliderSoundVolume.Value;

			if (Closed != null)
			{
				Closed(this, EventArgs.Empty);
			}
		}
	}
}