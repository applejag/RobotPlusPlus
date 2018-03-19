using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace RobotPlusPlus.Linguist.Utility
{
	public static class AttributesUtilities
	{
		public static TAttr GetEnumAttribute<TAttr>(this Enum enumVal)
		{
			return enumVal.GetEnumAttributes<TAttr>().FirstOrDefault();
		}

		public static TAttr[] GetEnumAttributes<TAttr>(this Enum enumVal)
		{
			MemberInfo[] memInfo = enumVal.GetType().GetMember(enumVal.ToString());
			return memInfo[0].GetCustomAttributes(typeof(TAttr), true).OfType<TAttr>().ToArray();
		}

		public static TAttr GetPropertyAttribute<TClass, TAttr>(string propertyName)
		{
			return (TAttr)typeof(TClass).GetProperty(propertyName).GetCustomAttributes(typeof(TAttr), true)
				.FirstOrDefault();
		}

		public static TAttr[] GetPropertyAttributes<TClass, TAttr>(string propertyName)
		{
			return typeof(TClass).GetProperty(propertyName).GetCustomAttributes(typeof(TAttr), true)
				.Cast<TAttr>().ToArray();
		}

		public static class Presets
		{
			public static int? GetMinLength<TClass>(string propertyName)
			{
				return GetPropertyAttribute<TClass, MinLengthAttribute>(propertyName)?.Length;
			}

			public static int? GetMaxLength<TClass>(string propertyName)
			{
				return GetPropertyAttribute<TClass, MaxLengthAttribute>(propertyName)?.Length;
			}

			public static string GetDisplayName<TClass>(string propertyName)
			{
				return GetPropertyAttribute<TClass, DisplayAttribute>(propertyName)?.Name;
			}
		}
	}
}