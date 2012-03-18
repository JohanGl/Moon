using System;
using Framework.Core.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Contexts;
using MoonLib.Helpers;

namespace MoonLib.Scenes.Levels
{
	public class ChallengeCompleted : Entity
	{
		private Vector2 target;
		private float timeScalar;
		private AnimationHandler<int> animationHandler;

		public bool IsAnimating
		{
			get
			{
				return (int)target.Y != -Texture.Height;
			}
		}

		public void Initialize(GameContext context)
		{
			Texture = context.Content.Load<Texture2D>("Scenes/Levels/ChallengeCompleted");
			HalfSize = new Vector2(Texture.Width / 2f, Texture.Height / 2f);

			animationHandler = new AnimationHandler<int>();
			animationHandler.Animations.Add(0, new Animation(0, 1, TimeSpan.FromSeconds(2.5)));

			Position = new Vector2((int)(Device.HalfWidth - HalfSize.X), -Texture.Height);
			target = Position;
		}

		public void Start()
		{
			Position = new Vector2((int)(Device.HalfWidth - HalfSize.X), -Texture.Height);
			target = new Vector2(Position.X, 0);
			animationHandler.Animations[0].Start();
		}

		public void Update(GameTimerEventArgs e)
		{
			animationHandler.Update();

			timeScalar = (float)e.ElapsedTime.TotalMilliseconds;

			var delta = new Vector2(0, (target.Y - Position.Y) * 0.01f);
			Position += delta * timeScalar;

			if (animationHandler.Animations[0].HasCompleted)
			{
				target = new Vector2(target.X, -Texture.Height);
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Texture, Position, Color.White);
		}
	}
}