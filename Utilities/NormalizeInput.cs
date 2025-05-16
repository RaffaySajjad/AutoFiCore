namespace AutoFiCore.Utilities
{
    public class NormalizeInput
    {
        public static string? NormalizeMakeModel(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            input = input.Trim();

            if (input.Equals("Any_Makes", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("Any_Models", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return input;
        }
    }
}
