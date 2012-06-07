using System;
using System.Collections.Generic;

namespace MoonLib.Scenes.Levels
{
	public class LevelInfo
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string TexturePath { get; set; }
		public int Score { get; set; }
		public List<LevelChallenge> Challenges { get; set; }
		public Type LevelType { get; set; }

		public LevelInfo()
		{
			Challenges = new List<LevelChallenge>();
		}
	}
}