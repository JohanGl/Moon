using Microsoft.Xna.Framework;

namespace MoonLib.Helpers
{
	public static class Device
	{
		public static int Width { get; private set; }
		public static int Height { get; private set; }

		public static int HalfWidth { get; private set; }
		public static int HalfHeight { get; private set; }

		public static Rectangle Size { get; set; }

		static Device()
		{
			Width = 480;
			Height = 800;

			HalfWidth = Width / 2;
			HalfHeight = Height / 2;

			Size = new Rectangle(0, 0, Width, Height);
		}
	}
}