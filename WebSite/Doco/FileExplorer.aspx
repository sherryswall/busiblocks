<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="FileExplorer.aspx.cs" Inherits="Doco_FileExplorer" %>

<%@ Register Src="../Controls/AccessControl.ascx" TagName="AccessControl" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <asp:MultiView ID="mvFiler" runat="server">
        <%-- PRIMARY VIEW --%>
        <asp:View ID="vView" runat="server">
            <asp:UpdatePanel ID="upPrimary" runat="server">
                <ContentTemplate>
                    <div id="buttons">
                        <asp:Label ID="lblRoot" runat="server" CssClass="title" />
                        <br />
                        <asp:Button ID="btnNewFile" runat="server" CausesValidation="false" CssClass="btn"
                            Text="New File" OnClick="btnNewFile_Click" />
                        <asp:Button ID="btnNewFolder" runat="server" CausesValidation="false" CssClass="btn"
                            Text="New Folder" OnClick="btnNewFolder_Click" />
                        <asp:Button ID="btnUpload" runat="server" CausesValidation="false" CssClass="btn"
                            Text="Upload File" OnClick="btnUpload_Click" />
                    </div>
                    <asp:PlaceHolder ID="phDisplay" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:View>
        <%-- RENAME --%>
        <asp:View ID="vRename" runat="server">
            <asp:Panel ID="pnlRename" runat="server" DefaultButton="btnRenameSave">
                <table border="0" cellpadding="5" cellspacing="0">
                    <thead>
                        <tr>
                            <td colspan="2" class="header">
                                <asp:Label ID="lblRename" runat="server" />
                            </td>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>
                                <asp:RequiredFieldValidator ID="rfvRename" runat="server" ErrorMessage="The new name should not be empty"
                                    ControlToValidate="txtRename" SetFocusOnError="True">*&nbsp;</asp:RequiredFieldValidator>Enter
                                new name:
                            </td>
                            <td>
                                <asp:TextBox ID="txtRename" runat="server" Columns="35" />
                            </td>
                        </tr>
                    </tbody>
                    <tfoot>
                        <tr>
                            <td align="right" colspan="2">
                                <asp:Button ID="btnRenameCancel" runat="server" CausesValidation="false" CssClass="button"
                                    Text="Cancel" OnClick="Cancel" UseSubmitBehavior="false" />
                                <asp:Button ID="btnRenameSave" runat="server" CssClass="button" Text="Save" OnClick="btnRenameSave_Click" />
                            </td>
                        </tr>
                    </tfoot>
                </table>
            </asp:Panel>
        </asp:View>
        <%-- NEW FILE --%>
        <asp:View ID="vNewFile" runat="server">
            <asp:Panel ID="pnlNewFile" runat="server" DefaultButton="btnNewFileSave">
                <table border="0" cellpadding="5" cellspacing="0">
                    <thead>
                        <tr>
                            <td colspan="2" class="header">
                                New File
                            </td>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>
                                <asp:RequiredFieldValidator ID="rfvNewFile" runat="server" ErrorMessage="The new file name should not be empty"
                                    ControlToValidate="txtNewFile" SetFocusOnError="True">*&nbsp;</asp:RequiredFieldValidator>Enter
                                new file name:
                            </td>
                            <td>
                                <asp:TextBox ID="txtNewFile" runat="server" Columns="35" />
                            </td>
                        </tr>
                    </tbody>
                    <tfoot>
                        <tr>
                            <td align="right" colspan="2">
                                <asp:Button ID="btnNewFileCancel" runat="server" CausesValidation="false" CssClass="button"
                                    Text="Cancel" OnClick="Cancel" UseSubmitBehavior="false" />
                                <asp:Button ID="btnNewFileSave" runat="server" CssClass="button" Text="Save" OnClick="btnNewFileSave_Click" />
                            </td>
                        </tr>
                    </tfoot>
                </table>
            </asp:Panel>
        </asp:View>
        <%-- NEW FOLDER --%>
        <asp:View ID="vFolder" runat="server">
            <asp:Panel ID="pnlNewFolder" runat="server" DefaultButton="btnNewFolderSave">
                <table border="0" cellpadding="5" cellspacing="0">
                    <thead>
                        <tr>
                            <td colspan="2" class="header">
                                New Folder
                            </td>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>
                                <asp:RequiredFieldValidator ID="rfvNewFolder" runat="server" ErrorMessage="The new folder name should not be empty"
                                    ControlToValidate="txtNewFolder" SetFocusOnError="True">*&nbsp;</asp:RequiredFieldValidator>Enter
                                new folder name:
                            </td>
                            <td>
                                <asp:TextBox ID="txtNewFolder" runat="server" Columns="35" />
                            </td>
                        </tr>
                    </tbody>
                    <tfoot>
                        <tr>
                            <td>
                                Access:
                            </td>
                            <td>
                                <uc1:AccessControl ID="AccessControl1" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td align="right" colspan="2">
                                <asp:Button ID="btnNewFolderCancel" runat="server" CausesValidation="false" CssClass="btn"
                                    Text="Cancel" OnClick="Cancel" UseSubmitBehavior="false" />
                                <asp:Button ID="btnNewFolderSave" runat="server" CssClass="btn" Text="Save" OnClick="btnNewFolderSave_Click" />
                            </td>
                        </tr>
                    </tfoot>
                </table>
            </asp:Panel>
        </asp:View>
        <%-- UPLOAD A FILE --%>
        <asp:View ID="vUpload" runat="server">
            <asp:Panel ID="pnlUpload" runat="server" DefaultButton="btnUploadSave">
                <table border="0" cellpadding="5" cellspacing="0">
                    <thead>
                        <tr>
                            <td class="header">
                                Upload a File
                            </td>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>
                                <asp:RequiredFieldValidator ID="rfvUpload" runat="server" ErrorMessage="A file name is required"
                                    ControlToValidate="fuUpload" SetFocusOnError="true">*&nbsp;</asp:RequiredFieldValidator>
                                <asp:FileUpload ID="fuUpload" runat="server" CssClass="button" Width="400px" /><br />
                                <asp:CheckBox ID="chkEnabled" runat="server" Text="Enabled" /><br />
                                <asp:CheckBox ID="chkAcknowledge" runat="server" Text="User needs to acknowledge when read" /><br />
                            </td>
                        </tr>
                    </tbody>
                    <tfoot>
                        <tr>
                            <td align="right">
                                <asp:Button ID="btnUploadCancel" runat="server" CausesValidation="false" CssClass="btn"
                                    Text="Cancel" UseSubmitBehavior="false" OnClick="Cancel" />
                                <asp:Button ID="btnUploadSave" runat="server" CssClass="btn" Text="Upload" OnClick="btnUploadSave_Click" />
                            </td>
                        </tr>
                    </tfoot>
                </table>
            </asp:Panel>
        </asp:View>
    </asp:MultiView>
    <asp:ValidationSummary ID="valSummary" runat="server" />
</asp:Content>
