using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using MoonLib.Contexts;
using MoonLib.Helpers;

namespace MoonLib.Scenes
{
	public class MainMenuScene : IScene
	{
		private GameContext gameContext;
		private Texture2D background;
		private Texture2D yellowMoon;
		private Texture2D greenMoon;
		private Texture2D title;
		private Texture2D tapToPlay;

		private Vector2 yellowMoonPosition;
		private Vector2 greenMoonPosition;
		private Vector2 titlePosition;
		private Vector2 tapToPlayPosition;

		private Vector2 titleOffset;
		private Vector2 yellowMoonOffset;

		private double tapToPlayCounter;
		private bool showTapToPlay;

		private float angle = 320;

		public List<ISceneMessage> Messages { get; set; }

		public MainMenuScene()
		{
			Messages = new List<ISceneMessage>();
		}

		public void Initialize(GameContext context)
		{
			gameContext = context;

			background = context.Content.Load<Texture2D>("Backgrounds/Background01");
			yellowMoon = context.Content.Load<Texture2D>("Scenes/MainMenu/YellowMoon");
			greenMoon = context.Content.Load<Texture2D>("Scenes/MainMenu/GreenMoon");
			title = context.Content.Load<Texture2D>("Scenes/MainMenu/Title");
			tapToPlay = context.Content.Load<Texture2D>("Scenes/MainMenu/TapToPlay");

			yellowMoonPosition = new Vector2(-276, 287);
			greenMoonPosition = new Vector2(200, -47);
			titlePosition = new Vector2((Device.HalfWidth - (title.Width / 2)), 203);
			tapToPlayPosition = new Vector2(Device.HalfWidth - (tapToPlay.Width / 2), 718);
		}

		public void Unload()
		{
		}

		public void Update(GameTimerEventArgs e)
		{
			UpdateAnimations(e);
			UpdateTapToPlay(e);
			UpdateTouch();
		}

		private void UpdateAnimations(GameTimerEventArgs e)
		{
			angle += (float)(e.ElapsedTime.TotalMilliseconds * 0.01d);

			titleOffset = new Vector2(255f * (float)Math.Sin(MathHelper.ToRadians(angle * 2f)), 0);
			yellowMoonOffset = new Vector2(30f * (float)Math.Sin(MathHelper.ToRadians(angle * 2f)), 0);
		}

		private void UpdateTapToPlay(GameTimerEventArgs e)
		{
			tapToPlayCounter += e.ElapsedTime.TotalMilliseconds;

			if (tapToPlayCounter > 1600)
			{
				tapToPlayCounter = 0;
			}

			showTapToPlay = (tapToPlayCounter > 400);
		}

		private void UpdateTouch()
		{
			while (TouchPanel.IsGestureAvailable)
			{
				var gesture = TouchPanel.ReadGesture();

				switch (gesture.GestureType)
				{
					case GestureType.Tap:
						Messages.Add(new TapMessage());
						break;
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(background, Vector2.Zero, Color.White);
			spriteBatch.Draw(greenMoon, greenMoonPosition - yellowMoonOffset, Color.White);
			spriteBatch.Draw(title, titlePosition - titleOffset, Color.White);
			spriteBatch.Draw(yellowMoon, yellowMoonPosition + yellowMoonOffset, Color.White);

			if (showTapToPlay)
			{
				spriteBatch.Draw(tapToPlay, tapToPlayPosition, Color.White);
			}
		}
	}
}