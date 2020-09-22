<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ViewCategory.aspx.cs"
    Inherits="Doco_ViewCategory" Title="View category" %>
    
<%@ Register TagPrefix="uc" TagName="ArticleList" Src="~/Controls/ArticleList.ascx" %>
<%@ Register TagPrefix="pm" TagName="PermissionMatrix" Src="~/Controls/CategoryPermissionMatrix.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <h1>
        Category: <span id="lblDisplayName" runat="server"></span>
    </h1>
    <%--    <p id="lblDescription" runat="server">
    </p>--%>
    <p>
        <a class="newitem" id="linkNew" runat="server" visible="false">New Document</a>
    </p>
    <pm:PermissionMatrix id="pmm" runat="server" />
    <uc:ArticleList ID="list" runat="server" />
</asp:Content>
