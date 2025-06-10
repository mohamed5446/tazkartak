using System.Reflection;
using System.ComponentModel.DataAnnotations;


namespace Tazkartk.Application.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .FirstOrDefault()?
                            .GetCustomAttribute<DisplayAttribute>()?
                            .GetName()??enumValue.ToString();
        }
       
    }
}
