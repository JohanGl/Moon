using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MoonLib
{
	public class Entity
	{
		public Vector2 Position { get; set; }
		public Vector2 Velocity { get; set; }
		public float Angle { get; set; }
		public float CollisionRadius { get; set; }

		public Vector2 HalfSize { get; set; }

		public Texture2D Texture;
	}
}