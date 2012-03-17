using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Contexts;

namespace MoonLib.Scenes.Levels
{
	public class StarRating : Entity
	{
		public bool IsVisible { get; set; }
		public int Rating { get; set; }

		public void Initialize(GameContext context)
		{
			Texture = context.Content.Load<Texture2D>("Scenes/Levels/StarRating");
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

		public void SetSingleStarRatingByIndex(int index, int rating)
		{
			int customRating = rating;

			if (index == 1)
			{
				customRating -= 2;
			}
			else if (index == 2)
			{
				customRating -= 4;
			}

			if (customRating >= 2)
			{
				Rating = 2;
			}
			else if (customRating < 0)
			{
				Rating = 0;
			}
			else
			{
				Rating = customRating;	
			}
		}
	}
}