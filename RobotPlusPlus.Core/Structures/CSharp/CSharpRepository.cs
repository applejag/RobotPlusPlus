using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace RobotPlusPlus.Core.Structures.CSharp
{
	public class CSharpRepository : IRepository
	{
		public static readonly IReadOnlyDictionary<string, Type> families = new Dictionary<string, Type>
		{
			["string"] = typeof(string),
			["String"] = typeof(string),
			["int"] = typeof(int),
			["Int32"] = typeof(int),
			["long"] = typeof(long),
			["Int64"] = typeof(long),
			["double"] = typeof(double),
			["Double"] = typeof(double),
			["float"] = typeof(float),
			["Single"] = typeof(float),

			["DateTime"] = typeof(DateTime),
			["Rectangle"] = typeof(Rectangle),
			["Point"] = typeof(Point),
			["Size"] = typeof(Size),

			["Console"] = typeof(Console),
			["Array"] = typeof(Array),
			["List"] = typeof(List<object>),
		};

		public IEnumerable<(string id, Type type)> RegisterVariables()
		{
			return new(string id, Type type)[0];
		}

		public IEnumerable<(string id, Type type)> RegisterReadOnlyVariables()
		{
			return new(string id, Type type)[0];
		}

		public IEnumerable<(string id, Type type)> RegisterStaticTypes()
		{
			return families.Select(pair => (pair.Key, pair.Value));
		}

		public MethodInfo GetMethod(string family, string method, Type[] paramters)
		{
			return families.GetValueOrDefault(family)?
				.GetMethod(method, BindingFlags.Public
				                   | BindingFlags.Static
				                   | BindingFlags.Instance,
					null, paramters, null);
		}
	}
}