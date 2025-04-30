using System.Text.RegularExpressions;

namespace Tazkartk.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValidEmail(this string Email)
        {
            string pattern = "^[^@]+@[^@]+\\.[^@]+$";
            return Regex.IsMatch(Email, pattern);
        }
    }
}
