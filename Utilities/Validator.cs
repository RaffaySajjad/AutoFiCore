namespace AutoFiCore.Utilities
{
    public class Validator
    {
        public static string? ValidatePagination(int pageView, int offset) { 
        
            if (pageView <= 0)
                return "'pageView' must be greater than 0.";
            if (offset < 0)
                return "'offset' must be 0 or greater.";
            return null;
        }
        public static string? ValidateMakeOrModel(string makeORmodel)
        {
            if (string.IsNullOrWhiteSpace(makeORmodel) || makeORmodel is null)
                return "'make' is required.";

            return null;
        }
    }
}
