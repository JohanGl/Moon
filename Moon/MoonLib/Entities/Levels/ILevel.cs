using Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MoonLib.Entities.Levels
{
	public interface ILevel
	{
		Player Player { get; set; }
		StarHandler StarHandler { get; set; }

		bool Completed { get; }

		void Initialize(ContentManager contentManager, IAudioHandler audioHandler);
		void Reset();
		void Update(GameTimerEventArgs e);
		void Draw(GraphicsDevice device, SpriteBatch spriteBatch);
	}
}