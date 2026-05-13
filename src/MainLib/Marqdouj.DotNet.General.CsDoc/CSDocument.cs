using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Xml.Linq;

namespace Marqdouj.DotNet.General.CsDoc
{
    /// <summary>
    ///  Collection of <see cref="CSDocumentItem"/> based on the <see cref="Type"/>.
    ///  Items are added to the collection if they have an associated <see cref="DisplayAttribute"/> or <see cref="CSDocumentXml"/>
    /// </summary>
    public interface ICSDocument
    {
        /// <summary>
        /// The <see cref="System.Type"/> associated with the xml documenation.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Readonly collection of <see cref="CSDocumentItem"/> based on the <see cref="Type"/>.
        /// </summary>
        IReadOnlyCollection<CSDocumentItem> Items { get; }

        /// <summary>
        /// Get the first <see cref="CSDocumentItem"/> with the <paramref name="name"/>
        /// </summary>
        /// <param name="name"><see cref="CSDocumentItem.Name"/></param>
        /// <param name="parameters"><see cref="CSDocumentXml.Parameters"/></param>
        /// <returns><see cref="CSDocumentItem"/></returns>
        CSDocumentItem? GetItem(string name, string parameters = "");

        /// <summary>
        /// Get all <see cref="CSDocumentItem"/> with the same <paramref name="name"/>
        /// </summary>
        /// <param name="name"><see cref="CSDocumentItem.Name"/></param>
        /// <returns><see cref="List{CSDocumentItem}"/></returns>
        List<CSDocumentItem> GetItems(string name);

        /// <summary>
        /// Get the first <see cref="CSDocumentItem"/> with the <paramref name="memberType"/> and <paramref name="name"/>
        /// </summary>
        /// <param name="memberType"><see cref="MemberTypes"/></param>
        /// <param name="name"><see cref="CSDocumentItem.Name"/></param>
        /// <param name="parameters"><see cref="CSDocumentXml.Parameters"/></param>
        /// <returns><see cref="CSDocumentItem"/></returns>
        CSDocumentItem? GetItem(MemberTypes memberType, string name, string parameters = "");
    }

    internal class CSDocument : ICSDocument
    {
        private readonly List<CSDocumentItem> items = [];

        public CSDocument(Type type, XDocument? xmlDoc, bool allMembers)
        {
            Type = type;
            Items = new ReadOnlyCollection<CSDocumentItem>(items);
            ProcessItems(xmlDoc, allMembers);
        }

        public Type Type { get; }

        public IReadOnlyCollection<CSDocumentItem> Items { get; }

        public CSDocumentItem? GetItem(string name, string parameters = "")
        {
            var item = Items.FirstOrDefault(e =>
                e.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                e.Parameters.Equals(parameters, StringComparison.OrdinalIgnoreCase));

            if (item == null)
            {
                var items = GetItems(name);
                if (items.Count == 1)
                    item = items[0];
            }

            return item;
        }

        public CSDocumentItem? GetItem(MemberTypes memberType, string name, string parameters = "")
        {
            return Items.FirstOrDefault(e =>
                e.MemberType == memberType &&
                e.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                e.Parameters.Equals(parameters, StringComparison.OrdinalIgnoreCase));
        }

        public List<CSDocumentItem> GetItems(string name)
        {
            return [.. Items.Where(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase))];
        }

        private void ProcessItems(XDocument? xmlDoc, bool allMembers)
        {
            var name = Type.GetXmlDocMemberName(out var cleanName, out var parameters);
            var node = xmlDoc?.Descendants("member").FirstOrDefault(m => m.Attribute("name")?.Value == name);
            var attribute = Type.GetDisplayAttribute();
            CSDocumentXml? comment = null;

            if (node != null)
                comment = new CSDocumentXml(node, name, Type.Name);

            if (allMembers || attribute != null || comment != null)
                items.Add(new CSDocumentItem(Type.Name, MemberTypes.TypeInfo, attribute, comment));

            if (Type.IsEnum)
            {
                ProcessEnum(xmlDoc, allMembers, ref attribute, ref comment);
            }
            else
            {
                ProcessMembers(Type, xmlDoc, allMembers);

                if (Type.IsInterface)
                {
                    var derivedInterfaces = Type.Assembly
                        .GetTypes()
                        .Where(t => t.IsInterface && t != Type && Type.IsAssignableTo(t))
                        .ToList();

                    foreach (var derivedInterface in derivedInterfaces)
                    {
                        var members = derivedInterface.GetMembers();

                        ProcessMembers(derivedInterface, xmlDoc, allMembers);
                    }
                }
            }
        }

        private void ProcessMembers(Type type, XDocument? xmlDoc, bool allMembers)
        {
            foreach (var member in type.GetMembers())
            {
                if (member is MethodInfo m)
                {
                    //Ignore methods such as 'get_' or 'set_' (accessors) etc.
                    if (m.IsSpecialName)
                        continue;

                    //Ignore built-in methods such as 'GetType()', 'GetHashCode()' etc. unless they are overrides.
                    var isCustom = m.DeclaringType == type;
                    if (!isCustom)
                    {
                        bool isOverride = m.GetBaseDefinition().DeclaringType != m.DeclaringType;
                        if (!isOverride) continue;
                    }
                }

                var name = member.GetXmlDocMemberName(out var cleanName, out var parameters);
                var attribute = member.GetDisplayAttribute();
                CSDocumentXml? comment = null;

                var mNode = xmlDoc?.Descendants("member").FirstOrDefault(m => m.Attribute("name")?.Value == name);

                if (mNode != null)
                    comment = new CSDocumentXml(mNode, name, member.Name, parameters);

                var isConstructor = member.MemberType == MemberTypes.Constructor;

                if ((allMembers && !isConstructor) || attribute != null || comment != null)
                    items.Add(new CSDocumentItem(member.Name, member.MemberType, attribute, comment));
            }
        }

        private void ProcessEnum(XDocument? xmlDoc, bool allMembers, ref DisplayAttribute? attribute, ref CSDocumentXml? comment)
        {
            var memberType = "F";
            foreach (Enum member in Enum.GetValues(Type))
            {
                var memberName = member.ToString()!;
                var value = Type.GetField(memberName);
                attribute = value?.GetDisplayAttribute();
                comment = null;

                var propName = $"{memberType}:{Type.FullName}.{memberName}";
                var propNode = xmlDoc?.Descendants("member").FirstOrDefault(m => m.Attribute("name")?.Value == propName);

                if (propNode != null)
                    comment = new CSDocumentXml(propNode, propName, memberName);

                if (allMembers || attribute != null || comment != null)
                    items.Add(new CSDocumentItem(memberName, MemberTypes.Field, attribute, comment));
            }
        }
    }
}
