using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BusiBlocks.Membership;
using BusiBlocks.DocoBlock;
using BusiBlocks;
using BusiBlocks.Versioning;
using BusiBlocks.Audit;

/// <summary>
/// Summary description for Utilities
/// </summary>
public static class Utilities
{
    private static string Anonymous = "anonymous";

    //Traffic Light Image urls;
    private const string ImageCubeGreen = "../app_themes/default/icons/cube_green.png";
    private const string ImageCubeRed = "../app_themes/default/icons/cube_red.png";
    private const string ImageCubeYellow = "../app_themes/default/icons/cube_yellow.png";
    //Traffic Light Image Tags;
    private const string HtmlElementImageCubeGreen = "<img src='../app_themes/default/icons/cube_green.png' class='center' />";
    private const string HtmlElementImageCubeRed = "<img src='../app_themes/default/icons/cube_red.png' class='center' />";
    private const string HtmlElementImageCubeYellow = "<img src='../app_themes/default/icons/cube_yellow.png' class='center' />";

    public static string GetDateTimeForDisplay(DateTime? date)
    {
        if (date == null)
            return string.Empty;
        else
        {
            //TimeSpan diff = DateTime.Now - date.Value;
            //if (diff.TotalDays < 1)
            //    return diff.Hours.ToString() + "hrs " + diff.Minutes.ToString() + "min ago";
            //else
            return date.Value.ToString("dd/MM/yy - HH:mm");
        }
    }

    public static string GetDisplayUser(string user)
    {
        if (user == null || user.Length == 0)
            return "[" + Anonymous + "]";
        else
            return user;
    }

    public static string GetDisplayUserId(string userId)
    {
        if (userId == null || userId.Length == 0)
            return "[" + Anonymous + "]";
        else
        {
            User user = MembershipManager.GetUser(userId);
            return GetPersonDisplayName(user);
        }
    }

    public static string GetDisplayUserName(string userName)
    {
        if (string.IsNullOrEmpty(userName))
            throw new ArgumentNullException(userName);

        User user = MembershipManager.GetUserByName(userName);
        if (user == null)
            return Anonymous;
        else
            return GetPersonDisplayName(user);
    }

    public static string GetDisplayUserFirstName(string userName)
    {
        if (string.IsNullOrEmpty(userName))
            throw new ArgumentNullException(userName);

        User user = MembershipManager.GetUserByName(userName);
        if (user == null)
            return Anonymous;
        else
            return user.Person.FirstName;
    }

    public static string GetUserName(string userName)
    {
        if (string.IsNullOrEmpty(userName))
            throw new ArgumentNullException(userName);

        User user = MembershipManager.GetUserByName(userName);
        if (user == null)
            return Anonymous;
        else
            return user.Name;
    }
    private static string GetPersonDisplayName(User user)
    {
        return user.Person.FirstName + " " + user.Person.LastName;
    }

    public static string FormatDate(DateTime date)
    {
        return date.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
    }

    public static DateTime ParseDate(string dtString)
    {
        return DateTime.ParseExact(dtString, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
    }

    public static string FormatException(Exception ex)
    {
        string message = ex.Message;

        //Add the inner exception if present (showing only the first 50 characters of the first exception)
        if (ex.InnerException != null)
        {
            if (message.Length > 50)
                message = message.Substring(0, 50);

            message += "...->" + ex.InnerException.Message;
        }

        return message;
    }

    public static Control FindControlRecursive(Control Root, string Id)
    {
        if (Root.ID == Id)
            return Root;

        foreach (Control Ctl in Root.Controls)
        {
            Control FoundCtl = FindControlRecursive(Ctl, Id);

            if (FoundCtl != null)
                return FoundCtl;
        }

        return null;
    }

    public static string EscapeSpecialCharacters(string badString)
    {
        return Microsoft.JScript.GlobalObject.escape(badString);
    }
    /// <summary>
    /// returns the image tag for the traffic light url.
    /// </summary>
    /// <param name="requiresAck"></param>
    /// <param name="viewedOrAcked"></param>
    /// <returns></returns>
    public static string GetTrafficLightImageTag(bool requiresAck, bool viewedOrAcked)
    {
        if (requiresAck)
        {
            if (viewedOrAcked)
                return HtmlElementImageCubeGreen;
            return HtmlElementImageCubeRed;
        }
        if (viewedOrAcked)
            return HtmlElementImageCubeGreen;
        return HtmlElementImageCubeYellow;
    }

    /// <summary>
    /// returns the url for the traffic light image
    /// </summary>
    /// <param name="requiresAck"></param>
    /// <param name="viewedOrAcked"></param>
    /// <returns></returns>
    public static string GetTrafficLightImageUrl(bool requiresAck, bool viewedOrAcked)
    {
        if (requiresAck)
        {
            if (viewedOrAcked)
                return ImageCubeGreen;
            return ImageCubeRed;
        }
        if (viewedOrAcked)
            return ImageCubeGreen;
        return ImageCubeYellow;
    }
}
