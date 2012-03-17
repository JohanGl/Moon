using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Contexts;
using MoonLib.Entities.Backgrounds;
using MoonLib.Helpers;

namespace MoonLib.Scenes.Levels
{
	public class Level03 : ILevel
	{
		private StarHandler starHandler { get; set; }
		private Player Player { get; set; }
		private DefaultBackground background;
		private PlayerInfo playerInfo;
		private float timeScalar;
		private float movementAngle;

		public LevelInfo Info
		{
			get
			{
				return new LevelInfo()
				{
					Name = "Level 3",
					TexturePath = "Scenes/LevelSelect/Level03",
				};
			}
		}

		public bool Completed
		{
			get
			{
				return starHandler.Stars.Count == 0;
			}
		}

		public bool Failed
		{
			get
			{
				if (Player.IsStationary && starHandler.Stars.Count > 0)
				{
					for (int angle = 0; angle <= 360; angle += 10)
					{
						float x = Device.HalfWidth + 160 * (float)Math.Cos(MathHelper.ToRadians(angle));
						float y = (140 + 128) + 160 * (float)Math.Sin(MathHelper.ToRadians(angle));
						var starPosition = new Vector2(x, y);

						if (Vector2.Distance(Player.Center, starPosition) < (Player.CollisionRadius + (starHandler.Stars[0] as Entity).CollisionRadius))
						{
							return false;
						}
					}
				}

				return (Player.IsStationary && !playerInfo.GotMovesLeft);
			}
		}

		public int Score
		{
			get
			{
				return playerInfo.CalculateRating();
			}
		}

		public void Initialize(GameContext context)
		{
			// Initialize the background
			background = new DefaultBackground();
			background.Initialize(context);

			starHandler = new StarHandler(context);

			Player = new Player();
			Player.Initialize(context);

			playerInfo = new PlayerInfo();
			playerInfo.Initialize(context, 2);

			Reset();
		}

		public void Reset()
		{
			// Stars
			InitializeStars();

			// Player
			Player.Velocity = Vector2.Zero;
			EntityHelper.HorizontalAlign(Player, HorizontalAlignment.Center);
			EntityHelper.VerticalAlign(Player, VerticalAlignment.Bottom, 64);
		}

		private void InitializeStars()
		{
			starHandler.ResetStarPitch();
			starHandler.Stars.Clear();

			movementAngle = 0;

			for (int i = 0; i < 8; i++)
			{
				starHandler.CreateStar(new Vector2(-32, -32), 0);
				starHandler.Stars[starHandler.Stars.Count - 1].Id = (360 / 8) * i;
			}
		}

		public void Update(GameTimerEventArgs e)
		{
			rotateStars(e);

			background.Update(e);
			Player.Update(e);
			starHandler.Update(e);

			// Remove stars that collide with the player
			starHandler.CheckPlayerCollisions(Player);
		}

		private void rotateStars(GameTimerEventArgs e)
		{
			timeScalar = (float)(e.ElapsedTime.TotalMilliseconds * 0.075f);

			movementAngle += timeScalar;

			if (movementAngle >= 360f)
			{
				movementAngle -= 360f;
			}

			for (int i = 0; i < starHandler.Stars.Count; i++)
			{
				var star = starHandler.Stars[i];

				float x = Device.HalfWidth + 160 * (float)Math.Cos(MathHelper.ToRadians(movementAngle + star.Id));
				float y = (140 + 128) + 160 * (float)Math.Sin(MathHelper.ToRadians(movementAngle + star.Id));

				(star as Entity).Position = new Vector2(x - 16, y - 16);
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			background.Draw(spriteBatch);
			Player.Draw(spriteBatch);
			starHandler.Draw(spriteBatch);

			playerInfo.Draw(spriteBatch);
		}

		public void Move(Vector2 velocity)
		{
			if (Player.IsAllowedToMove && playerInfo.GotMovesLeft)
			{
				Player.SetVelocity(velocity);
				playerInfo.Move();
			}
		}
	}
}