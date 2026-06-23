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
    public class XmlDocumentMemberParameter
    {
        internal XmlDocumentMemberParameter(XmlDocumentMemberParameterType parameterType, string name, string value)
        {
            ParameterType = parameterType;
            Name = name;
            Value = value;
        }

        /// <summary>
        /// <inheritdoc cref="XmlDocumentMemberParameterType"/>
        /// </summary>
        public XmlDocumentMemberParameterType ParameterType { get; }

        /// <summary>
        /// The 'name' value for the node.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The resolved text for the node.
        /// </summary>
        public string Value { get; }
    }
}