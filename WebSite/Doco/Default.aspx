<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs"
    Inherits="Doco_Default" Title="Documents" %>

<%@ Register TagPrefix="tree" TagName="TreeView" Src="~/Controls/TreeView.ascx" %> 
<%@ Register TagPrefix="uc" TagName="ArticleList" Src="~/Controls/ArticleList.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <h1 class="sectionhead" id="docoblock">
        Documents</h1>
    <h2>
        Categories</h2>
       <tree:TreeView runat="server" ID="tree1" />
    <asp:Label ID="lblSelectedNodePath" runat="server" Visible="false" />
    <asp:Label ID="lblSelectedNode" runat="server" Visible="false" />

    <!-- Query result -->
    <h2>
        Documents</h2>
    <uc:ArticleList ID="articleList" runat="server" />
</asp:Content>
