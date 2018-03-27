using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace RobotPlusPlus.Core.Utility
{
	public static class ResourcesUtilities
	{
		public static Stream GetResourceStream(string resource)
		{
			return Assembly.GetExecutingAssembly()
				.GetManifestResourceStream(resource);
		}

		public static XmlSchema LoadEmbeddedXMLSchema(string resource,
			ValidationEventHandler validationEventHandler)
		{
			using (Stream stream = GetResourceStream(resource))
				return XmlSchema.Read(stream, validationEventHandler);
		}

		public static void AddEmbedded(this XmlSchemaSet schemas, string resource)
		{
			schemas.Add(LoadEmbeddedXMLSchema(resource, null));
		}

		public static void ValidateWithEmbedded(this XDocument source, string resource)
		{
			var schemas = new XmlSchemaSet();
			schemas.AddEmbedded(resource);
			source.Validate(schemas, null);
		}

		public static XDocument LoadEmbeddedXML(string resource)
		{
			using (Stream stream = GetResourceStream(resource))
				return XDocument.Load(stream);
		}

		public static XDocument LoadAndValidateEmbeddedXML(string xmlResource, string xsdResource)
		{
			XDocument xml = LoadEmbeddedXML(xmlResource);
			xml.ValidateWithEmbedded(xsdResource);
			return xml;
		}
	}
}