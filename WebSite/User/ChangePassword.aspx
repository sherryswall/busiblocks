<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ChangePassword.aspx.cs" Inherits="ChangePassword" Title="Change password" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" Runat="Server">
    <h1 id="heading" runat="server">Change password</h1>

    <asp:ChangePassword ID="ChangePassword1" runat="server" 
        CancelDestinationPageUrl="~/Default.aspx" 
        ContinueDestinationPageUrl="~/Default.aspx" 
        SuccessPageUrl="~/Default.aspx"
        ChangePasswordButtonStyle-CssClass="btn"
        CancelButtonStyle-CssClass="btn"
        CssClass="formTable"
        ContinueButtonStyle-CssClass="btn">
    </asp:ChangePassword>

</asp:Content>

