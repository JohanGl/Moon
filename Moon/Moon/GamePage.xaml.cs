using System;
using System.Windows;
using System.Windows.Navigation;
using Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using MoonLib.Scenes;

namespace Moon
{
	public partial class GamePage
	{
		private IScene scene;

		private ContentManager contentManager;
		private GameTimer timer;
		private SpriteBatch spriteBatch;
		private IAudioHandler audioHandler;

		public GamePage()
		{
			InitializeComponent();

			// Get the content manager from the application
			contentManager = (Application.Current as App).Content;

			// Create a timer for this page running at 30 fps
			timer = new GameTimer();
			timer.UpdateInterval = TimeSpan.FromTicks(33333);
			timer.Update += OnUpdate;
			timer.Draw += OnDraw;

			// Very important to display 32-bit colors instead of the default 16-bit which looks horrible with gradient images
			SharedGraphicsDeviceManager.Current.PreferredBackBufferFormat = SurfaceFormat.Color;

			TouchPanel.EnabledGestures = GestureType.Flick | GestureType.Tap;

			InitializeAudio();
		}

		private void InitializeAudio()
		{
			FrameworkDispatcher.Update();
			audioHandler = new DefaultAudioHandler(contentManager, "");
			audioHandler.LoadSong("BGM1", "Audio/BGM01");

			for (int i = 1; i <= 16; i++)
			{
				audioHandler.LoadSound("Star" + i, string.Format("Audio/Star{0:00}", i));				
			}
	
			audioHandler.LoadSound("IceStar", "Audio/IceStar");

			//audioHandler.PlaySong("BGM1", true);
			audioHandler.MusicVolume = 1f;
			audioHandler.SoundVolume = 0.75f;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			// Set the sharing mode of the graphics device to turn on XNA rendering
			SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);

			// Initialize the level scene
			//scene = new LevelScene();
			//scene.Initialize(contentManager, audioHandler);

			scene = new LevelSelectScene();
			scene.Initialize(contentManager, audioHandler);

			base.OnNavigatedTo(e);

			// Start the timer
			timer.Start();
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			// Stop the timer
			timer.Stop();

			// Set the sharing mode of the graphics device to turn off XNA rendering
			SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

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
			audioHandler.Update(e);
		}

		/// <summary>
		/// Allows the page to draw itself.
		/// </summary>
		private void OnDraw(object sender, GameTimerEventArgs e)
		{
			var device = SharedGraphicsDeviceManager.Current.GraphicsDevice;

			spriteBatch.Begin();
			scene.Draw(device, spriteBatch);
			spriteBatch.End();
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
						scene = new LevelScene((message as LevelSelectedMessage).LevelIndex);
						scene.Initialize(contentManager, audioHandler);

						return;
					}
				}
			}
		}
	}
}