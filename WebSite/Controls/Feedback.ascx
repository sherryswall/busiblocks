<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Feedback.ascx.cs" Inherits="Controls_Feedback" %>

<asp:Panel runat="server" ID="pnlFeedback" Visible="false">
    <asp:Panel runat="server" ID="pnlContent">
        <asp:Panel runat="server" ID="pnlLeft"></asp:Panel>
        <asp:Panel runat="server" ID="pnlMiddle" CssClass="feedbackmiddle">
            <asp:Literal runat="server" ID="litMessage"></asp:Literal>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlRight" CssClass="feedbackright"></asp:Panel>
    </asp:Panel>    
</asp:Panel>
