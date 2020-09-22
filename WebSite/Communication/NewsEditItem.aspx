<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="NewsEditItem.aspx.cs"
    Inherits="Communication_NewsEditItem" Title="Edit Announcement" ValidateRequest="false" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register TagName="AccessControl" TagPrefix="uc1" Src="~/Controls/AccessControl.ascx" %>

<%@ Register TagPrefix="ctrl" TagName="NewsCatTreeView" Src="~/Controls/TreeView/NewsCategoryTreeView.ascx" %>

<%@ Register TagPrefix="pm" TagName="PermissionMatrix" Src="~/Controls/CategoryPermissionMatrix.ascx" %>
<%@ Register TagPrefix="comments" TagName="EditorialComments" Src="~/Controls/EditorialComments.ascx" %>
<%@ Register Src="~/Controls/ModalPopup.ascx" TagName="ModalPopup" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadGrid1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadGrid1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <uc1:ModalPopup ID="popReject" runat="server" Content="Enter the reason for rejecting this announcement below"
        AcceptButtonText="Reject" Title="Reject Announcement" CancelButtonText="Cancel"
        OnAcceptClick="btnReject_Click" InputName="Reason" />
    <telerik:RadCodeBlock runat="server" ID="radCodeBlock1">
        <script language="javascript" type="text/javascript">

            $(document).ready(function () {
                if (getParameterByName("mode") == "approve") {
                    $("#divStepOne").hide();
                    $("#divStepTwo").show();
                }
                else {
                    $("#divStepOne").show();
                    $("#divStepTwo").hide();
                }
            }); 

            var txtTitleId = "<%=txtTitle.ClientID %>";
            var lblTitleId = "<%=lblTitle.ClientID %>";
            var isGridOnPage = false;
            var curSelNodeVal = "<%= CurrentSelNodeVal.ClientID %>";
            var curSelNodeName = "<%= CurrentSelNodeName.ClientID %>";

            var NextClick = function () {
                if (Page_ClientValidate('requiredText')) {
                    $("#divStepOne").hide();

                    var textBoxValue = $("#" + txtTitleId).val();
                    $("#" + lblTitleId).text(textBoxValue);

                    $("#divStepTwo").show();
                }
            }

            var BackClick = function () {
                $("#divStepOne").show();
                $("#divStepTwo").hide();
            }

            $(document).ready(function () {
                // Disable the approvers drop down list if the value of rblApprove is 'notrequired'
                var rbApprovalStatusValue = $("input[@name=rblApprovalStatus]:radio:checked").val();
                if (rbApprovalStatusValue == 'required') {
                    document.getElementById('<%= ddlApprovers.ClientID %>').disabled = false;
                } else {
                    document.getElementById('<%= ddlApprovers.ClientID %>').disabled = true;
                }

                var ackReqd = document.getElementById('<%=ackReqd.ClientID %>');
                var ackNotReqd = document.getElementById('<%=ackNotReqd.ClientID %>');
                
                $('#rblStatus input:radio').live("click", function() {
                    var rbStatusValue = $(this).val();
                    if (rbStatusValue == 'required') {
                        $('#' + ackReqd.id).removeClass("hideElement");
                        $('#' + ackNotReqd.id).addClass("hideElement");
                    }
                    else {
                        $('#' + ackReqd.id).addClass("hideElement");
                        $('#' + ackNotReqd.id).removeClass("hideElement");
                    }
                });
                $('#rblApprovalStatus input:radio').click(function () {
                    var rbApprovalValue = $(this).val();
                    var ddl = $('#ddlApprovers');
                    if (rbApprovalValue == 'required') {
                        document.getElementById('<%= ddlApprovers.ClientID %>').disabled = false;
                        if (document.getElementById('<%= btnCreate.ClientID %>') != null)
                            document.getElementById('<%= btnCreate.ClientID %>').disabled = false;
                    }
                    else {
                        document.getElementById('<%= ddlApprovers.ClientID %>').disabled = true;
                        if (document.getElementById('<%= btnCreate.ClientID %>') != null)
                            document.getElementById('<%= btnCreate.ClientID %>').disabled = true;
                    }
                });
            });

            var getParameterByName = function (name) {
                name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
                var regexS = "[\\?&]" + name + "=([^&#]*)";
                var regex = new RegExp(regexS);
                var results = regex.exec(window.location.search);
                if (results == null)
                    return "";
                else
                    return decodeURIComponent(results[1].replace(/\+/g, " "));
            }

            var RejectClick = function () {
                popReject.Show();
            }
        </script>
    </telerik:RadCodeBlock>
    <asp:HiddenField ID="CurrentSelNodeVal" runat="server" />
    <asp:HiddenField ID="CurrentSelNodeName" runat="server" />
    <h1 class="sectionhead" id="announce">
        <asp:Label runat="server" ID="lblPageHeading"></asp:Label></h1>
    <div id="divStepOne">
        <div class="newsTitle">
            <label for="txtTitle">
                Title:</label>
            <asp:TextBox ID="txtTitle" runat="server" MaxLength="100" Columns="50"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvTitle" runat="server" ControlToValidate="txtTitle"
                ErrorMessage="Title is required" ValidationGroup="requiredText"></asp:RequiredFieldValidator>
        </div>
        <div class="newsBody">
            <div class="newsAnnouncement">
                <telerik:RadEditor runat="server" ID="txtDescription" EditModes="Design" Height="120px"
                    SpellCheckSettings-SpellCheckProvider="MicrosoftWordProvider" ContentAreaMode="Div"
                    NewLineMode="Br" EnableResize="false">
                    <Tools>
                        <telerik:EditorToolGroup>
                            <telerik:EditorTool Name="FormatBlock" Text="Normal" />
                            <telerik:EditorTool Name="Bold" />
                            <telerik:EditorTool Name="Italic" />
                            <telerik:EditorTool Name="Underline" />
                            <telerik:EditorTool Name="InsertUnorderedList" />
                            <telerik:EditorTool Name="InsertOrderedList" />
                        </telerik:EditorToolGroup>
                    </Tools>
                </telerik:RadEditor>
            </div>
            <div class="newsnext">
                <asp:Button ID="btnNext" runat="server" Text="Next" CssClass="btn" OnClientClick="javascript:NextClick(); return false;" />
                <asp:Button ID="btnCancel1" runat="server" Text="Cancel" CssClass="btn" OnClick="btCancel_Click"
                    CausesValidation="false" />
                <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription"
                    ErrorMessage="Content is required" ValidationGroup="requiredText"></asp:RequiredFieldValidator>
            </div>
        </div>
    </div>
    <div id="divStepTwo">
        <div class="bindingBox">
            <asp:UpdatePanel runat="server" ID="pnl" UpdateMode="Always">
                <ContentTemplate>
                    <div class="newsTwoRight">
                        <label for="treeCategory">
                            Category:</label>
                        <div class="whiteBox">
                            
                                <ctrl:NewsCatTreeView id="tv" runat="server"  MenuMode="Off" showitemcount="false" PermissionMode="Edit" />
                            
                        </div>
                        <br />
                        <pm:PermissionMatrix ID="pmm" runat="server" />
                    </div>
                    <div class="newsTwoLeft">
                        <table class="newsTable">
                            <tr>
                                <td style="width: 130px;">
                                    <label for="lblTitle">
                                        Title:</label>
                                </td>
                                <td>
                                    <asp:Label ID="lblTitle" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label for="ddlOwner">
                                        Owner:</label>  
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlOwner" runat="server">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr class="hideElement">
                                <td>
                                    <label for="txtApprovalRequired">
                                        Approval required:</label>
                                </td>
                                <td>
                                    <div id="rblApprovalStatus">
                                        <asp:RadioButtonList runat="server" ID="rblApproval" RepeatDirection="Horizontal"
                                            BorderStyle="None" RepeatLayout="flow" Enabled="false">
                                            <asp:ListItem Value="notRequired" Selected="True">Not Required</asp:ListItem>
                                            <asp:ListItem Value="required">Required</asp:ListItem>
                                        </asp:RadioButtonList>
                                        <asp:DropDownList ID="ddlApprovers" runat="server" Enabled="false" />
                                    </div>
                                </td>
                            </tr>
                            <%--<tr>
                                <td>
                                    <label for="ddlApprovers">
                                        Approver:
                                    </label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlApprovers" runat="server">
                                    </asp:DropDownList>
                                </td>
                            </tr>--%>
                            <%--<tr>
                                <td>
                                    <label for="txtExpiry">
                                        Expiry Date:</label>
                                </td>
                                <td>
                                    <telerik:RadDatePicker runat="server" ID="rdpExpiry" CssClass="datePicker">
                                    </telerik:RadDatePicker>
                                </td>
                            </tr>--%>
                            <tr class="lessPad">
                                <td>
                                    <label for="chkListGroups">
                                        Acknowledgment:</label>
                                </td>
                                <td>
                                    <div id="rblStatus">
                                        <asp:RadioButtonList AutoPostBack="false" runat="server" ID="rblAcknowledge" RepeatDirection="Horizontal"
                                            BorderStyle="None" RepeatLayout="flow">
                                            <asp:ListItem Value="notRequired" Selected="True">Not Required</asp:ListItem>
                                            <asp:ListItem Value="required">Required</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td>
                                    <asp:Panel CssClass="statusBox" runat="server" ID="pnlStatusBox">
                                        Example:
                                        <asp:Panel ID="ackNotReqd" runat="server">
                                            <span class="notView">Not Viewed&nbsp;<asp:Label runat="server" ID="lblNotViewed"></asp:Label></span>
                                            <span class="view">Viewed&nbsp;<asp:Label runat="server" ID="lblViewed"></asp:Label></span>
                                        </asp:Panel>
                                        <asp:Panel ID="ackReqd" runat="server">
                                            <span class="notAck">Not Acknowledged&nbsp;<asp:Label runat="server" ID="lblNotAcked"></asp:Label></span>
                                            <span class="ack">Acknowledged&nbsp;<asp:Label runat="server" ID="lblAcked"></asp:Label></span>
                                        </asp:Panel>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                        <table class="editTable">
                            <tr runat="server" id="trEditDetails">
                                <td>
                                    <label for="rblEditDetails">
                                        Edit Details:</label>
                                </td>
                                <td>
                                    <asp:RadioButtonList runat="server" ID="rblEditDetails" RepeatDirection="Horizontal"
                                        RepeatLayout="Flow" BorderStyle="None">
                                        <asp:ListItem Text="Minor" Value="minor"></asp:ListItem>
                                        <asp:ListItem Text="Major" Value="major"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <comments:EditorialComments runat="server" ID="ctrlEditComments" />
                        </table>
                    </div>
                    <div class="clear">
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="newsTwoButtons">
            <div class="functionButtons approveFunctions" runat="server" id="divApprove">
                <h3>
                    Approve Changes</h3>
                <asp:Button runat="server" Text="Cancel" OnClick="btnCancel_Click" ID="btnCancelApprove"
                    CssClass="btn" />
                <asp:Button runat="server" Text="Edit" OnClick="btnEdit_Click" ID="btnEditApprove"
                    CssClass="btn" />
                <asp:Button runat="server" Text="Reject" OnClientClick="RejectClick(); return false;"
                    ID="btnReject" CssClass="btn" />
                <asp:Button runat="server" Text="Approve" OnClick="btnApprove_Click" ID="btnApprove"
                    CssClass="btn" />
            </div>
            <div class="functionButtons saveFunctions" runat="server" id="divSaveFunctions">
                <h3>
                    Save Changes</h3>
                <asp:Button ID="btnHold" runat="server" Text="Hold" CssClass="btn" OnClick="btnHold_Click" />
                <asp:Button ID="btnCheckin" runat="server" Text="Check In" CssClass="btn" OnClick="btnCheckin_Click" />
                <asp:Button ID="btnPublish" runat="server" Text="Publish" CssClass="btn" OnClick="btnPublish_Click" />
            </div>
            <div class="functionButtons actionFunctions" runat="server" id="divActionFunctions">
                <h3>
                    Actions</h3>
                <asp:Button ID="btnCancel2" runat="server" Text="Cancel" CssClass="btn" OnClick="btCancel_Click"
                    CausesValidation="false" />
                <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="btn" OnClientClick="javascript:BackClick(); return false;" />
                <asp:Button ID="btnCreatePublish" runat="server" Text="Create &amp; Publish" CssClass="btn"
                    OnClick="btnCreatePublish_Click" Visible="false" />
                <asp:Button ID="btnCreate" runat="server" Text="Create" CssClass="btn" OnClick="btnCreate_Click"
                    Visible="false" />
                <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn" OnClick="btnDelete_Click"
                    Visible="false" />
            </div>
        </div>
        <div class="versionNumber">
            <asp:HyperLink runat="server" ID="lnkVersionNumber"></asp:HyperLink>
        </div>
    </div>
</asp:Content>
