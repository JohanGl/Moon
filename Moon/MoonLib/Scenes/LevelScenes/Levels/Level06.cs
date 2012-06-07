using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Contexts;
using MoonLib.Entities.Backgrounds;
using MoonLib.Entities.Items;
using MoonLib.Helpers;
using MoonLib.IsolatedStorage;
using MoonLib.Services;
using ShakeGestures;
using HorizontalAlignment = MoonLib.Helpers.HorizontalAlignment;
using VerticalAlignment = MoonLib.Helpers.VerticalAlignment;

namespace MoonLib.Scenes.Levels
{
	public class Level06 : ILevel
	{
		private StarHandler starHandler { get; set; }
		private Player Player { get; set; }
		private DefaultBackground background;
		private PlayerInfo playerInfo;
		private StorageHandler storage;

		private LevelInfo info;
		public LevelInfo Info
		{
			get
			{
				if (info == null)
				{
					info = new LevelInfo()
					{
						Id = 6001,
						LevelType = typeof(Level06),
						Name = "Level 6",
						Score = storage.GetLevelScore(6001),
						TexturePath = "Scenes/LevelSelect/Level06",
						Challenges =
						{
							new LevelChallenge()
							{
								Id = 6002,
								Name = "Icebreaker",
								Description = "Shaken, not stirred",
								IsCompleted = storage.IsChallengeCompleted(6002)
							}
						}
					};
				}

				return info;
			}
		}

		public bool Completed
		{
			get
			{
				return starHandler.Stars.Count == 0;
			}
		}

		public bool Failed
		{
			get
			{
				return (Player.IsStationary && !playerInfo.GotMovesLeft);
			}
		}

		public int Score
		{
			get
			{
				return playerInfo.CalculateRating();
			}
		}

		public Level06()
		{
			storage = new StorageHandler();
		}

		public void Initialize(GameContext context)
		{
			// Initialize the background
			background = new DefaultBackground();
			background.Initialize(context);

			starHandler = new StarHandler(context);

			Player = new Player();
			Player.Initialize(context);

			playerInfo = new PlayerInfo();
			playerInfo.Initialize(context, 5);

			InitializeShakeGestures();

			Reset();
		}

		private void InitializeShakeGestures()
		{
			// register shake event
			ShakeGesturesHelper.Instance.ShakeGesture += Instance_ShakeGesture;

			// optional, set parameters
			ShakeGesturesHelper.Instance.MinimumShakeVectorsNeededForShake = 30;
			ShakeGesturesHelper.Instance.MinimumRequiredMovesForShake = 5;

			// start shake detection
			ShakeGesturesHelper.Instance.Active = true;
		}

		private void Instance_ShakeGesture(object sender, ShakeGestureEventArgs e)
		{
			Deployment.Current.Dispatcher.BeginInvoke(() => { Shake(); });  
		}

		public void Shake()
		{
			bool affectedStars = false;
			bool cracked = false;

			for (int i = starHandler.Stars.Count - 1; i >= 0; i--)
			{
				var star = starHandler.Stars[i];

				if (star is IceStar)
				{
					var iceStar = (star as IceStar);

					if (!iceStar.IsCracked)
					{
						iceStar.IsCracked = true;
						cracked = true;
						affectedStars = true;
					}
					else
					{
						starHandler.BreakIceStar(iceStar, Vector2.Zero);
						affectedStars = true;
					}
				}
			}

			if (affectedStars)
			{
				if (cracked)
				{
					ServiceLocator.Get<GameContext>().AudioHandler.PlaySound("Star16");
					ServiceLocator.Get<GameContext>().AudioHandler.PlaySound("Star16");
					ServiceLocator.Get<GameContext>().AudioHandler.PlaySound("Star16");
				}
				else
				{
					ServiceLocator.Get<GameContext>().AudioHandler.PlaySound("Star16");
					ServiceLocator.Get<GameContext>().AudioHandler.PlaySound("Star14");
					ServiceLocator.Get<GameContext>().AudioHandler.PlaySound("Star12");

					// First challenge
					if (!Info.Challenges[0].IsCompleted)
					{
						Info.Challenges[0].IsCompleted = true;
                        LevelSelectScene.SetLevelChallengeCompleted(Info.Challenges[0].Id);
					}
				}
			}
		}

		public void Reset()
		{
			// Stars
			InitializeStars();

			// Player
			Player.Velocity = Vector2.Zero;
			EntityHelper.HorizontalAlign(Player, HorizontalAlignment.Center);
			EntityHelper.VerticalAlign(Player, VerticalAlignment.Bottom, 64);
		}

		private void InitializeStars()
		{
			starHandler.ResetStarPitch();
			starHandler.Stars.Clear();

			starHandler.CreateIceStar(new Vector2(Device.HalfWidth - 96, Device.HalfHeight - 128), 0);
			starHandler.CreateIceStar(new Vector2(Device.HalfWidth, Device.HalfHeight - 128), 0);
			starHandler.CreateIceStar(new Vector2(Device.HalfWidth + 96, Device.HalfHeight - 128), 0);
			starHandler.CreateIceStar(new Vector2(Device.HalfWidth, Device.HalfHeight - 256), 0);
		}

		public void Update(GameTimerEventArgs e)
		{
			background.Update(e);
			Player.Update(e);
			starHandler.Update(e);

			// Remove stars that collide with the player
			starHandler.CheckPlayerCollisions(Player);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			background.Draw(spriteBatch);
			Player.Draw(spriteBatch);
			starHandler.Draw(spriteBatch);

			playerInfo.Draw(spriteBatch);
		}

		public void Move(Vector2 velocity)
		{
			if (Player.IsAllowedToMove && playerInfo.GotMovesLeft)
			{
				Player.SetVelocity(velocity);
				playerInfo.Move();
			}
		}
	}
}