using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Contexts;
using MoonLib.Entities.Backgrounds;
using MoonLib.Entities.Items;
using MoonLib.Helpers;
using MoonLib.IsolatedStorage;

namespace MoonLib.Scenes.Levels
{
	public class Level08 : ILevel
	{
		private StarHandler starHandler { get; set; }
		private BlackHole[] blackHoles;
		private Player Player { get; set; }
		private DefaultBackground background;
		private PlayerInfo playerInfo;
		private StorageHandler storage;
		private int caughtStars;
		private double starCreationDelay;
		private bool gotBlackHolePenalty;
		private GameContext context;
		private float blackHoleAngle;

		private LevelInfo info;
		public LevelInfo Info
		{
			get
			{
				if (info == null)
				{
					info = new LevelInfo()
					{
						Id = 8001,
						Name = "Level 8",
						Score = storage.GetLevelScore(8001),
						TexturePath = "Scenes/LevelSelect/Level08",
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
				return caughtStars >= 6;
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

		public Level08()
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
			playerInfo.Initialize(context, 8);

			blackHoles = new BlackHole[2];
			blackHoles[0] = new BlackHole();
			blackHoles[0].Initialize(context);
			blackHoles[0].Position = new Vector2(32, 32);

			Reset();
		}

		public void Reset()
		{
			caughtStars = 0;
			starCreationDelay = 0;
			blackHoleAngle = 0;

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
			starHandler.CreateStar(new Vector2(Device.HalfWidth, Device.HalfWidth), 0);
		}

		public void Update(GameTimerEventArgs e)
		{
			background.Update(e);
			blackHoles[0].Update(e);
			Player.Update(e);
			starHandler.Update(e);

			float x = Device.HalfWidth + 200 * (float)Math.Sin(MathHelper.ToRadians(blackHoleAngle));
			float y = Device.HalfHeight + 200 * (float)Math.Cos(MathHelper.ToRadians(blackHoleAngle));
			blackHoles[0].Position = new Vector2(x, y);
			blackHoleAngle += (float)e.ElapsedTime.TotalMilliseconds * 0.033f;

			if (blackHoleAngle > 360f)
			{
				blackHoleAngle -= 360f;
			}

			CheckCreateNewStars(e);
			UpdateBlackHoleForceOnPlayer();

			// Remove stars that collide with the player
			starHandler.CheckPlayerCollisions(Player);
		}

		private void CheckCreateNewStars(GameTimerEventArgs e)
		{
			if (starHandler.Stars.Count == 0 && starCreationDelay == 0)
			{
				starCreationDelay = e.TotalTime.TotalMilliseconds + 750;
			}
			else if (starCreationDelay != 0 && e.TotalTime.TotalMilliseconds >= starCreationDelay)
			{
				starCreationDelay = 0;
				caughtStars++;

				float x = Device.HalfWidth + 200 * (float)Math.Sin(MathHelper.ToRadians(blackHoleAngle - 2));
				float y = Device.HalfHeight + 200 * (float)Math.Cos(MathHelper.ToRadians(blackHoleAngle - 2));

				switch (caughtStars)
				{
					case 1:
						starHandler.CreateStar(new Vector2(x, y), 0);
						SetStarVelocity();
						break;

					case 2:
						starHandler.CreateStar(new Vector2(x, y), 0);
						SetStarVelocity();
						break;

					case 3:
						starHandler.CreateStar(new Vector2(x, y), 0);
						SetStarVelocity();
						break;

					case 4:
						starHandler.CreateIceStar(new Vector2(x, y), 0, 1);
						SetStarVelocity();
						break;

					case 5:
						starHandler.CreateIceStar(new Vector2(x, y), 0, 1);
						SetStarVelocity();
						break;
				}
			}
		}

		private void SetStarVelocity()
		{
			var star = (Entity)starHandler.Stars[0];
			var velocity = new Vector2(Device.HalfWidth, Device.HalfHeight) - blackHoles[0].Position;
			velocity.Normalize();
			velocity *= 0.01f;
			star.Velocity = velocity;
		}

		private void UpdateBlackHoleForceOnPlayer()
		{
			if (Player.IsStationary)
			{
				return;
			}

			for (int i = 0; i < 1; i++)
			{
				var direction = blackHoles[i].Position - Player.Center;
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
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			background.Draw(spriteBatch);
			blackHoles[0].Draw(spriteBatch);
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