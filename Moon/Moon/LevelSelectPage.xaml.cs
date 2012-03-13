using System;
using System.Windows.Navigation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using MoonLib.Contexts;
using MoonLib.Scenes;
using MoonLib.Services;

namespace Moon
{
	public partial class LevelSelectPage
	{
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

			TouchPanel.EnabledGestures = GestureType.Flick | GestureType.Tap;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			App.InitializeGamePageGraphics(gameContext);
			InitializeScene();
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