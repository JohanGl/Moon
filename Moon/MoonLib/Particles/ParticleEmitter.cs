using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MoonLib.Particles
{
	public class ParticleEmitter
	{
		public Vector2 Position { get; set; }
		public Particle[] Particles { get; set; }

		private ContentManager contentManager;
		private float timeScalar;
		private float timeScalarLife;
		private Random random;

		public ParticleEmitter(int particleCapacity, ContentManager contentManager)
		{
			Particles = new Particle[particleCapacity];
			this.contentManager = contentManager;
			random = new Random(DateTime.Now.Millisecond);
		}

		public void Emit()
		{
			for (int i = 0; i < Particles.Length; i++)
			{
				if (Particles[i].Life == 0f)
				{
					Particles[i].Life = 10f;
					Particles[i].LifeBurnout = 0.01f;
					Particles[i].Texture = contentManager.Load<Texture2D>("Effects/Particles/Default");
					Particles[i].Position = Position;
					Particles[i].Velocity = RandomCircularVelocity();
					break;
				}
			}
		}

		private Vector2 RandomCircularVelocity()
		{
			float angle = random.Next(360);
			float x = (float)Math.Sin(MathHelper.ToRadians(angle));
			float y = (float)Math.Cos(MathHelper.ToRadians(angle));

			return new Vector2(x, y);
		}

		public void Update(GameTimerEventArgs e)
		{
			timeScalar = (float)(e.ElapsedTime.TotalMilliseconds * 1d);
			timeScalarLife = (float)e.ElapsedTime.TotalMilliseconds;

			for (int i = 0; i < Particles.Length; i++)
			{
				// Skip inactive particles
				if (Particles[i].Life == 0f)
				{
					continue;
				}

				// Update the particle position and velocity slowdown
				Particles[i].Position += new Vector2(Particles[i].Velocity.X * timeScalar, Particles[i].Velocity.Y * timeScalar);
				Particles[i].Velocity *= 0.75f;

				// Decrease the lifespan of the particle
				Particles[i].Life -= (Particles[i].LifeBurnout * timeScalarLife);

				if (Particles[i].Life < 0f)
				{
					Particles[i].Life = 0;
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			for (int i = 0; i < Particles.Length; i++)
			{
				// Skip inactive particles
				if (Particles[i].Life == 0f)
				{
					continue;
				}

				spriteBatch.Draw(Particles[i].Texture, Particles[i].Position, Color.White);
			}
		}
	}
}