using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Contexts;
using MoonLib.Helpers;

namespace MoonLib.Entities.Items
{
	public class Star : Entity, IStar
	{
		public int Id { get; set; }
		public bool OverrideBoundsCheck { get; set; }

		private float timeScalar;
		private Vector2 fixedHalfSize;

		public void Initialize(GameContext context)
		{
			Texture = context.Content.Load<Texture2D>("Items/Star");
			HalfSize = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
			fixedHalfSize = new Vector2((int)HalfSize.X, (int)HalfSize.Y);
		}

		public void Update(GameTimerEventArgs e)
		{
			// Calculate the time/movement scalar for this entity
			timeScalar = (float)(e.ElapsedTime.TotalMilliseconds * 8d);

			// Update the player position based on the current velocity
			Position += new Vector2(Velocity.X * timeScalar, Velocity.Y * timeScalar);

			// Decrease the velocity for a slowdown effect
			Velocity *= 0.99f;

			BoundsCheck();
		}

		private void BoundsCheck()
		{
			if (OverrideBoundsCheck)
			{
				return;
			}

			// Keep the player within the bounds of the screen
			if (Position.X < 0)
			{
				Position += new Vector2(-Position.X, 0);
				Velocity = new Vector2(-Velocity.X, Velocity.Y);
			}
			else if (Position.X + Texture.Width > Device.Width)
			{
				Position -= new Vector2((Position.X + Texture.Width) - Device.Width, 0);
				Velocity = new Vector2(-Velocity.X, Velocity.Y);
			}

			if (Position.Y < 0)
			{
				Position += new Vector2(0, -Position.Y);
				Velocity = new Vector2(Velocity.X, -Velocity.Y);
			}
			else if (Position.Y + Texture.Height > Device.Height)
			{
				Position -= new Vector2(0, (Position.Y + Texture.Height) - Device.Height);
				Velocity = new Vector2(Velocity.X, -Velocity.Y);
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Texture, Position + fixedHalfSize, null, Color.White, Angle, fixedHalfSize, 1f, SpriteEffects.None, 0f);
		}
	}
}