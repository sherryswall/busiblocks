<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ManageComm.aspx.cs"
    Inherits="Admin_ManageComm" Title="Manage Communication Block" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../Controls/AccessControl.ascx" TagName="AccessControl" TagPrefix="uc1" %>
<%@ Register TagPrefix="tree" TagName="TreeView" Src="~/Controls/TreeView.ascx" %>
<%@ Register TagPrefix="ctrl" TagName="TreeView" Src="~/Controls/TreeView/NewsCategoryTreeView.ascx" %>

<asp:Content ContentPlaceHolderID="HeadContent" ID="Content2" runat="server" type="text/javascript">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" runat="Server">

    <h1 class="sectionhead" id="managedoco">
        Manage Communication Block</h1>
    <h2>
        Announcement Categories</h2>  
    <ctrl:treeview id="tv" runat="server" menumode="Administration" />
</asp:Content>
