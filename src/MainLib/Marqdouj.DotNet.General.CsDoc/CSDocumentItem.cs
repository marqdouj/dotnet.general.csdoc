using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Marqdouj.DotNet.General.CsDoc
{
    /// <summary>
    /// Container for a <see cref="DisplayAttribute"/> and <see cref="CSDocumentXml"/>
    /// asscociated with a class or one of it's members.
    /// </summary>
    public class CSDocumentItem
    {
        internal CSDocumentItem(string name, MemberTypes memberType, DisplayAttribute? attribute, CSDocumentXml? comment)
        {
            Name = name;
            MemberType = memberType;
            DisplayAttribute = attribute;
            Comment = comment ?? new();
        }

        /// <summary>
        /// The name asscociated with the class or it's member.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the <see cref="DisplayAttribute.Name"/> if it has a value, otherwise the <see cref="Name"/>.
        /// </summary>
        public string? DisplayName => string.IsNullOrEmpty(DisplayAttribute?.Name) ? Name : DisplayAttribute?.Name;

        /// <summary>
        /// <see cref="DisplayAttribute"/>
        /// </summary>
        public DisplayAttribute? DisplayAttribute { get; }

        /// <summary>
        /// <see cref="CSDocumentXml"/>
        /// </summary>
        public CSDocumentXml Comment { get; }

        /// <summary>
        /// <see cref="MemberTypes"/>
        /// </summary>
        public MemberTypes MemberType { get; }

        /// <summary>
        /// <see cref="object.ToString"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{MemberType}:{Name}";
    }
}
