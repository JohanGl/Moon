using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Contexts;
using MoonLib.Entities.Backgrounds;
using MoonLib.Helpers;
using MoonLib.IsolatedStorage;

namespace MoonLib.Scenes.Levels
{
	public class Level05 : ILevel
	{
		private StarHandler starHandler { get; set; }
		private Player Player { get; set; }
		private DefaultBackground background;
		private PlayerInfo playerInfo;
		private float movementAngle;
		private StorageHandler storage;
		private Texture2D rails;
		private bool challengeFailed;
		private bool checkChallenge;
		private int challengeBounceCount;

		private LevelInfo info;
		public LevelInfo Info
		{
			get
			{
				if (info == null)
				{
					info = new LevelInfo()
					{
						Id = 5001,
						LevelType = typeof(Level05),
						Name = "Level 5",
						Score = storage.GetLevelScore(5001),
						TexturePath = "Scenes/LevelSelect/Level05",
						Challenges =
						{
							new LevelChallenge()
							{
								Id = 5002,
								Name = "Dodgeball",
								Description = "Bounce against the top and bottom walls in one shot|without hitting any stars... Twice!",
								IsCompleted = storage.IsChallengeCompleted(5002)
							}
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

		public Level05()
		{
			storage = new StorageHandler();
		}

		public void Initialize(GameContext context)
		{
			// Initialize the background
			background = new DefaultBackground();
			background.Initialize(context);

			rails = context.Content.Load<Texture2D>("Scenes/Levels/Custom/Rails");

			starHandler = new StarHandler(context);

			Player = new Player();
			Player.Initialize(context);

			playerInfo = new PlayerInfo();
			playerInfo.Initialize(context, 4);

			Reset();
		}

		public void Reset()
		{
			challengeFailed = false;
			challengeBounceCount = 0;

			// Stars
			InitializeStars();

			// Player
			Player.Velocity = Vector2.Zero;
			EntityHelper.HorizontalAlign(Player, HorizontalAlignment.Center);
			EntityHelper.VerticalAlign(Player, VerticalAlignment.Bottom, 32);
		}

		private void InitializeStars()
		{
			starHandler.ResetStarPitch();
			starHandler.Stars.Clear();

			movementAngle = 0;

			for (int i = 0; i < 3; i++)
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

			var removedStars = starHandler.StarsRemovedThisUpdate;

			if (removedStars.Count > 0)
			{
				challengeFailed = true;
			}

			for (int i = 0; i < removedStars.Count; i++)
			{
				if (removedStars[i].Id >= 3)
				{
					continue;
				}

				starHandler.CreateStar(new Vector2(-32, -32), 0);
				starHandler.Stars[starHandler.Stars.Count - 1].Id = removedStars[i].Id + 3;

				if (Player.Center.X < Device.HalfWidth)
				{
					(starHandler.Stars[starHandler.Stars.Count - 1] as Entity).Tag = 0;
				}
				else
				{
					(starHandler.Stars[starHandler.Stars.Count - 1] as Entity).Tag = 180;
				}
			}

			CheckChallenges();
		}

		private void CheckChallenges()
		{
			if (challengeFailed || !checkChallenge)
			{
				return;
			}

			// First challenge
			if (!Info.Challenges[0].IsCompleted)
			{
				if (Player.IsAllowedToMove &&
					Player.BouncesDuringLastMove > 1 &&
					Player.HitTopWallDuringLastMove &&
					Player.HitBottomWallDuringLastMove)
				{
					challengeBounceCount++;
					checkChallenge = false;

					if (challengeBounceCount >= 2)
					{
						Info.Challenges[0].IsCompleted = true;
                        LevelSelectScene.SetLevelChallengeCompleted(Info.Challenges[0].Id);
					}
				}
			}
		}

		private void MoveStars(GameTimerEventArgs e)
		{
			var playerSpeed = Math.Min(0.3f, Player.Velocity.Length());
			movementAngle += playerSpeed * 3.5f;

			if (movementAngle >= 360f)
			{
				movementAngle -= 360f;
			}

			for (int i = 0; i < starHandler.Stars.Count; i++)
			{
				var star = starHandler.Stars[i];
				float x = 0;
				float y = 0;

				var entity = (Entity)star;

				if (star.Id == 0)
				{
					x = Device.HalfWidth + 185 * (float)Math.Cos(MathHelper.ToRadians(movementAngle));
					y = 58;
				}
				else if (star.Id == 1)
				{
					x = Device.HalfWidth + 185 * (float)Math.Cos(MathHelper.ToRadians(movementAngle + 180));
					y = 269;
				}
				else if (star.Id == 2)
				{
					x = Device.HalfWidth + 185 * (float)Math.Cos(MathHelper.ToRadians(movementAngle + 90));
					y = 480;
				}
				else
				{
					var angle = (float)Convert.ToDouble(entity.Tag);

					angle += playerSpeed * 3.5f;

					if (angle >= 360f)
					{
						angle -= 360f;
					}

					entity.Tag = angle;

					// New stars
					if (star.Id == 3)
					{
						x = Device.HalfWidth + 185 * (float)Math.Cos(MathHelper.ToRadians(angle));
						y = 58;
					}
					else if (star.Id == 4)
					{
						x = Device.HalfWidth + 185 * (float)Math.Cos(MathHelper.ToRadians(angle));
						y = 269;
					}
					else if (star.Id == 5)
					{
						x = Device.HalfWidth + 185 * (float)Math.Cos(MathHelper.ToRadians(angle));
						y = 480;
					}
				}

				entity.Position = new Vector2(x - 16, y - 16);
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			background.Draw(spriteBatch);
			spriteBatch.Draw(rails, Vector2.Zero, Color.White);
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
				checkChallenge = true;
			}
		}
	}
}