using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Marqdouj.DotNet.General.CsDoc
{
    /// <summary>
    /// Creates instances of <see cref="CSDocument{T}"/>.
    /// </summary>
    public sealed class CSDocumentReader
    {
        private XDocument? xmlDoc;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyName">
        /// Assembly name associated with the <see langword="class"/> or <see langword="enum"/> <see langword="type"/>
        /// you will be creating documents for.
        /// </param>
        public CSDocumentReader(string assemblyName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(assemblyName);
            AssemblyName = assemblyName;
        }

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
        /// Assembly name associated with the Xml cocumentation file.
        /// </summary>
        public string? AssemblyName { get; internal set; }

        /// <summary>
        /// The resolved filename for the Xml documentation file.
        /// </summary>
        public string? Filename { get; internal set; }

        /// <summary>
        /// Indicates if the xml documentation file has been successfully loaded.
        /// </summary>
        /// <remarks>The document may be loaded, but there may be issues. Check the <see cref="LoadXmlException"/></remarks>
        public bool XmlLoaded { get; internal set; }

        /// <summary>
        /// An exception that has occurred during the xml documentation load operation, if any.
        /// </summary>
        /// <remarks>If no exception was thrown during loading, this property returns <see langword="null"/>.</remarks>
        public Exception? LoadXmlException { get; internal set; }

        /// <summary>
        /// Creates an instance of <see cref="ICSDocument"/> based on <typeparamref name="T"/>
        /// </summary>
        /// <param name="allMembers">
        /// <see langword="true"/> to create items for all custom or override members; 
        /// otherwise just those that have a <see cref="DisplayAttribute"/> or xml commment. Default is <see langword="true"/>. </param>
        /// <typeparam name="T">Must be <see langword="class"/> or <see langword="enum"/></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ICSDocument CreateDocument<T>(bool allMembers = true)
        {
            ValidateType<T>();

            var doc = new CSDocument<T>(xmlDoc, allMembers);
            var cdocAssemblyName = doc.Type.Assembly.GetName().Name;

            if (AssemblyName != cdocAssemblyName)
                throw new Exception($"Manager {nameof(AssemblyName)} [{AssemblyName}] does not match document [{cdocAssemblyName}].");

            return doc;
        }

        private static void ValidateType<T>() 
        {
            // Runtime enforcement: ensure T is either class or enum
            if (!(typeof(T).IsClass || typeof(T).IsEnum))
            {
                throw new InvalidOperationException(
                    $"Type parameter '{typeof(T).Name}' must be a class or an enum."
                );
            }
        }
    }
}
