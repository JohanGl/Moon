using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MoonLib.Particles
{
	public class StarSparkParticleEmitter : IParticleEmitter
	{
		public Vector2 Position { get; set; }
		private Particle[] particles { get; set; }

		private ContentManager contentManager;
		private float timeScalarLife;
		private Random random;

		public void Initialize(ContentManager contentManager, int particleCapacity)
		{
			particles = new Particle[particleCapacity];
			this.contentManager = contentManager;
			random = new Random(DateTime.Now.Millisecond);
		}

		public void Emit()
		{
			for (int i = 0; i < particles.Length; i++)
			{
				if (particles[i].Life == 0f)
				{
					particles[i].Life = 1f;
					particles[i].LifeBurnout = 0.01f;
					particles[i].Texture = contentManager.Load<Texture2D>("Effects/particles/Default");
					particles[i].Position = Position;
					particles[i].Velocity = Vector2.Zero;
					particles[i].Scale = 5f + (float)(random.NextDouble() * 3d);
					particles[i].Opacity = 1f;

					break;
				}
			}
		}

		public void Update(GameTimerEventArgs e)
		{
			timeScalarLife = (float)e.ElapsedTime.TotalMilliseconds;

			for (int i = 0; i < particles.Length; i++)
			{
				// Skip inactive particles
				if (particles[i].Life == 0f)
				{
					continue;
				}

				// Decrease the lifespan of the particle
				particles[i].Life -= (particles[i].LifeBurnout * timeScalarLife);

				particles[i].Opacity = particles[i].Life - 0.01f;

				if (particles[i].Life < 0f)
				{
					particles[i].Life = 0;
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			for (int i = 0; i < particles.Length; i++)
			{
				// Skip inactive particles
				if (particles[i].Life == 0f)
				{
					continue;
				}

				spriteBatch.Draw(particles[i].Texture, particles[i].Position, null, Color.White * particles[i].Opacity, 0, new Vector2(particles[i].Scale, particles[i].Scale), particles[i].Scale, SpriteEffects.None, 0);
			}
		}
	}
}