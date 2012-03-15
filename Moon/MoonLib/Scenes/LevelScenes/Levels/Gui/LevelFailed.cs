using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Contexts;
using MoonLib.Helpers;

namespace MoonLib.Scenes.Levels
{
	public class LevelFailed : Entity
	{
		public void Initialize(GameContext context)
		{
			Texture = context.Content.Load<Texture2D>("Scenes/Levels/LevelFailed");
			HalfSize = new Vector2(Texture.Width / 2f, Texture.Height / 2f);

			Position = new Vector2((int)(Device.HalfWidth - HalfSize.X), (int)(Device.HalfHeight - HalfSize.Y));
		}

		public void Update(GameTimerEventArgs e)
		{
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Texture, Position, Color.White);
		}
	}
}