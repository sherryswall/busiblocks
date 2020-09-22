namespace BusiBlocks
{
    /// <summary>
    /// BusiBlocks specific constants.
    /// </summary>
    public class BusiBlocksConstants
    {   
        /// <summary>
        /// The string representation of the Administrators group.
        /// </summary>
        public readonly static string AdministratorsGroup = "System Administrators";
        public readonly static string AdministratorsRole = "core:administrator";

        public static class Blocks
        {
            public static class Administration
            {
                public readonly static string LongName = "administration";
                public readonly static string ShortName = "admin";
                public readonly static string BlockName = "adminblock";
            }

            public static class Documents
            {
                public readonly static string LongName = "documents";
                public readonly static string ShortName = "doco";
                public readonly static string BlockName = "docoblock";
            }

            public static class Communication
            {
                public readonly static string LongName = "communication";
                public readonly static string ShortName = "comms";
                public readonly static string BlockName = "commsblock";
            }

            public static class Directory
            {
                public readonly static string LongName = "directory";
                public readonly static string ShortName = "dir";
                public readonly static string BlockName = "dirblock";
            }

        }
    }
}   