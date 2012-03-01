namespace MoonLib.Helpers
{
	public static class DeviceHelper
	{
		public static int Width { get; private set; }
		public static int Height { get; private set; }

		public static int HalfWidth { get; private set; }
		public static int HalfHeight { get; private set; }

		static DeviceHelper()
		{
			Width = 480;
			Height = 800;

			HalfWidth = Width / 2;
			HalfHeight = Height / 2;
		}
	}
}