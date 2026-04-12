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

            foreach (XNode element in node.Element("summary")?.Nodes() ?? [])
            {
                Summary = ParseCRef(element, "summary", Summary);
            }

            Remarks = node.Element("remarks")?.Value.Trim();

            foreach (XNode element in node.Element("remarks")?.Nodes() ?? [])
            {
                Remarks = ParseCRef(element, "remarks", Remarks);
            }

            Returns = node.Element("returns")?.Value.Trim();

            foreach (XNode element in node.Element("returns")?.Nodes() ?? [])
            {
                Returns = ParseCRef(element, "returns", Returns);
            }
        }

        private static string? ParseCRef(XNode element, string source, string? value)
        {
            var elementValue = element.ToString().Trim();
            var cRefPosn = elementValue.IndexOf("cref=", StringComparison.OrdinalIgnoreCase);

            if (cRefPosn > -1)
            {
                var firstQuotePosn = elementValue.IndexOf('"', cRefPosn);
                if (firstQuotePosn > -1)
                {
                    var secondQuotePosn = elementValue.IndexOf('"', firstQuotePosn + 1);
                    if (secondQuotePosn > -1)
                    {
                        var cref = elementValue[(firstQuotePosn + 1)..secondQuotePosn];
                        var xmlDoc = element.Document;
                        var crefNode = xmlDoc?.Descendants("member").FirstOrDefault(m => m.Attribute("name")?.Value == cref);

                        if (crefNode != null)
                        {
                            var cRefValue = crefNode.Element(source)?.Value.Trim();
                            if (!string.IsNullOrWhiteSpace(cRefValue))
                                value = $"{value} {cRefValue}".Trim();
                        }
                    }
                }
            }

            //In some scenarios, the value might start with a period, so we want to trim it off.
            if (value != null && value.StartsWith('.'))
                value = value.TrimStart('.');

            return value?.Trim();
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
