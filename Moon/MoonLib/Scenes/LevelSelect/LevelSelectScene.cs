using System.Collections.Generic;
using ContentLib;
using Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace MoonLib.Scenes
{
	public class LevelSelectScene : IScene
	{
		private int index;
		private Vector2 offset;
		private Vector2 targetOffset;
		private List<LevelInfo> levels;
		private SpriteFont fontDefault;
		private SpriteFont fontChallenges;
		private float timeScalar;
		private Color challengeDescriptionColor = Color.FromNonPremultiplied(120, 120, 120, 255);

		public List<ISceneMessage> Messages { get; set; }

		public LevelSelectScene()
		{
			Messages = new List<ISceneMessage>();
		}

		public void Initialize(ContentManager contentManager, IAudioHandler audioHandler)
		{
			InitializeLevels(contentManager);

			SetIndex(0);
			offset = new Vector2(0, 0);

			fontDefault = contentManager.Load<SpriteFont>("Fonts/DefaultBold");
			fontChallenges = contentManager.Load<SpriteFont>("Fonts/Challenges");
		}

		private void InitializeLevels(ContentManager contentManager)
		{
			levels = new List<LevelInfo>();

			var levelsContent = contentManager.Load<List<LevelInfoContent>>("Levels");
			var currentPosition = new Vector2(0, 45);

			foreach (var levelContent in levelsContent)
			{
				var level = new LevelInfo();
				level.Name = levelContent.Name;
				level.Texture = contentManager.Load<Texture2D>(levelContent.Texture);
				level.Position = currentPosition;
				level.Bounds = new Rectangle((int)level.Position.X, (int)level.Position.Y, level.Texture.Width, level.Texture.Height);

				foreach (var challengeContent in levelContent.Challenges)
				{
					var challenge = new LevelChallenge();
					challenge.Name = challengeContent.Name;
					challenge.Description = challengeContent.Description;

					level.Challenges.Add(challenge);
				}

				levels.Add(level);

				currentPosition += new Vector2(192 + 10, 0);
			}
		}

		private void MoveIndex(int direction)
		{
			if (index + direction < 0)
			{
				SetIndex(0);
			}
			else if (index + direction > levels.Count - 1)
			{
				SetIndex(levels.Count - 1);
			}
			else
			{
				SetIndex(index + direction);
			}
		}

		private void SetIndex(int newIndex)
		{
			index = newIndex;

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
						if (levels[index].Bounds.Contains((int)(gesture.Position.X + offset.X), (int)gesture.Position.Y))
						{
							Messages.Add(new LevelSelectedMessage() { LevelIndex = index });
						}
						break;

					case GestureType.Flick:
						if (gesture.Delta.X > 0)
						{
							MoveIndex(-1);
						}
						else if (gesture.Delta.X < 0)
						{
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

		public void Draw(GraphicsDevice device, SpriteBatch spriteBatch)
		{
			device.Clear(Color.Black);

			Vector2 position;

			for (int i = 0; i < levels.Count; i++)
			{
				var level = levels[i];
				position = level.Position - offset;

				spriteBatch.Draw(levels[i].Texture, position - new Vector2(-2, -24), Color.White);
				spriteBatch.DrawString(fontDefault, level.Name, position, Color.White);
			}

			// Challenges
			position = new Vector2(10, 415);
			spriteBatch.DrawString(fontDefault, "Level Challenges", position, Color.White);
			position += new Vector2(40, 50);

			for (int i = 0; i < levels[index].Challenges.Count; i++)
			{
				var challenge = levels[index].Challenges[i];

				spriteBatch.DrawString(fontDefault, challenge.Name, position, Color.White);
				position += new Vector2(0, 24);

				// Description rows
				var rows = challenge.Description.Split(new char[] { '|' });
				for (int j = 0; j < rows.Length; j++)
				{
					spriteBatch.DrawString(fontChallenges, rows[j], position, challengeDescriptionColor);
					position += new Vector2(0, 24);
				}

				position += new Vector2(0, 20);
			}
		}
	}
}