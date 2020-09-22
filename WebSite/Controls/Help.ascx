<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Help.ascx.cs" Inherits="Controls_Help" %>

<%@ Register Src="~/Controls/ModalPopup.ascx" TagName="ModalPopup" TagPrefix="uc1" %>

<script language="javascript" type="text/javascript">

    var <%=this.ID%> = new function () {

        this.Show = function () {
            modHelp.Show();
            $(".rwWindowContent").attr('class', 'rwWindowContent helpDialog');
        }
    }


    $(document).ready(function() {
        $(".popupModalTitle").attr('class', '');
        $(".popupModalButtons").attr('class', '');
    });


</script>

<asp:Panel runat="server" ID="pnlHelp" CssClass="helpContent">
    <asp:Panel ID="pnlTitle" runat="server" CssClass="helpTitle" />
    <asp:Panel ID="pnlPurpose" runat="server" CssClass="helpPurpose" />
    <asp:Panel ID="pnlWorks" runat="server" CssClass="helpWorks" />
    <asp:Panel ID="pnlUse" runat="server" CssClass="helpUse" />
</asp:Panel>
