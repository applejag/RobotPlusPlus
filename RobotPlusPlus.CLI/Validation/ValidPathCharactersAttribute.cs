using System.ComponentModel.DataAnnotations;
using System.IO;

namespace RobotPlusPlus.CLI.Validation
{
	public class ValidPathCharactersAttribute : ValidationAttribute
	{
		public ValidPathCharactersAttribute()
			: base("The path for {0} contains invalid characters")
		{ }

		protected override ValidationResult IsValid(object value, ValidationContext context)
		{
			// Null, whitespace, or invalid chars
			if (string.IsNullOrWhiteSpace(value as string) || ContainsInvalidChars((string)value))
				return new ValidationResult(FormatErrorMessage(context.DisplayName));

			return ValidationResult.Success;
		}

		public static bool ContainsInvalidChars(string path)
		{
			return path.IndexOfAny(Path.GetInvalidPathChars()) != -1;
		}
	}
}