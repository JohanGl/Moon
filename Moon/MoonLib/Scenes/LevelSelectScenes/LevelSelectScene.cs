using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using MoonLib.Contexts;
using MoonLib.IsolatedStorage;
using MoonLib.Scenes.LevelSelectScenes;
using MoonLib.Scenes.Levels;

namespace MoonLib.Scenes
{
    public class LevelSelectScene : IScene
	{
		public static int ChapterIndex = 0;
		public const int TotalChapters = 1;

		public static List<Chapter> Chapters { get; set; }
		public static Chapter CurrentChapter { get { return Chapters[ChapterIndex]; } }

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

			if (Chapters == null)
			{
				Chapters = new List<Chapter>();
				Chapters.Add(new Chapter() { Title = "Chapter 1", TotalLevels = 8 });

				InitializeChapter1Levels();
			}

			InitializeChapterScrollOffsets();

			rating = new StarRating();
			rating.Initialize(context);
			rating.IsVisible = true;

			fontDefault = gameContext.Content.Load<SpriteFont>("Fonts/DefaultBold");
			fontChallenges = gameContext.Content.Load<SpriteFont>("Fonts/Challenges");
		}

		private void InitializeChapterScrollOffsets()
		{
			int previousChapterIndex = ChapterIndex;

			// Initialize all chapter scroll offsets
			for (ChapterIndex = 0; ChapterIndex < Chapters.Count; ChapterIndex++)
			{
				SetIndex(CurrentChapter.LevelIndex);
				CurrentChapter.Offset = CurrentChapter.TargetOffset;
			}

			// Select the first chapter
			ChapterIndex = previousChapterIndex;
			SetIndex(CurrentChapter.LevelIndex);
			CurrentChapter.Offset = CurrentChapter.TargetOffset;
		}

		public void Unload()
		{
		}

		private void InitializeChapter1Levels()
		{
			var chapter = Chapters[0];

			chapter.Levels = new List<LevelInfoPresentation>();

			var currentPosition = new Vector2(0, 50);

			foreach (var info in GetLevelInfoChapter1())
			{
				var level = new LevelInfoPresentation();
				level.Id = info.Id;
				level.Name = info.Name;
				level.Score = info.Score;
				level.Texture = gameContext.Content.Load<Texture2D>(info.TexturePath);
				level.Position = currentPosition;
				level.Bounds = new Rectangle((int)level.Position.X, (int)level.Position.Y, level.Texture.Width, level.Texture.Height);

				foreach (var challengeContent in info.Challenges)
				{
					var challenge = new LevelChallengePresentation();
					challenge.Id = challengeContent.Id;
					challenge.Name = challengeContent.Name;
					challenge.Description = challengeContent.Description;
					challenge.IsCompleted = challengeContent.IsCompleted;
					level.Challenges.Add(challenge);
				}

				chapter.Levels.Add(level);
				currentPosition += new Vector2(192 + 10, 0);
			}
		}

		private List<LevelInfo> GetLevelInfoChapter1()
		{
			var result = new List<LevelInfo>();

			for (int i = 1; i <= Chapters[0].TotalLevels; i++)
			{
				var type = Type.GetType(string.Format("MoonLib.Scenes.Levels.Level{0:00}, MoonLib", i));
				var level = (ILevel)Activator.CreateInstance(type);

				result.Add(level.Info);
			}

			return result;
		}

		private List<LevelInfo> GetLevelInfoChapter2()
		{
			var result = new List<LevelInfo>();

			for (int i = 1; i <= Chapters[1].TotalLevels; i++)
			{
				var type = Type.GetType(string.Format("MoonLib.Scenes.Levels.Level{0:00}, MoonLib", i + 20));
				var level = (ILevel)Activator.CreateInstance(type);

				result.Add(level.Info);
			}

			return result;
		}

		private void MoveIndex(int direction)
		{
			if (CurrentChapter.LevelIndex + direction < 0)
			{
				SetIndex(0);
			}
			else if (CurrentChapter.LevelIndex + direction > CurrentChapter.Levels.Count - 1)
			{
				SetIndex(CurrentChapter.Levels.Count - 1);
			}
			else
			{
				SetIndex(CurrentChapter.LevelIndex + direction);
			}
		}

		private void SetIndex(int newIndex)
		{
			CurrentChapter.LevelIndex = newIndex;

			float x = (192 + 10) * newIndex;
			x -= 144;

			CurrentChapter.TargetOffset = new Vector2(x, 0);
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
						if (CurrentChapter.Levels[CurrentChapter.LevelIndex].Bounds.Contains((int)(gesture.Position.X + CurrentChapter.Offset.X), (int)gesture.Position.Y))
						{
							Messages.Add(new LevelSelectedMessage() { LevelIndex = CurrentChapter.LevelIndex });
						}
						break;

					case GestureType.Flick:
						// Swipes horizontally
						if (Math.Abs(gesture.Delta.X) > Math.Abs(gesture.Delta.Y))
						{
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
						}
						break;
				}
			}

			if (CurrentChapter.Offset.X != CurrentChapter.TargetOffset.X)
			{
				var delta = new Vector2((CurrentChapter.TargetOffset.X - CurrentChapter.Offset.X) * 0.01f, 0);
				CurrentChapter.Offset += delta * timeScalar;
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			gameContext.GraphicsDevice.Clear(Color.Black);

			for (int i = 0; i < CurrentChapter.Levels.Count; i++)
			{
				var level = CurrentChapter.Levels[i];
				var position = level.Position - CurrentChapter.Offset;

				spriteBatch.Draw(CurrentChapter.Levels[i].Texture, position - new Vector2(-2, -24), GetLevelColor(i));
				spriteBatch.DrawString(fontDefault, level.Name, position, Color.White);
			}

			DrawScore(spriteBatch);
			DrawChallenges(spriteBatch);
		}

		private Color GetLevelColor(int index)
		{
			float alpha = 1f;

			if (index != CurrentChapter.LevelIndex)
			{
				alpha = Math.Max(0f, 1f - (0.75f * Math.Abs(CurrentChapter.LevelIndex - index)));
			}

			return Color.White * alpha;
		}

		private void DrawScore(SpriteBatch spriteBatch)
		{
			var position = new Vector2(40, 440);
			spriteBatch.DrawString(fontDefault, "Level Score", position, Color.White);
			position += new Vector2(0, 30);

			int score = CurrentChapter.Levels[CurrentChapter.LevelIndex].Score;

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
			if (CurrentChapter.Levels[CurrentChapter.LevelIndex].Challenges.Count == 0)
			{
				return;
			}

			// Challenges
			var position = new Vector2(40, 530);
			spriteBatch.DrawString(fontDefault, "Level Challenges", position, Color.White);
			position += new Vector2(0, 30);

			for (int i = 0; i < CurrentChapter.Levels[CurrentChapter.LevelIndex].Challenges.Count; i++)
			{
				var challenge = CurrentChapter.Levels[CurrentChapter.LevelIndex].Challenges[i];

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

		public static void UpdateLevelScore(int levelId, int score)
		{
			var storage = new StorageHandler();
			storage.SetLevelScore(levelId, score);

			for (int i = 0; i < CurrentChapter.Levels.Count; i++)
			{
				var level = CurrentChapter.Levels[i];

				if (level.Id == levelId)
				{
					level.Score = score;
					break;
				}
			}
		}

		public static void SetLevelChallengeCompleted(int challengeId)
		{
			var storage = new StorageHandler();
			storage.SetChallengeCompleted(challengeId);

			for (int i = 0; i < CurrentChapter.Levels.Count; i++)
			{
				var level = CurrentChapter.Levels[i];

				if (level.Challenges != null)
				{
					for (int j = 0; j < level.Challenges.Count; j++)
					{
						var challenge = level.Challenges[j];

						if (challenge.Id == challengeId)
						{
							challenge.IsCompleted = true;
							i = CurrentChapter.Levels.Count;
							break;
						}
					}
				}
			}
		}
	}
}