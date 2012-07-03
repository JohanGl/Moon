using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using MoonLib.Contexts;
using MoonLib.Scenes;
using MoonLib.Scenes.Levels;
using MoonLib.Services;

namespace Moon
{
	public partial class GamePage
	{
		private IScene scene;
		private GameContext gameContext;
		private GameTimer timer;

		public GamePage()
		{
			InitializeComponent();

			gameContext = ServiceLocator.Get<GameContext>();

			// Create a timer for this page running at 30 fps
			timer = new GameTimer();
			timer.UpdateInterval = TimeSpan.FromTicks(33333);
			timer.Update += OnUpdate;
			timer.Draw += OnDraw;

			TouchPanel.EnabledGestures = GestureType.Flick | GestureType.Tap;

			InitializeAudio();
		}

		private void InitializeAudio()
		{
			FrameworkDispatcher.Update();
	
			var audioHandler = gameContext.AudioHandler;
			audioHandler.LoadSong("BGM1", "Audio/BGM01");

			for (int i = 1; i <= 16; i++)
			{
				audioHandler.LoadSound("Star" + i, string.Format("Audio/Star{0:00}", i));
			}

			audioHandler.LoadSound("IceStar", "Audio/IceStar");

			var settings = ServiceLocator.Get<GameContext>().Settings;

			if (settings.MusicVolume > 0.0d)
			{
				audioHandler.PlaySong("BGM1", true);
			}

			audioHandler.MusicVolume = (float)settings.MusicVolume;
			audioHandler.SoundVolume = (float)settings.SoundVolume;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			App.InitializeGamePageGraphics(gameContext);

			// Initialize the level scene
			scene = new LevelScene();
			scene.Initialize(gameContext);

			base.OnNavigatedTo(e);
			timer.Start();
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

		private void HandleSceneMessages()
		{
			if (scene.Messages.Count == 0)
			{
				return;
			}

			for (int i = 0; i < scene.Messages.Count; i++)
			{
				var message = scene.Messages[i];

				if (scene is LevelScene)
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