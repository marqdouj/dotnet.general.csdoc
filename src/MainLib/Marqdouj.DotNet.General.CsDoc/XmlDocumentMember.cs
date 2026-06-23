using System.Collections.ObjectModel;
using System.Reflection;

namespace Marqdouj.DotNet.General.CsDoc
{
    /// <summary>
    /// Contains member node information found in the XML documentation file.
    /// </summary>
    public class XmlDocumentMember
    {
        private readonly List<XmlDocumentMemberParameter> parameters = [];

        internal XmlDocumentMember(int position, string fullname, string name)
        {
            Position = position;
            Fullname = fullname;
            Name = name;
            Parameters = new ReadOnlyCollection<XmlDocumentMemberParameter>(parameters);
        }

        /// <summary>
        /// The position of the node in the XML documentation file.
        /// </summary>
        public int Position { get; }

        /// <summary>
        /// The member node name.
        /// </summary>
        public string Fullname { get; }

        /// <summary>
        /// The resolved name 'signature'. Normally used for searching.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The Type for members where <see cref="MemberType"/> is <see cref="MemberTypes.TypeInfo"/>.
        /// </summary>
        /// <remarks>An attempt will be made to resolve the <see cref="Type"/> when creating the instance.
        /// If the attempt fails the value will be <see langword="null"/>.</remarks>
        public Type? Type { get; internal set; }

        /// <summary>
        /// The TypeName for members where <see cref="MemberType"/> is <see cref="MemberTypes.TypeInfo"/>.
        /// </summary>
        public string TypeName => Type != null ? Type.FullName! : (MemberType == MemberTypes.TypeInfo ? Fullname[2..] : "");

        /// <summary>
        /// <see cref="MemberTypes"/>
        /// </summary>
        public MemberTypes? MemberType { get; internal set; }

        /// <summary>
        /// The 'Summary' node information.
        /// </summary>
        public string? Summary { get; internal set; }

        /// <summary>
        /// The 'Remarks' node information.
        /// </summary>
        public string? Remarks { get; internal set; }

        /// <summary>
        /// The 'Returns' node information.
        /// </summary>
        public string? Returns { get; internal set; }

        /// <summary>
        /// The 'Param/TypeParam' node information.
        /// </summary>
        public IReadOnlyCollection<XmlDocumentMemberParameter> Parameters { get; }

        internal void SetParameters(IEnumerable<XmlDocumentMemberParameter> parameters)
        {
            this.parameters.Clear();
            this.parameters.AddRange(parameters);
        }
    }
}