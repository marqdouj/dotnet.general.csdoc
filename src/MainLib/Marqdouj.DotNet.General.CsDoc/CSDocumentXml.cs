using System.Xml.Linq;

namespace Marqdouj.DotNet.General.CsDoc
{
    /// <summary>
    /// Extracted values from the xml documentation file.
    /// </summary>
    /// <remarks>Supported comments are Summary, Remarks, and Returns</remarks>
    public class CSDocumentXml
    {
        internal CSDocumentXml() { }

        internal CSDocumentXml(XElement node, string fullname, string? memberName)
        {
            Fullname = fullname;
            MemberName = memberName;
            Summary = node.Element("summary")?.Value.Trim();
            Remarks = node.Element("remarks")?.Value.Trim();
            Returns = node.Element("returns")?.Value.Trim();
        }

        /// <summary>
        /// 'name' attribute for the xml document member.
        /// </summary>
        public string? Fullname { get; }

        /// <summary>
        /// Property name (if applicable).
        /// </summary>
        public string? MemberName { get; set; }

        /// <summary>
        /// 'Member' element type.
        /// </summary>
        public string? Type => Fullname?[..1];

        /// <summary>
        /// 'Member' element name (without Type).
        /// </summary>
        public string? Name => Fullname?[2..];

        /// <summary>
        /// Value for the 'summary' element in the xml document.
        /// </summary>
        public string? Summary { get; }

        /// <summary>
        /// Value for the 'remarks' element in the xml document.
        /// </summary>
        public string? Remarks { get; }

        /// <summary>
        /// Value for the 'returns' element in the xml document.
        /// </summary>
        public string? Returns { get; }

        /// <summary>
        /// <see cref="object.ToString"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Fullname ?? "";
    }
}
