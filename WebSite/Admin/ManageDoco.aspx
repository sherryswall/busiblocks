<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ManageDoco.aspx.cs"
    Inherits="Admin_ManageDoco" Title="Manage Documents Block" %>

<%@ Register TagPrefix="tree" TagName="TreeView" Src="~/Controls/TreeView.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <telerik:RadCodeBlock runat="server" ID="radCodeBlock">
        <script type="text/javascript" language="javascript">
            var isGridOnPage = false;
            var treeViewType = 'Category';
            var treeViewName = 'Doco';
        </script>
    </telerik:RadCodeBlock>
    <h1 class="sectionhead" id="managedoco">
        Manage Documents Block</h1>
    <h2>
        Document Categories</h2>
    <tree:TreeView runat="server" ID="tree1" />
</asp:Content>
