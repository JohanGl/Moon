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

		public Vector2 HalfSize
		{
			get
			{
				return new Vector2(Texture.Width / 2f, Texture.Height / 2f);
			}
		}

		public Texture2D Texture;
	}
}