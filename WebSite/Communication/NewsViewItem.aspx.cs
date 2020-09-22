using System;
using System.Linq;
using System.Collections.Generic;
using BusiBlocks.CommsBlock.News;
using BusiBlocks;
using System.Web.UI;
using BusiBlocks.Audit;
using BusiBlocks.DocoBlock;
using System.Web.Services;
using BusiBlocks.Versioning;
using BusiBlocks.ApproverLayer;
using BusiBlocks.ItemApprovalStatusLayer;
using BusiBlocks.Membership;

public partial class Communication_NewsViewItem : System.Web.UI.Page
{
    private const string SessionKeyItem = "item";
    private const string ErrorCategoryPermission = "Permission to category denied!";

    private bool IsRestore { get; set; }

    private string ItemId
    {
        get
        {
            return Request[SessionKeyItem];
        }
    }

    private string VersionId
    {
        get
        {
            //ItemId = versionId --when coming from VersionHistory page;
            //find version by Id first if null then by item id . if not found then return null.
            VersionItem vi = VersionManager.GetVersionById(ItemId);
            if (vi == null)
                vi = VersionManager.GetVersionByItemId(ItemId);

            if (vi == null)
                return string.Empty;
            else
                return vi.Id;
        }
    }

    private Item _item;

    private Item Item
    {
        get
        {
            if (_item == null)
            {
                //accessing item via versionID!!!
                if (!string.IsNullOrEmpty(VersionId))
                {
                    VersionItem vi = VersionManager.GetVersionById(VersionId);
                    _item = NewsManager.GetItem(vi.ItemId);
                }
                //accessing item via newsItem!!!
                else { _item = NewsManager.GetItem(ItemId); }
            }
            return _item;
        }
        set
        {
            _item = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            BindPage();
    }

    private void BindPage()
    {
        BindItem();
        BindTrafficLight();
        BindButtons();
    }

    private void BindItem()
    {
        try
        {
            Item item = NewsManager.GetItem(Item.Id);

            lblPageHeading.Text = Item.Title;
            lblAuthor.Text = Item.Author;
            lblDate.Text = Utilities.GetDateTimeForDisplay(Item.NewsDate);
            txtDetails.Text = Item.Description;

            if (!Item.RequiresAck)
            {
                ackText.Visible = false;
                // If this page has the Approval menu, then don't set it as viewed.
                if (!approveFooter.Visible)
                    AuditManager.Audit(Page.User.Identity.Name, Item.Id, AuditRecord.AuditAction.Viewed);
            }

            VersionItem versionItem = VersionManager.GetVersionById(VersionId);

            if (versionItem != null)
            {
                int index = versionItem.VersionNumber.IndexOf(".", versionItem.VersionNumber.IndexOf(".") + 1);
                string publishedVersion = versionItem.VersionNumber.Substring(0, index);
                lnkVersion.Text = "Version&nbsp;" + versionItem.VersionNumber;
                lblVersionNumber.Text = publishedVersion;
                lnkVersion.NavigateUrl = Navigation.Communication_NewsVersionHistory(versionItem.GroupId).GetServerUrl(true);
            }
        }
        catch (Exception exception)
        {
            throw exception;
            ((IFeedback)Master).SetException(GetType(), exception);
        }
    }

    private void BindButtons()
    {
        if (Request.QueryString.Get("mode") == "approve")
        {
            if (Item.ApprovalStatus.Id == ItemApprovalStatusManager.GetForEditApprovalStatus().Id)
            {
                //set approval for edit mode
                divApprovalBar.Visible = true;
                divActionButton.Visible = false;
            }
            else
            {
                bool userCanApprove = ApproverManager.IsApprover(Page.User.Identity.Name, (VersionId ?? Item.Id));

                divApprovalBar.Visible = userCanApprove;
                divActionButton.Visible = !userCanApprove;
            }
        }        
        else
        {
            divApprovalBar.Visible = false;
            //hide edit buttons when viewing announcement
            if (!string.IsNullOrEmpty(Request.QueryString["mode"]) && Request.QueryString["mode"].Equals("view"))
            {
                h2LblAction.Visible = false;
                approveFooter.Visible = false;
            }
            else
                SetButtons();
        }
    }

    private void SetButtons()
    {
        
        if (SecurityHelper.CheckWriteAccess(Page.User.Identity.Name, Item.Category.Id))
        {
            //if any of the versions is open for editing then don't allow editing or restoring.
            if (VersionManager.IsLatestVersion(VersionId))
            {
                btnRestore.Visible = false;
                btnEdit.Visible = true;
                lblAction.Text = "Edit";
            }
            else
            {
                btnRestore.Visible = true;
                btnEdit.Visible = false;
                lblAction.Text = "Restore";
            }
        }
        else
        {
            btnRestore.Visible = false;
            btnEdit.Visible = false;
        }
    }

    protected void btnAcknowledge_Click(object sender, EventArgs e)
    {
        AuditManager.Audit(Utilities.GetUserName(Page.User.Identity.Name), Item.Id, AuditRecord.AuditAction.Acknowledged);
        BindTrafficLight();
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Navigation.Communication_News().Redirect(this);
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        Navigation.Communication_NewsEditItem(VersionId).Redirect(this);
    }

    protected void btnRestore_Click(object sender, EventArgs e)
    {
        //checkout the version and lock down for editing
        VersionManager.CheckOutVersion(VersionId, Page.User.Identity.Name);
        Navigation.Communication_NewsEditItem(VersionId).Redirect(this);
    }

    protected void btnApprove_Click(object sender, EventArgs e)
    {
        Navigation.Communication_NewsEditItemApproval(Request.QueryString.Get("item")).Redirect(this);
    }

    private void BindTrafficLight()
    {
        //check for reader access. If not in reader's access than do not apply traffic lights and no auditing required.
        if (SecurityHelper.CanUserView(Page.User.Identity.Name, Item.Category.Id))
        {
            if (!string.IsNullOrEmpty(Request.QueryString["mode"]) && Request.QueryString["mode"].Equals("approve"))
            {
                lblAck.CssClass = "ack";
                divAckButton.Attributes["class"] = "divAckChecked";
                btnAcknowledge.Enabled = false;
            }
            else if (Item.RequiresAck)
            {
                if (Item.Acknowledged)
                {
                    lblAck.CssClass = "ack";
                    divAckButton.Attributes["class"] = "divAckChecked";
                    btnAcknowledge.Enabled = false;
                }
                else
                {
                    lblAck.CssClass = "notAck";
                    divAckButton.Attributes["class"] = "divAckButton";
                }
                IList<AuditRecord> records = AuditManager.GetAuditItems(Page.User.Identity.Name, Item.Id, AuditRecord.AuditAction.Acknowledged);
                if (records.Count > 0)
                    lblAck.Text = records[0].TimeStamp.ToString();
            }
            else
            {
                IList<AuditRecord> records = AuditManager.GetAuditItems(Page.User.Identity.Name, Item.Id, AuditRecord.AuditAction.Viewed);
                if (records.Count == 0)
                {
                    AuditManager.Audit(Page.User.Identity.Name, Item.Id, AuditRecord.AuditAction.Viewed);
                }
                ackText.Visible = false;
                lblAck.CssClass = "view";
            }
        }
        else
        {
            HideTrafficLight();
        }
    }

    private void HideTrafficLight()
    {
        divAckButton.Visible = false;
        spanAck.Visible = false;
    }

    protected void btnReject_Click(object sender, EventArgs e)
    {
        Item.ApprovalStatus = ItemApprovalStatusManager.GetDraftStatus();
        Item.ActionedByPersonId = MembershipManager.GetUserByName(Page.User.Identity.Name).Id;
        Item.ActionedNotes = popReject.Value;
        Item.ActionedOnDate = DateTime.Now;

        NewsManager.UpdateItem(Item);
        Navigation.Communication_News().Redirect(this);
    }

    protected void btnReturn_Click(object sender, EventArgs e)
    {
        Navigation.Communication_News().Redirect(this);
    }
}
