using Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MoonLib.Contexts
{
	public class GameContext
	{
		public ContentManager Content { get; set; }
		public GraphicsDevice GraphicsDevice { get; set; }
		public IAudioHandler AudioHandler { get; set; }
		public SpriteBatch SpriteBatch { get; set; }
		public GameSettings Settings { get; set; }
	}
}