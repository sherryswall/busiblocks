<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ViewArticleVersions.aspx.cs"
    Inherits="Doco_ViewArticleVersions" Title="View Document Versions" %>

<%@ Register TagPrefix="uc" TagName="ArticleVersionList" Src="~/Controls/ArticleVersionList.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    
    <h1>Versions</h1>
    
    <uc:ArticleVersionList ID="viewArticle" runat="server" />    
    
</asp:Content>
