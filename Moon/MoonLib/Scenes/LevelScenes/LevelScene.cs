using System;
using System.Collections.Generic;
using Framework.Core.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using MoonLib.Contexts;
using MoonLib.Helpers;

namespace MoonLib.Scenes.Levels
{
	public class LevelScene : IScene
	{
		private enum AnimationType
		{
			LevelCompletedFade,
			LevelCompletedPause,
			LevelFailedFade,
			LevelFailedPause
		}

		private int currentLevel = 1;
		private const int totalLevels = 4;

		private bool initializeLevelCompleted;
		private LevelCompleted levelCompleted;

		private bool initializeLevelFailed;
		private LevelFailed levelFailed;

		private bool tapToContinue;
		private Texture2D backgroundFadeOut;
		private AnimationHandler<AnimationType> animationHandler;

		private GameContext gameContext;

		public ILevel Level { get; set; }
		public List<ISceneMessage> Messages { get; set; }

		public LevelScene(int currentLevel)
		{
			Messages = new List<ISceneMessage>();
			this.currentLevel = currentLevel + 1;
		}

		public void Initialize(GameContext context)
		{
			gameContext = context;

			animationHandler = new AnimationHandler<AnimationType>();
			animationHandler.Animations.Add(AnimationType.LevelCompletedFade, new Animation(0f, 0.5f, TimeSpan.FromSeconds(1)));
			animationHandler.Animations.Add(AnimationType.LevelCompletedPause, new Animation(0f, 1f, TimeSpan.FromSeconds(1)));
			animationHandler.Animations.Add(AnimationType.LevelFailedFade, new Animation(0f, 0.5f, TimeSpan.FromSeconds(1)));
			animationHandler.Animations.Add(AnimationType.LevelFailedPause, new Animation(0f, 1f, TimeSpan.FromSeconds(1)));

			initializeLevelCompleted = true;
			initializeLevelFailed = true;

			// Load textures
			backgroundFadeOut = gameContext.Content.Load<Texture2D>("Backgrounds/BackgroundFade");

			// Initialize the level data
			levelCompleted = new LevelCompleted();
			levelCompleted.Initialize(context);

			levelFailed = new LevelFailed();
			levelFailed.Initialize(gameContext);

			LoadLevel();
		}

		public void Unload()
		{
		}

		public void Update(GameTimerEventArgs e)
		{
			if (Level.Completed)
			{
				HandleLevelCompleted(e);
			}
			else if (Level.Failed)
			{
				HandleLevelFailed(e);
			}
			else
			{
				while (TouchPanel.IsGestureAvailable)
				{
					var gesture = TouchPanel.ReadGesture();

					switch (gesture.GestureType)
					{
						case GestureType.Flick:
							Level.Move(gesture.Delta * 0.0001f);
							break;
					}
				}
			}

			Level.Update(e);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			Level.Draw(spriteBatch);

			if (Level.Completed)
			{
				DrawLevelCompleted(spriteBatch);
			}
			else if (Level.Failed)
			{
				DrawLevelFailed(spriteBatch);
			}
		}

		private void HandleLevelCompleted(GameTimerEventArgs e)
		{
			animationHandler.Update();
			levelCompleted.Update(e);

			if (initializeLevelCompleted)
			{
				animationHandler.Animations[AnimationType.LevelCompletedPause].Start();
				animationHandler.Animations[AnimationType.LevelCompletedFade].Start();
				levelCompleted.Show(Level.Score);
				initializeLevelCompleted = false;
				tapToContinue = false;
			}

			if (animationHandler.Animations[AnimationType.LevelCompletedPause].HasCompleted)
			{
				tapToContinue = true;
			}

			while (TouchPanel.IsGestureAvailable)
			{
				var gesture = TouchPanel.ReadGesture();

				switch (gesture.GestureType)
				{
					case GestureType.Tap:
						if (tapToContinue)
						{
							NextLevel();
							LoadLevel();
							initializeLevelCompleted = true;
						}
						break;
				}
			}
		}

		private void HandleLevelFailed(GameTimerEventArgs e)
		{
			animationHandler.Update();
			levelFailed.Update(e);

			if (initializeLevelFailed)
			{
				animationHandler.Animations[AnimationType.LevelFailedPause].Start();
				animationHandler.Animations[AnimationType.LevelFailedFade].Start();
				initializeLevelFailed = false;
				tapToContinue = false;
			}

			if (animationHandler.Animations[AnimationType.LevelFailedPause].HasCompleted)
			{
				tapToContinue = true;
			}

			while (TouchPanel.IsGestureAvailable)
			{
				var gesture = TouchPanel.ReadGesture();

				switch (gesture.GestureType)
				{
					case GestureType.Tap:
						if (tapToContinue)
						{
							LoadLevel();
							initializeLevelFailed = true;
						}
						break;
				}
			}
		}

		private void NextLevel()
		{
			currentLevel++;

			if (currentLevel > totalLevels)
			{
				currentLevel = 1;
			}

			//currentLevel = 3;
		}

		private void LoadLevel()
		{
			var type = Type.GetType(string.Format("MoonLib.Scenes.Levels.Level{0:00}, MoonLib", currentLevel));

			Level = (ILevel)Activator.CreateInstance(type);
			Level.Initialize(gameContext);

			tapToContinue = false;
		}

		private void DrawLevelCompleted(SpriteBatch spriteBatch)
		{
			float opacity = (animationHandler.Animations[AnimationType.LevelCompletedFade].IsRunning) ? animationHandler.Animations[AnimationType.LevelCompletedFade].CurrentValue : animationHandler.Animations[AnimationType.LevelCompletedFade].To;
			spriteBatch.Draw(backgroundFadeOut, Device.Size, null, Color.White * opacity);

			levelCompleted.Draw(spriteBatch);
		}

		private void DrawLevelFailed(SpriteBatch spriteBatch)
		{
			float opacity = (animationHandler.Animations[AnimationType.LevelFailedFade].IsRunning) ? animationHandler.Animations[AnimationType.LevelFailedFade].CurrentValue : animationHandler.Animations[AnimationType.LevelFailedFade].To;
			spriteBatch.Draw(backgroundFadeOut, Device.Size, null, Color.White * opacity);

			levelFailed.Draw(spriteBatch);
		}
	}
}