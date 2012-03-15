using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonLib.Contexts;
using MoonLib.Scenes.Levels;
using MoonLib.Services;

namespace MoonLib.Scenes.LevelScenes.Levels.Snapshots
{
	public class LevelSnapshotHandler
	{
		public List<byte[]> Images = new List<byte[]>();

		public void SaveSnapshots()
		{
			Images.Clear();
			SaveSnapshot(new Level01(), 1);
		}

		public void SaveSnapshot(ILevel level, int id)
		{
			var context = ServiceLocator.Get<GameContext>();

			var target = new RenderTarget2D(context.GraphicsDevice, 480, 800);

			context.GraphicsDevice.SetRenderTarget(target);

			context.SpriteBatch.Begin();

			level.Initialize(context);
			level.Update(new GameTimerEventArgs(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(0)));
			level.Draw(context.SpriteBatch);

			context.SpriteBatch.End();

			context.GraphicsDevice.SetRenderTarget(null);

			//Stream stream = File.OpenWrite(string.Format("d:\\temp\\snapshots\\Level{0:00}.png", id));
			
			var stream = new MemoryStream();
			target.SaveAsPng(stream, 480, 800);

			Images.Add(stream.ToArray());

			stream.Close();
			target.Dispose();
		}
	}
}
