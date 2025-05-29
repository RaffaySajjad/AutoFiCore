using AutoFiCore.Models;

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
        public static string? ValidateMileage(int? mileage)
        {

            if (mileage.HasValue && mileage < 0)
                return "'mileage' must be greater than 0.";
            return null;
        }
        public static string? ValidateMakeOrModel(string makeORmodel)
        {
            if (string.IsNullOrWhiteSpace(makeORmodel) || makeORmodel is null)
                return "'make' is required.";

            return null;
        }
        public static bool ValidatePrice(decimal? startPrice, decimal? endPrice)
        {
            if (startPrice.HasValue && endPrice.HasValue)
            {
                return startPrice.Value <= endPrice.Value;
            }

            return true;
        }

        public static string? ValidateStringField(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return $"{fieldName} is required and cannot be empty.";
            }
            return null;
        }
     
        public static List<string> ValidateContactInfo(ContactInfo contactInfo)
        {
            var errors = new List<string>();

            void AddError(string? error)
            {
                if (!string.IsNullOrWhiteSpace(error))
                    errors.Add(error);
            }

            AddError(ValidateStringField(contactInfo.FirstName, "FirstName"));
            AddError(ValidateStringField(contactInfo.LastName, "LastName"));
            AddError(ValidateStringField(contactInfo.SelectedOption, "SelectedOption"));
            AddError(ValidateStringField(contactInfo.VehicleName, "VehicleName"));
            AddError(ValidateStringField(contactInfo.PostCode, "PostCode"));
            AddError(ValidateStringField(contactInfo.Email, "Email"));
            AddError(ValidateStringField(contactInfo.PhoneNumber, "PhoneNumber"));
            AddError(ValidateStringField(contactInfo.PreferredContactMethod, "PreferredContactMethod"));

            return errors;
        }
    }
}
