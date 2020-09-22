
<%@ Page Title="Error" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Error.aspx.cs" Inherits="Error"  %>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <asp:Panel ID="pnlRelease" runat="server">
        <div class="errorLeft">
            <p>
                <img src="App_Themes/Default/images/blockLogo.png" /></p>
        </div>
        <div class="errorRight">
            <p>
                <img src="App_Themes/Default/images/blockText.png" /></p>
            <p>
                BusiBlocks has encountered an error on this page</p>
            <p>
                A report of this error has been sent to the support team and they are attempting
                to get this problem resolved as soon as possible.</p>
        </div>
        <h2>
            Suggestions:</h2>
        <ul>
            <li>Use the navigation links above to attempt the process again</li>
            <li>Try the process again later - we may be having issues with our servers</li>
            <li>If the problem persists, send us a detailed report by clicking the <b>Feedback</b>
                button below</li>
        </ul>
        <p>
            BusiBlocks apologises for the inconvenience, and hopes that the problem will be
            resolved quickly.</p>
    </asp:Panel>
    <asp:Panel ID="pnlDebug" runat="server" Visible="false">
        <hr />
        <b>Technical Information</b>
        <br />
        <asp:Label ID="lbError" runat="server"></asp:Label>
    </asp:Panel>
</asp:Content>
