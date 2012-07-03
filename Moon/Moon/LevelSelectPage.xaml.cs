using System;
using System.ComponentModel;
using System.Windows.Navigation;
using Microsoft.Advertising;
using Microsoft.Advertising.Mobile.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using MoonLib.Contexts;
using MoonLib.Scenes;
using MoonLib.Services;

namespace Moon
{
	public partial class LevelSelectPage
	{
		private DrawableAd drawableAd;
		private IScene scene;
		private GameContext gameContext;
		private GameTimer timer;

		public LevelSelectPage()
		{
			InitializeComponent();

			gameContext = ServiceLocator.Get<GameContext>();

			// Create a timer for this page running at 30 fps
			timer = new GameTimer();
			timer.UpdateInterval = TimeSpan.FromTicks(33333);
			timer.Update += OnUpdate;
			timer.Draw += OnDraw;

			InitializeAudio();

			TouchPanel.EnabledGestures = GestureType.Flick | GestureType.Tap;
		}

		private void InitializeAudio()
		{
			FrameworkDispatcher.Update();

			var audioHandler = gameContext.AudioHandler;
			audioHandler.LoadSong("BGM2", "Audio/BGM02");
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			App.InitializeGamePageGraphics(gameContext);
			InitializeScene();
			base.OnNavigatedTo(e);
			timer.Start();

			// Initialize audio
			var settings = ServiceLocator.Get<GameContext>().Settings;

			var audioHandler = gameContext.AudioHandler;
			audioHandler.StopSong();

			if (settings.MusicVolume > 0.0d)
			{
				audioHandler.PlaySong("BGM2", true);
			}
	
			audioHandler.MusicVolume = (float)settings.MusicVolume;
			audioHandler.SoundVolume = (float)settings.SoundVolume;

			#if DEBUG
				AdComponent.Initialize("test_client");
				drawableAd = AdComponent.Current.CreateAd("Image480_80", new Rectangle(0, Device.Height - 80, 480, 80), true);
			#else
				AdComponent.Initialize("41af360b-5268-4cee-ac68-5825d51962c0");
				drawableAd = AdComponent.Current.CreateAd("90977", new Rectangle(0, 0, 480, 80), true);
			#endif

			drawableAd.AdRefreshed += DrawableAdOnAdRefreshed;
			drawableAd.ErrorOccurred += DrawableAdOnErrorOccurred;
		}

		private void DrawableAdOnErrorOccurred(object sender, AdErrorEventArgs adErrorEventArgs)
		{
		}

		private void DrawableAdOnAdRefreshed(object sender, EventArgs eventArgs)
		{
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			timer.Stop();
			scene.Unload();

			App.UnInitializeGamePageGraphics(gameContext);
			base.OnNavigatedFrom(e);
		}

		protected override void OnBackKeyPress(CancelEventArgs e)
		{
			base.OnBackKeyPress(e);

			var uri = new Uri("/MainMenuPage.xaml", UriKind.Relative);
			NavigationService.Navigate(uri);
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

			AdComponent.Current.Update(e.ElapsedTime);
		}

		/// <summary>
		/// Allows the page to draw itself.
		/// </summary>
		private void OnDraw(object sender, GameTimerEventArgs e)
		{
			gameContext.SpriteBatch.Begin();
			scene.Draw(gameContext.SpriteBatch);
			gameContext.SpriteBatch.End();

			AdComponent.Current.Draw();
		}

		private void InitializeScene()
		{
			scene = new LevelSelectScene();
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

				if (scene is LevelSelectScene)
				{
					if (message is LevelSelectedMessage)
					{
						int levelIndex = (message as LevelSelectedMessage).LevelIndex;
						var uri = new Uri(string.Format("/GamePage.xaml?level={0}", levelIndex), UriKind.Relative);
						NavigationService.Navigate(uri);
					}
				}
			}
		}
	}
}