using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
using BusiBlocks;
using System.Net;
using System.Web;
using System.Text;


namespace BusiBlocks
{
    /// <summary>
    /// Class to format and validate an XHTML valid snippet of text.
    /// </summary>
    public class XHTMLText
    {
        #region Helper methods
        /// <summary>
        /// Convert a plain text to an XHTML valid text.
        /// </summary>
        public static string FromPlainText(string plainText, PlainTextMode mode)
        {
            if (mode == PlainTextMode.CssPlainText)
            {
                string htmlEncoded = System.Web.HttpUtility.HtmlEncode(plainText);

                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                builder.Append("<div class=\"plainText\">");
                builder.Append(htmlEncoded);
                builder.Append("</div>");

                return builder.ToString();
            }
            else if (mode == PlainTextMode.XHtmlConversion)
            {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                using (System.IO.StringReader reader = new System.IO.StringReader(plainText))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line != null && line.Length > 0)
                        {
                            //Replace the space and tab characters at the begind of the line with a nont breaking space
                            for (int col = 0; col < line.Length; col++)
                            {
                                if (line[col] == ' ')
                                {
                                    builder.Append("&#160;");
                                }
                                else if (line[col] == '\t')
                                {
                                    builder.Append("&#160;");
                                }
                                else
                                {
                                    string subLine = System.Web.HttpUtility.HtmlEncode(line.Substring(col));

                                    builder.Append(subLine);

                                    break;
                                }
                            }
                        }

                        builder.AppendLine("<br />");
                    }
                }

                return builder.ToString();
            }
            else
                throw new BusiBlocksException("Mode not valid");
        }
        #endregion

        private System.Xml.XmlDocument mDoc = new System.Xml.XmlDocument();
        /// <summary>
        /// Constructor
        /// </summary>
        public XHTMLText()
        {
        }

        /// <summary>
        /// Load an xhtml snippet.
        /// </summary>
        /// <param name="xhtml"></param>
        public void Load(string xhtml)
        {
            mDoc = CreateXHTMLDoc(xhtml);
        }

        private System.Xml.XmlDocument CreateXHTMLDoc(string xml)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.Append("<div>");
            builder.Append(xml);
            builder.Append("</div>");

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(builder.ToString());

            return doc;
        }

        /// <summary>
        /// Check if the html is a valid text using the XML document validator.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="error"></param>
        public bool IsValid(XHtmlMode mode, out Exception error)
        {
            try
            {
                CheckElements(mDoc, mode);

                error = null;
                return true;
            }
            catch (Exception ex)
            {
                error = ex;
                return false;
            }
        }

        private static string[] VALID_TAGS;
        private static string[] INVALID_TAGS;

        private void CheckStrictValidation(System.Xml.XmlElement element)
        {
            if (VALID_TAGS == null)
            {
                VALID_TAGS = new string[] { "a", "b", "p", "pre", "img", "i", "br", "sub", "sup",
                                            "cite", "strong", "dfn", "em", "kbd", "blockquote", "address", 
                                            "div", "span", 
                                            "h1", "h2", "h3", "h4", "h5", "h6",
                                            "big", "small",
                                            "ul", "li", "ol",
                                            "code", "samp", 
                                            "table", "td", "tbody", "tr", "th", "thead", "tfoot", "caption", "colgroup",  
                                            "label", "hr", 
                                            "map", "area",
                                            "form", "input", "button", "fieldset", "select", "option", "optgroup", "legend", "textarea", 
                                            "object", "param", 
                                            "iframe"};
                Array.Sort(VALID_TAGS);
            }

            string tag = element.Name.ToLowerInvariant();
            int index = Array.BinarySearch(VALID_TAGS, tag);
            if (index < 0)
                throw new TagInvalidException(element.Name);

            ////Check for style attribute
            //foreach (System.Xml.XmlAttribute attribute in element.Attributes)
            //{
            //    if (string.Equals(attribute.Name, "style", StringComparison.InvariantCultureIgnoreCase))
            //        throw new TagAttributeInvalidException(attribute.Name);
            //}
        }

        private void CheckBasicValidation(System.Xml.XmlElement element)
        {
            if (INVALID_TAGS == null)
            {
                INVALID_TAGS = new string[] { "html", "body", "head", "script" };
                Array.Sort(INVALID_TAGS);
            }

            string tag = element.Name.ToLowerInvariant();
            int index = Array.BinarySearch(INVALID_TAGS, tag);
            if (index >= 0)
                throw new TagInvalidException(element.Name);
        }

        private void CheckElements(System.Xml.XmlDocument doc, XHtmlMode mode)
        {
            if (mode == XHtmlMode.None)
                return;

            //Select all the nodes
            System.Xml.XmlNodeList list = doc.SelectNodes("//*");
            foreach (System.Xml.XmlElement element in list)
            {
                if (mode == XHtmlMode.StrictValidation)
                    CheckStrictValidation(element);
                else if (mode == XHtmlMode.BasicValidation)
                    CheckBasicValidation(element);
                else
                    throw new ArgumentException("XHtmlMode not supported", "mode");
            }
        }

        /// <summary>
        /// Automatically create the table of contents, insert the ID attribute for the heading tag if not found
        /// </summary>
        public string GenerateTOC()
        {
            return HTMLHeadingParser.GenerateTOC(mDoc);
        }

        /// <summary>
        /// Search for a div element with the TOC id and insert inside this element the TOC content.
        /// Returns true if the TOC element is presend otherwise false
        /// </summary>
        public bool InsertTOC(string TOC)
        {
            System.Xml.XmlNode tocNode = mDoc.SelectSingleNode("//div[@id='TOC']");

            if (tocNode != null)
            {
                tocNode.InnerXml = TOC;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Replace the links searching for any anchor (a) or image (img) element.
        /// The delegate (callback method) passed is used to generate the new link based on the previous value.
        /// </summary>
        public void ReplaceLinks(ReplaceLinkHandler replaceMethod)
        {
            System.Xml.XmlNodeList anchors = mDoc.SelectNodes("//a");
            System.Xml.XmlNodeList images = mDoc.SelectNodes("//img");

            foreach (System.Xml.XmlElement element in anchors)
            {
                string href = element.GetAttribute("href");

                string newUrl;
                replaceMethod(href, out newUrl);
                element.SetAttribute("href", newUrl);
            }

            foreach (System.Xml.XmlElement element in images)
            {
                string href = element.GetAttribute("src");

                string newUrl;
                replaceMethod(href, out newUrl);
                element.SetAttribute("src", newUrl);
            }
        }

        /// <summary>
        /// Delegate method used to replace the links
        /// </summary>
        /// <param name="oldUrl"></param>
        /// <param name="newUrl"></param>
        public delegate void ReplaceLinkHandler(string oldUrl, out string newUrl);

        /// <summary>
        /// Get a short text of the current xhtml
        /// </summary>
        /// <returns></returns>
        public string GetShortText()
        {
            string shortText = RenderText();

            //Take only the first 100 characters
            if (shortText.Length > 100)
                shortText = shortText.Substring(0, 100);

            return shortText;
        }

        /// <summary>
        /// Return the text without xhtml tags
        /// </summary>
        /// <returns></returns>
        public string RenderText()
        {
            return mDoc.InnerText;
        }

        /// <summary>
        /// Return the text complete with xhtml tags
        /// </summary>
        /// <returns></returns>
        public string RenderHTML()
        {
            return mDoc.InnerXml;
        }

        /// <summary>
        /// Return the xhtml completed with the tags and well indented
        /// </summary>
        /// <returns></returns>
        public string Xhtml
        {
            get
            {
                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(stream, System.Text.Encoding.UTF8);
                    writer.Formatting = System.Xml.Formatting.Indented;
                    mDoc.DocumentElement.WriteContentTo(writer);
                    writer.Flush();

                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                    System.IO.StreamReader reader = new System.IO.StreamReader(stream, System.Text.Encoding.UTF8);

                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Returns XTHML content with all the formatting(style attributes only at this sprint; 15-09-2011) stripped out
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <returns></returns>
        public string FormatXHTML(string htmlContent)
        {
            if (!string.IsNullOrEmpty(htmlContent))
            {
                string[] htmlTags = htmlContent.Split('>');
                List<string> strippedHTML = new List<string>();

                foreach (string str in htmlTags)
                {
                    if (!str.ToLower().Contains("img"))
                    {

                        if (htmlTags[htmlTags.Length - 1] != str)
                            strippedHTML.Add(str + ">");
                        else
                            strippedHTML.Add(str); // this is avoid adding '>' at the end.

                    }
                    else
                    {
                        if (htmlTags[htmlTags.Length - 1] != str)
                            strippedHTML.Add(str + ">");
                        else
                            strippedHTML.Add(str); // this is avoid adding '>' at the end.
                    }
                }



                //reinitialise the string to hold the new stripped HTML
                htmlContent = string.Empty;

                //combine all html tags to form the content.
                foreach (string item in strippedHTML)
                {
                    htmlContent += item;
                }
            }
            //strip font tags.
            htmlContent = Regex.Replace(htmlContent, "<font .*?>", string.Empty);
            htmlContent = Regex.Replace(htmlContent, "</font .*?>", string.Empty);

            return htmlContent;
        }

        public string StripWordTags(string htmlContent)
        {
            string replacedImgTag = htmlContent.Replace("v:imagedata", "img");
            string htmlCont = Regex.Replace(replacedImgTag, "<v:.*?>", string.Empty);
            htmlCont = Regex.Replace(htmlCont, "</v:.*?>", string.Empty);
            htmlCont = Regex.Replace(htmlCont, "<o:.*?>", string.Empty);
            htmlCont = Regex.Replace(htmlCont, "</o:.*?>", string.Empty);

            MatchCollection collection = Regex.Matches(htmlCont, "<img .*>");
            collection = Regex.Matches(htmlCont, "<IMG .*?>");

            foreach (Match item in collection)
            {
                string imgTag = item.Value.ToString();
                Match m = Regex.Match(imgTag, "src=\".*\" ");
                //UploadImage(m.Value);
            }

            return htmlCont;
        }

        public IList<string> GetSubChapters(string htmlContent)
        {
            List<string> subChapters = new List<string>();
            if (!string.IsNullOrEmpty(htmlContent))
            {
                string[] htmlTags = htmlContent.Split('>');

                foreach (string str in htmlTags)
                {
                    if (str.ToLower().Contains("/h2"))
                    {
                        string h3Heading = str.Substring(0, str.IndexOf("<"));
                        subChapters.Add(h3Heading);
                    }
                }
            }
            return subChapters;
        }

        public string AddAnchorTags(string htmlContent)
        {
            List<string> taggedContent = new List<string>();

            if (!string.IsNullOrEmpty(htmlContent))
            {
                string[] htmlTags = htmlContent.Split('>');

                foreach (string str in htmlTags)
                {
                    if (str.ToLower().Contains("/h2")) //only add h3 tags - might be modified later to include other h tags.
                    {
                        string name = str.Substring(0, str.IndexOf("<"));
                        name = name.Trim();//remove any trailing white spaces.

                        if (name.Contains(" "))
                            name = name.Replace(" ", "+");// replace  space b/w words with a UrlEncode character.

                        string tag = "<a name=\"" + name + "\"></a>";
                        string tagged = tag + str; //adding tag.

                        if (htmlTags[htmlTags.Length - 1] != str) //close the tag
                            taggedContent.Add(tagged + ">");
                        else
                            taggedContent.Add(tagged);
                    }
                    else
                    {
                        //close any other tags.
                        if (htmlTags[htmlTags.Length - 1] != str)
                            taggedContent.Add(str + ">");
                        else
                            taggedContent.Add(str);
                    }
                }
            }
            htmlContent = string.Empty;
            foreach (string item in taggedContent)
            {
                htmlContent += item;
            }
            return htmlContent;
        }

        public void UploadImage(string src)
        {
            src = src.TrimEnd(' ');//trim the end for white spaces
            src = src.Substring(13, (src.Length - 1) - 13); //this is the source for file to be uploaded.
            try
            {
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string AddChapNumbers(string htmlContent, int chapNumb)
        {
            if (!string.IsNullOrEmpty(htmlContent))
            {
                string[] htmlTags = htmlContent.Split('>');
                List<string> numberedHTML = new List<string>();

                int h2Numb = 1;
                int h3Numb = 1;
                int h4Numb = 1;
                int h5Numb = 1;


                foreach (string str in htmlTags)
                {
                    if (str.ToLower().Contains("/h2"))
                    {
                        h3Numb = 1;
                        h4Numb = 1;

                        string tag = "<label>" + chapNumb.ToString() + "." + h2Numb.ToString() + "&nbsp;" + "</label>";
                        string tagged = tag + str; //adding tag.

                        if (htmlTags[htmlTags.Length - 1] != str) //close the tag
                            numberedHTML.Add(tagged + ">");
                        else
                            numberedHTML.Add(tagged);

                        h2Numb++;
                    }
                    else if (str.ToLower().Contains("/h3"))
                    {
                        h4Numb = 1;
                        h5Numb = 1;

                        string tag = "<label>" + chapNumb.ToString() + "." + (h2Numb - 1).ToString() + "." + h3Numb.ToString() + "&nbsp;" + "</label>";
                        string tagged = tag + str; //adding tag.

                        if (htmlTags[htmlTags.Length - 1] != str) //close the tag
                            numberedHTML.Add(tagged + ">");
                        else
                            numberedHTML.Add(tagged);

                        h3Numb++;
                    }
                    else if (str.ToLower().Contains("/h4"))
                    {
                        h5Numb = 1;

                        string tag = "<label>" + chapNumb.ToString() + "." + (h2Numb - 1).ToString() + "." + (h3Numb - 1).ToString() + "." + h4Numb.ToString() + "&nbsp;" + "</label>";
                        string tagged = tag + str; //adding tag.

                        if (htmlTags[htmlTags.Length - 1] != str) //close the tag
                            numberedHTML.Add(tagged + ">");
                        else
                            numberedHTML.Add(tagged);
                        h4Numb++;
                    }
                    else if (str.ToLower().Contains("/h5"))
                    {
                        string tag = "<label>" + chapNumb.ToString() + "." + (h2Numb - 1).ToString() + "." + (h3Numb - 1).ToString() + "." + (h4Numb - 1).ToString() + "." + h5Numb.ToString() + "&nbsp;" + "</label>";
                        string tagged = tag + str; //adding tag.

                        if (htmlTags[htmlTags.Length - 1] != str) //close the tag
                            numberedHTML.Add(tagged + ">");
                        else
                            numberedHTML.Add(tagged);
                        h5Numb++;
                    }
                    else
                    {
                        //close any other tags.
                        if (htmlTags[htmlTags.Length - 1] != str)
                            numberedHTML.Add(str + ">");
                        else
                            numberedHTML.Add(str);
                    }
                }
                htmlContent = string.Empty;
                foreach (string item in numberedHTML)
                {
                    htmlContent += item;
                }

            }
            return htmlContent;
        }
    }
}