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
	public class Level07 : ILevel
	{
		private StarHandler starHandler { get; set; }
		private BlackHole blackHole;
		private Player Player { get; set; }
		private DefaultBackground background;
		private PlayerInfo playerInfo;
		private StorageHandler storage;
		private int caughtStars;
		private bool gotBlackHolePenalty;
		private GameContext context;

		private LevelInfo info;
		public LevelInfo Info
		{
			get
			{
				if (info == null)
				{
					info = new LevelInfo()
					{
						Id = 7001,
						Name = "Level 7",
						Score = storage.GetLevelScore(7001),
						TexturePath = "Scenes/LevelSelect/Level07",
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
				return caughtStars >= 5;
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
				return Math.Min(6, playerInfo.CalculateRating() + 2);
			}
		}

		public Level07()
		{
			storage = new StorageHandler();
		}

		public void Initialize(GameContext context)
		{
			this.context = context;

			// Initialize the background
			background = new DefaultBackground();
			background.Initialize(context);

			starHandler = new StarHandler(context);

			Player = new Player();
			Player.Initialize(context);

			playerInfo = new PlayerInfo();
			playerInfo.Initialize(context, 6);

			blackHole = new BlackHole();
			blackHole.Initialize(context);
			blackHole.Position = new Vector2(Device.HalfWidth, Device.HalfHeight);

			Reset();
		}

		public void Reset()
		{
			caughtStars = 0;

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

			starHandler.CreateStar(new Vector2(Device.HalfWidth, Device.HalfHeight - 265), 0);
		}

		public void Update(GameTimerEventArgs e)
		{
			background.Update(e);
			blackHole.Update(e);
			Player.Update(e);
			starHandler.Update(e);

			CheckCreateNewStars();
			UpdateBlackHoleForceOnPlayer();

			// Remove stars that collide with the player
			starHandler.CheckPlayerCollisions(Player);
		}

		private void CheckCreateNewStars()
		{
			if (starHandler.Stars.Count == 0)
			{
				caughtStars++;

				switch (caughtStars)
				{
					case 1:
						starHandler.CreateStar(new Vector2(Device.HalfWidth, Device.HalfHeight + 265), 0);
						break;

					case 2:
						starHandler.CreateStar(new Vector2(Device.HalfWidth + 64, Device.HalfHeight), 0);
						break;

					case 3:
						starHandler.CreateStar(new Vector2(Device.HalfWidth - 64, Device.HalfHeight), 0);
						break;

					case 4:
						starHandler.CreateStar(new Vector2(Device.HalfWidth, 64), 0);
						starHandler.CreateStar(new Vector2(Device.HalfWidth, Device.Height - 64), 0);
						break;
				}
			}
		}

		private void UpdateBlackHoleForceOnPlayer()
		{
			if (Player.IsStationary)
			{
				return;
			}

			var direction = blackHole.Position - Player.Center;
			var distance = direction.Length();

			// Penalty for getting too close to the black hole
			if (distance < 20 && !gotBlackHolePenalty)
			{
				gotBlackHolePenalty = true;
				context.AudioHandler.PlaySound("Star16");
				playerInfo.Move();
				Player.Shake();
			}

			// Calculate the force based on distance
			var force = 800 - Math.Min(800, distance);

			var playerForce = Player.Velocity.Length();
			if (playerForce < 0.025f)
			{
				Player.Velocity *= 0.99f;
			}

			// Dampen the distance effect
			force *= (playerForce * 0.00001f);

			direction.Normalize();

			Player.Velocity += direction * force;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			background.Draw(spriteBatch);
			blackHole.Draw(spriteBatch);
			Player.Draw(spriteBatch);
			starHandler.Draw(spriteBatch);

			playerInfo.Draw(spriteBatch);
		}

		public void Move(Vector2 velocity)
		{
			if (Player.IsAllowedToMove && playerInfo.GotMovesLeft)
			{
				gotBlackHolePenalty = false;
				Player.SetVelocity(velocity);
				playerInfo.Move();
			}
		}
	}
}