using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Marqdouj.DotNet.General.CsDoc
{
    /// <summary>
    ///  Collection of <see cref="CSDocumentItem"/> for a class/enum.
    ///  Items are added to the collection if they have an associated <see cref="DisplayAttribute"/> or <see cref="CSDocumentXml"/>
    /// </summary>
    /// <remarks>Supports Class/Enum Type, Constructor, Method, and Property.</remarks>
    public interface ICSDocument
    {
        /// <summary>
        /// The <see cref="System.Type"/> for the class.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Readonly collection of <see cref="CSDocumentItem"/> for the class.
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

    /// <summary>
    /// <see cref="ICSDocument"/>
    /// </summary>
    internal class CSDocument : ICSDocument
    {
        private readonly List<CSDocumentItem> items = [];

        internal CSDocument(Type type, XDocument? xmlDoc, bool allMembers)
        {
            Items = new ReadOnlyCollection<CSDocumentItem>(items);
            Type = type;
            ProcessItems(xmlDoc, allMembers);
        }

        /// <summary>
        /// <see cref="System.Type"/>
        /// </summary>
        public Type Type { get; } 

        /// <summary>
        /// Readonly collection of <see cref="CSDocumentItem"/>.
        /// </summary>
        public IReadOnlyCollection<CSDocumentItem> Items { get; }

        /// <summary>
        /// Gets the <see cref="CSDocumentItem"/> associated with the <paramref name="name"/>
        /// </summary>
        /// <param name="name"><see cref="CSDocumentItem.Name"/></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets all the <see cref="CSDocumentItem"/> items associated with the <paramref name="name"/>
        /// </summary>
        /// <param name="name"><see cref="CSDocumentItem.Name"/></param>
        /// <returns></returns>
        public List<CSDocumentItem> GetItems(string name)
        {
            return [.. Items.Where(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase))];
        }

        /// <summary>
        /// Gets the <see cref="CSDocumentXml"/> associated with the <paramref name="memberType"/> and <paramref name="name"/>
        /// </summary>
        /// <param name="memberType"><see cref="MemberTypes"/></param>
        /// <param name="name"><see cref="CSDocumentItem.Name"/></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CSDocumentItem? GetItem(MemberTypes memberType, string name, string parameters = "")
        {
            return Items.FirstOrDefault(e => 
                e.MemberType == memberType && 
                e.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                e.Parameters.Equals(parameters, StringComparison.OrdinalIgnoreCase));
        }

        private void ProcessItems(XDocument? xmlDoc, bool allMembers)
        {
            //Process the class
            var name = $"T:{Type.Namespace}.{Type.Name}";
            var node = xmlDoc?.Descendants("member").FirstOrDefault(m => m.Attribute("name")?.Value == name);
            var attribute = Type.GetDisplayAttribute();
            CSDocumentXml? comment = null;

            if (node != null)
                comment = new CSDocumentXml(node, name, Type.Name);

            if (allMembers || attribute != null || comment != null)
                items.Add(new CSDocumentItem(Type.Name, MemberTypes.TypeInfo, attribute, comment));

            if (Type.IsEnum)
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
            else
            {
                //Process the members.
                var members = Type.GetMembers();

                ProcessMembers(xmlDoc, allMembers, ref attribute, ref comment, members);

                if (Type.IsInterface)
                {
                    var derivedInterfaces = Type.Assembly
                        .GetTypes()
                        .Where(t => t.IsInterface && t != Type && Type.IsAssignableTo(t)) 
                        .ToList();

                    foreach (var derivedInterface in derivedInterfaces)
                    {
                        members = derivedInterface.GetMembers();

                        ProcessMembers(xmlDoc, allMembers, ref attribute, ref comment, members);
                    }
                }
            }
        }

        private void ProcessMembers(XDocument? xmlDoc, bool allMembers, ref DisplayAttribute? attribute, ref CSDocumentXml? comment, MemberInfo[] members)
        {
            foreach (var member in members)
            {
                var memberName = member.Name;
                var mParameters = "";

                if (member is MethodInfo m)
                {
                    //Ignore methods such as 'get_' or 'set_' (accessors) etc.
                    if (m.IsSpecialName)
                        continue;

                    //Ignore built-in methods such as 'GetType()', 'GetHashCode()' etc. unless they are overrides.
                    var isCustom = m.DeclaringType == Type;
                    if (!isCustom)
                    {
                        bool isOverride = m.GetBaseDefinition().DeclaringType != m.DeclaringType;
                        if (!isOverride) continue;
                    }

                    mParameters = m.BuildParameters(out string? memberNameSuffix);
                    memberName = $"{memberName}{memberNameSuffix}";
                }

                string? memberType = null;
                var isConstructor = false;

                switch (member.MemberType)
                {
                    case MemberTypes.Constructor:
                        memberType = "M";
                        memberName = "#ctor";
                        isConstructor = true;
                        break;
                    case MemberTypes.Event:
                        break;
                    case MemberTypes.Field:
                        break;
                    case MemberTypes.Method:
                        memberType = "M";
                        break;
                    case MemberTypes.Property:
                        memberType = "P";
                        break;
                    case MemberTypes.TypeInfo:
                        break;
                    case MemberTypes.Custom:
                        break;
                    case MemberTypes.NestedType:
                        break;
                    case MemberTypes.All:
                        break;
                    default:
                        break;
                }

                if (memberType != null)
                {
                    attribute = member.GetDisplayAttribute();
                    comment = null;

                    var mName = $"{memberType}:{member.DeclaringType?.Namespace}.{member.DeclaringType?.Name}.{memberName}{mParameters}";
                    var mNode = xmlDoc?.Descendants("member").FirstOrDefault(m => m.Attribute("name")?.Value == mName);

                    if (mNode != null)
                        comment = new CSDocumentXml(mNode, mName, member.Name, mParameters);

                    if ((allMembers && !isConstructor) || attribute != null || comment != null)
                        items.Add(new CSDocumentItem(member.Name, member.MemberType, attribute, comment));
                }
            }
        }
    }

    /// <summary>
    /// <see cref="ICSDocument"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class CSDocument<T> : CSDocument
    {
        private readonly List<CSDocumentItem> items = [];

        internal CSDocument(XDocument? xmlDoc, bool allMembers) : base(typeof(T), xmlDoc, allMembers) 
        { 
        }
    }
}
