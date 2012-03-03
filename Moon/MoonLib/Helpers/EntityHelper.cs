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
			return Vector2.Distance(a.Position + a.HalfSize, b.Position + b.HalfSize) < (a.CollisionRadius + b.CollisionRadius);
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