using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusiBlocks.DocoBlock;
using System.IO;
using Resources;
using System.Globalization;
using WebFiler;
using BusiBlocks.Audit;
using BusiBlocks;

public partial class Doco_ViewUploadedDoc : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string documentId;
        documentId = Request["id"]; //check for id in URL

        if (!string.IsNullOrEmpty(documentId))
        {
            Article article = DocoManager.GetArticle(documentId);

            if ((article.RequiresAck && article.Acknowledged) || !article.RequiresAck)
                ackText.Visible = false;
            else
                ackText.Visible = true;

            lnkFileName.BorderStyle = BorderStyle.None;
            lnkFileName.Text = article.FileName;
            imgFileName.ImageUrl = Utility.GetImageUrlType(article.FileName.Substring(article.FileName.IndexOf('.') + 1), article.IsUpload);

            lblTitle.Text = article.Name;
            lblDescription.Text = article.Description;
            lblCategory.Text = article.Category.DisplayName;
            lblOwner.Text = Utilities.GetDisplayUserName(article.Owner);
            lblDocumentType.Text = "Uploaded";
            lblPublished.Text = Utilities.GetDateTimeForDisplay(article.UpdateDate);
            lblVersion.Text = "1.0";// article.Version.ToString();
            //lblComments.Text = article.
            SetTrafficLight(article);
            if (!article.RequiresAck)
                ackLabel.InnerText = "Viewed:";
        }
    }

    protected void btnAcknowledge_Click(object sender, EventArgs e)
    {
        string documentId;
        documentId = Request["id"]; //check for id in URL

        if (!string.IsNullOrEmpty(documentId))
        {
            Article article = DocoManager.GetArticle(documentId);
            AuditManager.Audit(Page.User.Identity.Name, article.Id, AuditRecord.AuditAction.Acknowledged);
            article = DocoManager.GetArticle(documentId);
            SetTrafficLight(article);
        }
    }

    protected void lnkFileName_Click(object sender, EventArgs e)
    {
        string documentId = Request["id"]; //check for id in URL

        if (!string.IsNullOrEmpty(documentId))
        {
            Article article = DocoManager.GetArticle(documentId);
            AuditManager.Audit(Page.User.Identity.Name, article.Id, AuditRecord.AuditAction.Viewed);
            article = DocoManager.GetArticle(documentId);
            SetTrafficLight(article);

            string navigateUrl =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resource.NewObjectPath,
                        "Files",
                        article.Category.Id);

            if (!string.IsNullOrEmpty(article.FileName))
            {
                navigateUrl = string.Format(
                    CultureInfo.InvariantCulture,
                    Resource.DocoFilesLoc + Resource.FileOpen,
                    UrlEncoding.Encode(Path.Combine(navigateUrl, article.FileName)));
                navigateUrl = navigateUrl.Insert(navigateUrl.IndexOf("&"), "&aid=" + article.Id);

                // go to that url
                Page.Response.Redirect(navigateUrl);
            }
        }
    }

    protected void btnReturn_Click(object sender, EventArgs e)
    {
        Navigation.Doco_Default().Redirect(this);
    }

    private void SetTrafficLight(Article article)
    {
        if (SecurityHelper.CanUserView(Page.User.Identity.Name, article.Category.Id))
        {
            imgAck.ImageUrl = Utility.GetTrafficLight(article);

            //if (!article.RequiresAck || article.Acknowledged)
            //    btnAcknowledge.Enabled = false;

            if (article.RequiresAck)
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
                
                ackText.Visible = false;
            }
        }
        else
        {
            divAckButton.Visible = false;
            trStatus.Visible = false;
        }
    }
}