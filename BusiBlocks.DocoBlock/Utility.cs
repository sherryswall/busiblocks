// -----------------------------------------------------------------------
// <copyright file="Utility.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BusiBlocks.DocoBlock
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class Utility
    {

        public static string GetTrafficLight(ArticleBase article)
        {
            if (article.RequiresAck)
            {
                if (article.Acknowledged)
                    return "~/app_themes/default/icons/cube_green.png";
                else
                    return "~/app_themes/default/icons/cube_red.png";
            }
            if (article.Viewed)
                return "~/app_themes/default/icons/cube_green.png";
            return "~/app_themes/default/icons/cube_yellow.png";
        }

        public static string GetImageUrlType(string extension, bool isUpload)
        {
            const string prefix = "../app_themes/default/icons/";
            if (!isUpload)
                return prefix + "onlineDoc.gif";
            switch (extension)
            {
                //Text icons
                case "rtf":
                case "text":
                case "txt":
                case "csv":
                    return prefix + "textIcon.gif";
                //PDF icons
                case "pdf":
                    return prefix + "pdfIcon.gif";
                //Excel icons
                case "xls":
                case "xlsx":
                    return prefix + "excelIcon.gif";
                //Powerpoint icons
                case "ppt":
                case "pptx":
                    return prefix + "powerpointIcon.gif";
                //Html icons
                case "html":
                case "htm":
                case "xml":
                case "asp":
                case "aspx":
                case "js":
                case "xhtml":
                case "php":
                case "css":
                    return prefix + "htmlIcon.gif";
                //Word Doc icons
                case "doc":
                case "docx":
                    return prefix + "wordIcon.gif";
                //Image icons
                case "bmp":
                case "jpg":
                case "jpeg":
                case "gif":
                case "png":
                case "tif":
                case "wbmp":
                    return prefix + "imageIcon.png";
                //Audio icons
                case "aif":
                case "iff":
                case "mp3":
                case "mid":
                case "wav":
                case "wma":
                    return prefix + "audioIcon.png";
                //Video icons
                case "avi":
                case "mov":
                case "mp4":
                case "wmv":
                case "swf":
                    return prefix + "videoIcon.png";
                //Font icons
                case "fnt":
                case "fon":
                case "oft":
                case "ttf":
                    return prefix + "fontIcon.png";
                //Font icons
                case "zip":
                case "rar":
                    return prefix + "compressIcon.png";
                //Default icon
                default:
                    return prefix + "uploadedDoc.gif";
            }
        }
    }
}
