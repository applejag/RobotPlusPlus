using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace RobotPlusPlus.Core.Utility
{
	public class SerializationUtilities
	{
		[NotNull]
		public static T Deserialize<T>([NotNull] XDocument doc)
		{
			var xmlSerializer = new XmlSerializer(typeof(T));

			using (XmlReader reader = doc.Root?.CreateReader()
				?? throw new NullReferenceException("Document has no root element!"))
			{
				return (T)xmlSerializer.Deserialize(reader);
			}
		}

		[NotNull]
		public static XDocument Serialize<T>([NotNull] T value)
		{
			var xmlSerializer = new XmlSerializer(typeof(T));

			var doc = new XDocument();
			using (XmlWriter writer = doc.CreateWriter())
			{
				xmlSerializer.Serialize(writer, value);
			}

			return doc;
		}
	}
}