using Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Entities.Backgrounds;
using MoonLib.Entities.Items;
using MoonLib.Helpers;

namespace MoonLib.Entities.Levels
{
	public class Level04 : ILevel
	{
		public Player Player { get; set; }
		public StarHandler StarHandler { get; set; }
		private BlackHole blackHole;

		private DefaultBackground background;

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

			blackHole = new BlackHole();
			blackHole.Initialize(contentManager);
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
			StarHandler.ResetStarPitch();
			StarHandler.Stars.Clear();

			StarHandler.CreateStar(new Vector2(Device.HalfWidth, Device.HalfHeight - 64), 0);
		}

		public void Update(GameTimerEventArgs e)
		{
			background.Update(e);
			blackHole.Update(e);
			Player.Update(e);
			StarHandler.Update(e);

			// Remove stars that collide with the player
			StarHandler.CheckPlayerCollisions(Player);
		}

		public void Draw(GraphicsDevice device, SpriteBatch spriteBatch)
		{
			background.Draw(device, spriteBatch);
			blackHole.Draw(spriteBatch);
			Player.Draw(spriteBatch);
			StarHandler.Draw(spriteBatch);
		}
	}
}