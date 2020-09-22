<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="PersonDetails.aspx.cs"
    Inherits="Directory_PersonDetails" title="User Details" %>
<%@ Register TagPrefix="pd" TagName="PersonalDetails" Src="~/Controls/PersonalDetailsView.ascx" %>
<asp:Content ContentPlaceHolderID="HeadContent" ID="Content2" runat="server" type="text/javascript">
    <script type="text/javascript" language="javascript">

        function button_click(tb, buttonId) {
            if (window.event.keyCode == 13) {
                document.getElementById(buttonId).focus();
                document.getElementById(buttonId).click();
            }
        }

    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" runat="server">
    <h1 class="sectionhead" id="directory">
        Directory - User Details</h1>
    <pd:PersonalDetails ID="pdUser" runat="server" />
    <asp:Button runat="server" Text="Return to Directory" CssClass="btn" ID="btnReturn" OnClick="btnReturn_Click" />
</asp:Content>
