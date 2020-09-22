<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="DefaultOld.aspx.cs"
    Inherits="Forum_Default" Title="Communication" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Communication/NewsDefault.ascx" TagName="News" TagPrefix="ucn" %>
<%@ Register Src="~/Communication/ForumDefault.ascx" TagName="Forum" TagPrefix="ucf" %>
<%@ Register Src="~/Communication/ForumDetails.ascx" TagName="ForumDetails" TagPrefix="ucf" %>
<%@ Register Src="~/Communication/ForumNewMessage.ascx" TagName="ForumNewMessage" TagPrefix="ucf" %>
<%@ Register Src="~/Communication/ForumNewTopic.ascx" TagName="ForumNewTopic" TagPrefix="ucf" %>
<%@ Register Src="~/Communication/ForumSearch.ascx" TagName="ForumSearch" TagPrefix="ucf" %>
<%@ Register Src="~/Communication/ForumViewForum.ascx" TagName="ForumViewForum" TagPrefix="ucf" %>
<%@ Register Src="~/Communication/ForumViewTopic.ascx" TagName="ForumViewTopic" TagPrefix="ucf" %>
<%@ Register Src="~/Communication/DefaultPrivateMessages.ascx" TagName="PrivateMessages" TagPrefix="uc1" %>
<%@ Register Src="~/Communication/DefaultForums.ascx" TagName="Forums" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <telerik:RadCodeBlock runat="server" ID="radCodeBlock">
        <script src="../jquery/DocoBlock.js" type="text/javascript"></script>
        <script src="../jquery/TreeView.js" type="text/javascript"></script>
        <script type="text/javascript" language="javascript">
            var treeViewType = 'Category';
            var treeViewName = 'News';
            var nodeName = '';
            var nodeId = '';
            var isGridOnPage = true;
        </script>
    </telerik:RadCodeBlock>
    <h1 class="sectionhead" id="messages">
        Communication</h1>
    <ajax:TabContainer ID="tabCommunication" runat="server" ActiveTabIndex="0">
        <ajax:TabPanel ID="tbpNews" runat="server" HeaderText="Announcements">
            <ContentTemplate>
                <ucn:News ID="NewsDefault" runat="server" />
            </ContentTemplate>
        </ajax:TabPanel>
        <ajax:TabPanel ID="tbpPrivateMessages" runat="server" HeaderText="Private Messages"
            Enabled="true">
            <ContentTemplate>
                <uc1:PrivateMessages runat="server" />
            </ContentTemplate>
        </ajax:TabPanel>
        <ajax:TabPanel ID="tppForum" runat="server" HeaderText="Forums" Enabled="false">
            <ContentTemplate>
                <ucf:Forum ID="ctrlForum" runat="server" />
                <ucf:ForumDetails ID="ctrlForumDetails" runat="server" Visible="false" />
                <ucf:ForumNewTopic ID="ctrlForumNewTopic" runat="server" Visible="false" />
                <ucf:ForumSearch ID="ctrlForumSearch" runat="server" Visible="false" />
                <ucf:ForumViewForum ID="ctrlForumViewForum" runat="server" Visible="false" />
                <ucf:ForumViewTopic ID="ctrlForumViewTopic" runat="server" Visible="false" />
                <ucf:ForumNewMessage ID="ctrlForumNewMessage" runat="server" Visible="false" />
            </ContentTemplate>
        </ajax:TabPanel>
    </ajax:TabContainer>
</asp:Content>
