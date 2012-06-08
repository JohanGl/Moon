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
		public static int ChapterIndex;
		public const int TotalChapters = 1;

		public static List<Chapter> Chapters { get; set; }
		public static Chapter CurrentChapter { get { return Chapters[ChapterIndex]; } }

		private double score;
		private double challengeScore;
		private double totalScore { get { return score + challengeScore; } }

		private SpriteFont fontDefault;
		private SpriteFont fontChallenges;
		private float timeScalar;
		private Color challengeDescriptionColor = Color.FromNonPremultiplied(140, 140, 140, 255);
		private StarRating rating;

		private float mrMoonAngle;
		private float mrMoonFloatY;
		private Texture2D mrMoonBackground;
		private Texture2D mrMoonAngry;
		private Texture2D mrMoonSurprised;
		private Texture2D evilMoonDefault;
		private Texture2D evilMoonLost;

		private GameContext gameContext;

		public List<ISceneMessage> Messages { get; set; }

		public LevelSelectScene()
		{
			Messages = new List<ISceneMessage>();
		}

		public void Initialize(GameContext context)
		{
			gameContext = context;

			fontDefault = gameContext.Content.Load<SpriteFont>("Fonts/DefaultBold");
			fontChallenges = gameContext.Content.Load<SpriteFont>("Fonts/Challenges");
			mrMoonBackground = gameContext.Content.Load<Texture2D>("Scenes/LevelSelect/MrMoon/Background");
			mrMoonAngry = gameContext.Content.Load<Texture2D>("Scenes/LevelSelect/MrMoon/Angry");
			mrMoonSurprised = gameContext.Content.Load<Texture2D>("Scenes/LevelSelect/MrMoon/Surprised");

			evilMoonDefault = gameContext.Content.Load<Texture2D>("Scenes/LevelSelect/BadMoon/Default");
			evilMoonLost = gameContext.Content.Load<Texture2D>("Scenes/LevelSelect/BadMoon/Lost");

			if (Chapters == null)
			{
				Chapters = new List<Chapter>();
				Chapters.Add(new Chapter() { Title = "Chapter 1", TotalLevels = 9 + 2 });

				InitializeChapter1Levels();

				//UpdateLevelScore(1001, 6);
				//UpdateLevelScore(2001, 6);
				//UpdateLevelScore(3001, 6);

				//UpdateLevelScore(4001, 6);
				//UpdateLevelScore(5001, 6);
				//UpdateLevelScore(6001, 6);

				//UpdateLevelScore(7001, 6);
				//UpdateLevelScore(8001, 6);
				//UpdateLevelScore(9001, 6);

				//SetLevelChallengeCompleted(1002);
				//SetLevelChallengeCompleted(1003);
				//SetLevelChallengeCompleted(2002);
				//SetLevelChallengeCompleted(3002);
				//SetLevelChallengeCompleted(4002);
				//SetLevelChallengeCompleted(5002);
			}

			UpdateCurrentScore();
			UpdateMrMoonStates();

			InitializeChapterScrollOffsets();

			rating = new StarRating();
			rating.Initialize(context);
			rating.IsVisible = true;
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

		private void UpdateCurrentScore()
		{
			score = 0;
			challengeScore = 0;

			for (int i = 0; i < CurrentChapter.Levels.Count; i++)
			{
				var level = CurrentChapter.Levels[i];

				for (int j = 0; j < level.Challenges.Count; j++)
				{
					var challenge = level.Challenges[j];

					if (challenge.IsCompleted)
					{
						challengeScore++;
					}
				}

				score += level.Score * 0.5d;
			}

			if (totalScore >= 10)
			{
				CurrentChapter.Levels[3].LevelInfoMrMoon.IsCompleted = true;
			}
		}

		private void UpdateMrMoonStates()
		{
			for (int i = 0; i < CurrentChapter.Levels.Count; i++)
			{
				var level = CurrentChapter.Levels[i];

				if (level.IsMrMoon)
				{
					level.LevelInfoMrMoon.IsCompleted = false;

					if (totalScore >= level.LevelInfoMrMoon.RequiredStars)
					{
						level.LevelInfoMrMoon.IsCompleted = true;
					}
				}
			}
		}

		public void Unload()
		{
		}

		private void InitializeChapter1Levels()
		{
			var chapter = Chapters[0];

			chapter.Levels = new List<LevelInfoPresentation>();

			var currentPosition = new Vector2(0, 50);

			int i = 0;

			foreach (var info in GetLevelInfoChapter1())
			{
				var level = new LevelInfoPresentation();
				level.Id = info.Id;
				level.LevelType = info.LevelType;
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

				i++;

				if (i == 3)
				{
					chapter.Levels.Add(GetFirstMrMoonLevel(currentPosition));
					currentPosition += new Vector2(192 + 10, 0);
				}
				else if (i == 6)
				{
					chapter.Levels.Add(GetSecondMrMoonLevel(currentPosition));
					currentPosition += new Vector2(192 + 10, 0);
				}
				else if (i == 9)
				{
					chapter.Levels.Add(GetThirdMrMoonLevel(currentPosition));
					currentPosition += new Vector2(192 + 10, 0);
				}
			}
		}

		private LevelInfoPresentation GetFirstMrMoonLevel(Vector2 currentPosition)
		{
			var stopLevel = new LevelInfoPresentation();
			stopLevel.LevelInfoMrMoon = new LevelInfoMrMoon();
			stopLevel.Name = string.Empty;
			stopLevel.Texture = mrMoonBackground;
			stopLevel.Position = currentPosition;
			stopLevel.LevelInfoMrMoon.RequiredStars = 12;
			stopLevel.Bounds = new Rectangle((int)stopLevel.Position.X, (int)stopLevel.Position.Y, stopLevel.Texture.Width, stopLevel.Texture.Height);

			var script = new MrMoonScript();
			script.Title.Add("Hey! You there!");
			script.Title.Add("Someone has stolen all my stars!");
			script.Description.Add("I cant let you pass that easily knowing you might be");
			script.Description.Add(string.Format("a thief. Bring me back {0} stars and i just might let", stopLevel.LevelInfoMrMoon.RequiredStars));
			script.Description.Add("you off... with a warning.");
			stopLevel.LevelInfoMrMoon.Scripts.Add(script);

			script = new MrMoonScript();
			script.Title.Add("Amazing! Splendid work my friend.");
			script.Description.Add("I didnt think you would make it, least not this quick.");
			script.Description.Add("Please proceed. I guess you didnt steal my stars after all.");
			stopLevel.LevelInfoMrMoon.Scripts.Add(script);

			return stopLevel;
		}

		private LevelInfoPresentation GetSecondMrMoonLevel(Vector2 currentPosition)
		{
			var stopLevel = new LevelInfoPresentation();
			stopLevel.LevelInfoMrMoon = new LevelInfoMrMoon();
			stopLevel.Name = string.Empty;
			stopLevel.Texture = mrMoonBackground;
			stopLevel.Position = currentPosition;
			stopLevel.LevelInfoMrMoon.RequiredStars = 20;
			stopLevel.Bounds = new Rectangle((int)stopLevel.Position.X, (int)stopLevel.Position.Y, stopLevel.Texture.Width, stopLevel.Texture.Height);

			var script = new MrMoonScript();
			script.Title.Add("Thieves! Everywhere!");
			script.Description.Add("I cant believe this happened to me again. When i woke up");
			script.Description.Add("this morning, they were all gone. Would you please");
			script.Description.Add(string.Format("help me bring {0} stars back here?", stopLevel.LevelInfoMrMoon.RequiredStars));
			stopLevel.LevelInfoMrMoon.Scripts.Add(script);

			script = new MrMoonScript();
			script.Title.Add("Fantastic!");
			script.Description.Add("You sure saved the day once again my friend.");
			script.Description.Add("Please proceed. If you find that thief, wont you");
			script.Description.Add("bring him to me?");
			stopLevel.LevelInfoMrMoon.Scripts.Add(script);

			return stopLevel;
		}

		private LevelInfoPresentation GetThirdMrMoonLevel(Vector2 currentPosition)
		{
			var stopLevel = new LevelInfoPresentation();
			stopLevel.Name = string.Empty;
			stopLevel.Texture = mrMoonBackground;
			stopLevel.Position = currentPosition;
			stopLevel.Bounds = new Rectangle((int)stopLevel.Position.X, (int)stopLevel.Position.Y, stopLevel.Texture.Width, stopLevel.Texture.Height);
			stopLevel.LevelInfoMrMoon = new LevelInfoMrMoon();
			stopLevel.LevelInfoMrMoon.IsEvil = true;
			stopLevel.LevelInfoMrMoon.RequiredStars = 32;

			var script = new MrMoonScript();
			script.Title.Add("Been looking for me?");
			script.Description.Add("I wont give up my precious stars that easily.");
			script.Description.Add("Im always up for a challenge though, so ill tell you what.");
			script.Description.Add(string.Format("If you can collect {0} stars i will leave your friend alone.", stopLevel.LevelInfoMrMoon.RequiredStars));
			stopLevel.LevelInfoMrMoon.Scripts.Add(script);

			script = new MrMoonScript();
			script.Title.Add("Grrr!");
			script.Description.Add("I cant believe i lost! How did you manage to collect all");
			script.Description.Add("those stars!? Im a moon of my words so ive sent all stars");
			script.Description.Add("back to where they belong.");
			stopLevel.LevelInfoMrMoon.Scripts.Add(script);

			return stopLevel;
		}

		private List<LevelInfo> GetLevelInfoChapter1()
		{
			var result = new List<LevelInfo>();

			for (int i = 1; i <= Chapters[0].TotalLevels - 2; i++)
			{
				var type = Type.GetType(string.Format("MoonLib.Scenes.Levels.Level{0:00}, MoonLib", i));
				var level = (ILevel)Activator.CreateInstance(type);

				result.Add(level.Info);
			}

			return result;
		}

		private void MoveIndex(int direction)
		{
			// Block the player from moving if the current level is mr moon and his task isnt completed
			if (direction > 0 &&
				CurrentChapter.CurrentLevel.IsMrMoon &&
				!CurrentChapter.CurrentLevel.LevelInfoMrMoon.IsCompleted)
			{
				return;
			}

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

			mrMoonAngle += (float)(e.ElapsedTime.TotalMilliseconds * 0.075d);
			mrMoonFloatY = 10f * (float)Math.Sin(MathHelper.ToRadians(mrMoonAngle));

			while (TouchPanel.IsGestureAvailable)
			{
				var gesture = TouchPanel.ReadGesture();

				switch (gesture.GestureType)
				{
					case GestureType.Tap:
						if (!CurrentChapter.CurrentLevel.IsMrMoon)
						{
							if (CurrentChapter.CurrentLevel.Bounds.Contains((int)(gesture.Position.X + CurrentChapter.Offset.X), (int)gesture.Position.Y))
							{
								Messages.Add(new LevelSelectedMessage() { LevelIndex = CurrentChapter.LevelIndex });
							}
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
				var levelColor = GetLevelColor(i);

				spriteBatch.Draw(CurrentChapter.Levels[i].Texture, position - new Vector2(-2, -24), levelColor);
				spriteBatch.DrawString(fontDefault, level.Name, position, Color.White);

				if (level.IsMrMoon)
				{
					if (level.LevelInfoMrMoon.IsEvil)
					{
						if (level.LevelInfoMrMoon.IsCompleted)
						{
							spriteBatch.Draw(evilMoonLost, position + new Vector2(3, 80 + mrMoonFloatY), levelColor);
						}
						else
						{
							spriteBatch.Draw(evilMoonDefault, position + new Vector2(3, 80 + mrMoonFloatY), levelColor);
						}
					}
					else
					{
						if (level.LevelInfoMrMoon.IsCompleted)
						{
							spriteBatch.Draw(mrMoonSurprised, position + new Vector2(3, 60 + mrMoonFloatY), levelColor);
						}
						else
						{
							spriteBatch.Draw(mrMoonAngry, position + new Vector2(3, 60 + mrMoonFloatY), levelColor);
						}
					}
				}
			}

			if (CurrentChapter.CurrentLevel.IsMrMoon)
			{
				DrawMrMoonContent(spriteBatch);
				return;
			}

			DrawScore(spriteBatch);
			DrawChallenges(spriteBatch);
		}

		private void DrawMrMoonContent(SpriteBatch spriteBatch)
		{
			int scriptIndex = CurrentChapter.CurrentLevel.LevelInfoMrMoon.IsCompleted ? 1 : 0;

			// Title
			var position = new Vector2(40, 440);

			for (int i = 0; i < CurrentChapter.CurrentLevel.LevelInfoMrMoon.Scripts[scriptIndex].Title.Count; i++)
			{
				var text = CurrentChapter.CurrentLevel.LevelInfoMrMoon.Scripts[scriptIndex].Title[i];
				spriteBatch.DrawString(fontDefault, text, position, Color.White);
				position += new Vector2(0, 30);
			}

			// Description
			position += new Vector2(0, 15);

			for (int i = 0; i < CurrentChapter.CurrentLevel.LevelInfoMrMoon.Scripts[scriptIndex].Description.Count; i++)
			{
				var text = CurrentChapter.CurrentLevel.LevelInfoMrMoon.Scripts[scriptIndex].Description[i];
				spriteBatch.DrawString(fontChallenges, text, position, Color.White);
				position += new Vector2(0, 25);
			}

			position += new Vector2(0, 15);

			if ((int)totalScore == 1)
			{
				spriteBatch.DrawString(fontChallenges, "You currently have 1 star.", position, Color.White);
			}
			else
			{
				spriteBatch.DrawString(fontChallenges, string.Format("You currently have {0} stars.", (int)totalScore), position, Color.White);
			}
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
			for (int i = 0; i < CurrentChapter.Levels.Count; i++)
			{
				var level = CurrentChapter.Levels[i];

				if (level.Id == levelId)
				{
					if (level.Score < score)
					{
						level.Score = score;

						var storage = new StorageHandler();
						storage.SetLevelScore(levelId, score);
					}

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