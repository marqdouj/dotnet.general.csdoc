using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Marqdouj.DotNet.General.CsDoc
{
    /// <summary>
    /// Reads an Xml documentation file.
    /// </summary>
    public class XmlDocumentReader
    {
        private readonly List<XmlDocumentMember> members = [];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyName"><see cref="AssemblyName"/></param>
        public XmlDocumentReader(string assemblyName)
        {
            AssemblyName = assemblyName;
            Members = new ReadOnlyCollection<XmlDocumentMember>(members);
        }

        /// <summary>
        /// Loads xml documentation for an assembly.
        /// </summary>
        /// <param name="folder">
        /// Optional path to where the xml document is located. 
        /// If <see langword="null"/> then the <see cref="AppDomain.CurrentDomain"/> / <see cref="AppDomain.BaseDirectory"/> is used.
        /// </param>
        /// <param name="logger">Optional <see cref="ILogger"/> for <see cref="Exception"/>.</param>
        /// <param name="throwAnyException">If <see langword="true"/> then throw any exception. Default is <see langword="false"/>.</param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <returns><see langword="true"/> if the document was loaded, but there may be associated issues. Also check the <see cref="LoadXmlException"/></returns>
        public bool LoadXml(string? folder = null, ILogger? logger = null, bool throwAnyException = false)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(AssemblyName);

                folder ??= AppDomain.CurrentDomain.BaseDirectory;
                Filename = Path.Combine(folder, $"{AssemblyName}.xml");

                if (!File.Exists(Filename))
                    throw new FileNotFoundException($"XML documentation file not found: {Filename}");

                XDocument xmlDoc = XDocument.Load(Filename);
                XmlLoaded = true;

                XElement? assemblyNode = null;

                try
                {
                    assemblyNode = xmlDoc.Descendants("assembly").First();
                    var name = (assemblyNode.Element("name")?.Value.Trim()) ?? throw new Exception("Missing 'assembly.name' element.");

                    if (!name.Equals(AssemblyName, StringComparison.OrdinalIgnoreCase))
                        throw new Exception($"Xml documentation file assembly name does not match. Xml documentation attribute name: '{name}'");
                }
                catch (Exception ex)
                {
                    throw new Exception("Error resolving 'assembly' node.", ex);
                }

                var assemblyFilename = Path.Combine(folder, $"{AssemblyName}.dll");
                var types = new List<Type>();

                if (File.Exists(assemblyFilename))
                {
                    try
                    {
                        var assembly = Assembly.LoadFrom(assemblyFilename);
                        types = [.. assembly.GetTypes()];
                    }
                    catch (Exception ex)
                    {
                        logger?.LogError(ex, "Failed to load assembly types.");
                    }
                }

                ParseMembers(xmlDoc, logger, types);
            }
            catch (Exception ex)
            {
                LoadXmlException = ex;
                logger?.LogError(ex, "Error loading XML documentation file.");
            }

            if (throwAnyException && LoadXmlException != null)
                throw LoadXmlException;

            return XmlLoaded;
        }

        /// <summary>
        /// Assembly name for the xml documentation file.
        /// </summary>
        public string? AssemblyName { get; }

        /// <summary>
        /// The resolved filename for the Xml documentation file.
        /// </summary>
        public string? Filename { get; private set; }

        /// <summary>
        /// Indicates if the xml documentation file was successfully loaded.
        /// </summary>
        /// <remarks>The document may be loaded, but there may be issues. Check the <see cref="LoadXmlException"/></remarks>
        public bool XmlLoaded { get; private set; }

        /// <summary>
        /// An exception that has occurred during the xml documentation load operation, if any.
        /// </summary>
        /// <remarks>If no exception was thrown during loading, this property returns <see langword="null"/>.</remarks>
        public Exception? LoadXmlException { get; private set; }

        /// <summary>
        /// Collection of all members found in the xml documentation file.
        /// </summary>
        public IReadOnlyCollection<XmlDocumentMember> Members { get; }

        /// <summary>
        /// Gets all members for a specific Type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<XmlDocumentMember> GetMembers<T>() => GetMembers(typeof(T));

        /// <summary>
        /// Gets all members for a specific Type.
        /// </summary>
        /// <param name="type"><inheritdoc cref="Type"/></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<XmlDocumentMember> GetMembers(Type type)
        {
            var cdocAssemblyName = type.Assembly.GetName().Name;

            if (AssemblyName != cdocAssemblyName)
                throw new Exception($"GetMember: {nameof(AssemblyName)} [{AssemblyName}] does not match document [{cdocAssemblyName}].");

            var members = new List<XmlDocumentMember>();
            var name = type.Name;
            var fullname = type.FullName ?? "";
            var index = fullname.IndexOf(name);
            
            if (index > -1)
            {
                var searchName = $":{fullname[..index]}{name}";
                var items = Members.Where(e => e.Fullname.Contains(searchName, StringComparison.OrdinalIgnoreCase));
                members.AddRange(items);
            }

            return members;
        }

        /// <summary>
        /// Gets all members for a specific TypeName.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public List<XmlDocumentMember> GetMembers(string typeName)
        {
            var type = members.FirstOrDefault(e => e.TypeName.Equals(typeName, StringComparison.OrdinalIgnoreCase))?.Type;
            if (type != null)
                return GetMembers(type);

            var searchName = $":{typeName}";
            var items = Members.Where(e => e.Fullname.Contains(searchName, StringComparison.OrdinalIgnoreCase));
            return [.. items];
        }

        private void ParseMembers(XDocument xmlDoc, ILogger? logger, List<Type> types)
        {
            members.Clear();

            var xmlMembers = xmlDoc!.Descendants("member");
            var position = 0;

            foreach (var xmlMember in xmlMembers)
            {
                var fullname = xmlMember.Attribute("name")?.Value ?? "";

                if (string.IsNullOrWhiteSpace(fullname))
                    logger?.LogError("Attribute 'name' is missing for member at position {position}.", position);

                var name = ParseNameFromFullname(fullname);
                var member = new XmlDocumentMember(position, fullname, name)
                {
                    MemberType = ParseMemberType(fullname),
                    Summary = ParseComment(xmlMember, "summary"),
                    Remarks = ParseComment(xmlMember, "remarks"),
                    Returns = ParseComment(xmlMember, "returns"),
                };

                member.SetParameters(GetParameters(xmlMember));
                member.Type = types.FirstOrDefault(e => !string.IsNullOrWhiteSpace(e.FullName) && e.FullName.Equals(member.TypeName, StringComparison.Ordinal));

                members.Add(member);
                position++;
            }
        }

        private static MemberTypes? ParseMemberType(string? fullname)
        {
            if (string.IsNullOrWhiteSpace(fullname) || fullname.Length < 2)
                return null;

            return fullname[..2] switch
            {
                "E:" => (MemberTypes?)MemberTypes.Event,
                "F:" => (MemberTypes?)MemberTypes.Field,
                "M:" => (MemberTypes?)(fullname.Contains(".#ctor", StringComparison.OrdinalIgnoreCase) ?
                                        MemberTypes.Constructor :
                                        MemberTypes.Method),
                "P:" => (MemberTypes?)MemberTypes.Property,
                "T:" => (MemberTypes?)MemberTypes.TypeInfo,
                _ => null,
            };
        }

        private static List<XmlDocumentMemberParameter> GetParameters(XElement node)
        {
            var items = new List<XmlDocumentMemberParameter>();
            var fullname = node.Attribute("name")?.Value ?? "";

            foreach (var element in node.Elements("param"))
            {
                var name = element.Attribute("name")?.Value ?? "#Missing#";
                var value = GetRawComment(element, "param") ?? "";
                value = ParseInheritDoc(element, value, fullname);
                value = ParseTypeParamRef(value);
                value = ParseSee(value);
                items.Add(new XmlDocumentMemberParameter(XmlDocumentMemberParameterType.Param, name, value));
            }

            foreach (var element in node.Elements("typeparam"))
            {
                var name = element.Attribute("name")?.Value ?? "#Missing#";
                var value = GetRawComment(element, "typeparam") ?? "";
                value = ParseInheritDoc(element, value, fullname);
                value = ParseTypeParamRef(value);
                value = ParseSee(value);
                items.Add(new XmlDocumentMemberParameter(XmlDocumentMemberParameterType.TypeParm, name, value));
            }

            return items;
        }

        private static string? ParseComment(XElement node, string elementName)
        {
            var summary = new StringBuilder();
            var fullname = node.Attribute("name")?.Value ?? "";

            foreach (var element in node.Elements(elementName))
            {
                var value = GetRawComment(element, elementName);
                value = ParseInheritDoc(element, value, fullname);
                value = ParseTypeParamRef(value);
                value = ParseSee(value);

                if (!string.IsNullOrWhiteSpace(value))
                    summary.AppendLine(value);

            }

            //return summary;
            return summary.ToString();
        }

        private static string GetRawComment(XElement element, string elementName)
        {
            var innerText = element.ToString().Trim();
            var comment = element.Value.Trim();
            var hasStart = innerText.StartsWith($"<{elementName}", StringComparison.OrdinalIgnoreCase);
            var hasEnd = innerText.EndsWith($"</{elementName}>", StringComparison.OrdinalIgnoreCase);

            //Must be enclosed within elementName to resolve raw comment.
            if (!(hasStart && hasEnd))
                return comment;

            var startPosnEnd = innerText.IndexOf('>');
            var endPosnStart = innerText.LastIndexOf($"</{elementName}>", StringComparison.OrdinalIgnoreCase);
            var commentLength = endPosnStart - startPosnEnd - 1;
            if (commentLength > 0)
                comment = innerText.Substring(startPosnEnd + 1, commentLength).Trim();

            return comment;
        }

        private static string ParseNameFromFullname(string refText)
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

        private static string ParseTypeParamRef(string value)
        {
            var startPosn = value.IndexOf("<typeparamref", StringComparison.OrdinalIgnoreCase);

            while (startPosn > -1)
            {
                var endPosn = value.IndexOf("/>", startPosn, StringComparison.OrdinalIgnoreCase);

                if (endPosn > -1)
                {
                    var text = value.Substring(startPosn, endPosn - startPosn + 2);
                    var refText = GetRefText(RefTextType.Name, text);
                    var cRefValue = string.IsNullOrWhiteSpace(refText) ? "'?'" : $"'{refText?.Trim()}'";

                    value = value.Replace(text, cRefValue);
                }
                else
                {
                    //Something went wrong
                    return value;
                }

                startPosn = value.IndexOf("<typeparamref", StringComparison.OrdinalIgnoreCase);
            }

            return value;
        }

        private static string ParseInheritDoc(XElement element, string value, string fullname)
        {
            var startPosn = value.IndexOf("<inheritdoc", StringComparison.OrdinalIgnoreCase);

            while (startPosn > -1)
            {
                var endPosn = value.IndexOf("/>", startPosn, StringComparison.OrdinalIgnoreCase);

                if (endPosn > -1)
                {
                    var text = value.Substring(startPosn, endPosn - startPosn + 2);
                    var crefText = GetRefText(RefTextType.CRef, text);
                    var cRefValue = "";

                    if (!string.IsNullOrWhiteSpace(crefText)) //If empty etc. most likely is <inheritdoc/>
                    {
                        var xmlDoc = element.Document;
                        var crefNode = xmlDoc?.Descendants("member").FirstOrDefault(m => m.Attribute("name")?.Value == crefText);

                        if (crefNode != null)
                        {
                            //Check for circular ref which will cause stack overflow.
                            if (crefText != fullname)
                            {
                                cRefValue = ParseComment(crefNode, "summary")?.Trim();
                            }
                            else
                            {
                                cRefValue = "#ERROR - Circular Reference.#";
                            }
                        }
                    }

                    value = value.Replace(text, cRefValue);
                }
                else
                {
                    //Something went wrong
                    return value;
                }

                startPosn = value.IndexOf("<inheritdoc", StringComparison.OrdinalIgnoreCase);
            }

            return value;
        }

        private static string ParseSee(string value)
        {
            var startPosn = value.IndexOf("<see", StringComparison.OrdinalIgnoreCase);

            while (startPosn > -1)
            {
                var endPosn = value.IndexOf("/>", startPosn, StringComparison.OrdinalIgnoreCase);

                if (endPosn > -1)
                {
                    var text = value.Substring(startPosn, endPosn - startPosn + 2);
                    var refText = GetRefText(RefTextType.CRef, text);
                    var subtext = "";

                    if (!string.IsNullOrWhiteSpace(refText))
                    {
                        subtext = ParseNameFromFullname(refText);
                    }

                    if (string.IsNullOrWhiteSpace(subtext))
                    {
                        subtext = GetRefText(RefTextType.HRef, text);
                    }

                    if (string.IsNullOrWhiteSpace(subtext))
                    {
                        subtext = GetRefText(RefTextType.Langword, text);
                    }

                    if (string.IsNullOrWhiteSpace(subtext))
                        return value; //Something went wrong or is not yet supported.

                    value = value.Replace(text, $"'{subtext}'");
                }
                else
                {
                    //Something went wrong
                    return value;
                }

                startPosn = value.IndexOf("<see", StringComparison.OrdinalIgnoreCase);
            }

            return value;
        }

        private enum RefTextType
        {
            CRef,
            HRef,
            Langword,
            Name,
        }

        private static string GetRefText(RefTextType textType, string value)
        {
            string? symbol = textType switch
            {
                RefTextType.CRef => "cref=",
                RefTextType.Langword => "langword=",
                RefTextType.HRef => "href=",
                RefTextType.Name => "name=",
                _ => throw new ArgumentOutOfRangeException(nameof(textType)),
            };
            var refPosn = value.IndexOf(symbol, StringComparison.OrdinalIgnoreCase);

            if (refPosn > -1)
            {
                var firstQuotePosn = value.IndexOf('"', refPosn);
                if (firstQuotePosn > -1)
                {
                    var secondQuotePosn = value.IndexOf('"', firstQuotePosn + 1);
                    if (secondQuotePosn > -1)
                    {
                        var refText = value[(firstQuotePosn + 1)..secondQuotePosn];
                        return refText;
                    }
                }
            }

            return "";
        }
    }
}