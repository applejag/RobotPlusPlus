namespace RobotPlusPlus.Core.Structures.G1ANT
{
	public struct Point
	{
		public int X { get; set; }
		public int Y { get; set; }
		public bool IsEmpty => X == 0 && Y == 0;
	}
}