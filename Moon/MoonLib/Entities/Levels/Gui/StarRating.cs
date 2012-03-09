using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MoonLib.Entities.Levels
{
	public class StarRating : Entity
	{
		public bool IsVisible { get; set; }
		public int Rating { get; set; }

		public void Initialize(ContentManager contentManager)
		{
			Texture = contentManager.Load<Texture2D>("Gui/StarRating");
		}

		public void Update(GameTimerEventArgs e)
		{
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (!IsVisible)
			{
				return;
			}

			var destination = new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
			var source = new Rectangle(Rating * 32, 0, 32, 32);

			spriteBatch.Draw(Texture, destination, source, Color.White);
		}
	}
}