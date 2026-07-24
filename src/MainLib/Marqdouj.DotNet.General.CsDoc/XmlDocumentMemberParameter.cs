namespace Marqdouj.DotNet.General.CsDoc
{
    /// <summary>
    /// Indicates which type of node contains the parameter information.
    /// </summary>
    public enum XmlDocumentMemberParameterType
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Param,
        TypeParm,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    /// <summary>
    /// The 'Parameter' node information.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="XmlDocumentMemberParameter"/> class.
    /// </remarks>
    /// <param name="parameterType"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public class XmlDocumentMemberParameter(XmlDocumentMemberParameterType parameterType, string name, string value)
    {

        /// <summary>
        /// <inheritdoc cref="XmlDocumentMemberParameterType"/>
        /// </summary>
        public XmlDocumentMemberParameterType ParameterType { get; } = parameterType;

        /// <summary>
        /// The 'name' value for the node.
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// The 'alias' value for the node.
        /// </summary>
        public string? Alias { get; set; }

        /// <summary>
        /// <inheritdoc cref="Name"/> if <see cref="Alias"/> is null or empty, otherwise <see cref="Alias"/>.
        /// </summary>
        public string? DisplayName => string.IsNullOrEmpty(Alias) ? Name : Alias;

        /// <summary>
        /// The resolved text for the node.
        /// </summary>
        public string Value { get; } = value;
    }
}