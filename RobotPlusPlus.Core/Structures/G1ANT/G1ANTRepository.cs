using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Compiling.Context;
using RobotPlusPlus.Core.Compiling.Context.Types;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Structures.G1ANT
{
	[XmlRoot("G1ANT", Namespace = "my://g1ant")]
	[Serializable]
	public class G1ANTRepository : IRepository
	{
		[XmlElement("Variables")]
		public VariablesElement Variables { get; set; }

		[XmlElement("Commands")]
		public CommandsElement Commands { get; set; }

		[NotNull, ItemNotNull]
		public List<ArgumentElement> ListCommandArguments([NotNull] CommandElement command, bool includeGlobal = true)
		{
			return command.Arguments
				.Concat(Commands.GlobalArguments.Arguments)
				.ToList();
		}

		public IEnumerable<(string id, Type type)> RegisterVariables()
		{
			return Variables.Variables
				.Select(v => (v.Name, v.EvaluateType()));
		}

		public IEnumerable<(string id, Type type)> RegisterReadOnlyVariables()
		{
			return new(string, Type)[0];
		}

		public IEnumerable<(string id, Type type)> RegisterStaticTypes()
		{
			return new(string id, Type type)[0];
		}

		public void RegisterOther(ValueContext context)
		{
			foreach (CommandElement command in Commands.Commands
				.Where(c => Commands.CommandFamilies.All(f => f.Name != c.Name)))
			{
				context.RegisterValueGlobally(new G1ANTCommand(command, Commands.GlobalArguments));
			}

			foreach (CommandFamilyElement family in Commands.CommandFamilies)
			{
				// Register hybrid family-command elements
				if (Commands.Commands.TryFirst(c => c.Name == family.Name, out CommandElement cmd))
					context.RegisterValueGlobally(new G1ANTFamilyCommand(family, Commands.GlobalArguments, cmd));
				// Register normal family
				else
					context.RegisterValueGlobally(new G1ANTFamily(family, Commands.GlobalArguments));
			}
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

		public static Type EvaluateType(Structure? type)
		{
			switch (type)
			{
				case Structure.Undefined: return typeof(object);
				case Structure.String: return typeof(string);
				case Structure.Integer: return typeof(int);
				case Structure.Float: return typeof(float);
				case Structure.Boolean: return typeof(bool);
				case Structure.Point: return typeof(Point);
				case Structure.Rectangle: return typeof(Rectangle);
				case Structure.Size: return typeof(Size);
				case Structure.List: return typeof(List<object>);
				case Structure.Date: return typeof(DateTime);
				case Structure.Time: return typeof(DateTime);
				case Structure.DateTime: return typeof(DateTime);
				case Structure.Variable: return typeof(Variable);
				case Structure.VariableName: return typeof(string);
				case Structure.Label: return typeof(Label);

				case Structure.HTML:
				case Structure.JSON:
				case Structure.XML:
				case Structure.Procedure:
					throw new NotImplementedException();

				default:
					throw new FormatException($"Unkown type <{type?.ToString() ?? "null"}>!");
			}
		}

		#endregion

		#region Element objects

		[Serializable]
		public class VariablesElement
		{
			[XmlElement("Variable")]
			public List<VariableElement> Variables { get; set; }
		}

		[Serializable]
		public class VariableElement
		{
			[XmlAttribute("Name")]
			public string Name { get; set; }

			[XmlAttribute("Type")]
			public Structure Type { get; set; }

			public Type EvaluateType()
			{
				return G1ANTRepository.EvaluateType(Type);
			}

			public override string ToString()
			{
				return $"{Name} ({Type})";
			}
		}

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

			[XmlElement("Overload")]
			public List<ArgumentOverloadElement> Overloads { get; set; }

			public override string ToString()
			{
				return $"{Name}({string.Join(", ", Arguments)}";
			}
		}

		[Serializable]
		public class ArgumentOverloadElement
		{
			[XmlElement("Argument")]
			public List<ArgumentElement> Arguments { get; set; }
		}

		[Serializable]
		public class ArgumentElement
		{
			[XmlAttribute("Name")]
			public string Name { get; set; }

			[XmlAttribute("Type")]
			public Structure Type { get; set; }

			[XmlAttribute("VariableType")]
			public Structure VariableType { get; set; }

			[XmlAttribute("Required")]
			public bool Required { get; set; } = false;

			public Type EvaluateType()
			{
				return G1ANTRepository.EvaluateType(Type);
			}

			public Type EvaluateVariableType()
			{
				if (Type == Structure.Variable)
					return G1ANTRepository.EvaluateType(VariableType);
				return EvaluateType();
			}

			public override string ToString()
			{
				var sb = new StringBuilder();

				sb.Append(Type);
				if (Type == Structure.Variable)
					sb.AppendFormat("<{0}>", VariableType);
				if (!Required)
					sb.Append('?');
				sb.Append(' ');
				sb.Append(Name);

				return sb.ToString();
			}
		}

		[Serializable]
		public enum Structure
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
			[XmlEnum("date")]
			Date,
			[XmlEnum("datetime")]
			DateTime,
			[XmlEnum("time")]
			Time,

			[XmlEnum("json")]
			JSON,
			[XmlEnum("html")]
			HTML,
			[XmlEnum("xml")]
			XML,
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