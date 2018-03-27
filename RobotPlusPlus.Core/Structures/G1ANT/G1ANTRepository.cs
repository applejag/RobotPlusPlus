using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Tokenizing.Tokens;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Structures.G1ANT
{
	public class G1ANTRepository
	{
		public List<Argument> GlobalArguments = new List<Argument>();
		public List<Command> Commands = new List<Command>();

		private G1ANTRepository()
		{

		}

		public static G1ANTRepository FromEmbeddedXML(string xmlResource, string xsdResource)
		{
			XDocument doc = ResourcesUtilities.LoadAndValidateEmbeddedXML(xmlResource, xsdResource);
			XElement root = doc.Root ?? throw new NullReferenceException("Document contains no root element!");
			XNamespace ns = root.Name.Namespace;

			XElement commands = root.Element(ns + "Commands") ?? throw new NullReferenceException("Document contains no Commands list!");

			return new G1ANTRepository
			{
				Commands = commands.Elements(ns + "Command")
					.Select(c => new Command(c, c.Elements(ns + "Argument")
						.Select(a => new Argument(a))))
					.Concat(commands.Elements(ns + "CommandFamily")
						.SelectMany(f => f.Elements(ns + "Command")
							.Select(c => new Command(c, f, c.Elements(ns + "Argument")
								.Select(a => new Argument(a))))))
					.ToList(),

				GlobalArguments = commands.Element(ns + "GlobalArguments")
					?.Elements(ns + "Argument")
					.Select(a => new Argument(a))
					.ToList(),
			};
		}

		public static G1ANTRepository FromEmbeddedXML()
		{
			return FromEmbeddedXML(
				xmlResource: "RobotPlusPlus.Core.XML.G1ANT.xml",
				xsdResource: "RobotPlusPlus.Core.XML.G1ANT.xsd"
			);
		}

		public struct Command
		{
			public string Family;
			public string Name;
			public List<Argument> Arguments;

			public Command([NotNull] XElement element,
				[NotNull] XElement family,
				[NotNull] IEnumerable<Argument> arguments)
			{
				Family = family.Attribute("Name")?.Value
				         ?? throw new FormatException("Command family contains no Name attribute!");

				Name = element.Attribute("Name")?.Value
				       ?? throw new FormatException("Command contains no Name attribute!");

				Arguments = arguments.ToList();
			}

			public Command([NotNull] XElement element,
				[NotNull] IEnumerable<Argument> arguments)
			{
				Family = null;

				Name = element.Attribute("Name")?.Value
				       ?? throw new FormatException("Command contains no Name attribute!");

				Arguments = arguments.ToList();
			}

			public override string ToString()
			{
				return (Family==null ? Name : $"{Family}.{Name}") + $"({string.Join(", ", Arguments)})";
			}
		}

		public struct Argument
		{
			public string Name;
			public Type Type;
			public bool Required;

			public Argument([NotNull] XElement element)
			{
				Name = element.Attribute("Name")?.Value
					   ?? throw new FormatException("Argument contains no Name attribute!");
				Type = EvaluateType(element.Attribute("Type")?.Value);
				Required = ParseBool(element.Attribute("Required")?.Value);
			}

			public static bool ParseBool(string val)
			{
				val = val?.Trim().ToLower();
				switch (val)
				{
					case "true":
					case "1":
						return true;
					default:
						return false;
				}
			}

			public static Type EvaluateType(string type)
			{
				switch (type)
				{
					case "string": return typeof(string);
					case "integer": return typeof(int);
					case "boolean": return typeof(bool);
					case "point": return typeof(Point);
					case "rectangle": return typeof(Rectangle);
					case "list": return typeof(List<object>);
					case "variable": return typeof(IdentifierToken);
					case "variablename": return typeof(string);

					case "label":
					case "procedure":
						throw new NotImplementedException();

					default:
						throw new FormatException($"Unkown type <{type ?? "null"}>!");
				}
			}

			public override string ToString()
			{
				return $"{Type.Name} {Name}{(Required ? "" : "?")}";
			}
		}
	}
}