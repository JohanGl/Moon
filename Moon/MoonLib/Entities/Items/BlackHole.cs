using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Contexts;

namespace MoonLib.Entities.Items
{
	public class BlackHole : Entity
	{
		private float timeScalar;
		private Rectangle sourceHole;
		private Rectangle sourceCorona1;
		private Rectangle sourceCorona2;
		private Vector2 center;

		public void Initialize(GameContext context)
		{
			Texture = context.Content.Load<Texture2D>("Items/BlackHole");
			HalfSize = new Vector2(Texture.Height / 2f, Texture.Height / 2f);

			sourceHole = new Rectangle(0, 0, Texture.Height, Texture.Height);
			sourceCorona1 = new Rectangle(Texture.Height, 0, Texture.Height, Texture.Height);
			sourceCorona2 = new Rectangle(Texture.Height * 2, 0, Texture.Height, Texture.Height);
			center = new Vector2(HalfSize.Y, HalfSize.Y);
		}

		public void Update(GameTimerEventArgs e)
		{
			// Calculate the time/movement scalar for this entity
			timeScalar = (float)e.ElapsedTime.TotalMilliseconds;

			Angle += 0.001f * timeScalar;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Texture, Position, sourceCorona1, Color.White, Angle, center, 1f, SpriteEffects.None, 0f);
			spriteBatch.Draw(Texture, Position, sourceCorona2, Color.White, -Angle, center, 1f, SpriteEffects.None, 0f);
			spriteBatch.Draw(Texture, Position, sourceHole, Color.White, 0, center, 1f, SpriteEffects.None, 0f);
		}
	}
}