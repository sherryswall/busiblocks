<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="CreateUser.aspx.cs" Inherits="CreateUser" Title="Register user" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" Runat="Server">
    
    <h1>Create new user</h1>
    
    <asp:CreateUserWizard ID="ctlCreateUserWizard" runat="server" CancelDestinationPageUrl="~/Default.aspx" ContinueDestinationPageUrl="~/Default.aspx">
        <WizardSteps>
            <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server">
            </asp:CreateUserWizardStep>
            <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server">
            </asp:CompleteWizardStep>
        </WizardSteps>
    </asp:CreateUserWizard>
    
    <p>
        <small>
            * The user name must be a 'unix' style name, cannot contains special characters or spaces.<br />
              To receive automatic notifications from the website (like forum notifications) you must insert a valid e-mail. You can insert or change the e-mail using the settings page of the website ('My settings').
        </small>
    </p>    
</asp:Content>

