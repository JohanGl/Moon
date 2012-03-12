using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MoonLib.Entities.Backgrounds
{
	public class DefaultBackground
	{
		private List<Texture2D> overlays;

		public void Initialize(ContentManager contentManager)
		{
			overlays = new List<Texture2D>();
			overlays.Add(contentManager.Load<Texture2D>("Backgrounds/Background01"));
		}

		public void Update(GameTimerEventArgs e)
		{
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(overlays[0], new Vector2(0, 0), Color.White);
		}
	}
}