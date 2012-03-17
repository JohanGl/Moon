using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Contexts;

namespace MoonLib.Scenes.Levels
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

		public int UsedMoves
		{
			get
			{
				return totalMoves - moves;
			}
		}

		public void Initialize(GameContext context, int totalMoves)
		{
			this.totalMoves = totalMoves;
			moves = totalMoves;
			font = context.Content.Load<SpriteFont>("Fonts/Default");
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
			var percentage = (moves + 1) / (double)totalMoves;
			var rating = (float)(6 * percentage);

			for (int i = 0; i <= 6; i++)
			{
				if (i >= rating)
				{
					return i;
				}
			}

			return 0;
		}
	}
}