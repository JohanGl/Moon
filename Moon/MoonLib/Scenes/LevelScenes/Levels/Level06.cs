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
	public class Level06 : ILevel
	{
		private StarHandler starHandler { get; set; }
		private Player Player { get; set; }
		private DefaultBackground background;
		private PlayerInfo playerInfo;
		private StorageHandler storage;

		private LevelInfo info;
		public LevelInfo Info
		{
			get
			{
				throw new NotImplementedException();
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

		public Level06()
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
			playerInfo.Initialize(context, 8);

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

			starHandler.CreateIceStar(new Vector2(Device.HalfWidth - 96, Device.HalfHeight - 128), 0);
			starHandler.CreateIceStar(new Vector2(Device.HalfWidth, Device.HalfHeight - 128), 0);
			starHandler.CreateIceStar(new Vector2(Device.HalfWidth + 96, Device.HalfHeight - 128), 0);
			starHandler.CreateIceStar(new Vector2(Device.HalfWidth, Device.HalfHeight - 256), 0);
		}

		public void Update(GameTimerEventArgs e)
		{
			background.Update(e);
			Player.Update(e);
			starHandler.Update(e);

			// Remove stars that collide with the player
			starHandler.CheckPlayerCollisions(Player);
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