using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Contexts;
using MoonLib.Entities.Backgrounds;
using MoonLib.Helpers;
using MoonLib.IsolatedStorage;

namespace MoonLib.Scenes.Levels
{
	public class Level04 : ILevel
	{
		private StarHandler starHandler { get; set; }
		private Player Player { get; set; }
		private DefaultBackground background;
		private PlayerInfo playerInfo;
		private float timeScalar;
		private float movementAngle;
		private int starsLeftBeforeLastMove;
		private StorageHandler storage;

		private LevelInfo info;
		public LevelInfo Info
		{
			get
			{
				if (info == null)
				{
					info = new LevelInfo()
					{
						Id = 4001,
						Name = "Level 4",
						Score = storage.GetLevelScore(4001),
						TexturePath = "Scenes/LevelSelect/Level04",
						Challenges =
						{
							new LevelChallenge()
							{
								Id = 4002,
								Name = "Mega combo",
								Description = "Get 5 or more stars in one shot",
								IsCompleted = storage.IsChallengeCompleted(4002)
							},
						}
					};
				}

				return info;
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
				return (Player.IsStationary && !playerInfo.GotMovesLeft);
			}
		}

		public int Score
		{
			get
			{
				return Math.Min(6, playerInfo.CalculateRating() + 1);
			}
		}

		public Level04()
		{
			storage = new StorageHandler();
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
			playerInfo.Initialize(context, 4);

			Reset();
		}

		public void Reset()
		{
			// Stars
			InitializeStars();

			// Player
			Player.Velocity = Vector2.Zero;
			EntityHelper.HorizontalAlign(Player, HorizontalAlignment.Center);
			EntityHelper.VerticalAlign(Player, VerticalAlignment.Center);
		}

		private void InitializeStars()
		{
			starHandler.ResetStarPitch();
			starHandler.Stars.Clear();

			movementAngle = 0;

			for (int i = 0; i < 6; i++)
			{
				starHandler.CreateStar(new Vector2(-32, -32), 0);
				starHandler.Stars[starHandler.Stars.Count - 1].Id = i;
			}
		}

		public void Update(GameTimerEventArgs e)
		{
			MoveStars(e);

			background.Update(e);
			Player.Update(e);
			starHandler.Update(e);

			// Remove stars that collide with the player
			starHandler.CheckPlayerCollisions(Player);

			CheckChallenges();
		}

		private void CheckChallenges()
		{
			if (!Player.IsAllowedToMove)
			{
				return;
			}

			// First challenge
			if (!Info.Challenges[0].IsCompleted)
			{
				if (starsLeftBeforeLastMove - starHandler.Stars.Count >= 5)
				{
					Info.Challenges[0].IsCompleted = true;
                    LevelSelectScene.SetLevelChallengeCompleted(Info.Challenges[0].Id);
				}
			}
		}

		private void MoveStars(GameTimerEventArgs e)
		{
			timeScalar = (float)(e.ElapsedTime.TotalMilliseconds * 0.05f);

			movementAngle += timeScalar;

			if (movementAngle >= 360f)
			{
				movementAngle -= 360f;
			}

			for (int i = 0; i < starHandler.Stars.Count; i++)
			{
				var star = starHandler.Stars[i];
				float x = 0;
				float y = 0;
				int margin = 40;
				int margin2 = 64;

				if (star.Id == 0)
				{
					x = margin;
					y = Device.HalfHeight + 270 * (float)Math.Sin(MathHelper.ToRadians(movementAngle));
				}
				else if (star.Id == 1)
				{
					x = Device.Width - margin;
					y = Device.HalfHeight + 270 * (float)Math.Sin(MathHelper.ToRadians(movementAngle + 180));
				}
				else if (star.Id == 2)
				{
					x = margin + margin2;
					y = Device.HalfHeight + 200 * (float)Math.Cos(MathHelper.ToRadians(movementAngle));
				}
				else if (star.Id == 3)
				{
					x = Device.Width - margin - margin2;
					y = Device.HalfHeight + 200 * (float)Math.Cos(MathHelper.ToRadians(movementAngle + 180));
				}
				else if (star.Id == 4)
				{
					x = Device.HalfWidth + 100 * (float)Math.Sin(MathHelper.ToRadians(movementAngle));
					y = margin;
				}
				else if (star.Id == 5)
				{
					x = Device.HalfWidth + 100 * (float)Math.Cos(MathHelper.ToRadians(movementAngle));
					y = Device.Height - margin;
				}
				
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
				starsLeftBeforeLastMove = starHandler.Stars.Count;
				Player.SetVelocity(velocity);
				playerInfo.Move();
			}
		}
	}
}