using CoreMVVM.Validation;
using System;

namespace CoreMVVM.Demo.Validation
{
    public class UsernameValidationAttribute : ValidationAttribute
    {
        protected override bool IsValid(object value)
        {
            if (value == null)
                return false;

            if (!(value is string input))
                throw new Exception("Invalid type");

            if (input.Length < 3 || input.Length > 16)
                return false;

            return true;
        }
    }
}