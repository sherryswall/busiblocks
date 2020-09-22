<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ViewUploadedDoc.aspx.cs"
    Inherits="Doco_ViewUploadedDoc" Title="View Uploaded Document" ValidateRequest="false" %>

<%@ Register Src="~/Controls/ModalPopup.ascx" TagName="ModalPopup" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <script type="text/javascript" language="javascript">
        var AcknowledgeClick = function () {
            popAck.Show();
        }                  
    </script>
    <h1 class="sectionhead" id="docoblock">
        Documents</h1>
    <uc1:ModalPopup runat="server" Title="Acknowledge Document" ID="popAck" Content="Do you confirm that you have read and understood this document?"
        OnAcceptClick="btnAcknowledge_Click" CancelButtonText="Cancel" AcceptButtonText="Acknowledge">
    </uc1:ModalPopup>
    <div class="ackTitle">
        <div id="ackText" class="ackText" runat="server">
            <div id="divAckButton" runat="server" class="divAckButton">
                I acknowledge I have read and understood.
                <asp:Button ID="btnAcknowledge" runat="server" OnClientClick="javascript:AcknowledgeClick(); return false;"
                    Width="32" Height="32" />
            </div>
        </div>
        <h1>
            <asp:Label ID="lblTitle" runat="server" /></h1>
    </div>
    <div>
        <table class="standardTable minimum">
            <tr class="primary">
                <td>
                    <label for="lnkFileName">
                        File Name:</label>
                </td>
                <td>
                    <asp:Image ID="imgFileName" runat="server" />
                    <asp:LinkButton ID="lnkFileName" runat="server" OnClick="lnkFileName_Click" />
                </td>
            </tr>
            <tr>
                <td>
                    <label for="lblDescription">
                        Description:</label>
                </td>
                <td>
                    <asp:Label ID="lblDescription" runat="server" MaxLength="200" Columns="50" />
                </td>
            </tr>
            <tr>
                <td>
                    <label for="lblCategory">
                        Category:</label>
                </td>
                <td>
                    <asp:Label ID="lblCategory" runat="server" MaxLength="200" Columns="50" />
                </td>
            </tr>
            <tr>
                <td>
                    <label for="lblOwner">
                        Owner:</label>
                </td>
                <td>
                    <asp:Label ID="lblOwner" runat="server" MaxLength="200" Columns="50" />
                </td>
            </tr>
            <tr>
                <td>
                    <label for="lblDocumentType">
                        Document Type:</label>
                </td>
                <td>
                    <asp:Label ID="lblDocumentType" runat="server" MaxLength="200" Columns="50" />
                </td>
            </tr>
            <tr>
                <td>
                    <label for="lblPublished">
                        Published:</label>
                </td>
                <td>
                    <asp:Label ID="lblPublished" runat="server" MaxLength="200" Columns="50" />
                </td>
            </tr>
            <tr>
                <td>
                    <label for="lblVersion">
                        Version:</label>
                </td>
                <td>
                    <asp:Label ID="lblVersion" runat="server" MaxLength="200" Columns="50" />
                </td>
            </tr>
            <tr runat="server" id="trStatus">
                <td>
                    <label id="ackLabel" runat="server" for="lblAcknowledged">
                        Acknowledged:</label>
                </td>
                <td>
                    <asp:Image ID="imgAck" runat="server" />
                </td>
            </tr>
            <%-- <tr>
                <td>
                    <label for="lblComments">
                        Comments:</label>
                </td>
                <td>
                    <asp:Label ID="lblComments" runat="server" MaxLength="200" Columns="50" />
                </td>
            </tr>--%>
        </table>
        <asp:Button runat="server" Text="Return to Documents" CssClass="btn" ID="btnReturn" OnClick="btnReturn_Click" />
    </div>
</asp:Content>
