using System.Collections.Generic;

namespace MoonLib.IsolatedStorage.Entities
{
	public class LevelsInfo
	{
		public Dictionary<int, bool> Completed { get; set; }

		public LevelsInfo()
		{
			Completed = new Dictionary<int, bool>();
		}
	}
}