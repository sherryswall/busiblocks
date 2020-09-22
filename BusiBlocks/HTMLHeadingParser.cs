using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace BusiBlocks
{
    /// <summary>
    /// Class used to parse an XHTML snippet and construct a TOC based on the heading tags
    /// </summary>
    internal class HTMLHeadingParser
    {
        private static readonly string[] TAG_HEADINGS = new[] {"h1", "h2", "h3", "h4", "h5", "h6"};

        /// <summary>
        /// Automatically create the table of contents for the specified document.
        /// Insert an ID in the document if the heading doesn't have it.
        /// Returns the XHTML for the TOC
        /// </summary>
        public static string GenerateTOC(XmlDocument doc)
        {
            XmlNodeList headings = doc.SelectNodes("//*");

            var root = new Heading("ROOT", null, 0);
            int index = 0;
            GenerateHeadings(headings, root, ref index);

            using (var stream = new MemoryStream())
            {
                var writer = new XmlTextWriter(stream, Encoding.UTF8);
                writer.WriteStartElement("div");
                root.WriteChildrenToXml(writer);
                writer.WriteEndElement();
                writer.Flush();

                stream.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(stream, Encoding.UTF8);

                return reader.ReadToEnd();
            }
        }

        private static void GenerateHeadings(XmlNodeList headings, Heading parent, ref int index)
        {
            while (index < headings.Count)
            {
                int headingLevel = GetHeadingLevel(headings[index].Name);

                if (headingLevel == 0)
                {
                    //not an heading
                    index++; // Skip
                }
                else if (headingLevel <= parent.Level)
                {
                    return;
                }
                else if (headingLevel > parent.Level)
                {
                    //Generate the header only for heading with ID attribute
                    string id = CheckForId(parent, (XmlElement) headings[index]);
                    var subHead = new Heading(headings[index].InnerText, id, headingLevel);
                    parent.AddChild(subHead);

                    index++; // Read next

                    GenerateHeadings(headings, subHead, ref index);
                }
                else
                    throw new ArgumentOutOfRangeException("index");
            }
        }

        private static string CheckForId(Heading parent, XmlElement element)
        {
            string id = element.GetAttribute("id");
            if (id == null || id.Length == 0)
            {
                var builder = new StringBuilder();
                foreach (char c in element.InnerText)
                {
                    if (char.IsLetterOrDigit(c))
                        builder.Append(c);
                    else if (char.IsWhiteSpace(c))
                    {
                        //don't consider whitespace
                    }
                    else
                        builder.Append('_');

                    if (builder.Length >= 100)
                        break;
                }

                id = builder.ToString();

                //Add the parent id
                if (parent.IsRoot() == false)
                    id = parent.Id + "_" + id;

                element.SetAttribute("id", id);
            }

            return id;
        }

        private static int GetHeadingLevel(string element)
        {
            int index = Array.BinarySearch(TAG_HEADINGS, element.ToLowerInvariant());
            if (index < 0)
                return 0;
            else
                return index + 1;
        }

        #region Nested type: Heading

        private class Heading
        {
            public static Heading ROOT = new Heading(null, null, 0);

            public readonly string Id;
            public readonly int Level;
            public readonly string Text;

            private readonly List<Heading> mChildren = new List<Heading>();

            public Heading(string pText, string pId, int pLevel)
            {
                Text = pText;
                Level = pLevel;
                Id = (string.IsNullOrEmpty(pId) ? "0" : pId);
            }

            public void AddChild(Heading child)
            {
                mChildren.Add(child);
            }

            public void WriteToXml(XmlTextWriter writer)
            {
                writer.WriteStartElement("li");
                writer.WriteAttributeString("id", "li_" + Id);
                writer.WriteAttributeString("level", Level.ToString());
                writer.WriteAttributeString("runat", "server");

                writer.WriteStartElement("a");
                writer.WriteAttributeString("href", "javascript:displaySection('" + Id + "')");
                writer.WriteAttributeString("id", Id);
                writer.WriteAttributeString("level", Level.ToString());
                writer.WriteAttributeString("runat", "server");

                writer.WriteString(Text);
                writer.WriteEndElement();

                if (mChildren.Count > 0)
                {
                    WriteChildrenToXml(writer);
                }

                writer.WriteEndElement();
            }

            public void WriteChildrenToXml(XmlTextWriter writer)
            {
                writer.WriteStartElement("ul");

                writer.WriteAttributeString("Id", "ul_" + Id);
                writer.WriteAttributeString("level", (Level + 1).ToString());
                writer.WriteAttributeString("runat", "server");

                foreach (Heading subHead in mChildren)
                {
                    subHead.WriteToXml(writer);
                }

                writer.WriteEndElement();
            }

            public bool IsRoot()
            {
                return Level == 0;
            }
        }

        #endregion
    }
}