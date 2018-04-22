using System;
using System.Collections.Generic;
using System.Drawing;
using RobotPlusPlus.Core.Compiling.Context;

namespace RobotPlusPlus.Core.Structures.CSharp
{
	public class CSharpRepository : IRepository
	{
		public IEnumerable<(string id, Type type)> RegisterVariables()
		{
			return new (string id, Type type)[0];
		}

		public IEnumerable<(string id, Type type)> RegisterReadOnlyVariables()
		{
			return new (string id, Type type)[0];
		}

		public IEnumerable<(string id, Type type)> RegisterStaticTypes()
		{
			return new[]
			{
				("string", typeof(string)),
				("String", typeof(string)),
				("int", typeof(int)),
				("Int32", typeof(int)),
				("long", typeof(long)),
				("Int64", typeof(long)),
				("double", typeof(double)),
				("Double", typeof(double)),
				("float", typeof(float)),
				("Single", typeof(float)),

				("DateTime", typeof(DateTime)),
				("Rectangle", typeof(Rectangle)),
				("Point", typeof(Point)),
				("Size", typeof(Size)),
			};
		}
	}
}