using System.Collections.Generic;
using Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MoonLib.Scenes
{
	public interface IScene
	{
		List<ISceneMessage> Messages { get; set; }

		void Initialize(ContentManager contentManager, IAudioHandler audioHandler);
		void Update(GameTimerEventArgs e);
		void Draw(GraphicsDevice device, SpriteBatch spriteBatch);
	}

	public interface ISceneMessage
	{
	}

	public class LevelSelectedMessage : ISceneMessage
	{
		public int LevelIndex { get; set; }
	}
}