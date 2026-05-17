namespace MedCorVis.Common.Validations;

using System.ComponentModel.DataAnnotations;

public sealed class StrongPasswordAttribute : ValidationAttribute
{
    public StrongPasswordAttribute()
    {
        ErrorMessage = 
            "Password must contain at least one uppercase letter, one lowercase letter, and one digit.";
    }

    public override bool IsValid(object? value)
    {
        if (value is not string password) return false;

        return password.Any(char.IsUpper) &&
               password.Any(char.IsLower) &&
               password.Any(char.IsDigit);
    }
}