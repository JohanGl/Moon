using Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Entities.Backgrounds;
using MoonLib.Entities.Levels.Gui;
using MoonLib.Helpers;

namespace MoonLib.Entities.Levels
{
	public class Level01 : ILevel
	{
		private StarHandler starHandler { get; set; }

		private Player Player { get; set; }
		private DefaultBackground background;
		private PlayerInfo playerInfo;

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

		public void Initialize(ContentManager contentManager, IAudioHandler audioHandler)
		{
			// Initialize the background
			background = new DefaultBackground();
			background.Initialize(contentManager);

			starHandler = new StarHandler(contentManager, audioHandler);

			Player = new Player();
			Player.Initialize(contentManager);

			playerInfo = new PlayerInfo();
			playerInfo.Initialize(contentManager, 5);

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
			if (Player.IsStationary && playerInfo.GotMovesLeft)
			{
				Player.SetVelocity(velocity);
				playerInfo.Move();
			}
		}
	}
}