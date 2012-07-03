using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using MoonLib.Contexts;
using MoonLib.Helpers;
using MoonLib.Scenes;
using MoonLib.Services;

namespace Moon
{
	public partial class MainMenuPage
	{
		private IScene scene;
		private GameContext gameContext;
		private GameTimer timer;
		private Popup popupAudioSettings;

		private static bool hasInitializedAudio;

		public MainMenuPage()
		{
			InitializeComponent();

			gameContext = ServiceLocator.Get<GameContext>();

			// Create a timer for this page running at 30 fps
			timer = new GameTimer();
			timer.UpdateInterval = TimeSpan.FromTicks(33333);
			timer.Update += OnUpdate;
			timer.Draw += OnDraw;

			TouchPanel.EnabledGestures = GestureType.Flick | GestureType.Tap;
			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			if (hasInitializedAudio)
			{
				return;
			}

			if (!MediaPlayer.GameHasControl)
			{
				timer.Stop();
				SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

				var settingsControl = new SettingsControl
				{
				    Width = Device.Width,
				    Height = Device.Height
				};

				settingsControl.Closed += (o, args) =>
				{
				    popupAudioSettings.IsOpen = false;
					hasInitializedAudio = true;
					SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);
					timer.Start();
				};

				popupAudioSettings = new Popup
				{
				    Child = settingsControl,
				    IsOpen = true
				};
			}
			else if (MediaPlayer.GameHasControl)
			{
				var settings = ServiceLocator.Get<GameContext>().Settings;
				settings.MusicVolume = 0.5f;
				settings.SoundVolume = 0.5f;
				
				hasInitializedAudio = true;
			}
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			if (NavigationService.CanGoBack)
			{
				while (NavigationService.RemoveBackEntry() != null)
				{
					NavigationService.RemoveBackEntry();
				}
			}

			App.InitializeGamePageGraphics(gameContext);
			InitializeScene();
			base.OnNavigatedTo(e);
			timer.Start();

			var settings = ServiceLocator.Get<GameContext>().Settings;
			var audioHandler = gameContext.AudioHandler;

			if (settings.MusicVolume > 0.0d)
			{
				audioHandler.StopSong();
				audioHandler.MusicVolume = (float)settings.MusicVolume;
				audioHandler.SoundVolume = (float)settings.SoundVolume;
			}
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			timer.Stop();
			scene.Unload();

			App.UnInitializeGamePageGraphics(gameContext);
			base.OnNavigatedFrom(e);
		}

		/// <summary>
		/// Allows the page to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		private void OnUpdate(object sender, GameTimerEventArgs e)
		{
			HandleSceneMessages();

			scene.Update(e);
			gameContext.AudioHandler.Update(e);
		}

		/// <summary>
		/// Allows the page to draw itself.
		/// </summary>
		private void OnDraw(object sender, GameTimerEventArgs e)
		{
			gameContext.SpriteBatch.Begin();
			scene.Draw(gameContext.SpriteBatch);
			gameContext.SpriteBatch.End();
		}

		private void InitializeScene()
		{
			scene = new MainMenuScene();
			scene.Initialize(gameContext);
		}

		private void HandleSceneMessages()
		{
			if (scene.Messages.Count == 0)
			{
				return;
			}

			for (int i = 0; i < scene.Messages.Count; i++)
			{
				var message = scene.Messages[i];

				if (scene is MainMenuScene)
				{
					if (message is TapMessage)
					{
						var uri = new Uri("/LevelSelectPage.xaml", UriKind.Relative);
						NavigationService.Navigate(uri);
					}
				}
			}
		}
	}
}