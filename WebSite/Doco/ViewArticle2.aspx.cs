using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BusiBlocks.DocoBlock;
using System.Web.Services;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Remoting.Contexts;
using BusiBlocks.Audit;
using System.Threading;
using Telerik.Web.UI;
using BusiBlocks;
using AjaxControlToolkit;

public partial class ViewArticle2 : System.Web.UI.Page
{
    #region Properties

    public Article Article { get; set; }
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
    /// Gets or sets the chapter Name.
    /// </summary>
    public static string ChapterName { get; set; }

    /// <summary>
    /// Gets or sets the ChapterVersion ID.
    /// </summary>
    public static string ChapterId { get; set; }
    public static string tempChapterId { get; set; }
    /// <summary>
    /// Gets or sets the Article/Document ID.
    /// </summary>
    public static string ArticleId { get; set; }
    public static string DraftId { get; set; }

    public static bool IsEditorChanged { get; set; }
    public static bool IsPreviewMode { get; set; }
    public static bool IsNameChanged { get; set; }
    public static bool IsTimerRunning { get; set; }
    public static bool IsAdminMode { get; set; }
    public static bool IsNumbChaps { get; set; }

    public static string contentMode { get; set; }
    public static string EditorContent { get; set; }

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

    #endregion

    #region PageMethods

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request.QueryString["name"]))
        {
            ArticleName = Request.QueryString["name"].ToString();
        }
        if (!Page.IsPostBack)
        {
            //Session["Article"] = null;
            if (!string.IsNullOrEmpty(Request.QueryString["name"]))
            {
                LoadArticle();
                BindTrafficLight();
            }
        }
        if (!string.IsNullOrEmpty(Request.QueryString["cMode"]))
        {
            contentMode = Request.QueryString["cMode"].ToString();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.UrlReferrer != null)
                ViewState["RefUrl"] = Request.UrlReferrer.ToString();

            BusiBlocks.DocoBlock.Article latestArticle = BusiBlocks.DocoBlock.DocoManager.GetArticleByName(ArticleName, false);

            LoadChapter(true);

            tempChapterId = ChapterId;
            CreateCustomToolBarBtns();
            AddParaHeadingStyles();
            BindTrafficLight();
        }
        else
        {
            LoadViewState();
        }
        BindArticles();
        BindSequence();
    }

    protected void pnlChapsList_Load(object sender, EventArgs e)
    {
        BindSequence();
    }

    protected void rlTemplate_ItemReorder(object sender, AjaxControlToolkit.ReorderListItemReorderEventArgs e)
    {
        ChapterVersion chapterVers = new ChapterVersion();

        IList<ChapterVersion> allChapters = DocoManager.GetAllItemsByArticleId(ArticleId);

        HiddenField lblIdSender = (HiddenField)e.Item.FindControl("hdnId");

        //item moving down  -- all items below it will move up starting from the new index
        if (e.NewIndex > e.OldIndex)
        {
            for (int i = e.NewIndex; i > e.OldIndex; i--)
            {
                HiddenField lblId = (HiddenField)ReorderList1.Items[i].FindControl("hdnId");
                chapterVers = allChapters.First(x => x.Id.Equals(lblId.Value));

                chapterVers.Sequence = i - 1;
                BusiBlocks.DocoBlock.DocoManager.UpdateChapterVersion(chapterVers);
            }
        }
        else if (e.NewIndex < e.OldIndex)
        {
            for (int i = e.NewIndex; i < e.OldIndex; i++)
            {
                HiddenField lblId = (HiddenField)ReorderList1.Items[i].FindControl("hdnId");
                chapterVers = allChapters.First(x => x.Id.Equals(lblId.Value));

                chapterVers.Sequence = i + 1;
                BusiBlocks.DocoBlock.DocoManager.UpdateChapterVersion(chapterVers);
            }
        }

        //update the item itself        
        chapterVers = allChapters.First(x => x.Id.Equals(lblIdSender.Value));

        chapterVers.Sequence = e.NewIndex;
        BusiBlocks.DocoBlock.DocoManager.UpdateChapterVersion(chapterVers);

        BindSequence();
    }

    protected void rlTemplate2_ItemDataBound(object sender, AjaxControlToolkit.ReorderListItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Header || e.Item.ItemType != ListItemType.Footer)
        {
            Repeater rptSubChaps = (Repeater)e.Item.FindControl("rptSubChapters");
            rptSubChaps.Visible = true;
            HiddenField cid = (HiddenField)e.Item.FindControl("hdnId");

            if (IsNumbChaps)
            {
                System.Web.UI.WebControls.Label lblChapterNumber = (System.Web.UI.WebControls.Label)e.Item.FindControl("lblChapNumbTOC");
                lblChapterNumber.Visible = true;
            }
            rptSubChaps.DataSource = DocoManager.GetSubChapters(cid.Value);
            rptSubChaps.DataBind();
        }
    }

    protected void rptSubChaps_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Header || e.Item.ItemType != ListItemType.Footer)
        {
            //display sub section numbers if numbering is on.
            if (IsNumbChaps)
            {
                System.Web.UI.WebControls.Label lblSubCHapterNumberTOC = (System.Web.UI.WebControls.Label)e.Item.FindControl("lblSubChapNumbTOC");
                lblSubCHapterNumberTOC.Text = (e.Item.ItemIndex + 1).ToString() + ".";
            }
            HyperLink a = (HyperLink)e.Item.FindControl("subChapAnchor");
            a.Text = e.Item.DataItem.ToString();
            a.NavigateUrl = "#" + Server.UrlEncode(e.Item.DataItem.ToString().Trim());
        }
    }

    //Binding Chapters' sub sections to the insert link list
    protected void rptChapsList_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Footer || e.Item.ItemType != ListItemType.Header)
        {
            HiddenField hfChapId = (HiddenField)e.Item.FindControl("hdnChapId");
            Repeater rptChapSecList = (Repeater)e.Item.FindControl("rptChapSecList");

            rptChapSecList.DataSource = DocoManager.GetChapterSubSection(hfChapId.Value);
            rptChapSecList.DataBind();
        }
    }

    /// <summary>
    /// Add a new chapter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        BindSequence();
    }

    protected void btnSaveContent_Click(object sender, EventArgs e)
    {
        //create a new version for chapter

        //add content to the chapter
        //SaveContent(ChapterId, "button");       
        if (!string.IsNullOrEmpty(ChapterId))
        {
            XHTMLText formattedHTML = new XHTMLText();

            string content = formattedHTML.FormatXHTML(txtEditor.Content);

            SaveDraft(ChapterId, content);

            DisplayUpdateMessage("Last saved at : " + DateTime.Now.ToLongTimeString());
            IsEditorChanged = false;
            //IsNameChanged = false;
            lstChangedChaps.Items.Add(ChapterId);
            //Bind the new version to the TOC
            BindSequence();
            //load the saved chapter.
            LoadChapter(false);
        }
    }

    protected void btnManage_Click(object sender, EventArgs e)
    {
        //updated chapters that have been changed.
        foreach (AjaxControlToolkit.ReorderListItem i in ReorderList1.Items)
        {
            LinkButton lnkBtn = new LinkButton();
            lnkBtn = (LinkButton)i.FindControl("lnkBtnChap");
            //go thru all the chapters in the TOC and update them. This helps in checking if any chapter has been deleted.
            //NOTE: Currently - this is direct publishing!.
            // foreach (ListItem item in lstChangedChaps.Items)
            // {
            // if (item.Text.Equals(lnkBtn.CommandName))
            // {            
            SaveContent(lnkBtn.CommandName.ToString(), i.DisplayIndex);
            //break;
            // }
            // }
        }
        Navigation.Doco_EditArticle(ArticleName).Redirect(this); //go to category page.
    }

    protected void btnSaveContent_ChapClick(object sender, EventArgs e)
    {
        //add content to the chapter

        //if (IsEditorChanged)
        //  SaveDraft(tempChapterId, txtEditor.Content); //SaveContent(tempChapterId, "link");

        // IsEditorChanged = false;

        //EditorContent = txtEditor.Content;
        //lstChangedChaps.Items.Add(tempChapterId); -- adding chapters to a temporary list which would be used in finish editing/publishing button
        //pnlChapsList_Load(sender, e);
    }

    protected void btnSaveDraft_Click(object sender, EventArgs e)
    {
        SaveDraft();
        lblChapterName.Text = ChapterName;
        DisplayUpdateMessage("Last draft saved on: " + DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToShortTimeString());

        if (((System.Web.UI.WebControls.Button)sender).CommandName.Equals("SaveDraftLoadChapter"))
            LoadChapter(false);
    }

    protected void btnImgUpload_Click(object sender, EventArgs e)
    {
        string root = Path.Combine(Path.Combine(Server.MapPath("~"), "Doco"), "Files");
        string path = Path.Combine(root, fuImg.FileName);

        //load the saved chapter.
        LoadChapter(false);

        Image img = new Image();

        img.Width = Unit.Pixel(Int32.Parse(string.IsNullOrEmpty(txtWidth.Text) ? "300" : txtWidth.Text));
        img.Height = Unit.Pixel(Int32.Parse(string.IsNullOrEmpty(txtHeight.Text) ? "300" : txtHeight.Text));

        fuImg.SaveAs(path);
    }

    protected void Timer1_Tick(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(ChapterId) && IsEditorChanged)
        {
            XHTMLText formattedHTML = new XHTMLText();

            string content = formattedHTML.FormatXHTML(txtEditor.Content);

            SaveDraft(ChapterId, content);
            DisplayUpdateMessage("Last saved at : " + DateTime.Now.ToLongTimeString());
            lstChangedChaps.Items.Add(ChapterId);
        }
    }

    private void DisplayUpdateMessage(string msg)
    {
        lblLastDraft.Text = "<div class='docFeedback'>" + msg + "</div>";
    }

    protected void btnAcknowledge_Click(object sender, EventArgs e)
    {
        AuditManager.Audit(Utilities.GetUserName(Page.User.Identity.Name), ArticleId, AuditRecord.AuditAction.Acknowledged);
        BindTrafficLight();
    }

    private void BindTrafficLight()
    {
        // The RadTabStrip1 is the edit/preview strip. This is visible when we are in edit mode.
        // Therefore don't show the acknowledge checkbox if we are in edit mode.

        if (this.RadTabStrip1.Visible)
        {
            ackText.Visible = false;
        }
        else
        {
            Article article = DocoManager.GetArticle(ArticleId);
            if (SecurityHelper.CanUserView(Page.User.Identity.Name, article.Category.Id))
            {
                if (!string.IsNullOrEmpty(Request.QueryString["mode"]) && Request.QueryString["mode"].Equals("approve"))
                {
                    divAckButton.Attributes["class"] = "divAckChecked";
                    btnAcknowledge.Enabled = false;
                }
                else if (article.RequiresAck)
                {
                    if (article.Acknowledged)
                    {
                        divAckButton.Attributes["class"] = "divAckChecked";
                        btnAcknowledge.Enabled = false;
                    }
                    else
                    {
                        divAckButton.Attributes["class"] = "divAckButton";
                    }
                }
                else
                {
                    //check the auti record for the article- audit if no records found.
                    IList<AuditRecord> records = AuditManager.GetAuditItems(Page.User.Identity.Name, ArticleId, AuditRecord.AuditAction.Viewed);
                    if (records.Count == 0)
                    {
                        AuditManager.Audit(Page.User.Identity.Name, ArticleId, AuditRecord.AuditAction.Viewed);
                    }
                    ackText.Visible = false;
                    
                }
            }
            else
            {
                divAckButton.Visible = false;
                spanAcked.Visible = false;
            }
        }
    }

    protected void btnReturn_Click(object sender, EventArgs e)
    {
        Navigation.Doco_Default().Redirect(this);
    }

    #region Article

    /// <summary>
    /// Binds the articles to the list in link selection div. 
    /// </summary>
    protected void BindArticles()
    {
        try
        {
            rptDocusList.DataSource = DocoManager.GetAllArticles();
            rptDocusList.DataBind();
        }
        catch (Exception ex)
        {
            //Log - unable to bind articles list.
            throw ex;
        }
    }

    /// <summary>
    /// Loads the meta information for the selected document. Called once on PageInit.
    /// </summary>
    protected void LoadArticle()
    {
        BusiBlocks.DocoBlock.ArticleBase article;

        BusiBlocks.DocoBlock.Article latestArticle = BusiBlocks.DocoBlock.DocoManager.GetArticleByName(ArticleName, false);

        //if (BusiBlocks.SecurityHelper.CanRead(Page.User, latestArticle.Category, latestArticle) == false)
        //  throw new BusiBlocks.InvalidPermissionException("read article");

        if (!BusiBlocks.SecurityHelper.CanUserView(Page.User.Identity.Name, latestArticle.Category.Id) &&
                !BusiBlocks.SecurityHelper.CheckWriteAccess(Page.User.Identity.Name, latestArticle.Category.Id))
            throw new BusiBlocks.InvalidGroupMembershipException();

        EditMode(latestArticle.Category.Id);

        if (ArticleVersion == 0)
            article = latestArticle;
        else
            article = BusiBlocks.DocoBlock.DocoManager.GetArticleByVersion(latestArticle, ArticleVersion);

        LoadArticleMeta(article);

        IsNumbChaps = latestArticle.NumberedChaps;

        //if admin is adding or editing the document then don't audit it.
        if (Request.UrlReferrer != null)
            if (Request.UrlReferrer.AbsoluteUri.Contains("NewArticle.aspx") == false && Request.UrlReferrer.AbsoluteUri.Contains("EditArticle.aspx") == false)
                AuditManager.Audit(Page.User.Identity.Name, article.Id + article.Version.ToString(), AuditRecord.AuditAction.Viewed);
    }

    protected void LoadArticleMeta(ArticleBase article)
    {
        ArticleName = article.Title;
        ArticleId = article.Id;
        lblAuthor.Text = Utilities.GetDisplayUserName(article.Owner);
        lblDate.InnerText = Utilities.GetDateTimeForDisplay(article.UpdateDate);
        lblVersion.InnerText = "1.0";// article.Version.ToString();
        if (!article.RequiresAck)
            ackLabel.Text = "Viewed:";
        imgAck.ImageUrl = BusiBlocks.DocoBlock.Utility.GetTrafficLight(article);
    }

    /// <summary>
    /// Checks if the article is to be viewed in edit mode. And configures the page accordingly.
    /// </summary>
    /// <param name="articleCategoryId"></param>
    protected void EditMode(string articleCategoryId)
    {
        if (!BusiBlocks.SecurityHelper.CheckWriteAccess(Page.User.Identity.Name, articleCategoryId))
        {
            IsAdminMode = false;
            ReorderList1.AllowReorder = false;
            pnlEditor.Visible = false;
            pnlAddChapter.Visible = false;
            txtBoxChapterName.Visible = false;
            RadTabStrip1.Visible = false;
            RadTabStrip1.SelectedIndex = 0;
            RadPageView1.Selected = true;
        }
        else
        {
            IsAdminMode = true;
            RadTabStrip1.SelectedIndex = 1;
            RadPageView2.Selected = true;
        }

        //dont display the switch mode tabs when in preview mode.
        if (Request.QueryString["cMode"].Equals("pub"))
        {
            RadTabStrip1.Visible = false;
            RadTabStrip1.SelectedIndex = 0;
            RadPageView1.Selected = true;
        }
    }

    protected void LoadViewState()
    {
        if (Session["Article"] != null)
        {
            ArticleBase arti = (Article)Session["Article"];
            LoadArticleMeta(arti);
        }
    }

    #endregion

    #region Chapter

    /// <summary>
    ///  Binds the reorder list on left to the chapters. Also binds the chapters to the list in link selection div. 
    /// </summary>
    protected void BindSequence()
    {
        IList<ChapterVersion> dsChapters = DocoManager.GetAllItemsByArticleId(ArticleId);

        //bind chapters to the list
        ReorderList1.DataSource = dsChapters;
        ReorderList1.DataBind();

        ReorderList2.DataSource = dsChapters;
        ReorderList2.DataBind();

        //bind chapters to the select list
        rptChapsList.DataSource = dsChapters;
        rptChapsList.DataBind();
    }


    /// <summary>
    /// Loads a chapter
    /// </summary>
    /// <param name="isDefault">True if called on !Postback which will load the default chapter for the document.</param>
    protected void LoadChapter(bool isDefault)
    {
        IList<ChapterVersion> chapters = DocoManager.GetAllItemsByArticleId(ArticleId);
        int lowestOrderNumber = DocoManager.GetLowestSequenceNumber(ArticleId);

        //Load the chapter from the docoID
        if (chapters.Count > 0)
        {
            divEditor.Style.Add("display", "block");
            List<ChapterVersion> chapVersions = new List<ChapterVersion>();
            if (isDefault)
            {
                var chaps = from c in chapters
                            where c.Sequence == lowestOrderNumber
                            select c;
                chapVersions = chaps.ToList<ChapterVersion>();
            }
            else
            {
                var chaps = from c in chapters
                            where c.Id == ChapterId
                            select c;
                chapVersions = chaps.ToList<ChapterVersion>();
            }

            ChapterVersion chVersion = chapVersions.First();
            lblChapterName.Text = chVersion.Name;
            txtBoxChapterName.Text = chVersion.Name;
            lblChapterNameEditor.InnerHtml = chVersion.Name;
            ChapterId = chVersion.Id;
            ChapterName = chVersion.Name;

            int chapNumb = (chVersion.Sequence + 1);

            if (!string.IsNullOrEmpty(contentMode) && contentMode.Equals("pub"))
            {
                //set the content in the editor or literal depending on the mode.                
                if (chapVersions.Count<ChapterVersion>() > 0)
                    DisplayContent(chVersion.Content, chapNumb, IsNumbChaps);
                else
                    DisplayContent("", chapNumb, IsNumbChaps);

                //Bind drafts if any found for this chapter.
                //divDraftsList.InnerHtml = BindDrafts(ChapterId);
            }
            else
            {
                IList<Draft> drafts = DocoManager.GetDraftsByChapterId(ChapterId);
                if (drafts.Count > 0)
                {

                    Draft draft = drafts.FirstOrDefault<Draft>();
                    if (draft != null)
                        DisplayContent(draft.Content, chapNumb, IsNumbChaps);
                    else
                        DisplayContent("", chapNumb, IsNumbChaps);
                }
                else //load published version by default
                {
                    if (chapVersions.Count<ChapterVersion>() > 0)
                        DisplayContent(chVersion.Content, chapNumb, IsNumbChaps);
                    else
                        DisplayContent("", chapNumb, IsNumbChaps);
                }
            }
        }
        else
        {
            previewDiv.Style.Add("display", "none");
            divEditor.Style.Add("display", "none");
        }
    }

    private void DisplayContent(string content, int chapNumb, bool isNumbChaps)
    {
        txtEditor.Content = content;

        if (IsNumbChaps)
        {
            XHTMLText numberredChapsHTML = new XHTMLText();
            litViewContent.InnerHtml = numberredChapsHTML.AddChapNumbers(content, chapNumb);

            lblChapNumb.Text = chapNumb.ToString() + ".&nbsp;";
        }
        else
            litViewContent.InnerHtml = content;
    }

    /// <summary>
    /// Saves content for the ediotr. Creates a new version entry in the DB
    /// </summary>     
    protected void SaveContent(string ChapID, string sender)
    {
        try
        {
            ChapterVersion chapterVersion = new ChapterVersion();

            IList<ChapterVersion> allChapters = DocoManager.GetAllItemsByArticleId(ArticleId);
            chapterVersion = allChapters.First(x => x.Id.Equals(ChapID));

            string temp = chapterVersion.Content;
            //  chapterVersion.Content = UltimateEditor1.EditorHtml;

            BusiBlocks.XHTMLText htmlText = new BusiBlocks.XHTMLText();
            //chapterVersion.Content = htmlText.FormatXHTML(txtEditor.Content);
            chapterVersion.Content = txtEditor.Content;

            //check if the user has changed the Chapter Name.
            if (txtBoxChapterName.Visible && !string.IsNullOrEmpty(txtBoxChapterName.Text) && txtBoxChapterName.Text != chapterVersion.Name)
                chapterVersion.Name = txtBoxChapterName.Text;

            //if this is the first chapter and no content has been uploaded then
            if (string.IsNullOrEmpty(temp)) // if the content is empty or is being added for the first time it will update the only existing single record.
                BusiBlocks.DocoBlock.DocoManager.UpdateChapterVersion(chapterVersion);
            else
            //Check if the chapter name has changed at all - if yes then pass in VersionUpdateType.Chapter
            //Update chapter ID if using the button to save content, if not then do no update since the wmLoadChapter does it.
            {
                if (sender == "button")
                    ChapterId = BusiBlocks.DocoBlock.DocoManager.CreateChapterVersion(ArticleId, chapterVersion, VersionUpdateType.Content).Id;
                else
                    BusiBlocks.DocoBlock.DocoManager.CreateChapterVersion(ArticleId, chapterVersion, VersionUpdateType.Content);
            }
            //perform draft operations.
            //DeleteDraft(chapterVersion.Id);
        }
        catch (Exception ex)
        {
            //Log - unable to save current content for the selected chapter.
            throw ex;
        }

        lblLastDraft.Text = "<i>Chapter content saved on: " + DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToShortTimeString() + "</i>";
    }

    protected void SaveContent(string ChapID, int sequence)
    {
        try
        {
            ChapterVersion chapterVersion = new ChapterVersion();
            BusiBlocks.XHTMLText xhtml = new BusiBlocks.XHTMLText();

            IList<ChapterVersion> allChapters = DocoManager.GetAllItemsByArticleId(ArticleId);

            chapterVersion = allChapters.First(x => x.Id.Equals(ChapID));

            Draft draft = DocoManager.GetDraftByChapterId(chapterVersion.Id);

            string temp = chapterVersion.Content;

            if (draft != null)
                chapterVersion.Content = draft.Content;

            //add anchor tags for subchapters.

            chapterVersion.Sequence = sequence;
            chapterVersion.Content = xhtml.AddAnchorTags(chapterVersion.Content);

            //if this is the first time chapter content is added and no content has been uploaded then
            if (string.IsNullOrEmpty(temp)) // if the content is empty or is being added for the first time it will update the only existing single record.
                BusiBlocks.DocoBlock.DocoManager.UpdateChapterVersion(chapterVersion);
            else
            //Update chapter ID if using the button to save content, if not then do no update since the wmLoadChapter does it.
            {
                BusiBlocks.DocoBlock.DocoManager.CreateChapterVersion(ArticleId, chapterVersion, VersionUpdateType.Content);
            }

            Article a = DocoManager.GetArticle(ArticleId);
            a.UpdateDate = DateTime.Now;
            DocoManager.UpdateArticle(a, false);

        }
        catch (Exception ex)
        {
            //Log - unable to save current content for the selected chapter.
            throw ex;
        }
    }

    /// <summary>
    /// Creates a new chapter with the name provided.
    /// <paramref name="none"/>
    /// </summary>
    protected void SaveChapterName()
    {
        try
        {
            //Check for chapter name.
            if (Page.IsValid)
            {
                LoadChapter(false);
            }
        }
        catch (Exception ex)
        {
            //Log - unable to save chapter name.
            throw ex;
        }
    }

    protected void ValidateChapterName(object sender, ServerValidateEventArgs args)
    {
        if (!BusiBlocks.DocoBlock.DocoManager.CheckChapterName(ArticleId, tbxChapName.Text))
        {
            args.IsValid = true;
        }
        else
        {
            args.IsValid = false;
        }
    }
    #endregion

    #region Draft

    /// <summary>
    /// Binds the list of drafts available for the selected chapter.
    /// </summary>
    /// Not in use for the moment
    //protected string BindDrafts(string chapterVersionID)
    //{
    //    try
    //    {          
    //        string tableHtml = string.Empty;

    //        IList<Draft> draftsList = DocoManager.GetDraftsByChapterId(chapterVersionID);

    //        if (draftsList.Count > 0)
    //        {
    //            tableHtml = "<table>";
    //            foreach (Draft d in draftsList)
    //            {
    //                // add a new row for every draft;
    //                string row = "<tr>";
    //                row += "<td class=\"draftsListImgSelect\"><a id=" + d.Id + " name=" + d.VersionId + " onclick=InsertDraft(this.id,this.Name);><img id=testImg src=../App_Themes/BusiBlocks/Images/yes.png alt=Select title=\"Select draft\" /></a></td>";
    //                row += "<td><i> Saved on - </i>" + d.SaveDate + "</td>";
    //                row += "</tr>";
    //                tableHtml += row;
    //            }
    //            tableHtml += "</table>";
    //        }
    //        else
    //        {
    //            tableHtml = "<table>";
    //            string row = "<tr>";
    //            row += "<td>There are no drafts available</td>";
    //            row += "</tr>";
    //            tableHtml += row;
    //            tableHtml += "</table>";
    //        }
    //        return tableHtml;
    //    }
    //    catch (Exception ex)
    //    {
    //        //Log - unable to bind drafts list.
    //        throw ex;
    //    }
    //}

    /// <summary>
    /// Saves the current content in the editor as a draft for the selected chapter.One overload.
    /// </summary>
    /// <returns>Draft object</returns>    
    protected Draft SaveDraft()
    {
        try
        {
            Draft draft = new Draft();
            draft.VersionId = ChapterId;
            draft.Content = txtEditor.Content;
            draft.SaveDate = DateTime.Now;
            draft = BusiBlocks.DocoBlock.DocoManager.UpsertDraft(draft);
            return draft;
        }
        catch (Exception ex)
        {
            //Log - unable to save current draft.
            throw ex;
        }
    }

    protected Draft SaveDraft(string ChaptId, string Content)
    {
        try
        {
            //if (IsNameChanged)
            // {
            ChapterVersion cv = DocoManager.GetAllChapterVersion().Where(x => x.Id.Equals(ChaptId)).First<ChapterVersion>();

            if (txtBoxChapterName.Text != cv.Name)
            {
                cv.Name = txtBoxChapterName.Text;
                DocoManager.UpdateChapterVersion(cv);
            }
            //IsNameChanged = false;
            //}
            Draft draft = new Draft();
            draft.VersionId = ChaptId;
            draft.Content = Content;
            draft.SaveDate = DateTime.Now;
            draft = BusiBlocks.DocoBlock.DocoManager.UpsertDraft(draft);
            //divDraftsList.InnerHtml = BindDrafts(ChaptId);

            //Reconstruct Menu;
            CreateSubChapterMenu(Content);
            return draft;
        }
        catch (Exception ex)
        {
            //Log - unable to save current draft.
            throw ex;
        }
    }

    protected static void SaveDraft(string ChaptId, string Content, string chapName)
    {
        try
        {
            // if (IsNameChanged)
            // {
            ChapterVersion cv = DocoManager.GetAllChapterVersion().Where(x => x.Id.Equals(ChaptId)).First<ChapterVersion>();

            if (chapName != cv.Name)
            {
                cv.Name = chapName;
                DocoManager.UpdateChapterVersion(cv);
            }
            //   IsNameChanged = false;
            //  }
            Draft draft = new Draft();
            draft.VersionId = ChaptId;
            draft.Content = Content;
            draft.SaveDate = DateTime.Now;
            draft = BusiBlocks.DocoBlock.DocoManager.UpsertDraft(draft);
            //divDraftsList.InnerHtml = BindDrafts(ChaptId);           
        }
        catch (Exception ex)
        {
            //Log - unable to save current draft.
            throw ex;
        }
    }

    protected bool CreateSubChapterMenu(string Content)
    {
        return true;
    }

    #endregion

    #endregion

    #region WebMethods

    [WebMethod]
    public static string wmAddChapter(string chapterName)
    {
        try
        {
            Chapter newChapter = BusiBlocks.DocoBlock.DocoManager.CreateChapter(ViewArticle2.ArticleId, chapterName);
            if (!string.IsNullOrEmpty(newChapter.Id))
            {
                ChapterVersion chapterVersion = new ChapterVersion();
                chapterVersion.ChapterId = newChapter.Id;
                chapterVersion.Name = chapterName;
                chapterVersion.Content = "";
                chapterVersion.Version = "1.0"; // every version starts from 1.0 unless specified - custom version feature to be available later on.
                chapterVersion = BusiBlocks.DocoBlock.DocoManager.CreateChapterVersion(ArticleId, chapterVersion, VersionUpdateType.New);

                //ChapterId = chapterVersion.Id;
                ChapterName = chapterVersion.Name;

                return chapterVersion.Id;
            }
            else
                return "";
        }
        catch (Exception ex)
        {
            //Log - unable to save chapter name.
            throw ex;
        }
    }

    [WebMethod]
    public static string wmDeleteChapter(string chapterId)
    {
        BusiBlocks.DocoBlock.DocoManager.DeleteChapter(chapterId); //returns the previous chapter or default if none left.

        ViewArticle2 vw = new ViewArticle2();
        //       ReorderList list = (ReorderList)vw.FindControl("ReorderList1");
        return ChapterId;
    }

    [WebMethod]
    public static bool wmCheckChapterName(string ChapName)
    {
        if (DocoManager.CheckChapterName(ArticleId, ChapName))
        {
            return true;
        }
        return false;
    }

    [WebMethod]
    public static void wmClearChapterId()
    {
        ChapterId = string.Empty;
    }

    [WebMethod]
    public static void wmIsEditorChanged(bool EditorChanged)
    {
        IsEditorChanged = EditorChanged;
    }
    [WebMethod]
    public static void wmIsNameChanged(bool NameChanged)
    {
        //checking if chapter name changed or not.
        IsNameChanged = NameChanged;
    }

    /// <summary>
    /// Loads a chapter selected from the TOC on left hand side.
    /// </summary> 
    [WebMethod]
    public static object wmLoadChapter(string Id, bool IsEdtorChanged, string ChapName)
    {
        try
        {
            IsEditorChanged = IsEdtorChanged;

            tempChapterId = ChapterId; //hold the ID of previous chapter when switching.            
            ChapterId = Id;

            XHTMLText numberedChaps = new XHTMLText();

            ChapterVersion chapter = DocoManager.GetChapterVersion(ChapterId);

            //THIS HAS BEEN REPLACED BY THE AUTO SAVE SERVICE!.
            //if (IsEditorChanged)
            //SaveDraft(tempChapterId, EditorContent, ChapName);

            if (!string.IsNullOrEmpty(contentMode) && contentMode.Equals("draft"))
            {
                IList<Draft> drafts = DocoManager.GetDraftsByChapterId(chapter.Id);

                if (drafts.Count > 0)
                {
                    Draft draft = drafts.FirstOrDefault<Draft>();
                    draft.Name = chapter.Name;
                    draft.Sequence = chapter.Sequence + 1;
                    draft.Content = draft.Content; //numberedChaps.AddChapNumbers(draft.Content, draft.Sequence);
                    return draft;
                }
            }
            chapter.Sequence = chapter.Sequence + 1;
            chapter.Content = chapter.Content;// numberedChaps.AddChapNumbers(chapter.Content, (chapter.Sequence + 1));
            return chapter;
        }
        catch (Exception ex)
        {
            //Log - unable to load the requested chapter.
            throw ex;
        }
    }

    /// <summary>
    /// Returns the draft  saved for the version Id.
    /// </summary>
    /// <param name="Id">Version Id</param>
    /// <returns>Draft object</returns>
    [WebMethod]
    public static object wmLoadDraft(string Id)
    {
        try
        {
            Draft draft = DocoManager.GetDraftByChapterId(Id);

            //bind all the drafts saved for the chapter version.
            //wmBindDraftsList(draft.VersionId);

            ViewArticle2.DraftId = Id;

            return draft;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// Binds the list of drafts available for that chapter version
    /// </summary>
    /// <param name="Chapter Id"></param>
    /// <returns></returns>
    [WebMethod]
    public static object wmBindDraftsList(string chapterVersionId)
    {
        try
        {
            IList<Draft> draftsList = DocoManager.GetDraftsByChapterId(chapterVersionId);
            return draftsList;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    [WebMethod] //used to switch between edit and preview mode
    public static string wmSwitchModes()
    {
        return ViewArticle2.ChapterId;
    }

    [WebMethod]
    public static bool wmAdminMode()
    {
        return ViewArticle2.IsAdminMode;
    }

    [WebMethod]
    public static bool wmSetAdminMode()
    {
        ViewArticle2.IsAdminMode = false;
        return ViewArticle2.IsAdminMode;
    }

    [WebMethod]
    public static bool wmAutoSaveDraft(string ChaptId, string Content)
    {
        try
        {
            Draft draft = new Draft();
            draft.VersionId = ChaptId;
            draft.Content = Content;
            draft.SaveDate = DateTime.Now;
            draft = BusiBlocks.DocoBlock.DocoManager.UpsertDraft(draft);
            //divDraftsList.InnerHtml = BindDrafts(ChaptId);          
        }
        catch (Exception ex)
        {
            //Log - unable to save current draft.
            throw ex;
        }
        return true;
    }

    /// <summary>
    /// Returns a list which has IDS for next and previous chapter links for the currently selected chapter.    /// 
    /// </summary>
    /// <param name="ChaptId">Accepts current chapter's ID</param>
    /// <returns>List object.</returns>
    [WebMethod]
    public static object wmGetNextChapter(string ChaptId)
    {
        List<string> chapPaginationIds = new List<string>();

        chapPaginationIds.Add(DocoManager.GetPreviousChapter(ChaptId, ArticleId).Id);
        chapPaginationIds.Add(DocoManager.GetNextChapter(ChaptId, ArticleId).Id);

        return chapPaginationIds;
    }

    [WebMethod]
    public static string wmGetChapId()
    {
        return ViewArticle2.ChapterId;
    }

    [WebMethod]
    public static bool wmGetIsNumbChaps()
    {
        return ViewArticle2.IsNumbChaps;
    }

    #endregion

    #region Editor

    protected void CreateCustomToolBarBtns()
    {
        txtEditor.EnsureToolsFileLoaded();

        //Custom toolbar item - insert custom links (to other documents);
        Telerik.Web.UI.EditorTool custom1 = new Telerik.Web.UI.EditorTool();
        custom1.Name = "customInsertLink";
        custom1.ShortCut = "CTRL+l";
        custom1.Attributes.Add("title", "Insert Link");
        txtEditor.Tools[1].Tools.Add(custom1);

        //Custom toolbar item - Load draft if its publish mode and needs editing;
        Telerik.Web.UI.EditorTool custom2 = new Telerik.Web.UI.EditorTool();
        custom2.Name = "customDraftLink";
        custom2.ShortCut = "CTRL+d";
        custom2.Attributes.Add("title", "Load Draft");
        //  txtEditor.Tools[1].Tools.Add(custom2);

        RemoveToolBarItems();
    }

    protected void RemoveToolBarItems()
    {
        List<string> toolBarItems = new List<string>();

        toolBarItems.Add("InsertFormElement");
        toolBarItems.Add("AboutDialog");
        toolBarItems.Add("Zoom");
        toolBarItems.Add("ToggleTableBorder");
        toolBarItems.Add("ModuleManager");
        toolBarItems.Add("SpellCheck");
        toolBarItems.Add("FlashManager");
        toolBarItems.Add("TemplateManager");
        toolBarItems.Add("DocumentManager");
        toolBarItems.Add("MediaManager");
        toolBarItems.Add("InsertCustomLink");
        toolBarItems.Add("LinkManager");
        toolBarItems.Add("ToggleDocking");
        toolBarItems.Add("ImageMapDialog");
        toolBarItems.Add("InsertSnippet");
        toolBarItems.Add("FormatCodeBlock");
        toolBarItems.Add("XHTMLValidator");
        toolBarItems.Add("ApplyClass");

        foreach (string toolBarItem in toolBarItems)
        {
            foreach (Telerik.Web.UI.EditorToolGroup collection in txtEditor.Tools)
            {
                Telerik.Web.UI.EditorTool item = collection.FindTool(toolBarItem);
                if (item != null)
                    item.Visible = false;
            }
        }
    }

    protected void AddParaHeadingStyles()
    {
        txtEditor.Paragraphs.Add(new EditorParagraph("<p>", "<p>Normal</p>"));
        txtEditor.Paragraphs.Add(new EditorParagraph("<h2>", "<h2>Heading 2</h2>"));
        txtEditor.Paragraphs.Add(new EditorParagraph("<h3>", "<h3>Heading 3</h3>"));
        txtEditor.Paragraphs.Add(new EditorParagraph("<h4>", "<h4>Heading 4</h4>"));
        txtEditor.Paragraphs.Add(new EditorParagraph("<h5>", "<h5>Heading 5</h5>"));
        txtEditor.Paragraphs.Add(new EditorParagraph("<h6>", "<h6>Heading 6</h6>"));
        txtEditor.Paragraphs.Add(new EditorParagraph("<pre>", "<pre>Formatted</pre>"));
        txtEditor.Paragraphs.Add(new EditorParagraph("<address>", "<address>Address</address>"));
        txtEditor.Paragraphs.Add(new EditorParagraph("<important>", "<important>Important</important>"));
        txtEditor.Paragraphs.Add(new EditorParagraph("<caption>", "<caption>Caption</caption>"));
        txtEditor.Paragraphs.Add(new EditorParagraph("<quote>", "<quote>Quote</quote>"));
        txtEditor.Paragraphs.Add(new EditorParagraph("<reference>", "<reference>Reference</reference>"));
    }

    #endregion
}