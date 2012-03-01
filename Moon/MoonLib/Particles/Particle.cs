using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MoonLib.Particles
{
	public struct Particle
	{
		public Texture2D Texture;
		public Vector2 Position;
		public Vector2 Velocity;
		public float Angle;
		public float Life;
		public float LifeBurnout;
	}
}