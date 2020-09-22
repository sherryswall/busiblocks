<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ForumNewMessage.ascx.cs" Inherits="Communication_ForumNewMessage" %>

<%@ Register TagPrefix="uc" TagName="NewMessage" Src="~/Communication/Forum/NewMessage.ascx" %>
<%@ Register TagPrefix="uc" TagName="ViewMessage" Src="~/Communication/Forum/ViewMessage.ascx" %>


    <h1>New message</h1>

    <uc:ViewMessage ID="viewParentMessage" runat="server" ReplyLinkVisible="false" DeleteLinkVisible="false" />

    <br />

    <uc:NewMessage ID="newMessage" runat="server" />
    
    <p>
        <asp:Button ID="btSubmit" runat="server" Text="Submit" CssClass="btn" OnClick="btSubmit_Click" />
        <asp:Button ID="btCancel" runat="server" Text="Cancel" CssClass="btn" OnClick="btCancel_Click" CausesValidation="False" />

        <asp:HiddenField runat="server" ID="hidParentId" Value="" />
    </p>

