using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Contexts;
using MoonLib.Entities.Backgrounds;
using MoonLib.Helpers;
using MoonLib.IsolatedStorage;

namespace MoonLib.Scenes.Levels
{
	public class Level01 : ILevel
	{
		private StarHandler starHandler { get; set; }
		private Player Player { get; set; }
		private DefaultBackground background;
		private PlayerInfo playerInfo;
		private DateTime levelStartTime;
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
						Id = 1001,
						Name = "Level 1",
						Score = storage.GetLevelScore(1001),
						TexturePath = "Scenes/LevelSelect/Level01",
						Challenges =
						{
							new LevelChallenge()
							{
								Id = 1001,
								Name = "Bouncer",
								Description = "Get all stars in one shot with at least two wall bounces",
								IsCompleted = storage.IsChallengeCompleted(1001)
							},
							new LevelChallenge()
							{
								Id = 1002,
								Name = "Speed king",
								Description = "Complete the level within 1 second",
								IsCompleted = storage.IsChallengeCompleted(1002)
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
				return playerInfo.CalculateRating();
			}
		}

		public Level01()
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
			playerInfo.Initialize(context, 5);

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

			levelStartTime = DateTime.Now;
		}

		private void InitializeStars()
		{
			starHandler.ResetStarPitch();
			starHandler.Stars.Clear();

			int x = Device.HalfWidth;
			for (int y = 0; y < 8; y++)
			{
				starHandler.CreateStar(new Vector2(x, (y * 64) + 96), 0);
			}
		}

		public void Update(GameTimerEventArgs e)
		{
			background.Update(e);
			Player.Update(e);
			starHandler.Update(e);

			// Remove stars that collide with the player
			starHandler.CheckPlayerCollisions(Player);

			CheckChallenges();
		}

		private void CheckChallenges()
		{
			if (!Completed)
			{
				return;
			}

			// First challenge
			if (!Info.Challenges[0].IsCompleted)
			{
				if (playerInfo.UsedMoves == 1 && Player.BouncesDuringLastMove >= 2)
				{
					LevelSelectScene.SetLevelChallengeCompleted(Info.Challenges[0].Id);
				}
			}

			// Second challenge
			if (!Info.Challenges[1].IsCompleted)
			{
				if ((DateTime.Now - levelStartTime).TotalSeconds <= 1)
				{
					LevelSelectScene.SetLevelChallengeCompleted(Info.Challenges[1].Id);
				}
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