using System;
using System.Globalization;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Resources;

    #region Comments
    /// <summary>
    /// Provides common functionality for all pages.
    /// </summary>
    /// <remarks>
    /// <h3>Changes</h3>
    /// <list type="table">
    /// 	<listheader>
    /// 		<th>Author</th>
    /// 		<th>Date</th>
    /// 		<th>Details</th>
    /// 	</listheader>
    /// 	<item>
    /// 		<term>Mark Merrens</term>
    /// 		<description>17/03/2010</description>
    /// 		<description>Created</description>
    /// 	</item>
    /// </list>
    /// </remarks>
    #endregion

    public abstract class BasePage : Page
    {
        #region Overrides
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Page.InitComplete"/> event after page initialization.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnInitComplete(EventArgs e)
        {
            base.OnInitComplete(e);

            


            // Get and use the embedded styles.
            string styles = Page.ClientScript.GetWebResourceUrl(typeof(BasePage), Resource.STYLES);
            ((HtmlHead)Page.Header).Controls.Add(new LiteralControl(String.Format(Resource.STYLE_LINK, styles)));

            // Set the title.
            Title = Resource.Title;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //// Add the copyright and version text.
            //Label label = new Label();
            //label.Text =
            //    string.Format(
            //        CultureInfo.InvariantCulture,
            //        Resource.Copyright,
            //        Assembly.GetExecutingAssembly().GetName().Version);
            //label.CssClass = Resource.Copyright_CSS;
            //Controls.Add(label);
        }
        #endregion
    }

