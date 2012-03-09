using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MoonLib.Entities.Levels.Gui
{
	public class PlayerInfo
	{
		private SpriteFont font;
		private int totalMoves;
		private int moves;

		public bool GotMovesLeft
		{
			get
			{
				return moves > 0;
			}
		}

		public void Initialize(ContentManager contentManager, int totalMoves)
		{
			this.totalMoves = totalMoves;
			moves = totalMoves;
			font = contentManager.Load<SpriteFont>("Fonts/Default");
		}

		public void Move()
		{
			if (moves > 0)
			{
				moves--;
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.DrawString(font, "MOVES: " + moves, new Vector2(8, 8), Color.White);
		}

		public int CalculateRating()
		{
			var percentage = moves / (double)totalMoves;
			var rating = (float)(6 * percentage);

			//for (int i = 0; i <= 6; i++)
			//{
			//    if (rating)
			//}

			return 0;
		}
	}
}