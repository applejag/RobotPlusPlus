using System;
using System.Diagnostics;

namespace RobotPlusPlus.Compiler
{
	public static class Parser
	{
		public static string Parse(string code)
		{
			if (code == null)
				throw new ArgumentNullException(nameof(code), "Code cannot be null!");

			return code;
		}
	}
}
