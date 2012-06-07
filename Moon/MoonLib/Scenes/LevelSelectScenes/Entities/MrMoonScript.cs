using System.Collections.Generic;

namespace MoonLib.Scenes.LevelSelectScenes
{
	public class MrMoonScript
	{
		public List<string> Title { get; set; }
		public List<string> Description { get; set; }

		public MrMoonScript()
		{
			Title = new List<string>();
			Description = new List<string>();
		}
	}
}