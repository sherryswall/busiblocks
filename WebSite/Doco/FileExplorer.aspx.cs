﻿using System;
using System.Web;
using System.Web.UI.WebControls;
using Resources;
using System.Globalization;
using System.IO;
using System.Web.Configuration;
using BusiBlocks.AccessLayer;
using BusiBlocks;
using BusiBlocks.DocoBlock;
using BusiBlocks.Audit;

public partial class Doco_FileExplorer : BasePage
{
    // todo im The file explorer is no longer used as a standalone page. However
    // we are using the open file feature of this page. This should be refactored
    // into a new class that provides file opening functionality.

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    #region Private Members
    /// <summary>
    /// The FileInfo object.
    /// </summary>
    FileInfo fi;
    #endregion

    #region Properties
    /// <summary>
    /// Gets the root path.
    /// </summary>
    /// <value>The _root.</value>
    string _root
    {
        get
        {
            // Is the root in the session?
            string root = WebFiler.UrlEncoding.Decode(Request.QueryString[Resource.QSR]);

            // If it's not there use the default.
            if (string.IsNullOrEmpty(root))
            {
                root = Server.MapPath(WebConfigurationManager.AppSettings.Get(Resource.Root));
            }

            return root;
        }
    }


    /// <summary>
    /// Gets the source or return path.<br/>
    /// Note: it is up to the user to ensure that the config value contains a valid url.
    /// </summary>
    /// <value>The _return.</value>
    string _return
    {
        get { return WebConfigurationManager.AppSettings.Get(Resource.Return); }
    }

    /// <summary>
    /// Get the mode.<br/>
    /// 0: View.<br/>
    /// 1: Rename.<br/>
    /// 2: New File.<br/>
    /// 3: New Folder.<br/>
    /// 4: Upload File.
    /// </summary>
    /// <value>The mode.</value>
    int _mode
    {
        get { return Convert.ToInt32(Request.QueryString[Resource.QSM]); }
    }

    /// <summary>
    /// Gets the full path to the file or folder.
    /// </summary>
    /// <value>The _file.</value>
    string _file
    {
        get { return WebFiler.UrlEncoding.Decode(Request.QueryString[Resource.QSF]); }
    }
    #endregion

    #region Overrides
    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
    /// </summary>
    /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        //Root = Server.MapPath("Test");
        Setup();
    }
    #endregion
    
    #region Setup
    /// <summary>
    /// Setups this instance.
    /// </summary>
    void Setup()
    {
        // Should the Return button be available?
        // If there is a config entry for WFReturn and it has a value then display the Return button.
        //if (!string.IsNullOrEmpty(_return))
        //{
        //    btnReturn.Visible = true;
        //}

        // Display the correct view.
        mvFiler.ActiveViewIndex = _mode;

        // Instantiate according to need.
        switch (_mode)
        {
            case 0:	// View
                SetupView();
                break;
            case 1:	// Rename
                // Get the FileInfo object.
                fi = new FileInfo(_file);

                if (!IsPostBack)
                {
                    lblRename.Text =
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resource.RenameLabel,
                            fi.Name);

                    txtRename.Text = fi.Name;
                }

                txtRename.Focus();
                break;
            case 2:	//New File.
                txtNewFile.Focus();
                break;
            case 3:	// New Folder.
                txtNewFolder.Focus();
                break;
            case 4:	// Upload.
                fuUpload.Focus();
                break;
        }

        lblRoot.Text = _root;
    }

    #endregion

    #region General Events
    /// <summary>
    /// Handles the Click event of the btnCancel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Cancel(object sender, EventArgs e)
    {
        Refresh();
    }
    #endregion

    #region View Events
    /// <summary>
    /// Handles the Click event of the btnHome control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void btnHome_Click(object sender, EventArgs e)
    {
        // Return here with no query string items.
        Response.Redirect(Resource.RRHome, true);
    }

    /// <summary>
    /// Handles the Click event of the btnNewFile control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void btnNewFile_Click(object sender, EventArgs e)
    {
        string newFile =
            string.Format(
                CultureInfo.InvariantCulture,
                Resource.RRNewFile,
                WebFiler.UrlEncoding.Encode(_root));
        Response.Redirect(newFile, true);
    }

    /// <summary>
    /// Handles the Click event of the btnNewFolder control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void btnNewFolder_Click(object sender, EventArgs e)
    {
        string newFolder =
            string.Format(
                CultureInfo.InvariantCulture,
                Resource.RRNewFolder,
                WebFiler.UrlEncoding.Encode(_root));
        Response.Redirect(newFolder, true);
    }

    /// <summary>
    /// Handles the Click event of the btnUpload control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        string upload =
            string.Format(
                CultureInfo.InvariantCulture,
                Resource.RRUpload,
                WebFiler.UrlEncoding.Encode(_root));
        Response.Redirect(upload, true);
    }

    /// <summary>
    /// Handles the Click event of the btnReturn control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void btnReturn_Click(object sender, EventArgs e)
    {
        // If we reach here then we've already tested that we can do this in Setup.
        Response.Redirect(_return, true);
    }
    #endregion

    #region Rename Events
    /// <summary>
    /// Handles the Click event of the btnSave control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void btnRenameSave_Click(object sender, EventArgs e)
    {
        if (IsValid)
        {
            // Change the name to the new one.
            string newName = txtRename.Text;
            string newPath = fi.FullName.Replace(fi.Name, newName);

            // Is this a file or a folder?
            if (fi.Attributes == FileAttributes.Directory)
            {
                Directory.Move(_file, newPath);
            }
            else
            {
                File.Move(_file, newPath);
            }

            Refresh();
        }
    }
    #endregion

    #region New File Events
    /// <summary>
    /// Handles the Click event of the btnSave control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void btnNewFileSave_Click(object sender, EventArgs e)
    {
        if (IsValid)
        {
            string file = txtNewFile.Text;
            string path =
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resource.NewObjectPath,
                    _root,
                    file);

            StreamWriter writer = File.CreateText(path);
            writer.Close();

            Refresh();
        }
    }
    #endregion

    #region New Folder Events
    /// <summary>
    /// Handles the Click event of the btnSave control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void btnNewFolderSave_Click(object sender, EventArgs e)
    {
        if (IsValid)
        {
            string folder = txtNewFolder.Text;
            string path =
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resource.NewObjectPath,
                    _root,
                    folder);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Folder f = new Folder() { Path = path };

            AccessManager.AddFolderKey(f);

            foreach (Access v in AccessControl1.AccessList)
            {
                v.ItemId = f.Id;
                v.ItemType = ItemType.FileFolder;
                AccessManager.AddAccess(v);
            }

            Refresh();
        }
    }
    #endregion

    #region Upload Events
    /// <summary>
    /// Handles the Click event of the btnSave control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void btnUploadSave_Click(object sender, EventArgs e)
    {
        if (IsValid)
        {
            // Leave if no file selected.
            if (!fuUpload.HasFile)
            {
                fuUpload.Focus();
                return;
            }

            // Create the path for the new file.
            string path =
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resource.NewObjectPath,
                    _root,
                    fuUpload.FileName);

            // Save to the current folder.
            fuUpload.SaveAs(path);

            string articleId = WebFiler.UrlEncoding.Decode(Request.QueryString[Resource.QSAID]);
            Article article = DocoManager.GetArticle(articleId);
            article.IsUpload = true;
            article.FileName = fuUpload.FileName;
            article.RequiresAck = chkAcknowledge.Checked;
            if (chkEnabled.Checked)
                article.Enabled = chkEnabled.Checked;
            DocoManager.UpdateArticle(article, true);

            Navigation.Doco_ViewCategory(article.Category.Id).Redirect(this);
            //Refresh();
        }
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Set up the view.
    /// </summary>
    void SetupView()
    {
        // If the folder is bogus...
        if (!Directory.Exists(_root))
        {
            Label label = new Label();
            label.Text =
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resource.FolderNotFound,
                    _root);
            phDisplay.Controls.Add(label);
            return;
        }

        Folder folder = AccessManager.GetFolderKey(_root);

        ProfileCommon profile = HttpContext.Current.Profile as ProfileCommon;

        if (folder != null)
        {
            if (!SecurityHelper.CanUserView(HttpContext.Current.User.Identity.Name, folder.Id))
            {
                Label label = new Label();
                label.Text =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Access Denied",
                        _root);
                phDisplay.Controls.Add(label);
                btnNewFile.Visible = btnNewFolder.Visible = btnUpload.Visible = false;
                return;
            }
        }

        // Get a new table of files and folders.
        Table table = new TableEx(this, _root).Create();

        // Display the table.
        if (table != null)
        {
            phDisplay.Controls.Add(table);
        }
        else
        {
            // Table wasn't created.
            Label label = new Label();
            label.Text = Resource.CantCreateTable;
            phDisplay.Controls.Add(label);
        }

        // Finally, if a file link was clicked, open it.
        OpenFile();
    }

    /// <summary>
    /// Refreshes the page.
    /// </summary>
    void Refresh()
    {
        string refresh =
            string.Format(
                CultureInfo.InvariantCulture,
                Resource.RRRefresh,
                WebFiler.UrlEncoding.Encode(_root));
        Response.Redirect(refresh, true);
    }

    /// <summary>
    /// Display the given file.
    /// </summary>
    void OpenFile()
    {
        // Someone clicked on a file or folder...
        // Doesn't matter what's in x: if it has anything then open the file.
        if (!string.IsNullOrEmpty(Request.QueryString[Resource.QSX]))
        {
            // Get the object.
            string file = Request.QueryString[Resource.QSO];
            FileInfo fi = new FileInfo(Path.Combine(Server.MapPath("."), file));

            if (!fi.Exists)
            {
                // The file does not exist.
                throw new FileNotFoundException("File not found: " + fi.DirectoryName + "\\" + fi.Name);
            }

            // Set it as viewed
            string articleId = Request.QueryString["aid"];
            if (!string.IsNullOrEmpty(articleId))
                AuditManager.Audit(Page.User.Identity.Name, articleId, AuditRecord.AuditAction.Viewed);

            // Display the file.
            // 'application/octet-stream' should be okay for most cases.
            string attachment =
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resource.ContentDispositionAttachment,
                    fi.Name);
            Response.ContentType = Resource.ContentType;
            Response.AppendHeader(Resource.ContentDisposition, attachment);
            Response.TransmitFile(fi.FullName);
            Response.End();

            // Clear the querystring.
            Request.QueryString.Remove(Resource.QSX);
        }
    }
    #endregion
}