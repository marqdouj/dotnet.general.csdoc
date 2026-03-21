using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Marqdouj.DotNet.General.CsDoc
{
    /// <summary>
    /// 
    /// </summary>
    public static class Extensions
    {
        extension(MemberInfo member)
        {
            /// <summary>
            /// Gets the DisplayAttribute applied to a member, if any.
            /// </summary>
            public DisplayAttribute? GetDisplayAttribute()
            {
                return member.GetCustomAttributes(typeof(DisplayAttribute), inherit: true)
                             .Cast<DisplayAttribute>()
                             .FirstOrDefault();
            }
        }

        extension<TEnum>(TEnum value) where TEnum : Enum
        {
            /// <summary>
            /// Get Display attribute name if present.
            /// </summary>
            /// <param name="useNameIfNotFound">Flag to use the Name (ToString()) if Display attribute is not found. Default is <see langword="true"/></param>
            /// <returns></returns>
            public string? GetDisplayName(bool useNameIfNotFound = true)
            {
                var name = value.ToString();
                var field = typeof(TEnum).GetField(name);
                var displayAttr = field?.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
                return displayAttr?.Name ?? (useNameIfNotFound ? name : null);
            }

            /// <summary>
            /// Gets the Display attribute (if present).
            /// </summary>
            /// <returns></returns>
            public DisplayAttribute? GetDisplayAttribute()
            {
                var name = value.ToString();
                var field = typeof(TEnum).GetField(name);
                var displayAttr = field?.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
                return displayAttr;
            }
        }

        extension(PropertyInfo prop)
        {
            /// <summary>
            /// Get Display attribute name if present.
            /// </summary>
            /// <param name="useNameIfNotFound">Flag to use the Name if Display attribute is not found. Default is <see langword="true"/></param>
            /// <returns></returns>
            public string? GetDisplayName(bool useNameIfNotFound = true)
            {
                var displayAttr = prop.GetCustomAttributes<DisplayAttribute>().FirstOrDefault();
                return displayAttr?.Name ?? (useNameIfNotFound ? prop.Name : null);
            }

            /// <summary>
            /// Gets the Display attribute (if present).
            /// </summary>
            /// <returns></returns>
            public DisplayAttribute? GetDisplayAttribute()
            {
                var attr = prop.GetCustomAttributes<DisplayAttribute>().FirstOrDefault();
                return attr;
            }
        }

        extension(Type type)
        {
            /// <summary>
            /// Get Display attribute name if present.
            /// </summary>
            /// <param name="useNameIfNotFound">Flag to use the Name if Display attribute is not found. Default is <see langword="true"/></param>
            /// <returns></returns>
            public string? GetDisplayName(bool useNameIfNotFound = true)
            {
                var attr = type
                    .GetCustomAttributes(typeof(DisplayAttribute), false)
                    .Cast<DisplayAttribute>()
                    .FirstOrDefault();

                return attr?.Name ?? (useNameIfNotFound ? type.Name : null);
            }

            /// <summary>
            /// Gets the Display attribute (if present).
            /// </summary>
            /// <returns></returns>
            public DisplayAttribute? GetDisplayAttribute()
            {
                var attr = type
                    .GetCustomAttributes(typeof(DisplayAttribute), false)
                    .Cast<DisplayAttribute>()
                    .FirstOrDefault();

                return attr;
            }
        }
    }
}
