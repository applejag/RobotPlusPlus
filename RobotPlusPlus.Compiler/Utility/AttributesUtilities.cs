using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace RobotPlusPlus.Utility
{
	public static class AttributesUtilities
	{
		public static TAttr GetEnumAttribute<TAttr>(this Enum enumVal) where TAttr : Attribute
		{
			return enumVal.GetEnumAttributes<TAttr>().FirstOrDefault();
		}

		public static TAttr[] GetEnumAttributes<TAttr>(this Enum enumVal) where TAttr : Attribute
		{
			MemberInfo[] memInfo = enumVal.GetType().GetMember(enumVal.ToString());
			object[] attributes = memInfo[0].GetCustomAttributes(typeof(TAttr), true);
			return (TAttr[])attributes;
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