<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="EditArticle.aspx.cs"
    Inherits="Doco_EditArticle" Title="Edit Document" ValidateRequest="false" %>
    
<%@ Register TagPrefix="pm" TagName="PermissionMatrix" Src="~/Controls/CategoryPermissionMatrix.ascx" %>
<asp:Content ContentPlaceHolderID="HeadContent" ID="headContent1" runat="server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <script language="javascript" type="text/javascript">
        function showUploadRow() {
            var uploadRow = document.getElementById('rowUploadFile2');
            var btnShowUpRow = document.getElementById('btnShowUplodRow');

            $('#' + btnShowUpRow.id).hide();
            $('#' + uploadRow.id).show();
        }
        function cancelUpload() {
            var uploadRow = document.getElementById('rowUploadFile2');
            var btnShowUpRow = document.getElementById('btnShowUplodRow');

            $('#' + btnShowUpRow.id).show();
            $('#' + uploadRow.id).hide();

        }
        $(document).ready(function () {
            $('div.chapterReorderList').click().toggle(function () {
                $('div.subChapList').slideDown('normal');
            }, function () {
                $('div.subChapList').slideUp('normal');
            });
        });
    </script>
    <h1>
        Edit Document</h1>
    <div>
        <pm:PermissionMatrix id="pmm" runat="server" />
        <table class="formTable">
            <tr>
                <td>
                    <label for="txtTitle">
                        Title:</label>
                </td>
                <td>
                    <asp:TextBox ID="txtTitle" runat="server" MaxLength="100" Columns="50"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="validatorTitle" runat="server" ControlToValidate="txtTitle"
                        ErrorMessage="Title field is required"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>
                    <label for="txtDescription">
                        Description:</label><br />
                </td>
                <td>
                    <asp:TextBox ID="txtDescription" runat="server" MaxLength="300" Columns="50" TextMode="MultiLine"
                        Rows="3"></asp:TextBox><br />
                </td>
            </tr>
            <tr>
                <td>
                    <label for="txtCategory">
                        Category:</label><br />
                </td>
                <td>
                    <asp:DropDownList ID="cmbCategory" runat="server" Width="370px" />
                </td>
            </tr>
            <tr>
                <td>
                    <label for="txtOwner">
                        Owner:</label>
                </td>
                <td>
                    <asp:TextBox ID="txtOwner" runat="server" MaxLength="100" Enabled="false"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <label for="txtAuthor">
                        Author:</label>
                </td>
                <td>
                    <asp:TextBox ID="txtAuthor" runat="server" MaxLength="100"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <label for="RadioButtonList1">
                        Document Type:</label><br />
                </td>
                <td>
                    <div id="rblDocoType">
                        <asp:RadioButtonList AutoPostBack="true" runat="server" ID="rbListDocoType" RepeatDirection="Horizontal"
                            Enabled="false">
                            <asp:ListItem Value="online">Online</asp:ListItem>
                            <asp:ListItem Value="uploaded">Uploaded</asp:ListItem>
                        </asp:RadioButtonList>
                        <asp:RequiredFieldValidator ID="rfvDocoType" ControlToValidate="rbListDocoType" runat="server"
                            ErrorMessage="Document type is a required field"></asp:RequiredFieldValidator>
                    </div>
                </td>
            </tr>
            <tr id="rowUploadFile" runat="server" visible="false">
                <td>
                    <label for="lnkFilename">
                        File Name:</label><br />
                </td>
                <td>
                    <asp:HyperLink runat="server" ID="lnkFilename"></asp:HyperLink>
                    <button class="btn" type="button" id="btnShowUplodRow" onclick="showUploadRow();">
                        Change file</button>
                </td>
            </tr>
            <tr id="rowUploadFile2" style="display: none;">
                <td>
                </td>
                <td>
                    <asp:FileUpload ID="fuUpload" runat="server" CssClass="btn" Width="400px" />
                    <asp:Button runat="server" ID="btnUpload" Text="Upload" OnClick="btnUpload_Click"
                        CssClass="btn" />
                    <button type="button" class="btn" value="Cancel" onclick="cancelUpload();">
                        Cancel</button>
                </td>
            </tr>
            <tr id="rowNumberedChapters" runat="server">
                <td>
                    <label for="rblNumberedChapters">
                        Numbered Chapters:</label><br />
                </td>
                <td>
                    <asp:RadioButtonList runat="server" ID="rblNumberedChapters" RepeatDirection="Horizontal">
                        <asp:ListItem Value="yes">Yes</asp:ListItem>
                        <asp:ListItem Value="no">No</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td>
                    <label for="rblAcknowledge">
                        Acknowledgement:</label><br />
                </td>
                <td>
                    <asp:RadioButtonList AutoPostBack="false" runat="server" ID="rblAcknowledge" RepeatDirection="Horizontal">
                        <asp:ListItem Value="notRequired">Not Required</asp:ListItem>
                        <asp:ListItem Value="required">Required</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
        </table>
        <asp:Button ID="btUpdate" runat="server" Text="Save" CssClass="btn" OnClick="btSave_Click" />
        <asp:Button ID="btCancel" runat="server" Text="Cancel" CssClass="btn" OnClick="btCancel_Click" CausesValidation="false" />
    </div>
</asp:Content>
