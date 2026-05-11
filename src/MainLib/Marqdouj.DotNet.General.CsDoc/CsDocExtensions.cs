using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace Marqdouj.DotNet.General.CsDoc
{
    /// <summary>
    /// 
    /// </summary>
    public static class CsDocExtensions
    {
        extension(MethodInfo info)
        {
            /// <summary>
            /// Builds XmlDocument parameter signature for a method i.e. (System.Double,System.String).
            /// </summary>
            /// <returns></returns>
            public string BuildParameters(out string nameSuffix)
            {
                nameSuffix = "";
                var parameters = info.GetParameters().ToList();
                var mParameters = "";

                if (parameters.Count > 0)
                {
                    var sb = new StringBuilder();
                    var genIdx = 0;

                    foreach (var param in parameters)
                    {
                        if (sb.Length > 0)
                            sb.Append(',');

                        var type = param.ParameterType;

                        if (type.IsValueType || type == typeof(string))
                        {
                            sb.Append(param.ParameterType.FullName);
                        }
                        else if (param.IsIEnumerable())
                        {
                            var name = type.FullName ?? type.Name;

                            // If you want to remove the generic arity suffix (`1, `2, etc.)
                            string cleanName = type.IsGenericType
                                ? name[..name.IndexOf('`')]
                                : name;

                            var arg = type.GenericTypeArguments?.FirstOrDefault();
                            var argValue = $"{{{arg?.FullName}}}" ?? "";
                            sb.Append($"{cleanName}{argValue}");
                        }
                        else
                        {
                            if (type.IsGenericParameter)
                            {
                                sb.Append($"``{genIdx}");
                                genIdx++;
                            }
                            else
                            {
                                sb.Append(type.FullName);
                            }
                        }
                    }

                    if (genIdx > 0)
                        nameSuffix = $"``{genIdx}";

                    if (sb.Length > 0)
                    {
                        sb.Insert(0, '(');
                        sb.Append(')');
                    }

                    mParameters = sb.ToString();
                    //Console.WriteLine(mParameters);
                }

                return mParameters;
            }
        }

        extension(ParameterInfo parameter)
        {
            /// <summary>
            /// Checks if a ParameterInfo represents an IEnumerable (generic or non-generic).
            /// </summary>
            bool IsIEnumerable()
            {
                if (parameter == null) return false;

                Type type = parameter.ParameterType;

                // Handle arrays (they are IEnumerable)
                if (type.IsArray) return true;

                // Check non-generic IEnumerable
                if (typeof(IEnumerable).IsAssignableFrom(type)) return true;

                // Check generic IEnumerable<T>
                if (type.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                {
                    return true;
                }

                return false;
            }
        }

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
