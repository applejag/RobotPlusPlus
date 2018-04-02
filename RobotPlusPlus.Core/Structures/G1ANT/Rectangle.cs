namespace RobotPlusPlus.Core.Structures.G1ANT
{
	public struct Rectangle
	{
		private Point _location;
		private Size _size;

		public Point Location
		{
			get => _location;
			set => _location = value;
		}

		public Size Size
		{
			get => _size;
			set => _size = value;
		}

		public int X
		{
			get => _location.X;
			set => _location.X = value;
		}

		public int Y
		{
			get => _location.Y;
			set => _location.Y = value;
		}

		public int Width
		{
			get => _size.Width;
			set => _size.Width = value;
		}

		public int Height
		{
			get => _size.Height;
			set => _size.Height = value;
		}

		public int Left => X;
		public int Top => Y;
		public int Right => X + Width;
		public int Bottom => Y + Height;

		public bool IsEmpty => Size.IsEmpty && Location.IsEmpty;
	}
}