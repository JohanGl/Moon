using System;
using System.Collections.Generic;
using Framework.Audio;
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
		public List<IStar> Stars;
		private List<IStar> starsToRemove;

		private const int totalStarPitch = 16;

		private float timeScalar;
		private float currentStarAngle;
		private IParticleEmitter[] emitters;
		private ContentManager contentManager;
		private IAudioHandler audioHandler;
		private int currentStarPitch;

		public StarHandler(ContentManager contentManager, IAudioHandler audioHandler)
		{
			this.contentManager = contentManager;
			this.audioHandler = audioHandler;

			Stars = new List<IStar>();
			starsToRemove = new List<IStar>();
			ResetStarPitch();

			emitters = new IParticleEmitter[3];
	
			emitters[0] = new StarParticleEmitter();
			emitters[0].Initialize(contentManager, 200);

			emitters[1] = new StarSparkParticleEmitter();
			emitters[1].Initialize(contentManager, 20);

			emitters[2] = new IceParticleEmitter();
			emitters[2].Initialize(contentManager, 200);
		}

		public void ResetStarPitch()
		{
			currentStarPitch = 1;
		}

		public void CheckPlayerCollisions(Player player)
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
						var starVelocity = player.Velocity * 0.05f;
						(star as IceStar).Velocity = player.Velocity * 0.05f;

						if (Math.Abs(player.Velocity.X) >= Math.Abs(player.Velocity.Y))
						{
							player.Velocity = new Vector2(-player.Velocity.X, player.Velocity.Y);
						}
						else
						{
							player.Velocity = new Vector2(player.Velocity.X, -player.Velocity.Y);
						}

						player.Velocity *= 0.9f;

						for (int t = 0; t < 100; t++)
						{
							player.Position += player.Velocity;

							if (!EntityHelper.Instersects(player, (Entity)star))
							{
								break;
							}
						}

						if (!(star as IceStar).IsCracked)
						{
							(star as IceStar).IsCracked = true;
						}
						else
						{
							
							BreakIceStar((IceStar)star, starVelocity);
						}
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
				audioHandler.PlaySound("Star" + currentStarPitch, 1f, 0f, 0f);

				if (currentStarPitch < totalStarPitch)
				{
					currentStarPitch++;
				}
			}
		}

		private void BreakIceStar(IceStar star, Vector2 velocity)
		{
			Stars.Remove((IStar)star);
			
			CreateStar(star.Position + new Vector2(star.HalfSize.X, star.HalfSize.Y), star.Angle);
			(Stars[Stars.Count - 1] as Star).Velocity = velocity;

			emitters[2].Position = star.Position + new Vector2(star.HalfSize.X, star.HalfSize.Y);
			emitters[2].Emit();

			audioHandler.PlaySound("IceStar");
		}

		public void CreateStar(Vector2 position, float angle)
		{
			var star = new Star();
			star.Initialize(contentManager);
			star.Position = new Vector2((int)(position.X - star.HalfSize.X), (int)(position.Y - star.HalfSize.Y));
			star.Angle = angle;
			star.CollisionRadius = 12;

			Stars.Add(star);
		}

		public void CreateIceStar(Vector2 position, float angle)
		{
			var star = new IceStar();
			star.Initialize(contentManager, Stars.Count);
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