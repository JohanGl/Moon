using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Contexts;

namespace MoonLib.Scenes.Levels
{
	public interface ILevel
	{
		bool Completed { get; }
		bool Failed { get; }
		int Score { get; }

		void Initialize(GameContext context);
		void Reset();
		void Update(GameTimerEventArgs e);
		void Draw(SpriteBatch spriteBatch);
		void Move(Vector2 velocity);
	}
}