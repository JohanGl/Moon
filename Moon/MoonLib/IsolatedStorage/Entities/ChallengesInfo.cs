using System.Collections.Generic;

namespace MoonLib.IsolatedStorage.Entities
{
	public class ChallengesInfo
	{
		public Dictionary<int, bool> Completed { get; set; }

		public ChallengesInfo()
		{
			Completed = new Dictionary<int, bool>();
		}
	}
}