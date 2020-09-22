using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Collections;

namespace BusiBlocks
{
    public class BbTreeViewNodeMenu
    {
        private const string SpanCssClassDefault = "tvContext";
        private const string SpanNameDefault = "contextMenu";
        private const string SpanIdPrefixDefault = "Menu";
        private const string SpanStyleDefault = "display:none;";

        private string Id;
        private List<HtmlAnchor> MenuItems;

        public BbTreeViewNodeMenu(string id)
        {
            Id = id;
            MenuItems = new List<HtmlAnchor>();
        }

        public void Add(HtmlAnchor item)
        {
            if (MenuItems == null)
                MenuItems = new List<HtmlAnchor>();

            MenuItems.Add(item);
        }

        public void AddRange(List<HtmlAnchor> collection)
        {
            if (MenuItems == null)
                MenuItems = new List<HtmlAnchor>();

            MenuItems.AddRange(collection);
        }

        public void Remove(HtmlAnchor Anchor)
        {
            MenuItems.Remove(Anchor);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (MenuItems != null && MenuItems.Count > 0)
            {
                sb.Append("<span");
                sb.Append(String.Format(" class='{0}'", SpanCssClassDefault));
                sb.Append(String.Format(" name='{0}'", SpanNameDefault));
                sb.Append(String.Format(" style='{0}'", SpanStyleDefault));
                sb.Append(String.Format(" id='{0}{1}'", SpanIdPrefixDefault, Id));
                sb.Append(">");

                foreach (HtmlAnchor menuItem in MenuItems)
                {

                    sb.Append("<a");
                    
                    if(!string.IsNullOrEmpty(menuItem.HRef))
                        sb.Append(String.Format(" href='{0}'", menuItem.HRef));

                    IEnumerator keys = menuItem.Attributes.Keys.GetEnumerator();

                    while (keys.MoveNext())
                    {
                        String key = (String)keys.Current;
                        if (key != "InnerText")
                        {
                            sb.Append(String.Format(" {0}='{1}'", key, menuItem.Attributes[key]));
                        }
                    }                    

                    sb.Append(">");
                    sb.Append(menuItem.InnerText);
                    sb.Append("</a>");
                }
                sb.Append("</span>");
            }
            else
            {
                sb.Append("");
            }
            return sb.ToString();
        }
    }
}
