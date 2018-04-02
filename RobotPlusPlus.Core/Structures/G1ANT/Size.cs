namespace RobotPlusPlus.Core.Structures.G1ANT
{
	public struct Size
	{
		public int Width { get; set; }
		public int Height { get; set; }
		public bool IsEmpty => Width == 0 && Height == 0;
	}
}