using System;

namespace BusiBlocks
{
    /// <summary>
    /// Enum to define the validation mode for the XHTML snippets
    /// </summary>
    public enum XHtmlMode
    {
        /// <summary>
        /// No validation
        /// </summary>
        None = 0,

        /// <summary>
        /// Basic validation only. Check if not supported tags are present (like body, html, head, script, ...). See the XHTMLText for the full list.
        /// </summary>
        BasicValidation = 1,

        /// <summary>
        /// Strict validation. Only permits certain tags (p, a, br, table, h1, h2, ...). See the XHTMLText for the full list.
        /// </summary>
        StrictValidation = 2
    }

    /// <summary>
    /// Enum to define the backup mode used for Doco articles.
    /// </summary>
    public enum DocoBackupMode
    {
        /// <summary>
        /// Backup always
        /// </summary>
        Always = 0,

        /// <summary>
        /// Backup only if requested
        /// </summary>
        Request = 1,

        /// <summary>
        /// Backup disabled
        /// </summary>
        Never = 2
    }

    public enum PlainTextMode
    {
        CssPlainText = 1,
        XHtmlConversion = 2
    }

    public enum JunctionOperator
    {
        And = 0,
        Or = 1
    }

    public enum ValueOperator
    {
        Equal = 0,
        NotEqual = 1,
        StartWith = 2,
        EndWith = 3,
        Contains = 4
    }

    public enum AccessType
    {
        View = 0,
        Edit = 1,
        Approve = 2,
        Contribute = 3
    }

    public enum ItemType
    {
        DocoCategory = 0,
        NewsItem = 1,
        ForumTopic = 2,
        FileFolder = 3
    }

    public enum SaveType
    {
        Hold = 0,
        CheckIn = 1,
        Publish = 2
    }

    [Flags]
    public enum ConfigurationFlags
    {
        None = 0,
        Settings = 1,
        Mappings = 2,
        Interceptor = 4,
        Default = 7
    }
}