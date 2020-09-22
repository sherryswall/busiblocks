<%@ WebHandler Language="C#" Class="Attach" %>

using System;
using System.Web;

public class Attach : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        string articleName = context.Request["article"];
        string attachName = context.Request["attach"];
        string mode = context.Request["mode"];

        BusiBlocks.DocoBlock.Article article = BusiBlocks.DocoBlock.DocoManager.GetArticleByName(articleName, true);

        //if (BusiBlocks.SecurityHelper.CanRead(context.User, article.Category, null) == false)
        //    throw new BusiBlocks.InvalidPermissionException("Reading file for category " + article.Category.Name);

        BusiBlocks.DocoBlock.FileAttachment attachment = BusiBlocks.DocoBlock.DocoManager.GetFileAttachmentByName(article, attachName, true);

        if (mode == "download")
        {
            context.Response.ContentType = "application/x-download";
            context.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + attachment.Name + "\"");
        }
        else
        {
            context.Response.ContentType = attachment.ContentType;
            context.Response.AddHeader("Content-Disposition", "filename=\"" + attachment.Name + "\"");
        }

        context.Response.OutputStream.Write(attachment.ContentData, 0, attachment.ContentData.Length);
    }

    public bool IsReusable
    {
        get
        {
            return true;
        }
    }

}