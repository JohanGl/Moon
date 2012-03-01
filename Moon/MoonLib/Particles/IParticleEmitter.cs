using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MoonLib.Particles
{
	public interface IParticleEmitter
	{
		Vector2 Position { get; set; }

		void Initialize(ContentManager contentManager, int particleCapacity);
		void Emit();
		void Update(GameTimerEventArgs e);
		void Draw(SpriteBatch spriteBatch);
	}
}