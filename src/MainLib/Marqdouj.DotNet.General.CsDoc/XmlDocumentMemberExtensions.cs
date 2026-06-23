namespace Marqdouj.DotNet.General.CsDoc
{
    /// <summary>
    /// Extension methods for XmlDocumentMember.
    /// </summary>
    public static class XmlDocumentMemberExtensions
    {
        /// <summary>
        /// Gets the first member that matches the <paramref name="name"/>.
        /// </summary>
        /// <param name="members"></param>
        /// <param name="name"><inheritdoc cref="XmlDocumentMember.Name"/></param>
        /// <returns></returns>
        public static XmlDocumentMember? GetMemberByName(this IEnumerable<XmlDocumentMember> members, string name)  
            => members?.Where(m =>  m.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

        /// <summary>
        /// Gets all members that start with the <paramref name="name"/>.
        /// </summary>
        /// <param name="members"></param>
        /// <param name="name"><inheritdoc cref="XmlDocumentMember.Name"/></param>
        /// <returns></returns>
        public static List<XmlDocumentMember> GetMembersWithName(this IEnumerable<XmlDocumentMember> members, string name)
            => members?.Where(m => m.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase)).ToList() ?? [];
    }
}