<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" 
    Inherits="User_Default" Title="User" %>
    
<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" Runat="Server">
    <h1 class="sectionhead" id="user">User settings</h1>
    <ul>
        <li><a href="ChangePassword.aspx">Change password</a></li>
        <li><a runat="server" id="lnkUserInfo">Change user information</a></li>
    </ul>
</asp:Content>

