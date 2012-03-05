using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MoonLib.Entities.Items
{
	/// <summary>
	/// Marker class for all star types
	/// </summary>
	public interface IStar
	{
		void Update(GameTimerEventArgs e);
		void Draw(SpriteBatch spriteBatch);
	}
}