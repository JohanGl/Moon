using Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Entities.Backgrounds;
using MoonLib.Helpers;

namespace MoonLib.Entities.Levels
{
	public class Level02 : ILevel
	{
		private StarHandler starHandler { get; set; }

		private Player Player { get; set; }
		private DefaultBackground background;

		public bool Completed
		{
			get
			{
				return starHandler.Stars.Count == 0;
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
		}

		public void Draw(GraphicsDevice device, SpriteBatch spriteBatch)
		{
			background.Draw(device, spriteBatch);
			Player.Draw(spriteBatch);
			starHandler.Draw(spriteBatch);
		}

		public void Move(Vector2 velocity)
		{
			Player.SetVelocity(velocity);
		}
	}
}