using BitByByte.Models;
using System.ComponentModel.DataAnnotations;

namespace BitByByte.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class EndTimeValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (Sitting)validationContext.ObjectInstance;
            DateTime startTime = model.StartTime;
            DateTime endTime = model.EndTime;

            if (endTime <= startTime)
            {
                return new ValidationResult("The End Time must be greater than the Start Time.");
            }

            return ValidationResult.Success;
        }
    }
}
