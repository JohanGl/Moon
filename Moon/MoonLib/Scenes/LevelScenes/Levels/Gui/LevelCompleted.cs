using System;
using System.Collections.Generic;
using Framework.Core.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Contexts;
using MoonLib.Helpers;

namespace MoonLib.Scenes.Levels
{
	public class LevelCompleted : Entity
	{
		private int rating;
		private List<StarRating> starRatings;
		private AnimationHandler<int> animationHandler;
		private GameContext gameContext;
		
		public void Initialize(GameContext context)
		{
			gameContext = context;

			Texture = context.Content.Load<Texture2D>("Gui/LevelCompleted");
			HalfSize = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
			Position = new Vector2((int)(Device.HalfWidth - HalfSize.X), (int)(Device.HalfHeight - HalfSize.Y));

			starRatings = new List<StarRating>();

			for (int i = 0; i < 3; i++)
			{
				var starRating = new StarRating();
				starRating.Initialize(gameContext);
				starRating.Position = Position + new Vector2(114, 68) + new Vector2(34 * i, 0);
				starRatings.Add(starRating);
			}

			starRatings[0].Rating = 0;
			starRatings[1].Rating = 0;
			starRatings[2].Rating = 0;

			animationHandler = new AnimationHandler<int>();
		}

		public void Show(int rating)
		{
			this.rating = rating;

			for (int i = 0; i < 3; i++)
			{
				starRatings[i].Rating = 0;
				starRatings[i].IsVisible = false;
			}

			animationHandler.Animations.Clear();
			animationHandler.Animations.Add(0, new Animation(0, 1, TimeSpan.FromSeconds(0.5d)));
			animationHandler.Animations.Add(1, new Animation(0, 1, TimeSpan.FromSeconds(0.8d)));
			animationHandler.Animations.Add(2, new Animation(0, 1, TimeSpan.FromSeconds(1.1d)));

			animationHandler.Animations[0].Start();
			animationHandler.Animations[1].Start();
			animationHandler.Animations[2].Start();
		}

		public void Update(GameTimerEventArgs e)
		{
			animationHandler.Update();

			for (int i = 0; i < 3; i++)
			{
				if (animationHandler.Animations.ContainsKey(i) &&
					animationHandler.Animations[i].HasCompleted)
				{
					animationHandler.Animations.Remove(i);
					starRatings[i].Rating = GetSingleStarRatingByIndex(i);
					starRatings[i].IsVisible = true;
					gameContext.AudioHandler.PlaySound("Star" + (i + 1));
				}
			}
		}

		private int GetSingleStarRatingByIndex(int index)
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
				return 2;
			}
			else if (customRating < 0)
			{
				return 0;
			}

			return customRating;
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