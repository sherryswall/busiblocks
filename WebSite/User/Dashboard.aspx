<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Dashboard.aspx.cs"
    Inherits="Dashboard" Title="Home" %>

<%@ Register TagPrefix="uc" TagName="ViewArticle" Src="~/Controls/ViewArticle.ascx" %>
<%@ Register Src="~/Controls/DashboardNews.ascx" TagName="DashboardNews" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/DashboardDoco.ascx" TagName="DashboardDoco" TagPrefix="uc2" %>
<%@ Register Src="~/Controls/DashboardTraining.ascx" TagName="DashboardTraining"
    TagPrefix="uc3" %>
<%@ Register Src="~/Controls/DashboardMessages.ascx" TagName="DashboardMessages"
    TagPrefix="uc4" %>
<%@ Register Src="~/Controls/DashboardManager.ascx" TagName="DashboardManager" TagPrefix="uc5" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <h1 class="sectionhead" id="dashboard">
        Dashboard</h1>
        <uc5:DashboardManager ID="DashboardManager1" runat="server" Visible="false" />
        <div class="advert">
            <%--<object classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" width="350" height="294"
                id="CEO_message" align="middle">
                <param name="movie" value="CEO_message.swf" />
                <param name="quality" value="high" />
                <param name="bgcolor" value="#ffffff" />
                <param name="play" value="true" />
                <param name="loop" value="true" />
                <param name="wmode" value="window" />
                <param name="scale" value="showall" />
                <param name="menu" value="true" />
                <param name="devicefont" value="false" />
                <param name="salign" value="" />
                <param name="allowScriptAccess" value="sameDomain" />
                <!--[if !IE]>-->
                <object type="application/x-shockwave-flash" data="CEO_message.swf" width="350" height="294">
                    <param name="movie" value="CEO_message.swf" />
                    <param name="quality" value="high" />
                    <param name="bgcolor" value="#ffffff" />
                    <param name="play" value="true" />
                    <param name="loop" value="true" />
                    <param name="wmode" value="window" />
                    <param name="scale" value="showall" />
                    <param name="menu" value="true" />
                    <param name="devicefont" value="false" />
                    <param name="salign" value="" />
                    <param name="allowScriptAccess" value="sameDomain" />
                    <!--<![endif]-->
                    <a href="http://www.adobe.com/go/getflash">
                        <img src="http://www.adobe.com/images/shared/download_buttons/get_flash_player.gif"
                            alt="Get Adobe Flash player" />
                    </a>
                    <!--[if !IE]>-->
                </object>
                <!--<![endif]-->
            </object>
            <img src="../app_themes/default/images/advert.jpg" />--%></div>
    <uc1:DashboardNews ID="DashboardNews1" runat="server" />
    <uc2:DashboardDoco ID="DashboardDoco1" runat="server" />
    <uc3:DashboardTraining ID="DashboardTraining1" runat="server" Visible="false" />
    <uc4:DashboardMessages ID="DashboardMessages1" runat="server" />
</asp:Content>
