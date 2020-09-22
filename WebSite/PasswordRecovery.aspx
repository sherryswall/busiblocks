<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="PasswordRecovery.aspx.cs"
         Inherits="PasswordRecovery" Title="Password recovery" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" Runat="Server">

    <asp:PasswordRecovery ID="passwordRecovery" runat="server">
    </asp:PasswordRecovery>

    <small>* To recover the password your login must be configured with a valid e-mail. If you don't have a valid e-mail you must ask the administrator to reset the password.</small>
    
</asp:Content>

