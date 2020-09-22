<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeFile="NewsViewItem.aspx.cs"
    Inherits="Communication_NewsViewItem" Title="View Announcement" ValidateRequest="false" %>

<%@ Register Src="~/Controls/ModalPopup.ascx" TagName="ModalPopup" TagPrefix="uc1" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" runat="Server">

    <uc1:ModalPopup ID="popReject" runat="server" Content="Enter the reason for rejecting this announcement below"
        AcceptButtonText="Reject" Title="Reject Announcement" CancelButtonText="Cancel"
        OnAcceptClick="btnReject_Click" InputName="Reason" />

    <uc1:ModalPopup ID="popAck" runat="server" Content="Do you confirm that you have read and understood this announcement?"
        Title="Acknowledge Announcement" AcceptButtonText="Acknowledge" CancelButtonText="Cancel" OnAcceptClick="btnAcknowledge_Click"/>

        <script type="text/javascript" language="javascript">
            var RejectClick = function () {
                popReject.Show();
            }

            var AcknowledgeClick = function () {
                popAck.Show();
            }
        </script>

    <div class="annView">
        <div class="ackTitle">
            <div id="ackText" class="ackText" runat="server">
                <div id="divAckButton" class="divAckButton" runat="server">
                    I acknowledge I have read and understood. <asp:Button ID="btnAcknowledge" runat="server" OnClientClick="javascript: AcknowledgeClick(); return false;"/>
                </div>
            </div>
            <h1>
                <asp:Label runat="server" ID="lblPageHeading" />
            </h1>
        </div>
        <div>
            <div class="annDetails">
                <span><b>Author: </b><asp:Label runat="server" ID="lblAuthor" class="author" /></span> 
                <span><b>Date: </b><asp:Label runat="server" ID="lblDate" class="authordate" /></span> 
                <span runat="server" id="spanAck"><b>Acknowledged: </b><asp:Label runat="server" ID="lblAck">&nbsp;</asp:Label></span>
                <span><b>Version: </b><asp:Label runat="server" ID="lblVersionNumber" class="authordate" /></span>
            </div>
            <div class="annContent">
                <asp:Label runat="server" ID="txtDetails"></asp:Label>
            </div>
        </div>
        <div class="approveFooter" runat="server" id="approveFooter">
            <div class="functionButtons approveFunctions" id="divApprovalBar" runat="server">
                <h3 runat="server" id="h2LblAction"><asp:Label runat="server" ID="lblAction"></asp:Label>Approve Change</h3>
                <div class="approvalbuttons">
                    <asp:Button runat="server" Text="Cancel" OnClick="btnCancel_Click" ID="btnCancelApprove" CssClass="btn" />
                    <asp:Button runat="server" Text="Edit" OnClick="btnEdit_Click" ID="btnEditApprove" CssClass="btn" />
                    <asp:Button runat="server" Text="Reject" OnClientClick="RejectClick(); return false;" ID="btnReject" CssClass="btn" />
                    <asp:Button runat="server" Text="Approve" OnClick="btnApprove_Click" ID="btnApprove" CssClass="btn" />
                </div>
            </div>
            <div class="functionButtons actionFunctions" id="divActionButton" runat="server">
                <asp:Button runat="server" Text="Cancel" OnClick="btnCancel_Click" ID="btnCancel" CssClass="btn" />
                <asp:Button runat="server" Text="Edit" OnClick="btnEdit_Click" ID="btnEdit" CssClass="btn" />
                <asp:Button runat="server" Text="Restore" OnClick="btnRestore_Click" ID="btnRestore" CssClass="btn" />
            </div>
            <div class="versionNumber"><asp:HyperLink runat="server" ID="lnkVersion"></asp:HyperLink></div>
        </div>
        <asp:Button runat="server" Text="Return to Communication" CssClass="btn" ID="btnReturn" OnClick="btnReturn_Click" />
    </div>
</asp:Content>
