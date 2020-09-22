using System;
using System.Xml;

namespace BusiBlocks
{
    public class HelpHelper
    {
        #region Constants

        private const string FailedErrorMessage = "Error: BusiBlocks Help has failed.";

        private const string XmlFilename = "help.config";


        private const string XmlHelpElementTitle = "title";
        private const string XmlHelpElementPurpose = "purpose";
        private const string XmlHelpElementWorks = "works";
        private const string XmlHelpElementUse = "use";

        private const string XmlHelpElementRoot = "help";
        private const string XmlHelpElementPage = "page";
        private const string XmlElementNameIdentifier = "name";

        #endregion


        public static Help GetHelp(string pageName)
        {
            var doc = new XmlDocument();

            string configFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, XmlFilename);

            doc.Load(configFile);
            XmlNodeList pageNodes = doc.DocumentElement.SelectNodes(XmlHelpElementRoot).Item(0).ChildNodes;

            pageName = FormatPageName(pageName);

            Help help = new Help(pageName);

            foreach (XmlNode pageNode in pageNodes)
            {
                if (!pageNode.NodeType.Equals(XmlNodeType.Comment))
                {
                    if (pageNode.Attributes[XmlElementNameIdentifier].Value.ToUpper() == pageName.ToUpper())
                    {
                        foreach (XmlNode node in pageNode.ChildNodes)
                        {
                            switch (node.Name)
                            {
                                case XmlHelpElementTitle:
                                    help.Title = node.InnerXml;
                                    break;
                                case XmlHelpElementPurpose:
                                    help.Purpose = node.InnerXml;
                                    break;
                                case XmlHelpElementWorks:
                                    help.Works = node.InnerXml;
                                    break;
                                case XmlHelpElementUse:
                                    help.Use = node.InnerXml;
                                    break;
                                default: break;
                            }
                        }
                    }
                }
            }
            return help;
        }
        /// <summary>
        /// returns PageName that can be used to identify pages. Removes unidentifiable querystring  e.g. Default.aspx? = Default.aspx and default.aspx?item = default.aspx?item
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        protected static string FormatPageName(string pageName)
        {
            string[] urlWithQuery = pageName.Split('?');

            if (urlWithQuery.Length > 1)
            {
                if (!string.IsNullOrEmpty(urlWithQuery[1]))
                    return pageName;
                else
                    return urlWithQuery[0];
            }
            else
                return pageName;
        }
    }
}
