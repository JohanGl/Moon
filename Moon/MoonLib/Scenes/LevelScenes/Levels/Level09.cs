using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Contexts;
using MoonLib.Entities.Backgrounds;
using MoonLib.Entities.Items;
using MoonLib.Helpers;
using MoonLib.IsolatedStorage;

namespace MoonLib.Scenes.Levels
{
	public class Level09 : ILevel
	{
		private StarHandler starHandler { get; set; }
		private Player Player { get; set; }
		private DefaultBackground background;
		private PlayerInfo playerInfo;
		private DateTime levelStartTime;
		private StorageHandler storage;
		private Texture2D rail;
		private Vector2 railPosition;
		private float timeScalar;
		private bool missedStars;

		private LevelInfo info;
		public LevelInfo Info
		{
			get
			{
				if (info == null)
				{
					info = new LevelInfo()
					{
						Id = 9001,
						LevelType = typeof(Level09),
						Name = "Level 9",
						Score = storage.GetLevelScore(9001),
						TexturePath = "Scenes/LevelSelect/Level01",
						Challenges = new List<LevelChallenge>()
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
				return (Player.IsStationary && !playerInfo.GotMovesLeft) || missedStars;
			}
		}

		public int Score
		{
			get
			{
				return playerInfo.CalculateRating();
			}
		}

		public Level09()
		{
			storage = new StorageHandler();
		}

		public void Initialize(GameContext context)
		{
			// Initialize the background
			background = new DefaultBackground();
			background.Initialize(context);

			starHandler = new StarHandler(context);

			rail = context.Content.Load<Texture2D>("Scenes/Levels/Custom/Rail");

			Player = new Player();
			Player.Initialize(context);

			playerInfo = new PlayerInfo();
			playerInfo.Initialize(context, 10);

			Reset();

			railPosition = new Vector2(0, Player.Position.Y + 40 - 8);
		}

		public void Reset()
		{
			missedStars = false;

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

			int x = Device.HalfWidth / 2;
			for (int y = 0; y < 3; y++)
			{
				starHandler.CreateStar(new Vector2(x, (y * 64)), 0);
			}

			// Bridge
			x = Device.Width / 2;
			starHandler.CreateStar(new Vector2(x, 64 - 200), 0);
			starHandler.CreateStar(new Vector2(x - 96, 64 - 200), 0);
			starHandler.CreateStar(new Vector2(x + 96, 64 - 200), 0);

			x = Device.HalfWidth + (Device.HalfWidth / 2);
			for (int y = 0; y < 3; y++)
			{
				starHandler.CreateStar(new Vector2(x, (y * 64) - 400), 0);
			}

			// Bridge
			x = Device.Width / 2;
			starHandler.CreateStar(new Vector2(x, 64 - 600), 0);
			starHandler.CreateStar(new Vector2(x - 96, 64 - 600), 0);
			starHandler.CreateStar(new Vector2(x + 96, 64 - 600), 0);

			x = Device.HalfWidth / 2;
			for (int y = 0; y < 3; y++)
			{
				starHandler.CreateStar(new Vector2(x, (y * 64) - 800), 0);
			}

			for (int i = 0; i < starHandler.Stars.Count; i++)
			{
				(starHandler.Stars[i] as Star).OverrideBoundsCheck = true;
			}
		}

		public void Update(GameTimerEventArgs e)
		{
			background.Update(e);
			Player.Update(e);
			starHandler.Update(e);

			if (missedStars)
			{
				return;
			}

			timeScalar = (float)e.ElapsedTime.TotalMilliseconds;

			for (int i = 0; i < starHandler.Stars.Count; i++)
			{
				var star = (Star)starHandler.Stars[i];

				if (star.Position.Y < 900)
				{
					star.Position += new Vector2(0, 0.1f * timeScalar);
				}
				else
				{
					missedStars = true;
				}
			}

			// Remove stars that collide with the player
			starHandler.CheckPlayerCollisions(Player);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			background.Draw(spriteBatch);

			spriteBatch.Draw(rail, railPosition, Color.White);

			Player.Draw(spriteBatch);
			starHandler.Draw(spriteBatch);

			playerInfo.Draw(spriteBatch);
		}

		public void Move(Vector2 velocity)
		{
			velocity = new Vector2(velocity.X, 0);

			if (Player.IsAllowedToMove && playerInfo.GotMovesLeft)
			{
				Player.SetVelocity(velocity);
				playerInfo.Move();
			}
		}
	}
}