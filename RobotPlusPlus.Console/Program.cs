
namespace RobotPlusPlus.Console
{
	using System;

	public class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			Console.WriteLine(string.Join("\n", args));
			Console.ReadKey(true);
		}
	}
}
