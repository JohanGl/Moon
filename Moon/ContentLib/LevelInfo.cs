using System.Collections.Generic;

namespace ContentLib
{
	public class LevelInfoContent
	{
		public string Name { get; set; }
		public string Texture { get; set; }
		public List<LevelChallengeContent> Challenges { get; set; }

		public LevelInfoContent()
		{
			Challenges = new List<LevelChallengeContent>();
		}
	}
}