using System.Collections.Generic;

namespace MoonLib.Scenes.LevelSelectScenes
{
	public class LevelInfoMrMoon
	{
		public bool IsEvil { get; set; }
		public bool IsCompleted { get; set; }
		public int RequiredStars { get; set; }
		public List<MrMoonScript> Scripts { get; set; }

		public LevelInfoMrMoon()
		{
			Scripts = new List<MrMoonScript>();
		}
	}
}