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
		public StarHandler StarHandler { get; set; }

		private Player Player { get; set; }
		private DefaultBackground background;
		private PlayerInfo playerInfo;

		public bool Completed
		{
			get
			{
				return StarHandler.Stars.Count == 0;
			}
		}

		public void Initialize(ContentManager contentManager, IAudioHandler audioHandler)
		{
			// Initialize the background
			background = new DefaultBackground();
			background.Initialize(contentManager);

			StarHandler = new StarHandler(contentManager, audioHandler);

			Player = new Player();
			Player.Initialize(contentManager);

			playerInfo = new PlayerInfo();
			playerInfo.Initialize(contentManager, 3);

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
			StarHandler.ResetStarPitch();
			StarHandler.Stars.Clear();

			int x = Device.HalfWidth;
			for (int y = 0; y < 8; y++)
			{
				StarHandler.CreateStar(new Vector2(x, (y * 64) + 96), 0);
			}
		}

		public void Update(GameTimerEventArgs e)
		{
			background.Update(e);
			Player.Update(e);
			StarHandler.Update(e);

			// Remove stars that collide with the player
			StarHandler.CheckPlayerCollisions(Player);
		}

		public void Draw(GraphicsDevice device, SpriteBatch spriteBatch)
		{
			background.Draw(device, spriteBatch);
			Player.Draw(spriteBatch);
			StarHandler.Draw(spriteBatch);

			playerInfo.Draw(spriteBatch);
		}

		public void Move(Vector2 velocity)
		{
			Player.SetVelocity(velocity);
		}
	}
}