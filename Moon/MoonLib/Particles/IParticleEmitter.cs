using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Contexts;

namespace MoonLib.Particles
{
	public interface IParticleEmitter
	{
		Vector2 Position { get; set; }

		void Initialize(GameContext context, int particleCapacity);
		void Emit();
		void Update(GameTimerEventArgs e);
		void Draw(SpriteBatch spriteBatch);
	}
}