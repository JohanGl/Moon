using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MoonLib.Particles
{
	public class StarParticleEmitter : IParticleEmitter
	{
		public Vector2 Position { get; set; }
		private Particle[] particles { get; set; }

		private ContentManager contentManager;
		private float timeScalar;
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
			int total = 16;
			int counter = 0;

			float angle = random.Next(360);
			float angleStep = 360 / 15f;

			for (int i = 0; i < particles.Length; i++)
			{
				if (particles[i].Life == 0f)
				{
					particles[i].Life = 1f;
					particles[i].LifeBurnout = 0.00175f;
					particles[i].Texture = contentManager.Load<Texture2D>("Effects/particles/Default");
					particles[i].Position = Position;
					particles[i].Velocity = RandomCircularVelocity(angle);
					particles[i].Scale = 0.25f + (float)random.NextDouble();
					particles[i].Opacity = 1f;

					angle += angleStep;

					counter++;
					if (counter >= total)
					{
						break;
					}
				}
			}
		}

		private Vector2 RandomCircularVelocity(float angle)
		{
			float radius = 0.05f + ((float)random.NextDouble() * 0.12f);
			float x = (float)Math.Sin(MathHelper.ToRadians(angle)) * radius;
			float y = (float)Math.Cos(MathHelper.ToRadians(angle)) * radius;

			return new Vector2(x, y);
		}

		public void Update(GameTimerEventArgs e)
		{
			timeScalar = (float)(e.ElapsedTime.TotalMilliseconds * 1d);
			timeScalarLife = (float)e.ElapsedTime.TotalMilliseconds;

			for (int i = 0; i < particles.Length; i++)
			{
				// Skip inactive particles
				if (particles[i].Life == 0f)
				{
					continue;
				}

				// Update the particle position and velocity slowdown
				particles[i].Position += new Vector2(particles[i].Velocity.X * timeScalar, particles[i].Velocity.Y * timeScalar);
				particles[i].Velocity *= 0.991f;

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