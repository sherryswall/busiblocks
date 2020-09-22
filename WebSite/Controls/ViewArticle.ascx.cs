using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Xml;
using BusiBlocks.Audit;
using System.Reflection;

public partial class Controls_ViewArticle : System.Web.UI.UserControl
{
    protected string script;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BusiBlocks.DocoBlock.Article latestArticle = BusiBlocks.DocoBlock.DocoManager.GetArticleByName(ArticleName, false);

            hidReferrer.Value = Request.UrlReferrer.OriginalString;            

            if (latestArticle == null)
            {
                controlDiv.Visible = false;
                sectionError.Visible = true;
            }
            else
            {
                controlDiv.Visible = true;
                sectionError.Visible = false;
                LoadArticle(latestArticle);
            }
        }
    }

    #region Properties
    /// <summary>
    /// Gets or sets the article name to show. The value is saved in the ViewState.
    /// </summary>
    public string ArticleName
    {
        get
        {
            object val = ViewState["ArticleName"];
            return (string)val;
        }
        set { ViewState["ArticleName"] = value; }
    }

    /// <summary>
    /// Gets or sets the article version to show. 
    /// If 0 the latest version is used
    /// The value is saved in the ViewState.
    /// </summary>
    public int ArticleVersion
    {
        get
        {
            object val = ViewState["ArticleVersion"];
            return val == null ? 0 : (int)val;
        }
        set { ViewState["ArticleVersion"] = value; }
    }

    /// <summary>
    /// Gets or sets if show the actions section
    /// </summary>
    public bool SectionActionsVisible
    {
        get
        {
            object val = ViewState["SectionActionsVisible"];
            return val == null ? true : (bool)val;
        }
        set { ViewState["SectionActionsVisible"] = value; }
    }

    /// <summary>
    /// Gets or sets if show the properties section
    /// </summary>
    public bool SectionPropertiesVisible
    {
        get
        {
            object val = ViewState["SectionPropertiesVisible"];
            return val == null ? true : (bool)val;
        }
        set { ViewState["SectionPropertiesVisible"] = value; }
    }

    /// <summary>
    /// Gets or sets if before loading the article the attachments links must be replaced and the TOC inserted inside the article
    /// </summary>
    public bool ElaborateOutput
    {
        get
        {
            object val = ViewState["ElaborateOutput"];
            return val == null ? true : (bool)val;
        }
        set { ViewState["ElaborateOutput"] = value; }
    }

    public bool Printing { get; set; }

    #endregion

    private void LoadArticle(BusiBlocks.DocoBlock.Article latestArticle)
    {
        try
        {
            BusiBlocks.DocoBlock.ArticleBase article;

            if (!BusiBlocks.SecurityHelper.CanUserView(Page.User.Identity.Name, latestArticle.Category.Id))
                throw new BusiBlocks.InvalidGroupMembershipException();

            if (ArticleVersion == 0)
                article = latestArticle;
            else
                article = BusiBlocks.DocoBlock.DocoManager.GetArticleByVersion(latestArticle, ArticleVersion);

            lblArticleTitle.InnerText = article.Title;
            lblAuthor.InnerText = Utilities.GetDisplayUser(article.Author);
            lblDate.InnerText = Utilities.GetDateTimeForDisplay(article.UpdateDate);
            lblVersion.InnerText = article.Version.ToString();
            lblArticleDescription.InnerText = article.Description;

            string body = article.Body;
            if (ElaborateOutput)
                body = ElaborateXHTML(latestArticle, article);

            if (Printing)
            {
                sectionTOC.Visible = false;
                sectionBody.InnerHtml = body;
            }
            else
            {
                if (article.TOC == null)
                {
                    sectionTOC.Visible = false;
                    sectionBody.Attributes["class"] = "sectionNoTOC";
                }
                else if (article.TOC.Length > 0)
                {
                    Control TOC = ParseControl(article.TOC);
                    SetControlVisibility(TOC);
                    sectionTOC.Controls.Add(TOC);
                }
                else
                {
                    sectionBody.Attributes["class"] = "sectionNoTOC";
                    sectionTOC.Visible = false;
                }
                AuditManager.Audit(Page.User.Identity.Name, article.Id + article.Version.ToString(), AuditRecord.AuditAction.Viewed);
            }

            sectionBody.InnerHtml = body;

            linkLatestVersion.HRef = Navigation.Doco_ViewArticle(latestArticle.Name, 0).GetServerUrl(true);
            linkBrowseVersions.HRef = Navigation.Doco_ViewArticleVersions(latestArticle.Name).GetServerUrl(true);

            //Show the edit only if EditLinkVisible and this is the latest article version
            bool enabledEdit = BusiBlocks.SecurityHelper.CanUserEdit(Page.User.Identity.Name, latestArticle.Category.Id);
            linkEdit.Visible = enabledEdit && latestArticle == article;
            linkEdit.HRef = Navigation.Doco_EditArticle(ArticleName).GetServerUrl(true);
            linkPrint.HRef = Navigation.Doco_PrintArticle(ArticleName, ArticleVersion).GetServerUrl(true);

            sectionActions.Visible = SectionActionsVisible;

            sectionProperties.Visible = SectionPropertiesVisible;

            btnAcknowledge.Visible = article.RequiresAck && !article.Acknowledged;
        }
        catch(Exception ex)
        {
            throw ex;
            ((IFeedback)Page.Master).SetException(GetType(), ex);
        }
    }

    private string ElaborateXHTML(BusiBlocks.DocoBlock.Article latestArticle, BusiBlocks.DocoBlock.ArticleBase article)
    {
        var xhtml = new BusiBlocks.XHTMLText();
        xhtml.Load(article.Body);

        string[] attachments = BusiBlocks.DocoBlock.DocoManager.GetFileAttachments(latestArticle, BusiBlocks.DocoBlock.EnabledStatus.Enabled);
        Array.Sort<string>(attachments);

        xhtml.ReplaceLinks(delegate(string oldUrl, out string newUrl)
                            { ReplaceLink(latestArticle.Name, attachments, oldUrl, out newUrl); }
                            );

        return xhtml.Xhtml;
    }

    private void ReplaceLink(string articleName, string[] attachments, string oldUrl, out string newUrl)
    {
        const string BUSIBLOCKS_URL = "busiblocks:";

        //The returned url must be a plain text url (busiblocks automatically encode it when changing the html)

        //Is an attachment
        if (Array.BinarySearch<string>(attachments, oldUrl) >= 0)
            newUrl = Navigation.Doco_Attach(articleName, oldUrl, false).GetClientUrl(this, false);
        else
        {
            //Is a busiblocks path
            if (oldUrl.StartsWith(BUSIBLOCKS_URL, StringComparison.InvariantCultureIgnoreCase))
            {
                string linkArticle = oldUrl.Substring(BUSIBLOCKS_URL.Length, oldUrl.Length - BUSIBLOCKS_URL.Length);

                newUrl = Navigation.Doco_ViewArticle(linkArticle, 0).GetClientUrl(this, false);
            }
            else
                newUrl = oldUrl;
        }
    }

    private void ShowParents(Control control)
    {
        control.Visible = true;

        if (control.ID != sectionTOC.ID)
        {
            if(control.Parent != null)
                ShowParents(control.Parent);
        }
    }

    private void ShowChildren(Control control)
    {
        for (int i = 0; i < control.Controls.Count; i++)
        {
            control.Controls[i].Visible = true;

            ShowChildren(control.Controls[i]);
        }
    }    

    private string GetAttributeValue(Control control, string attributeName)
    {
        string attributeValue = "";

        if (control.GetType() == (new HtmlAnchor()).GetType())
        {
            attributeValue = ((HtmlAnchor)control).Attributes[attributeName];
        }
        else if (control.GetType() == (new HtmlGenericControl()).GetType())
        {
            attributeValue = ((HtmlGenericControl)control).Attributes[attributeName];
        }

        return attributeValue;
    }

    private void SetControlVisibility(Control control)
    {
        SetControlVisibility(control, null);
    }

    private void SetControlVisibility(Control parentControl, string articleNameId)
    {
        for (int i = 0; i < parentControl.Controls.Count; i++)
        {
            Control childControl = parentControl.Controls[i];

            if (!string.IsNullOrEmpty(childControl.ID))
            {
                int level = Int32.Parse(GetAttributeValue(childControl, "level"));

                if (level <= 2)
                {
                    childControl.Visible = true;
                    SetControlVisibility(childControl, articleNameId);
                }
                else if (level == 3)
                {
                    string articleOneLevelUp = GetArticleLevelUp(articleNameId, 1);
                    string articleTwoLevelUp = GetArticleLevelUp(articleNameId, 2);

                    if (childControl.ID == articleNameId || childControl.ID == "ul_" + articleNameId || childControl.ID == "li_" + articleNameId
                        || childControl.ID == articleOneLevelUp || childControl.ID == "ul_" + articleOneLevelUp || childControl.ID == "li_" + articleOneLevelUp
                        || childControl.ID == articleTwoLevelUp || childControl.ID == "ul_" + articleTwoLevelUp || childControl.ID == "li_" + articleTwoLevelUp)
                    {
                        childControl.Visible = true;
                        ShowChildren(childControl);
                        ShowParents(childControl);
                    }
                    else
                    {
                        childControl.Visible = false;
                        SetControlVisibility(childControl, articleNameId);
                    }
                }

                else if (level == 4)
                {
                    if (childControl.ID == articleNameId || childControl.ID == "ul_" + articleNameId || childControl.ID == "li_" + articleNameId)
                    {
                        childControl.Visible = true;
                        ShowChildren(childControl);
                        ShowParents(childControl);
                    }
                    else
                    {
                        childControl.Visible = false;
                        SetControlVisibility(childControl, articleNameId);
                    }
                }
                else
                {
                    // This code should be never reached as levels are only 4 deep.
                    childControl.Visible = false;
                    SetControlVisibility(childControl, articleNameId);
                }
            }
        }
    }

    private static string GetArticleLevelUp(string articleName, int levelsUp)
    {

        string newArticleName = articleName;

        for (int i = 0; i < levelsUp; i++)
        {
            if ((newArticleName != null) && (newArticleName.LastIndexOf('_') >= 0))
            {
                newArticleName = newArticleName.Substring(0, newArticleName.LastIndexOf('_'));
            }
        }

        return newArticleName;
    }

    protected void lnkSection_Click(object sender, EventArgs e)
    {
        string ArticleNameId = Request["__EVENTARGUMENT"];
        BusiBlocks.DocoBlock.Article latestArticle = BusiBlocks.DocoBlock.DocoManager.GetArticleByName(ArticleName, false);
        BusiBlocks.DocoBlock.ArticleBase article;

        if (ArticleVersion == 0)
            article = latestArticle;
        else
            article = BusiBlocks.DocoBlock.DocoManager.GetArticleByVersion(latestArticle, ArticleVersion);

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml("<root>" + article.Body + "</root>");

        XmlNode hTag = xmlDoc.DocumentElement.SelectSingleNode("//*[@id='" + Request["__EVENTARGUMENT"] + "']");

        string firstTag = hTag.OuterXml;

        int firstIndex = xmlDoc.DocumentElement.InnerXml.IndexOf(firstTag);

        int lastIndex = GetEndPosition(firstIndex, xmlDoc.DocumentElement.InnerXml, firstTag);

        if (lastIndex == -1) lastIndex = xmlDoc.DocumentElement.InnerXml.Length;

        int length = lastIndex - firstIndex;

        string section = xmlDoc.DocumentElement.InnerXml.Substring(firstIndex, length);

        sectionBody.InnerHtml = section;

        Control TOC = ParseControl(article.TOC);

        SetControlVisibility(TOC, ArticleNameId);
        
        sectionTOC.Controls.Add(TOC);
    }

    private int GetEndPosition(int startIndex, string body, string tag)
    {
        int level = Convert.ToInt32(tag.Substring(2, 1));

        int lastIndex = body.IndexOf(tag.Substring(0, 3), startIndex + 2);

        for (int i = level; i > 0; i--)
        {
            if (body.IndexOf("<h" + i.ToString(), startIndex + 2) < lastIndex)
                if (body.IndexOf("<h" + i.ToString(), startIndex + 2) > 0)
                    lastIndex = body.IndexOf("<h" + i.ToString(), startIndex + 2);
        }

        return lastIndex;
    }

    protected void btnAcknowledge_Click(object sender, EventArgs e)
    {
        BusiBlocks.DocoBlock.Article latestArticle = BusiBlocks.DocoBlock.DocoManager.GetArticleByName(ArticleName, false);

        AuditManager.Audit(Page.User.Identity.Name, latestArticle.Id + latestArticle.Version.ToString(), AuditRecord.AuditAction.Acknowledged);

        Response.Redirect(hidReferrer.Value);
    }

    protected void lnkBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(hidReferrer.Value);
    }
}
