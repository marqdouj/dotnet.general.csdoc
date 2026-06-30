using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;

namespace Marqdouj.DotNet.General.CsDoc
{
    /// <summary>
    /// Extension methods for XmlDocumentMember.
    /// </summary>
    public static class XmlDocumentMemberExtensions
    {
        /// <summary>
        /// Gets the first member that matches the <paramref name="name"/>.
        /// </summary>
        /// <param name="members"></param>
        /// <param name="name"><inheritdoc cref="XmlDocumentMember.Name"/></param>
        /// <returns></returns>
        public static XmlDocumentMember? GetMemberByName(this IEnumerable<XmlDocumentMember> members, string name)  
            => members?.FirstOrDefault(m =>  m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Gets all members that start with the <paramref name="name"/>.
        /// </summary>
        /// <param name="members"></param>
        /// <param name="name"><inheritdoc cref="XmlDocumentMember.Name"/></param>
        /// <returns></returns>
        public static List<XmlDocumentMember> GetMembersWithName(this IEnumerable<XmlDocumentMember> members, string name)
            => members?.Where(m => m.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase)).ToList() ?? [];

        /// <summary>
        /// Gets the first parameter that matches the <paramref name="name"/>.
        /// </summary>
        /// <param name="member"></param>
        /// <param name="name"><inheritdoc cref="XmlDocumentMemberParameter.Name"/></param>
        /// <returns></returns>
        public static XmlDocumentMemberParameter? GetParameterByName(this XmlDocumentMember member, string name)
            => member?.Parameters?.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        internal static void ProcessType(this List<XmlDocumentMember> members, Type type, bool addMissing)
        {
            var m = members.FirstOrDefault(m => m.MemberType == MemberTypes.TypeInfo && m.Name.Equals(type.Name));
            m?.Type = type;
            m?.DisplayAttribute = type.GetDisplayAttribute();
            Debug.Assert(m != null);

            if (type.IsEnum)
            {
                foreach (var tField in type
                    .GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(f => f.IsLiteral && !f.IsSpecialName))
                {
                    var fullname = $"F:{tField.DeclaringType?.FullName}.{tField.Name}";
                    var mMember = 
                        members.FirstOrDefault(m => m.Fullname.Equals(fullname, StringComparison.OrdinalIgnoreCase)) ??
                        members.AddMissingMember(fullname, addMissing);

                    mMember?.DisplayAttribute = tField.GetCustomAttribute<DisplayAttribute>();
                }
            }
            else
            {
                // Get public instance and static members (includes inherited)
                var tMembers = type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                                  // Exclude members declared on System.Object
                                  .Where(m => m.DeclaringType != typeof(object))
                                  .ToList();

                foreach (var tMember in tMembers)
                {
                    var fullname = tMember.GetXmlDocMemberName(out var cleanName, out var parameters);
                    var mMember = 
                        members.FirstOrDefault(m => m.Fullname.Equals(fullname, StringComparison.OrdinalIgnoreCase)) ?? 
                        members.AddMissingMember(fullname, addMissing);

                    mMember?.DisplayAttribute = tMember.GetDisplayAttribute();
                }
            }
        }

        private static XmlDocumentMember? AddMissingMember(this List<XmlDocumentMember> members, string fullname, bool addMissing)
        {
            XmlDocumentMember? mMember = null;

            if (addMissing)
            {
                var name = ParseNameFromFullname(fullname);
                mMember = new XmlDocumentMember(-999, fullname, name);
                members.Add(mMember);
            }

            return mMember;
        }

        internal static string ParseNameFromFullname(string refText)
        {
            var dotPosn = refText.LastIndexOf('.');
            var parenPosn = refText.IndexOf('(');
            var tickPosn = refText.IndexOf('`');
            var ctorPosn = refText.IndexOf("#ctor");
            var cutoffPosn = -1;

            if (ctorPosn > -1)
            {
                dotPosn = ctorPosn - 1;
            }
            else if (parenPosn > -1 && tickPosn > -1)
            {
                if (tickPosn < parenPosn)
                {
                    //If dot is in beween then use that posn.
                    var posn = refText.IndexOf('.', tickPosn);
                    if (posn > -1 && posn < parenPosn)
                    {
                        dotPosn = posn;
                    }
                    else if (dotPosn > parenPosn)
                    {
                        cutoffPosn = tickPosn;
                    }
                }
                else
                {
                    cutoffPosn = parenPosn;
                }
            }
            else if (parenPosn > -1)
            {
                cutoffPosn = parenPosn;
            }

            var name = "";

            if (cutoffPosn > -1)
            {
                var index = refText.IndexOf('.');
                dotPosn = index;

                while (index < cutoffPosn)
                {
                    index++;
                    var current = refText.IndexOf('.', index);


                    if (current < 0)
                    {
                        index = cutoffPosn;
                        continue;
                    }

                    if (current < cutoffPosn)
                        dotPosn = current;

                    index = current;
                }
            }

            if (dotPosn > -1)
                name = refText[(dotPosn + 1)..];

            return name;
        }
    }
}