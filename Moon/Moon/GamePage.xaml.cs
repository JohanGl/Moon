using System;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using MoonLib.Entities.Levels;

namespace Moon
{
    public partial class GamePage
    {
        private ContentManager contentManager;
		private GameTimer timer;
		private SpriteBatch spriteBatch;
    	private ILevel level;
		private IAudioHandler audioHandler;
		private DispatcherTimer levelCompletedTimer;

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

			levelCompletedTimer = new DispatcherTimer();
			levelCompletedTimer.Interval = TimeSpan.FromSeconds(2);

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
			audioHandler.LoadSound("Star1", "Audio/Star01");
			audioHandler.LoadSound("Star2", "Audio/Star02");
			audioHandler.LoadSound("Star3", "Audio/Star03");
			audioHandler.LoadSound("Star4", "Audio/Star04");
			audioHandler.LoadSound("Star5", "Audio/Star05");
			audioHandler.LoadSound("Star6", "Audio/Star06");
			audioHandler.LoadSound("Star7", "Audio/Star07");
			audioHandler.LoadSound("Star8", "Audio/Star08");
			audioHandler.LoadSound("Star9", "Audio/Star09");
			audioHandler.LoadSound("Star10", "Audio/Star10");
			audioHandler.MusicVolume = 0.33f;
			audioHandler.SoundVolume = 1f;
			audioHandler.PlaySong("BGM1", true);
		}

    	protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);

			// Initialize the level
			level = new Level01();
			level.Initialize(contentManager, audioHandler);

            // Start the timer
            timer.Start();

            base.OnNavigatedTo(e);
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
			if (level.Completed)
			{
				HandleLevelCompleted();
			}
			else
			{
				while (TouchPanel.IsGestureAvailable)
				{
					var gesture = TouchPanel.ReadGesture();

					switch (gesture.GestureType)
					{
						case GestureType.Flick:
							level.Player.SetVelocity(gesture.Delta * 0.0001f);
							break;
					}
				}
			}

			level.Update(e);

			audioHandler.Update(e);
        }

		private void HandleLevelCompleted()
		{
			if (!levelCompletedTimer.IsEnabled)
			{
				levelCompletedTimer.Tick += (o, args) =>
				{
					levelCompletedTimer.Stop();
					level.Reset();
					System.Diagnostics.Debug.WriteLine("A");
				};

				levelCompletedTimer.Start();
			}
		}

    	/// <summary>
        /// Allows the page to draw itself.
        /// </summary>
        private void OnDraw(object sender, GameTimerEventArgs e)
        {
        	var device = SharedGraphicsDeviceManager.Current.GraphicsDevice;

			spriteBatch.Begin();

			level.Draw(device, spriteBatch);

			spriteBatch.End();
        }
    }
}