using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Entities.Items;
using MoonLib.Helpers;
using MoonLib.Particles;

namespace MoonLib.Entities.Levels
{
	public class StarHandler
	{
		public List<Star> Stars;

		private float timeScalar;
		private float currentStarAngle;
		private List<Star> removal;
		private ParticleEmitter emitter;
		private ContentManager contentManager;

		public StarHandler(ContentManager contentManager)
		{
			this.contentManager = contentManager;

			Stars = new List<Star>();
			removal = new List<Star>();
			emitter = new ParticleEmitter(10, contentManager);
		}

		public void CheckPlayerCollisions(Player player)
		{
			removal.Clear();

			for (int i = 0; i < Stars.Count; i++)
			{
				if (EntityHelper.Instersects(player, Stars[i]))
				{
					removal.Add(Stars[i]);
				}
			}

			for (int i = 0; i < removal.Count; i++)
			{
				Stars.Remove(removal[i]);

				emitter.Position = removal[i].Position;
				emitter.Emit();
			}
		}

		public void CreateStar(Vector2 position, float angle)
		{
			var star = new Star();
			star.Initialize(contentManager);
			star.Position = position;
			star.Angle = angle;
			star.CollisionRadius = 12;

			Stars.Add(star);
		}

		public void Update(GameTimerEventArgs e)
		{
			// Calculate the time/movement scalar for this entity
			timeScalar = (float)e.ElapsedTime.TotalMilliseconds;

			// Calculate the angle once and use it for all stars (the last multiplier limits how widely the stars rotate)
			float starAngle = (float)Math.Sin(MathHelper.ToRadians(currentStarAngle)) * 0.6f;

			for (int i = 0; i < Stars.Count; i++)
			{
				Stars[i].Angle = starAngle;
				Stars[i].Update(e);
			}

			// Update the current stars angle for the next update
			currentStarAngle += 0.1f * timeScalar;

			if (currentStarAngle >= 360f)
			{
				currentStarAngle -= 360f;
			}

			UpdateParticles(e);
		}

		private void UpdateParticles(GameTimerEventArgs e)
		{
			// Update all particles
			emitter.Update(e);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			for (int i = 0; i < Stars.Count; i++)
			{
				Stars[i].Draw(spriteBatch);
			}

			emitter.Draw(spriteBatch);
		}
	}
}