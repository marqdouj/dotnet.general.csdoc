using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Marqdouj.DotNet.General.CsDoc
{
    /// <summary>
    /// <see cref="CSDocumentReader"/>
    /// </summary>
    public interface ICSDocumentReader
    {
        /// <summary>
        /// Assembly name associated with the Xml documentation file.
        /// </summary>
        string? AssemblyName { get; }

        /// <summary>
        /// Creates a <see cref="ICSDocument"/> based on the <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The Type associated with the xml documentation to read.</typeparam>
        /// <param name="allMembers">Flag to include all custom or override members.</param>
        /// <returns></returns>
        ICSDocument CreateDocument<T>(bool allMembers = true);

        /// <summary>
        /// Creates a <see cref="ICSDocument"/> based on the <paramref name="type"/>
        /// </summary>
        /// <param name="type">The Type associated with the xml documentation to read.</param>
        /// <param name="allMembers">Flag to include all custom or override members.</param>
        /// <returns></returns>
        ICSDocument CreateDocument(Type type, bool allMembers = true);
    }

    /// <summary>
    /// Reads an Xml documentation file.
    /// </summary>
    /// <param name="assemblyName">Assembly xml documentation file name without the .xml extension.</param>
    public class CSDocumentReader(string assemblyName) : ICSDocumentReader
    {
        private XDocument? xmlDoc;

        /// <summary>
        /// Reads xml documentation or <see cref="DisplayAttribute"/> for an assembly <see langword="class"/> or <see langword="enum"/> and it's members.
        /// </summary>
        /// <param name="folder">
        /// Optional path to where the xml document is located. 
        /// If <see langword="null"/> then the <see cref="AppDomain.CurrentDomain"/> / <see cref="AppDomain.BaseDirectory"/> is used.
        /// </param>
        /// <param name="logger">Optional <see cref="ILogger"/> for <see cref="Exception"/>.</param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <returns><see langword="true"/> if the document was loaded, but there may be associated issues. Also check the <see cref="LoadXmlException"/></returns>
        public bool LoadXml(string? folder = null, ILogger? logger = null)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(AssemblyName);

                folder ??= AppDomain.CurrentDomain.BaseDirectory;
                Filename = Path.Combine(folder, $"{AssemblyName}.xml");

                if (!File.Exists(Filename))
                    throw new FileNotFoundException($"XML documentation file not found: {Filename}");

                xmlDoc = XDocument.Load(Filename);
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
            }
            catch (Exception ex)
            {
                LoadXmlException = ex;
                logger?.LogError(ex, "Error loading XML documentation file.");
            }

            return XmlLoaded;
        }

        /// <summary>
        /// <inheritdoc cref="ICSDocumentReader.AssemblyName"/>
        /// </summary>
        public string? AssemblyName { get; } = assemblyName;

        /// <summary>
        /// The resolved filename for the Xml documentation file.
        /// </summary>
        public string? Filename { get; private set; }

        /// <summary>
        /// Indicates if the xml documentation file has been successfully loaded.
        /// </summary>
        /// <remarks>The document may be loaded, but there may be issues. Check the <see cref="LoadXmlException"/></remarks>
        public bool XmlLoaded { get; private set; }

        /// <summary>
        /// An exception that has occurred during the xml documentation load operation, if any.
        /// </summary>
        /// <remarks>If no exception was thrown during loading, this property returns <see langword="null"/>.</remarks>
        public Exception? LoadXmlException { get; private set; }

        /// <summary>
        /// <inheritdoc cref="ICSDocumentReader.CreateDocument{T}(bool)"/>
        /// </summary>
        /// <typeparam name="T">The Type associated with the xml documentation to read.</typeparam>
        /// <param name="allMembers">Flag to include all custom or override members.</param>
        /// <returns></returns>
        public ICSDocument CreateDocument<T>(bool allMembers = true) 
        {
            return CreateDocument(typeof(T), allMembers);
        }

        /// <summary>
        /// <inheritdoc cref="ICSDocumentReader.CreateDocument(Type, bool)"/>
        /// </summary>
        /// <param name="type">The Type associated with the xml documentation to read.</param>
        /// <param name="allMembers">Flag to include all custom or override members</param>
        /// <returns></returns>
        public ICSDocument CreateDocument(Type type, bool allMembers = true)
        {
            var doc = new CSDocument(type, xmlDoc, allMembers);
            var cdocAssemblyName = doc.Type.Assembly.GetName().Name;

            if (AssemblyName != cdocAssemblyName)
                throw new Exception($"Manager {nameof(AssemblyName)} [{AssemblyName}] does not match document [{cdocAssemblyName}].");

            return doc;
        }
    }
}
