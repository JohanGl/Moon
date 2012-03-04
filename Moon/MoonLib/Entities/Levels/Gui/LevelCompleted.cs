using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Helpers;

namespace MoonLib.Entities.Levels
{
	public class LevelCompleted : Entity
	{
		private List<StarRating> starRatings;

		public void Initialize(ContentManager contentManager)
		{
			Texture = contentManager.Load<Texture2D>("Gui/LevelCompleted");

			starRatings = new List<StarRating>();

			Position = new Vector2((int)(Device.HalfWidth - HalfSize.X), (int)(Device.HalfHeight - HalfSize.Y));

			for (int i = 0; i < 3; i++)
			{
				var starRating = new StarRating();
				starRating.Initialize(contentManager);
				starRating.Position = Position + new Vector2(114, 68) + new Vector2(34 * i, 0);
				starRatings.Add(starRating);
			}

			starRatings[0].Rating = 2;
			starRatings[1].Rating = 1;
			starRatings[2].Rating = 0;
		}

		public void Update(GameTimerEventArgs e)
		{
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Texture, Position, Color.White);

			for (int i = 0; i < 3; i++)
			{
				starRatings[i].Draw(spriteBatch);
			}
		}
	}
}