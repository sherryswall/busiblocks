<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FeedbackForm.ascx.cs"
    Inherits="Controls_FeedbackForm" %>
<%@ Register Src="~/Controls/ModalPopup.ascx" TagName="ModalPopup" TagPrefix="uc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:UpdatePanel runat="server" ID="pnlFeedback" UpdateMode="Always">
    <ContentTemplate>
        <script language="javascript" type="text/javascript">
            var btnSubmitHidden = "<%=btnSubmitHidden.ClientID %>";
            function closeThanks() {
                popFeedbackThanks.Hide();
                $('#' + btnSubmitHidden).click();
            }
            function showThanks() {
                var isPageValid = true;
                isPageValid = Page_ClientValidate();

                if (isPageValid) {
                    popFeedback.Hide();
                    popFeedbackThanks.Show();
                }
                return false;
            }
        </script>
        <p>
            BusiBlocks is constantly striving to improve their management software. By including
            your feedback below you are helping to make our product better for you.
        </p>
        <table class="loginTable">
            <tr>
                <td>
                    <label for="ddlFeedbackType">
                        Type of Feedback</label>
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="ddlFeedbackType" Width="300px">
                        <asp:ListItem Text="--Feedback Type--" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="Problem" Value="Problem"></asp:ListItem>
                        <asp:ListItem Text="Suggestion" Value="Suggestion"></asp:ListItem>
                        <asp:ListItem Text="General" Value="General"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlFeedbackType" ID="rfvDDL"
                        ValidationGroup="feedbackFormValGroup" InitialValue="--Feedback Type--" Text="*"
                        ForeColor="Red"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>
                    <label for="txtSubject">
                        Subject</label>
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtSubject" Width="300px"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtSubject" ID="rfvSubject"
                        ValidationGroup="feedbackFormValGroup" Text="*" ForeColor="Red"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <label for="txtComments">
                        Comments</label>
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtComments" TextMode="MultiLine" Height="120px"
                        Width="300px"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtComments" ID="rfvComments"
                        ValidationGroup="feedbackFormValGroup" Text="*" ForeColor="Red"></asp:RequiredFieldValidator>
                </td>
            </tr>
        </table>
        <br />
        <center>
            <asp:Button runat="server" CssClass="btn" ID="btnSubmit" Text="Submit" ValidationGroup="feedbackFormValGroup"
                OnClientClick="return showThanks();" CausesValidation="true" />
            <asp:Button runat="server" CssClass="hideElement" ID="btnSubmitHidden" OnClick="btnSubmit_OnClick" />
        </center>
        <uc1:ModalPopup runat="server" ID="popFeedbackThanks" Title="Feedback" OnClientAcceptClick="closeThanks();"
            AcceptButtonText="Close">
            <FormTemplateContainer>
                <p>
                    An email has been sent to our support team to review. Thank you for helping to improve
                    our software.
                </p>
            </FormTemplateContainer>
        </uc1:ModalPopup>
    </ContentTemplate>
</asp:UpdatePanel>
