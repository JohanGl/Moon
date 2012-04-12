using System;
using System.Windows.Navigation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using MoonLib.Contexts;
using MoonLib.Scenes;
using MoonLib.Services;

namespace Moon
{
	public partial class MainMenuPage
	{
		private IScene scene;
		private GameContext gameContext;
		private GameTimer timer;

		public MainMenuPage()
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

			//var audioHandler = gameContext.AudioHandler;
			//audioHandler.LoadSong("BGM2", "Audio/BGM02");
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			App.InitializeGamePageGraphics(gameContext);
			InitializeScene();
			base.OnNavigatedTo(e);
			timer.Start();

			var audioHandler = gameContext.AudioHandler;
			audioHandler.StopSong();
			//audioHandler.PlaySong("BGM2", true);
			audioHandler.MusicVolume = 1f;
			audioHandler.SoundVolume = 1f;
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