using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Contexts;

namespace MoonLib.Entities.Backgrounds
{
	public class DefaultBackground
	{
		private List<Texture2D> overlays;

		public void Initialize(GameContext context)
		{
			overlays = new List<Texture2D>();
			overlays.Add(context.Content.Load<Texture2D>("Backgrounds/Background01"));
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