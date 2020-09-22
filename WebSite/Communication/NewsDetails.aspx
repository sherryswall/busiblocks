<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="NewsDetails.aspx.cs" Inherits="NewsDetails" Title="Announcement Details" %>

<%@ Register src="~/Controls/AccessControl.ascx" tagname="AccessControl" tagprefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolder" runat="Server">

<script language="javascript" type="text/javascript">

    function CheckBoxListSelect(cbControl, state) {
        var chkBoxList = document.getElementById(cbControl);
        var chkBoxCount = chkBoxList.getElementsByTagName("input");
        for (var i = 0; i < chkBoxCount.length; i++) {
            chkBoxCount[i].checked = state;
        }

        return false;
    }
 
</script>

    <h1>
        Announcement Category Details</h1>
    <div>
        <asp:ValidationSummary ID="validationSummary1" runat="server" DisplayMode="List" />
        <table class="formTable">
            <tr>
                <td>
                    <label for="txtDisplayName">Name:</label></td>
                <td>
                    <asp:TextBox ID="txtDisplayName" runat="server" MaxLength="100" Width="370px"></asp:TextBox></td>
            </tr>
            <tr>
                <td>
                    <label for="txtDescription">Description:</label></td>
                <td>
                    <asp:TextBox ID="txtDescription" runat="server" MaxLength="300" Columns="50" Width="370px"></asp:TextBox></td>
            </tr>
            <tr>
                <td>
                    <label for="cmbParentCategory">Parent Category:</label></td>
                <td>
                    <asp:DropDownList ID="cmbParentCategory" runat="server" Width="370px"/></td>
            </tr>
        </table>

        <h2>Access</h2>

        <uc1:AccessControl ID="AccessControl1" runat="server" />

        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtDisplayName"
            Display="None" ErrorMessage="Name field is required"></asp:RequiredFieldValidator>
        <p>
            <asp:Button ID="btSave" runat="server" Text="Save" CssClass="btn" OnClick="btSave_Click" />
            <asp:Button ID="btCancel" runat="server" Text="Cancel" CssClass="btn" OnClick="btCancel_Click" CausesValidation="false" />
        </p>
    </div>    
</asp:Content>