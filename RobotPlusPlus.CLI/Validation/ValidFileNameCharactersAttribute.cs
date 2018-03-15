using System.ComponentModel.DataAnnotations;
using System.IO;

namespace RobotPlusPlus.CLI.Validation
{
	public class ValidFileNameCharactersAttribute : ValidationAttribute
	{
		public ValidFileNameCharactersAttribute()
			: base("The file name for {0} contains invalid characters")
		{ }

		protected override ValidationResult IsValid(object value, ValidationContext context)
		{
			// Null, whitespace, or invalid chars
			if (string.IsNullOrWhiteSpace(value as string) || ContainsInvalidChars((string)value))
				return new ValidationResult(FormatErrorMessage(context.DisplayName));

			return ValidationResult.Success;
		}

		public static bool ContainsInvalidChars(string filename)
		{
			return filename.IndexOfAny(Path.GetInvalidFileNameChars()) != -1;
		}
	}
}