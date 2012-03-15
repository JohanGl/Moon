using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Contexts;
using MoonLib.Helpers;

namespace MoonLib.Entities.Items
{
	public class IceStar : Entity, IStar
	{
		public int Id { get; set; }
		public bool IsCracked { get; set; }

		private float timeScalar;
		private Rectangle source;
		private Rectangle sourceCracked;
		private Vector2 fixedHalfSize;

		public void Initialize(GameContext context, int type)
		{
			Texture = context.Content.Load<Texture2D>("Items/IceStar");
			HalfSize = new Vector2(Texture.Height / 2f, Texture.Height / 2f);
			fixedHalfSize = new Vector2((int)HalfSize.X, (int)HalfSize.Y);

			source = new Rectangle(Texture.Height * (type % 4), 0, Texture.Height, Texture.Height);
			sourceCracked = new Rectangle(source.X + 256, 0, source.Width, source.Height);

			IsCracked = false;
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
			// Keep the player within the bounds of the screen
			if (Position.X < 0)
			{
				Position += new Vector2(-Position.X, 0);
				Velocity = new Vector2(-Velocity.X, Velocity.Y);
			}
			else if (Position.X + Texture.Height > Device.Width)
			{
				Position -= new Vector2((Position.X + Texture.Height) - Device.Width, 0);
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
			if (IsCracked)
			{
				spriteBatch.Draw(Texture, Position + fixedHalfSize, sourceCracked, Color.White, Angle, fixedHalfSize, 1f, SpriteEffects.None, 0f);
			}
			else
			{
				spriteBatch.Draw(Texture, Position + fixedHalfSize, source, Color.White, Angle, fixedHalfSize, 1f, SpriteEffects.None, 0f);
			}
		}
	}
}