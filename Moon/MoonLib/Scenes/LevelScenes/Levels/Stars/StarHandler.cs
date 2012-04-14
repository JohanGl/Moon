using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Contexts;
using MoonLib.Entities.Items;
using MoonLib.Helpers;
using MoonLib.Particles;

namespace MoonLib.Scenes.Levels
{
	public class StarHandler
	{
		public List<IStar> Stars;
		private List<IStar> starsToRemove;

		private const int totalStarPitch = 16;
		private float timeScalar;
		private float currentStarAngle;
		private IParticleEmitter[] emitters;
		private int currentStarPitch;

		private GameContext gameContext;

		public List<IStar> StarsRemovedThisUpdate
		{
			get
			{
				return starsToRemove;
			}
		}

		public StarHandler(GameContext context)
		{
			gameContext = context;

			Stars = new List<IStar>();
			starsToRemove = new List<IStar>();
			ResetStarPitch();

			emitters = new IParticleEmitter[3];

			emitters[0] = new StarParticleEmitter();
			emitters[0].Initialize(gameContext, 200);

			emitters[1] = new StarSparkParticleEmitter();
			emitters[1].Initialize(gameContext, 20);

			emitters[2] = new IceParticleEmitter();
			emitters[2].Initialize(gameContext, 200);
		}

		public void ResetStarPitch()
		{
			currentStarPitch = 1;
		}

		public void CheckPlayerCollisions(Entity player)
		{
			starsToRemove.Clear();

			for (int i = 0; i < Stars.Count; i++)
			{
				var star = Stars[i];

				if (star is Star)
				{
					if (EntityHelper.Instersects(player, (Entity)star))
					{
						starsToRemove.Add(Stars[i]);
					}
				}
				else if (star is IceStar)
				{
					if (EntityHelper.Instersects(player, (Entity)star))
					{
						BreakIceStar((IceStar)star, player.Velocity * 0.05f);
						player.Velocity *= 0.01f;
					}
				}
			}

			for (int i = 0; i < starsToRemove.Count; i++)
			{
				Stars.Remove(starsToRemove[i]);

				emitters[0].Position = (starsToRemove[i] as Star).Position;
				emitters[0].Emit();

				emitters[1].Position = emitters[0].Position;
				emitters[1].Emit();
			}

			if (starsToRemove.Count > 0)
			{
				gameContext.AudioHandler.PlaySound("Star" + currentStarPitch, 1f, 0f, 0f);

				if (currentStarPitch < totalStarPitch)
				{
					currentStarPitch++;
				}
			}
		}

		public void BreakIceStar(IceStar star, Vector2 velocity)
		{
			Stars.Remove(star);

			CreateStar(star.Position + new Vector2(star.HalfSize.X, star.HalfSize.Y), star.Angle);
			(Stars[Stars.Count - 1] as Star).Velocity = velocity;

			emitters[2].Position = star.Position + new Vector2(star.HalfSize.X, star.HalfSize.Y);
			emitters[2].Emit();

			gameContext.AudioHandler.PlaySound("IceStar");
		}

		public void CreateStar(Vector2 position, float angle)
		{
			var star = new Star();
			star.Initialize(gameContext);
			star.Position = new Vector2((int)(position.X - star.HalfSize.X), (int)(position.Y - star.HalfSize.Y));
			star.Angle = angle;
			star.CollisionRadius = 12;

			Stars.Add(star);
		}

		public void CreateIceStar(Vector2 position, float angle)
		{
			var star = new IceStar();
			star.Initialize(gameContext, Stars.Count);
			star.Position = new Vector2((int)(position.X - star.HalfSize.X), (int)(position.Y - star.HalfSize.Y));
			star.Angle = angle;
			star.CollisionRadius = 30;

			Stars.Add(star);
		}

		public void CreateIceStar(Vector2 position, float angle, int type)
		{
			var star = new IceStar();
			star.Initialize(gameContext, type);
			star.Position = new Vector2((int)(position.X - star.HalfSize.X), (int)(position.Y - star.HalfSize.Y));
			star.Angle = angle;
			star.CollisionRadius = 30;

			Stars.Add(star);
		}

		public void Update(GameTimerEventArgs e)
		{
			for (int i = 0; i < Stars.Count; i++)
			{
				Stars[i].Update(e);
			}

			UpdateStarAngles(e);
			UpdateParticles(e);
		}

		private void UpdateStarAngles(GameTimerEventArgs e)
		{
			// Calculate the time/movement scalar for this entity
			timeScalar = (float)(e.ElapsedTime.TotalMilliseconds * 1.5f);

			// Calculate the angle once and use it for all stars (the last multiplier limits how widely the stars rotate)
			float starAngle = (float)Math.Sin(MathHelper.ToRadians(currentStarAngle)) * 0.6f;
			float iceStarAngle = (float)Math.Sin(MathHelper.ToRadians(currentStarAngle)) * 0.1f;

			for (int i = 0; i < Stars.Count; i++)
			{
				var star = Stars[i];

				if (star is Star)
				{
					(star as Star).Angle = starAngle;
				}
				else if (star is IceStar)
				{
					(star as IceStar).Angle = iceStarAngle;
				}
			}

			// Update the current stars angle for the next update
			currentStarAngle += 0.1f * timeScalar;

			if (currentStarAngle >= 360f)
			{
				currentStarAngle -= 360f;
			}
		}

		private void UpdateParticles(GameTimerEventArgs e)
		{
			for (int i = 0; i < emitters.Length; i++)
			{
				emitters[i].Update(e);
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			for (int i = 0; i < Stars.Count; i++)
			{
				Stars[i].Draw(spriteBatch);
			}

			for (int i = 0; i < emitters.Length; i++)
			{
				emitters[i].Draw(spriteBatch);
			}
		}
	}
}