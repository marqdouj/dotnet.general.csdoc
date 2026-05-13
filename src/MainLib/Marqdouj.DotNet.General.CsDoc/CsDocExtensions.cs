using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Marqdouj.DotNet.General.CsDoc
{
    /// <summary>
    /// 
    /// </summary>
    public static class CsDocExtensions
    {
        extension(MemberInfo member)
        {
            /// <summary>
            /// Resolves the full Xml documenation name for a <see cref="MemberInfo"/>
            /// </summary>
            /// <param name="cleanName">The fullname without generic type suffix i.e. 'MyMethod``1' resolves to 'MyMethod'</param>
            /// <param name="parameters">The xml documentation parameters signature</param>
            /// <returns></returns>
            internal string GetXmlDocMemberName(out string cleanName, out string parameters)
            {
                ArgumentNullException.ThrowIfNull(member);

                cleanName = "";
                parameters = "";

                var prefixCode = member.MemberType switch
                {
                    MemberTypes.Constructor or MemberTypes.Method => 'M',
                    MemberTypes.Event => 'E',
                    MemberTypes.Field => 'F',
                    MemberTypes.Property => 'P',
                    MemberTypes.TypeInfo or MemberTypes.NestedType => 'T',
                    _ => ' ',
                };

                if (string.IsNullOrWhiteSpace(prefixCode.ToString()))
                    return "";

                var memberName = (member.MemberType == MemberTypes.Constructor) ? "#ctor" : member.Name;
                var fullName = "";

                if (member.DeclaringType != null)
                {
                    var tMemberName = member.DeclaringType.FullName ?? "<unresolved>";
                    var idx = tMemberName.IndexOf('[');
                    if (idx > 0)
                        tMemberName = tMemberName[..idx];
                    fullName = $"{tMemberName}.{memberName}";
                }
                else
                {
                    var tMember = (member as Type);
                    fullName = $"{(member as Type)?.Namespace ?? "<unresolved>"}.{memberName}";
                }

                cleanName = fullName;

                // Handle method parameters
                if (member is MethodBase method)
                {
                    var items = method.GetParameters();

                    if (items.Length > 0)
                    {
                        var nameSuffix = "";
                        var genIdx = 0;
                        var paramTypes = items
                            .Select(p => p.ParameterType.GetTypeName(ref genIdx))
                            .ToArray();

                        if (genIdx > 0)
                            nameSuffix = $"``{genIdx}";

                        parameters = $"({string.Join(",", paramTypes)})";
                        fullName = $"{fullName}{nameSuffix}{parameters}";
                    }
                }

                return $"{prefixCode}:{fullName}";
            }
        }

        extension(Type type)
        {
            private string? GetTypeName(ref int genIdx)
            {
                if (type.IsGenericType)
                {
                    var idx = 0;
                    var genericTypeName = type.GetGenericTypeDefinition().FullName;
                    genericTypeName = genericTypeName?[..genericTypeName.IndexOf('`')];
                    var genericArgs = string.Join(",", type.GetGenericArguments().Select(t => t.GetTypeName(ref idx)));
                    return $"{genericTypeName}{{{genericArgs}}}";
                }
                else if (type.IsGenericParameter)
                {
                    var genParam = $"``{genIdx}";
                    genIdx++;
                    return genParam;
                }

                return type.FullName;
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
