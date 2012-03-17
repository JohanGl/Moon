using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Contexts;

namespace MoonLib.Scenes
{
	public interface IScene
	{
		List<ISceneMessage> Messages { get; set; }

		void Initialize(GameContext gameContext);
		void Unload();
		void Update(GameTimerEventArgs e);
		void Draw(SpriteBatch spriteBatch);
	}
}