using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Contexts;
using MoonLib.Entities.Backgrounds;
using MoonLib.Entities.Items;
using MoonLib.Helpers;

namespace MoonLib.Scenes.Levels
{
	public class Level08 : ILevel
	{
		private StarHandler starHandler { get; set; }
		private BlackHole blackHole;
		private Player Player { get; set; }
		private DefaultBackground background;
		private PlayerInfo playerInfo;

		public LevelInfo Info
		{
			get
			{
				return new LevelInfo()
				{
					Name = "Level 8",
					TexturePath = "Scenes/LevelSelect/Level08",
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
			playerInfo.Initialize(context, 3);

			blackHole = new BlackHole();
			blackHole.Initialize(context);
			blackHole.Position = new Vector2(Device.HalfWidth, Device.HalfHeight);

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

			starHandler.CreateStar(new Vector2(Device.HalfWidth, Device.HalfHeight - 64), 0);
		}

		public void Update(GameTimerEventArgs e)
		{
			background.Update(e);
			blackHole.Update(e);
			Player.Update(e);
			starHandler.Update(e);

			// Remove stars that collide with the player
			starHandler.CheckPlayerCollisions(Player);
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
				Player.SetVelocity(velocity);
				playerInfo.Move();
			}
		}
	}
}