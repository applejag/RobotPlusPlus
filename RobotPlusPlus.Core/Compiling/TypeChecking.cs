using System;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Structures.G1ANT;
using RobotPlusPlus.Core.Tokenizing.Tokens;
using RobotPlusPlus.Core.Tokenizing.Tokens.Literals;

namespace RobotPlusPlus.Core.Compiling
{
	public static class TypeChecking
	{
		[NotNull, Pure]
		public static Type GetValueType([NotNull] this MemberInfo info)
		{
			switch (info)
			{
				case FieldInfo field:
					return field.FieldType;

				case PropertyInfo property:
					return property.PropertyType;

				default:
					throw new NotImplementedException();
			}
		}

		[Pure]
		public static bool CanRead([NotNull] this MemberInfo info)
		{
			switch (info)
			{
				case FieldInfo field:
					return field.IsPublic;

				case PropertyInfo property:
					return property.CanRead;

				default:
					return false;
			}
		}

		[Pure]
		public static bool CanWrite([NotNull] this MemberInfo info)
		{
			switch (info)
			{
				case FieldInfo field:
					return !field.IsInitOnly && !field.IsLiteral;

				case PropertyInfo property:
					return property.CanWrite;

				default:
					return false;
			}
		}

		[Pure, CanBeNull]
		public static Type GetReturnTypeG1ANT([NotNull] this MethodInfo info)
		{
			if (info is G1ANTMethodInfo g1)
				return g1.ResultType;

			return info.ReturnType;
		}

		[Pure]
		public static bool CanImplicitlyConvert(Type from, Type to)
		{
			// I feel filthy using this try-catch method, but until a better solution comes...
			try
			{
				// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
				Expression.Convert(Expression.Parameter(from, null), to);
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// x&gt;y, x&gt;=y, x&lt;y, x&lt;=y
		/// </summary>
		[CanBeNull, Pure]
		public static Type CanGreaterThan(Type x, Type y)
		{
			try
			{
				dynamic xVal = TryCreate(x);
				dynamic yVal = TryCreate(y);
				dynamic result = xVal > yVal;
				return xVal != null && yVal != null
					? result.GetType() : null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// x&gt;=y
		/// </summary>
		[CanBeNull, Pure]
		public static Type CanGreaterThanOrEqual(Type x, Type y)
		{
			try
			{
				dynamic xVal = TryCreate(x);
				dynamic yVal = TryCreate(y);
				dynamic result = xVal >= yVal;
				return xVal != null && yVal != null
					? result.GetType() : null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// x&lt;y
		/// </summary>
		[CanBeNull, Pure]
		public static Type CanLessThan(Type x, Type y)
		{
			try
			{
				dynamic xVal = TryCreate(x);
				dynamic yVal = TryCreate(y);
				dynamic result = xVal < yVal;
				return xVal != null && yVal != null
					? result.GetType() : null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// x&lt;=y
		/// </summary>
		[CanBeNull, Pure]
		public static Type CanLessThanOrEqual(Type x, Type y)
		{
			try
			{
				dynamic xVal = TryCreate(x);
				dynamic yVal = TryCreate(y);
				dynamic result = xVal <= yVal;
				return xVal != null && yVal != null
					? result.GetType() : null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// x&lt;&lt;y
		/// </summary>
		[CanBeNull, Pure]
		public static Type CanShiftLeft(Type x, Type y)
		{
			try
			{
				dynamic xVal = TryCreate(x);
				dynamic yVal = TryCreate(y);
				dynamic result = (xVal << yVal);
				return xVal != null && yVal != null
					? result.GetType() : null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// x&gt;&gt;y
		/// </summary>
		[CanBeNull, Pure]
		public static Type CanShiftRight(Type x, Type y)
		{
			try
			{
				dynamic xVal = TryCreate(x);
				dynamic yVal = TryCreate(y);
				dynamic result = (xVal >> yVal);
				return xVal != null && yVal != null
					? result.GetType() : null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// x+y
		/// </summary>
		[CanBeNull, Pure]
		public static Type CanAdd(Type x, Type y)
		{
			try
			{
				dynamic xVal = TryCreate(x);
				dynamic yVal = TryCreate(y);
				dynamic result = (xVal + yVal);
				return xVal != null && yVal != null
					? result.GetType() : null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// x-y
		/// </summary>
		[CanBeNull, Pure]
		public static Type CanSubtract(Type x, Type y)
		{
			try
			{
				dynamic xVal = TryCreate(x);
				dynamic yVal = TryCreate(y);
				dynamic result = (xVal - yVal);
				return xVal != null && yVal != null
					? result.GetType() : null;
			}
			catch
			{
				return null;
			}
		}


		/// <summary>
		/// +x, -x
		/// </summary>
		[CanBeNull, Pure]
		public static Type CanUnaryAdditive(Type x)
		{
			try
			{
				dynamic xVal = TryCreate(x);
				dynamic result = (+xVal);
				return xVal != null
					? result.GetType() : null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// -x
		/// </summary>
		[CanBeNull, Pure]
		public static Type CanUnarySubtractive(Type x)
		{
			try
			{
				dynamic xVal = TryCreate(x);
				dynamic result = (-xVal);
				return xVal != null
					? result.GetType() : null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// ~x
		/// </summary>
		[CanBeNull, Pure]
		public static Type CanUnaryBitwiseNegate(Type x)
		{
			try
			{
				dynamic xVal = TryCreate(x);
				dynamic result = (~xVal);
				return xVal != null
					? result.GetType() : null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// x*y
		/// </summary>
		[CanBeNull, Pure]
		public static Type CanMultiplicate(Type x, Type y)
		{
			try
			{
				dynamic xVal = TryCreate(x);
				dynamic yVal = TryCreate(y);
				dynamic result = (xVal * yVal);
				return xVal != null && yVal != null
					? result.GetType() : null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// x/y
		/// </summary>
		[CanBeNull, Pure]
		public static Type CanDivide(Type x, Type y)
		{
			try
			{
				dynamic xVal = TryCreate(x);
				dynamic yVal = TryCreate(y);
				dynamic result = (xVal / yVal);
				return xVal != null && yVal != null
					? result.GetType() : null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// x%y
		/// </summary>
		[CanBeNull, Pure]
		public static Type CanModulus(Type x, Type y)
		{
			try
			{
				dynamic xVal = TryCreate(x);
				dynamic yVal = TryCreate(y);
				dynamic result = (xVal % yVal);
				return xVal != null && yVal != null
					? result.GetType() : null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// x|y
		/// </summary>
		[CanBeNull, Pure]
		public static Type CanLogicalOr(Type x, Type y)
		{
			try
			{
				dynamic xVal = TryCreate(x);
				dynamic yVal = TryCreate(y);
				dynamic result = (xVal | yVal);
				return xVal != null && yVal != null
					? result.GetType() : null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// x&amp;y
		/// </summary>
		[CanBeNull, Pure]
		public static Type CanLogicalAnd(Type x, Type y)
		{
			try
			{
				dynamic xVal = TryCreate(x);
				dynamic yVal = TryCreate(y);
				dynamic result = (xVal & yVal);
				return xVal != null && yVal != null
					? result.GetType() : null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// x^y
		/// </summary>
		[CanBeNull, Pure]
		public static Type CanLogicalXor(Type x, Type y)
		{
			try
			{
				dynamic xVal = TryCreate(x);
				dynamic yVal = TryCreate(y);
				dynamic result = (xVal ^ yVal);
				return xVal != null && yVal != null
					? result.GetType() : null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// !x
		/// </summary>
		[CanBeNull, Pure]
		public static Type CanNegate(Type x)
		{
			try
			{
				dynamic xVal = TryCreate(x);
				dynamic result = (!xVal);
				return xVal != null
					? result.GetType() : null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// x==y
		/// </summary>
		[CanBeNull, Pure]
		public static Type CanEqual(Type x, Type y)
		{
			try
			{
				dynamic xVal = TryCreate(x);
				dynamic yVal = TryCreate(y);
				dynamic result = (xVal == yVal);
				return xVal != null && yVal != null
					? result.GetType() : null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// x!=y
		/// </summary>
		[CanBeNull, Pure]
		public static Type CanNotEqual(Type x, Type y)
		{
			try
			{
				dynamic xVal = TryCreate(x);
				dynamic yVal = TryCreate(y);
				dynamic result = (xVal != yVal);
				return xVal != null && yVal != null
					? result.GetType() : null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// x&amp;&amp;y
		/// </summary>
		[CanBeNull, Pure]
		public static Type CanConditionalAnd(Type x, Type y)
		{
			try
			{
				dynamic xVal = TryCreate(x);
				dynamic yVal = TryCreate(y);
				dynamic result = (xVal && yVal);
				return xVal != null && yVal != null
					? result.GetType() : null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// x||y
		/// </summary>
		[CanBeNull, Pure]
		public static Type CanConditionalOr(Type x, Type y)
		{
			try
			{
				dynamic xVal = TryCreate(x);
				dynamic yVal = TryCreate(y);
				dynamic result = (xVal || yVal);
				return xVal != null && yVal != null
					? result.GetType() : null;
			}
			catch
			{
				return null;
			}
		}

		[Pure]
		private static dynamic TryCreate(Type type)
		{
			try
			{
				return type == typeof(string)
					? string.Empty : Activator.CreateInstance(type);
			}
			catch
			{
				return default;
			}
		}
		
	}
}