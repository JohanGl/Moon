using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Helpers;

namespace MoonLib
{
	public class Player : Entity
	{
		private float timeScalar;

		public void Initialize(ContentManager contentManager)
		{
			Texture = contentManager.Load<Texture2D>("Moon");
			CollisionRadius = Texture.Width / 2f;
		}

		public void SetVelocity(Vector2 velocity)
		{
			if (Velocity.Length() < 0.01f)
			{
				Velocity = velocity;
			}
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
			else if (Position.X + Texture.Width > DeviceHelper.Width)
			{
				Position -= new Vector2((Position.X + Texture.Width) - DeviceHelper.Width, 0);
				Velocity = new Vector2(-Velocity.X, Velocity.Y);
			}

			if (Position.Y < 0)
			{
				Position += new Vector2(0, -Position.Y);
				Velocity = new Vector2(Velocity.X, -Velocity.Y);
			}
			else if (Position.Y + Texture.Height > DeviceHelper.Height)
			{
				Position -= new Vector2(0, (Position.Y + Texture.Height) - DeviceHelper.Height);
				Velocity = new Vector2(Velocity.X, -Velocity.Y);
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Texture, new Vector2((int)Position.X, (int)Position.Y), Color.White);
		}
	}
}