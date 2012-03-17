using System.Collections.Generic;

namespace MoonLib.IsolatedStorage.Entities
{
	public class LevelsInfo
	{
		public Dictionary<int, int> Score { get; set; }
		public Dictionary<int, bool> Completed { get; set; }

		public LevelsInfo()
		{
			Score = new Dictionary<int, int>();
			Completed = new Dictionary<int, bool>();
		}
	}
}