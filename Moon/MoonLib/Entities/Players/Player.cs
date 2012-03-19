using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Contexts;
using MoonLib.Helpers;

namespace MoonLib
{
	public class Player : Entity
	{
		private float timeScalar;

		public bool IsStationary
		{
			get
			{
				return Velocity.Length() < 0.0001f;
			}
		}

		public bool IsAllowedToMove
		{
			get
			{
				return Velocity.Length() < 0.01f;
			}
		}

		public int BouncesDuringLastMove { get; private set; }
		public bool HitTopWallDuringLastMove { get; private set; }
		public bool HitBottomWallDuringLastMove { get; private set; }

		public void Initialize(GameContext context)
		{
			Texture = context.Content.Load<Texture2D>("Player/Moon");
			HalfSize = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
			CollisionRadius = Texture.Width / 2f;
			ResetBounces();
		}

		private void ResetBounces()
		{
			BouncesDuringLastMove = 0;
			HitTopWallDuringLastMove = false;
			HitBottomWallDuringLastMove = false;
		}

		public void SetVelocity(Vector2 velocity)
		{
			Velocity = velocity;
			ResetBounces();
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
				BouncesDuringLastMove++;
			}
			else if (Position.X + Texture.Width > Device.Width)
			{
				Position -= new Vector2((Position.X + Texture.Width) - Device.Width, 0);
				Velocity = new Vector2(-Velocity.X, Velocity.Y);
				BouncesDuringLastMove++;
			}

			if (Position.Y < 0)
			{
				Position += new Vector2(0, -Position.Y);
				Velocity = new Vector2(Velocity.X, -Velocity.Y);
				BouncesDuringLastMove++;
				HitTopWallDuringLastMove = true;
			}
			else if (Position.Y + Texture.Height > Device.Height)
			{
				Position -= new Vector2(0, (Position.Y + Texture.Height) - Device.Height);
				Velocity = new Vector2(Velocity.X, -Velocity.Y);
				BouncesDuringLastMove++;
				HitBottomWallDuringLastMove = true;
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Texture, new Vector2((int)Position.X, (int)Position.Y), Color.White);
		}
	}
}