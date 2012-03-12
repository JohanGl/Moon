using Microsoft.Xna.Framework;

namespace MoonLib.Helpers
{
	public static class EntityHelper
	{
		public static void Center(Entity entity)
		{
			var x = Device.HalfWidth - (entity.Texture.Width / 2);
			var y = Device.HalfHeight - (entity.Texture.Height / 2);

			entity.Position = new Vector2(x, y);
		}

		public static void HorizontalAlign(Entity entity, HorizontalAlignment alignment)
		{
			HorizontalAlign(entity, alignment, 0);
		}

		public static void HorizontalAlign(Entity entity, HorizontalAlignment alignment, float padding)
		{
			float x = 0;

			switch (alignment)
			{
				case HorizontalAlignment.Left:
					x = padding;
					break;

				case HorizontalAlignment.Center:
					x = Device.HalfWidth - (entity.Texture.Width / 2);
					break;

				case HorizontalAlignment.Right:
					x = Device.Width - entity.Texture.Width - padding;
					break;
			}

			entity.Position = new Vector2(x, entity.Position.Y);
		}

		public static void VerticalAlign(Entity entity, VerticalAlignment alignment)
		{
			VerticalAlign(entity, alignment, 0);
		}

		public static void VerticalAlign(Entity entity, VerticalAlignment alignment, float padding)
		{
			float y = 0;

			switch (alignment)
			{
				case VerticalAlignment.Top:
					y = padding;
					break;

				case VerticalAlignment.Center:
					y = Device.HalfHeight - (entity.Texture.Height / 2);
					break;

				case VerticalAlignment.Bottom:
					y = Device.Height - entity.Texture.Height - padding;
					break;
			}

			entity.Position = new Vector2(entity.Position.X, y);
		}

		public static bool Instersects(Entity a, Entity b)
		{
			return Vector2.Distance(a.Center, b.Center) < (a.CollisionRadius + b.CollisionRadius);
		}

		public static void ResolveCollisions(Entity a, Entity b)
		{
			Vector2 collision = a.Center - b.Center;
			float distance = collision.Length();

			if (distance == 0.0f)
			{
				// hack to avoid div by zero
				collision = new Vector2(1, 0);
				distance = 1;
			}
			else if (distance > (a.CollisionRadius + b.CollisionRadius))
			{
				return;
			}

			// Get the components of the velocity vectors which are parallel to the collision.
			// The perpendicular component remains the same for both fish
			collision = collision / distance;
			float aci = Vector2.Dot(a.Velocity, collision);
			float bci = Vector2.Dot(collision, a.Velocity);
			//double aci = a.Velocity.dot(collision);
			//double bci = b.Velocity.dot(collision);

			// Solve for the new velocities using the 1-dimensional elastic collision equations.
			// Turns out it's really simple when the masses are the same.
			float acf = bci;
			float bcf = aci;

			var velocityA = new Vector2((acf - aci) * collision.X, (acf - aci) * collision.Y);
			var velocityB = new Vector2((bcf - bci) * collision.X, (bcf - bci) * collision.Y);

			// Replace the collision velocity components with the new ones
			a.Velocity += velocityA;
			b.Velocity += velocityB;
			//a.Velocity += (acf - aci) * collision;
			//b.velocity() += (bcf - bci) * collision;
		}
	}

	public enum HorizontalAlignment
	{
		Left,
		Center,
		Right
	}

	public enum VerticalAlignment
	{
		Top,
		Center,
		Bottom
	}
}