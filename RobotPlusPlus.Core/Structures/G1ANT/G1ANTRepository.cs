using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Compiling.Context.Types;
using RobotPlusPlus.Core.Tokenizing.Tokens;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Structures.G1ANT
{
	[XmlRoot("G1ANT", Namespace = "my://g1ant")]
	[Serializable]
	public class G1ANTRepository
	{
		[XmlElement("Commands")]
		public CommandsElement Commands { get; set; }

		[CanBeNull]
		public CommandElement FindCommand([NotNull] string commandName, [CanBeNull] string familyName = null)
		{
			List<CommandElement> pool = familyName != null
				? FindCommandFamily(familyName)?.Commands
				: Commands.Commands;

			return pool?.FirstOrDefault(c => c.Name == commandName);
		}

		[CanBeNull]
		public CommandFamilyElement FindCommandFamily([NotNull] string familyName)
		{
			return Commands.CommandFamilies.FirstOrDefault(fam => fam.Name == familyName);
		}

		[NotNull, ItemNotNull]
		public List<ArgumentElement> ListCommandArguments([NotNull] CommandElement command, bool includeGlobal = true)
		{
			return command.Arguments
				.Concat(Commands.GlobalArguments.Arguments)
				.ToList();
		}

		#region Static creators

		public static G1ANTRepository FromXDocument(XDocument document)
		{
			return SerializationUtilities.Deserialize<G1ANTRepository>(document);
		}

		public static G1ANTRepository FromEmbeddedXML(string xmlResource, string xsdResource)
		{
			XDocument doc = ResourcesUtilities.LoadAndValidateEmbeddedXML(xmlResource, xsdResource);
			return FromXDocument(doc);
		}

		public static G1ANTRepository FromEmbeddedXML()
		{
			return FromEmbeddedXML(
				xmlResource: nameof(RobotPlusPlus) + "." + nameof(Core) + ".XML.G1ANT.xml",
				xsdResource: nameof(RobotPlusPlus) + "." + nameof(Core) + ".XML.G1ANT.xsd"
			);
		}

		#endregion

		#region Element objects
		
		[Serializable]
		public class CommandsElement
		{
			[XmlElement("Command")]
			public List<CommandElement> Commands { get; set; }

			[XmlElement("CommandFamily")]
			public List<CommandFamilyElement> CommandFamilies { get; set; }

			[XmlElement("GlobalArguments")]
			public GlobalArgumentsElement GlobalArguments { get; set; }
		}

		[Serializable]
		public class GlobalArgumentsElement
		{
			[XmlElement("Argument")]
			public List<ArgumentElement> Arguments { get; set; }
		}

		[Serializable]
		public class CommandFamilyElement
		{
			[XmlAttribute("Name")]
			public string Name { get; set; }

			[XmlElement("Command")]
			public List<CommandElement> Commands { get; set; }

			public override string ToString()
			{
				return $"{Name}.{Commands?.Count ?? 0}";
			}
		}

		[Serializable]
		public class CommandElement
		{
			[XmlAttribute("Name")]
			public string Name { get; set; }

			[XmlElement("Argument")]
			public List<ArgumentElement> Arguments { get; set; }

			[XmlIgnore]
			public List<List<ArgumentElement>> RequiredArguments => 
				Arguments.Where(a => a.Required)
					.GroupBy(a => a.RequiredGroup == -1 ? a.Name.GetHashCode() : a.RequiredGroup,
						(k, elmList) => elmList.ToList())
					.ToList();

			public override string ToString()
			{
				return $"{Name}({string.Join(", ", Arguments)}";
			}
		}

		[Serializable]
		public class ArgumentElement
		{
			[XmlAttribute("Name")]
			public string Name { get; set; }

			[XmlAttribute("Type")]
			public ArgumentType Type { get; set; }

			[XmlAttribute("VariableType")]
			public ArgumentType VariableType { get; set; }

			[XmlAttribute("Required")]
			public bool Required { get; set; } = false;

			[XmlAttribute("RequiredGroup")]
			public int RequiredGroup { get; set; } = -1;
			
			public static Type EvaluateType(ArgumentType? type)
			{
				switch (type)
				{
					case ArgumentType.Undefined: return typeof(object);
					case ArgumentType.String: return typeof(string);
					case ArgumentType.Integer: return typeof(int);
					case ArgumentType.Float: return typeof(float);
					case ArgumentType.Boolean: return typeof(bool);
					case ArgumentType.Point: return typeof(Point);
					case ArgumentType.Rectangle: return typeof(Rectangle);
					case ArgumentType.Size: return typeof(Size);
					case ArgumentType.List: return typeof(List<object>);
					case ArgumentType.Variable: return typeof(Variable);
					case ArgumentType.VariableName: return typeof(string);
					case ArgumentType.Label: return typeof(Label);

					case ArgumentType.Procedure:
						throw new NotImplementedException();

					default:
						throw new FormatException($"Unkown type <{type?.ToString() ?? "null"}>!");
				}
			}

			public override string ToString()
			{
				var sb = new StringBuilder();

				sb.Append(Type);
				if (Type == ArgumentType.Variable)
					sb.AppendFormat("<{0}>",VariableType);
				if (!Required)
					sb.Append('?');
				sb.Append(' ');
				sb.Append(Name);

				return sb.ToString();
			}
		}

		[Serializable]
		public enum ArgumentType
		{
			[XmlEnum("undefined")]
			Undefined,

			[XmlEnum("string")]
			String,
			[XmlEnum("integer")]
			Integer,
			[XmlEnum("float")]
			Float,
			[XmlEnum("boolean")]
			Boolean,
			[XmlEnum("point")]
			Point,
			[XmlEnum("rectangle")]
			Rectangle,
			[XmlEnum("size")]
			Size,
			[XmlEnum("list")]
			List,
			[XmlEnum("dictionary")]
			Dictionary,
			[XmlEnum("label")]
			Label,
			[XmlEnum("variable")]
			Variable,
			[XmlEnum("variablename")]
			VariableName,
			[XmlEnum("procedure")]
			Procedure,
		}
		
		#endregion
	}
}