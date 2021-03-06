using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MoonLib.Scenes.LevelSelectScenes
{
	public class LevelInfoPresentation
	{
		public bool IsMrMoon { get { return LevelInfoMrMoon != null; } }
		public LevelInfoMrMoon LevelInfoMrMoon { get; set; }

		public int Id { get; set; }
		public Type LevelType { get; set; }
		public Vector2 Position { get; set; }
		public Rectangle Bounds { get; set; }
		public string Name { get; set; }
		public Texture2D Texture { get; set; }
		public int Score { get; set; }
		public List<LevelChallengePresentation> Challenges { get; set; }

		public LevelInfoPresentation()
		{
			Challenges = new List<LevelChallengePresentation>();
		}
	}
}