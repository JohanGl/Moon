using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MoonLib.Entities.Items
{
	/// <summary>
	/// Marker class for all star types
	/// </summary>
	public interface IStar
	{
		int Id { get; set; }
		void Update(GameTimerEventArgs e);
		void Draw(SpriteBatch spriteBatch);
	}
}