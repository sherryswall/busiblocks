<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ForumNewTopic.ascx.cs" Inherits="Communication_ForumNewTopic" %>

<%@ Register TagPrefix="uc" TagName="NewMessage" Src="~/Communication/Forum/NewMessage.ascx" %>

    
    <h1>New topic</h1>

    <uc:NewMessage ID="newMessage" runat="server" />
    
    <p>
        <asp:Button ID="btSubmit" runat="server" Text="Submit" CssClass="btn" OnClick="btSubmit_Click" />
        <asp:Button ID="btCancel" runat="server" Text="Cancel" CssClass="btn" CausesValidation="False" OnClick="btCancel_Click" />
    </p>
