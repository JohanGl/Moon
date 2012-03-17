using System.Collections.Generic;

namespace MoonLib.Scenes.Levels
{
	public class LevelInfo
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Overview { get; set; }
		public List<LevelChallenge> Challenges { get; set; }

		public LevelInfo()
		{
			Challenges = new List<LevelChallenge>();
		}
	}
}