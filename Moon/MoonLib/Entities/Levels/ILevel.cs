using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MoonLib.Entities.Levels
{
	public interface ILevel
	{
		Player Player { get; set; }

		void Initialize(ContentManager contentManager);
		void Update(GameTimerEventArgs e);
		void Draw(GraphicsDevice device, SpriteBatch spriteBatch);
	}
}