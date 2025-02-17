using System.ComponentModel.DataAnnotations;

namespace EventManagement_Application.Models
{
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime endDate)
            {
                var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
                if (property == null)
                {
                    return new ValidationResult($"Unknown property: {_comparisonProperty}");
                }

                var startDate = (DateTime)property.GetValue(validationContext.ObjectInstance);

                if (endDate <= startDate)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}
