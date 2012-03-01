using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MoonLib.Entities.Backgrounds
{
	public class DefaultBackground
	{
		private List<Texture2D> overlays;
		private Color backgroundColor;

		public void Initialize(ContentManager contentManager)
		{
			backgroundColor = Color.FromNonPremultiplied(0, 0, 18, 255);

			overlays = new List<Texture2D>();
			overlays.Add(contentManager.Load<Texture2D>("Backgrounds/Background01"));
		}

		public void Update(GameTimerEventArgs e)
		{
		}

		public void Draw(GraphicsDevice device, SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(overlays[0], new Vector2(0, 0), Color.White);
		}
	}
}