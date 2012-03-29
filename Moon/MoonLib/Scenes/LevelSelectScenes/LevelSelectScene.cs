using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using MoonLib.Contexts;
using MoonLib.Scenes.LevelSelectScenes;
using MoonLib.Scenes.Levels;

namespace MoonLib.Scenes
{
	public class LevelSelectScene : IScene
	{
		public static int LevelIndex = 7;
		public const int TotalLevels = 8;
		
		private Vector2 offset;
		private Vector2 targetOffset;
		private List<LevelInfoPresentation> levels;
		private SpriteFont fontDefault;
		private SpriteFont fontChallenges;
		private float timeScalar;
		private Color challengeDescriptionColor = Color.FromNonPremultiplied(140, 140, 140, 255);
		private StarRating rating;

		private GameContext gameContext;

		public List<ISceneMessage> Messages { get; set; }

		public LevelSelectScene()
		{
			Messages = new List<ISceneMessage>();
		}

		public void Initialize(GameContext context)
		{
			gameContext = context;

			InitializeLevels();

			SetIndex(LevelIndex);
			offset = targetOffset;

			rating = new StarRating();
			rating.Initialize(context);
			rating.IsVisible = true;

			fontDefault = gameContext.Content.Load<SpriteFont>("Fonts/DefaultBold");
			fontChallenges = gameContext.Content.Load<SpriteFont>("Fonts/Challenges");
		}

		public void Unload()
		{
		}

		private void InitializeLevels()
		{
			levels = new List<LevelInfoPresentation>();

			var currentPosition = new Vector2(0, 45);

			foreach (var info in GetLevelInfo())
			{
				var level = new LevelInfoPresentation();
				level.Name = info.Name;
				level.Score = info.Score;
				level.Texture = gameContext.Content.Load<Texture2D>(info.TexturePath);
				level.Position = currentPosition;
				level.Bounds = new Rectangle((int)level.Position.X, (int)level.Position.Y, level.Texture.Width, level.Texture.Height);

				foreach (var challengeContent in info.Challenges)
				{
					var challenge = new LevelChallengePresentation();
					challenge.Name = challengeContent.Name;
					challenge.Description = challengeContent.Description;
					challenge.IsCompleted = challengeContent.IsCompleted;
					level.Challenges.Add(challenge);
				}

				levels.Add(level);
				currentPosition += new Vector2(192 + 10, 0);
			}
		}

		private List<LevelInfo> GetLevelInfo()
		{
			var result = new List<LevelInfo>();

			for (int i = 1; i <= TotalLevels; i++)
			{
				var type = Type.GetType(string.Format("MoonLib.Scenes.Levels.Level{0:00}, MoonLib", i));
				var level = (ILevel)Activator.CreateInstance(type);

				result.Add(level.Info);
			}

			return result;
		}

		private void MoveIndex(int direction)
		{
			if (LevelIndex + direction < 0)
			{
				SetIndex(0);
			}
			else if (LevelIndex + direction > levels.Count - 1)
			{
				SetIndex(levels.Count - 1);
			}
			else
			{
				SetIndex(LevelIndex + direction);
			}
		}

		private void SetIndex(int newIndex)
		{
			LevelIndex = newIndex;

			float x = (192 + 10) * newIndex;
			x -= 144;

			targetOffset = new Vector2(x, 0);
		}

		public void Update(GameTimerEventArgs e)
		{
			// Calculate the time/movement scalar for this entity
			timeScalar = (float)(e.ElapsedTime.TotalMilliseconds * 1d);

			while (TouchPanel.IsGestureAvailable)
			{
				var gesture = TouchPanel.ReadGesture();

				switch (gesture.GestureType)
				{
					case GestureType.Tap:
						if (levels[LevelIndex].Bounds.Contains((int)(gesture.Position.X + offset.X), (int)gesture.Position.Y))
						{
							Messages.Add(new LevelSelectedMessage() { LevelIndex = LevelIndex });
						}
						break;

					case GestureType.Flick:
						if (gesture.Delta.X > 0)
						{
							// Extra power flips through multiple levels at once
							if (gesture.Delta.X > 3500)
							{
								MoveIndex(-1);
								MoveIndex(-1);
							}

							MoveIndex(-1);
						}
						else if (gesture.Delta.X < 0)
						{
							// Extra power flips through multiple levels at once
							if (gesture.Delta.X < -3500)
							{
								MoveIndex(1);
								MoveIndex(1);
							}

							MoveIndex(1);
						}
						break;
				}
			}

			if (offset.X != targetOffset.X)
			{
				var delta = new Vector2((targetOffset.X - offset.X) * 0.01f, 0);
				offset += delta * timeScalar;
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			gameContext.GraphicsDevice.Clear(Color.Black);

			for (int i = 0; i < levels.Count; i++)
			{
				var level = levels[i];
				var position = level.Position - offset;

				spriteBatch.Draw(levels[i].Texture, position - new Vector2(-2, -24), GetLevelColor(i));
				spriteBatch.DrawString(fontDefault, level.Name, position, Color.White);
			}

			DrawScore(spriteBatch);
			DrawChallenges(spriteBatch);
		}

		private Color GetLevelColor(int index)
		{
			float alpha = 1f;

			if (index != LevelIndex)
			{
				alpha = Math.Max(0f, 1f - (0.75f * Math.Abs(LevelIndex - index)));
			}

			return Color.White * alpha;
		}

		private void DrawScore(SpriteBatch spriteBatch)
		{
			var position = new Vector2(40, 420);
			spriteBatch.DrawString(fontDefault, "Level Score", position, Color.White);
			position += new Vector2(0, 30);

			int score = levels[LevelIndex].Score;

			rating.Position = position;
			rating.SetSingleStarRatingByIndex(0, score);
			rating.Draw(spriteBatch);

			rating.Position += new Vector2(34, 0);
			rating.SetSingleStarRatingByIndex(1, score);
			rating.Draw(spriteBatch);

			rating.Position += new Vector2(34, 0);
			rating.SetSingleStarRatingByIndex(2, score);
			rating.Draw(spriteBatch);
		}

		private void DrawChallenges(SpriteBatch spriteBatch)
		{
			// Nothing to draw
			if (levels[LevelIndex].Challenges.Count == 0)
			{
				return;
			}

			// Challenges
			var position = new Vector2(40, 510);
			spriteBatch.DrawString(fontDefault, "Level Challenges", position, Color.White);
			position += new Vector2(0, 30);

			for (int i = 0; i < levels[LevelIndex].Challenges.Count; i++)
			{
				var challenge = levels[LevelIndex].Challenges[i];

				// Title
				spriteBatch.DrawString(fontDefault, challenge.Name, position + new Vector2(40, 0), Color.White);

				// Rating
				rating.Position = position - new Vector2(0, -6);
				rating.Rating = challenge.IsCompleted ? 2 : 0;
				rating.Draw(spriteBatch);

				position += new Vector2(0, 24);

				// Description rows
				var rows = challenge.Description.Split(new char[] { '|' });
				for (int j = 0; j < rows.Length; j++)
				{
					spriteBatch.DrawString(fontChallenges, rows[j], position + new Vector2(40, 0), challengeDescriptionColor);
					position += new Vector2(0, 24);
				}

				position += new Vector2(0, 20);
			}
		}
	}
}