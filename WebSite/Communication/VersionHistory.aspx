<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="VersionHistory.aspx.cs" Inherits="Communication_VersionHistory" %>

<%@ Register TagPrefix="version" TagName="VersionHistory" Src="../Controls/VersionHistory.ascx" %>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <h1>
        Version History</h1>    
    <version:VersionHistory runat="server" ID="ctrlVersionHistory" />
</asp:Content>
