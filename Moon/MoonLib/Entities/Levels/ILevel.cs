using Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MoonLib.Entities.Levels
{
	public interface ILevel
	{
		bool Completed { get; }
		bool Failed { get; }
		int Score { get; }

		void Initialize(ContentManager contentManager, IAudioHandler audioHandler);
		void Reset();
		void Update(GameTimerEventArgs e);
		void Draw(SpriteBatch spriteBatch);
		void Move(Vector2 velocity);
	}
}