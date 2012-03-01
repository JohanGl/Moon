using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MoonLib.Entities.Items
{
	public class Star : Entity
	{
		public void Initialize(ContentManager contentManager)
		{
			Texture = contentManager.Load<Texture2D>("Items/Star");
		}

		public void Update(GameTimerEventArgs e)
		{
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Texture, Position + HalfSize, null, Color.White, Angle, new Vector2(16, 16), 1f, SpriteEffects.None, 0f);
		}
	}
}