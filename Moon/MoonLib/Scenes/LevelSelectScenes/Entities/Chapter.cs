using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MoonLib.Scenes.LevelSelectScenes;

namespace MoonLib.Scenes
{
    public class Chapter
    {
        public string Title { get; set; }
        public int LevelIndex { get; set; }
        public int TotalLevels { get; set; }
        public List<LevelInfoPresentation> Levels { get; set; }

        public Vector2 Offset { get; set; }
        public Vector2 TargetOffset { get; set; }

        public Chapter()
        {
            Levels = new List<LevelInfoPresentation>();
        }
    }
}