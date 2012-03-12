using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MoonLib.Scenes
{
	public class LevelInfo
	{
		public Vector2 Position { get; set; }
		public Rectangle Bounds { get; set; }
		public string Name { get; set; }
		public Texture2D Texture { get; set; }
		public int? StarRating { get; set; }

		public List<LevelChallenge> Challenges { get; set; }

		public LevelInfo()
		{
			Challenges = new List<LevelChallenge>();
		}
	}
}