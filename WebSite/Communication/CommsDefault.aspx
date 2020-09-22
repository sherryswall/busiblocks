<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="CommsDefault.aspx.cs" Inherits="Communication_CommsDefault" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" Runat="Server">
<h1 class="sectionhead" id="comms">Communication</h1>
    <div class="inlinelist">
        <ul>
            <li><a href="Default.aspx">
                <img src="../App_Themes/Default/icons/commAnnouncement_24.png" />Announcements</a></li>
            <li><a href="Forum/Default.aspx">
                <img src="../App_Themes/Default/icons/commForums_24.png" />Forums</a></li>
            <li><a href="PrivateMessages/Default.aspx">
                <img src="../App_Themes/Default/icons/commPvtMsgs_24.png" />Private Messages</a></li>
        </ul>
        <br style="clear: left" />
    </div>
</asp:Content>

