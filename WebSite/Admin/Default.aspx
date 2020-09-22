<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs"
    Inherits="Admin_Default" Title="Administration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <h1 class="sectionhead" id="adminLocPeop">Manage Locations And People</h1>
    <div class="inlinelist">
        <ul>
            <li><a href="ManageLocations.aspx">
                <img src="../App_Themes/Default/icons/managereg_24.png" />Manage Locations</a></li>
            <li id="listManageRolesGroups" runat="server"><a href="ManageRolesGroups.aspx">
                <img src="../App_Themes/Default/icons/groups.png" />Manage Groups</a></li>
            <li><a href="SearchUsers.aspx">
                <img src="../App_Themes/Default/icons/searchuser_24.png" />Manage Users</a></li>
        </ul>
        <br style="clear: left" />
    </div>
    <div class="inlinelist" id="divManageBlocks" runat="server">
        <h1 class="sectionhead" id="adminBlocks">
            Manage Blocks</h1>
        <ul>
            <li><a href="ManageComm.aspx">
                <img src="../App_Themes/Default/icons/managecomm_24.png" />Communication Block</a></li>
            <li><a href="ManageDoco.aspx">
                <img src="../App_Themes/Default/icons/managedoco_24.png" />Document Block</a></li>
        </ul>
    </div>
</asp:Content>
