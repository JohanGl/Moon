using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Contexts;
using MoonLib.Entities.Backgrounds;
using MoonLib.Helpers;
using MoonLib.IsolatedStorage;

namespace MoonLib.Scenes.Levels
{
	public class Level02 : ILevel
	{
		private StarHandler starHandler { get; set; }
		private Player Player { get; set; }
		private DefaultBackground background;
		private PlayerInfo playerInfo;
		private StorageHandler storage;
		private bool hitWalls;

		private LevelInfo info;
		public LevelInfo Info
		{
			get
			{
				if (info == null)
				{
					info = new LevelInfo()
					{
						Id = 2001,
						LevelType = typeof(Level02),
						Name = "Level 2",
						Score = storage.GetLevelScore(2001),
						TexturePath = "Scenes/LevelSelect/Level02",
						Challenges =
						{
							new LevelChallenge()
							{
								Id = 2002,
								Name = "Super accuracy",
								Description = "Get all stars without hitting any walls",
								IsCompleted = storage.IsChallengeCompleted(2002)
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
				return playerInfo.CalculateRating();
			}
		}

		public Level02()
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
			EntityHelper.HorizontalAlign(Player, HorizontalAlignment.Left, 16);
			EntityHelper.VerticalAlign(Player, VerticalAlignment.Bottom, 16);
		}

		private void InitializeStars()
		{
			starHandler.ResetStarPitch();
			starHandler.Stars.Clear();

			float x = 90;
			float xStep = (Device.HalfWidth - x) / 5f;

			float y = 640f;
			float yStep = (y - 32f) / 5f;

			for (int i = 0; i < 5; i++)
			{
				starHandler.CreateStar(new Vector2(x, y), 0);

				x += xStep;
				y -= yStep;
			}

			for (int i = 0; i <= 5; i++)
			{
				starHandler.CreateStar(new Vector2(x, y), 0);

				x += xStep;
				y += yStep;
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
			if (Player.BouncesDuringLastMove > 0)
			{
				hitWalls = true;
			}

			if (Completed && !hitWalls)
			{
				// First challenge
				if (!Info.Challenges[0].IsCompleted)
				{
					Info.Challenges[0].IsCompleted = true;
                    LevelSelectScene.SetLevelChallengeCompleted(Info.Challenges[0].Id);
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