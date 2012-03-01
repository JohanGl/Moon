using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Entities.Backgrounds;
using MoonLib.Helpers;

namespace MoonLib.Entities.Levels
{
	public class Level01 : ILevel
	{
		public Player Player { get; set; }

		private DefaultBackground background;
		private StarHandler starHandler;

		public void Initialize(ContentManager contentManager)
		{
			// Initialize the background
			background = new DefaultBackground();
			background.Initialize(contentManager);

			InitializeStars(contentManager);

			Player = new Player();
			Player.Initialize(contentManager);
			EntityHelper.HorizontalAlign(Player, HorizontalAlignment.Center);
			EntityHelper.VerticalAlign(Player, VerticalAlignment.Bottom, 64);
		}

		private void InitializeStars(ContentManager contentManager)
		{
			starHandler = new StarHandler(contentManager);

			int x = (480 / 2) - 16;
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

		public void Draw(GraphicsDevice device, SpriteBatch spriteBatch)
		{
			background.Draw(device, spriteBatch);
			starHandler.Draw(spriteBatch);
			Player.Draw(spriteBatch);
		}
	}
}