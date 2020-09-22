<%@ Page Title="Communication" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Communication_Announcements" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="NewsDefault.ascx" TagName="News" TagPrefix="ucn" %>

<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <ucn:News ID="NewsDefault" runat="server" />
</asp:Content>
