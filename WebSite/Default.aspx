<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">

    <script src="jquery/jquery.min.js" type="text/javascript" ></script>
    <script type="text/javascript">

        $(document).ready(function () {
            var $username = $('#ctl00_contentPlaceHolder_LoginView_Login_UserName');
            if ($username != null) {
                $username.focus();
            }
        });

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <h1>
        Login</h1>
    <asp:LoginView ID="LoginView" runat="server">
        <AnonymousTemplate>
            <div class="loginTable">
                <asp:Login ID="Login" runat="server" DisplayRememberMe="False" RememberMeSet="False"
                    DestinationPageUrl="~/User/Dashboard.aspx" TitleText="" OnLoggedIn="onLoggedIn">
                    <LoginButtonStyle CssClass="btn" />
                    <%--                            <asp:Login ID="Login" CssClass="formTable" runat="server"
                    DestinationPageUrl="~/User/Dashboard.aspx" TitleText="" PasswordRecoveryText="Lost password?" PasswordRecoveryUrl="~/PasswordRecovery.aspx">--%>
                </asp:Login>
            </div>
        </AnonymousTemplate>
        <LoggedInTemplate>
            <span class="welcome">
                Welcome
                <asp:LoginName ID="LoginName1" runat="server" />
            </span>
        </LoggedInTemplate>
    </asp:LoginView>
    <div class="splashbutton">
        <a href="Training/logon.html" target="_blank" class="helpme"><span>Logon</span></a>
    </div>
    <div class="splash">
    </div>
</asp:Content>
