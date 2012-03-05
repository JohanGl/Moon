using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MoonLib.Entities.Levels.Gui
{
	public class PlayerInfo
	{
		private SpriteFont font;
		private int totalMoves;

		public bool GotMovesLeft
		{
			get
			{
				return totalMoves > 0;
			}
		}

		public void Initialize(ContentManager contentManager, int totalMoves)
		{
			this.totalMoves = totalMoves;
			font = contentManager.Load<SpriteFont>("Fonts/Default");
		}

		public void Moved()
		{
			if (totalMoves > 0)
			{
				totalMoves--;
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.DrawString(font, "MOVES: 3", new Vector2(8, 8), Color.White);
		}
	}
}