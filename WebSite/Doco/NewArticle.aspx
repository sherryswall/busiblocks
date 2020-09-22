<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="NewArticle.aspx.cs"
    Inherits="Doco_NewArticle" Title="New Document" ValidateRequest="false" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="pm" TagName="PermissionMatrix" Src="~/Controls/CategoryPermissionMatrix.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            var $titleFocus = $('#ctl00_contentPlaceHolder_txtTitle');
            if ($titleFocus != null) {
                $titleFocus.focus();
            }
            $('#rblDocoType input:radio').click(function () {
                var rbDocoTypeValue = $(this).val();
                if (rbDocoTypeValue == 'uploaded') {
                    $('#rowChapNumbs').fadeOut('fast');
                    $('#rowTOCToggle').fadeOut('fast');
                    $('#rowUploadFile').fadeIn('slow');
                }
                else {
                    $('#rowUploadFile').fadeOut('fast');
                    $('#rowChapNumbs').fadeIn('slow');
                    $('#rowTOCToggle').fadeIn('slow');
                }
            });
        });
    </script>
    <h1>New Document</h1>
    <div><pm:PermissionMatrix id="pmm" runat="server" />
        <br />
        <table class="formTable minimum">
            <tr>
                <td><label for="txtTitle">Title:</label></td>
                <td><asp:TextBox ID="txtTitle" runat="server" MaxLength="200" Columns="50" Width="350px"></asp:TextBox></td>
                <td class="blankCell"><asp:RequiredFieldValidator ID="validatorTitle" runat="server" ControlToValidate="txtTitle" ErrorMessage="Title field is required" CssClass="validateText"></asp:RequiredFieldValidator></td>
            </tr>
            <tr>
                <td><label for="txtDescription">Description:</label></td>
                <td><asp:TextBox ID="txtDescription" runat="server" MaxLength="500" Columns="50" TextMode="MultiLine" Rows="3" Width="350px" ></asp:TextBox></td>
                <td class="blankCell"></td>
            </tr>
            <tr>
                <td>
                    <label for="rbListDocoType">Document Type:</label>
                </td>
                <td>
                    <div id="rblDocoType">
                        <asp:RadioButtonList AutoPostBack="false" runat="server" ID="rbListDocoType" RepeatDirection="Horizontal">
                            <asp:ListItem Value="online">Online</asp:ListItem>
                            <asp:ListItem Value="uploaded">Uploaded</asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </td>
                <td class="blankCell"><asp:RequiredFieldValidator ID="rfvDocoType" ControlToValidate="rbListDocoType" runat="server"
                            ErrorMessage="Document type is a required field" CssClass="validateText"></asp:RequiredFieldValidator></td>
            </tr>
            <tr id="rowChapNumbs" style="display: none;">
                <td>
                    <label for="">
                        Numbered Chapters:
                    </label>
                    <br />
                </td>
                <td>
                    <asp:Panel ID="pnlChapNumbs" runat="server">
                        <asp:RadioButtonList runat="server" ID="rblChapNumbs" RepeatDirection="Horizontal">
                            <asp:ListItem Text="Yes" Value="yes"></asp:ListItem>
                            <asp:ListItem Text="No" Value="no"></asp:ListItem>
                        </asp:RadioButtonList>
                    </asp:Panel>
                </td>
                <td class="blankCell"></td>
            </tr>
            <tr id="rowTOCToggle" class="hideElement">
                <td>
                    <label for="">
                        Table of Contents:
                    </label>
                    <br />
                </td>
                <td>
                    <asp:Panel ID="pnlTOCToggle" runat="server">
                        <asp:RadioButtonList runat="server" ID="rblTOCToggle" RepeatDirection="Horizontal">
                            <asp:ListItem Text="Yes" Value="yes"></asp:ListItem>
                            <asp:ListItem Text="No" Value="no"></asp:ListItem>
                        </asp:RadioButtonList>
                    </asp:Panel>
                </td>
                <td class="blankCell"></td>
            </tr>
            <tr id="rowUploadFile" style="display: none;">
                <td>
                    <label for="">Select a file</label><br />
                </td>
                <td>
                    <asp:Panel ID="pnlUpload" runat="server">
                        <asp:FileUpload ID="fuUpload" runat="server" CssClass="button" Width="400px" />
                        <asp:RequiredFieldValidator ID="rfvUpload" runat="server" InitialValue="" ErrorMessage="A file name is required"
                            ControlToValidate="fuUpload" SetFocusOnError="true" Enabled="false">*&nbsp;</asp:RequiredFieldValidator>
                    </asp:Panel>
                </td>
                <td class="blankCell"></td>
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
                <td class="blankCell"></td>
            </tr>
        </table>
        <asp:CheckBox ID="chkUpload" runat="server" Text="Uploaded document" Visible="false" />
        <asp:Button ID="btSave" runat="server" Text="Save" CssClass="btn" OnClick="btSave_Click" />
        <asp:Button ID="btCancel" runat="server" Text="Cancel" CssClass="btn" OnClick="btCancel_Click"
            CausesValidation="false" />
    </div>
</asp:Content>
