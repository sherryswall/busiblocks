<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PersonalDetails.ascx.cs"
    Inherits="Controls_PersonalDetails" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<telerik:RadCodeBlock runat="server" ID="radCodeBlock">
    <script type="text/javascript" language="javascript">
        var txtPasswordId = "<%=txtPassword.ClientID%>";
        var PasswordUnchangedText = "Password unchanged";
        var DictionaryPasswordChars = "abcdefghiklmnopqrstuvwxyz";
        var PasswordLength = 6;
        var userIdTouched = 0;

        $(document).ready(function () {
            userIdTouched = $('#<%= txtUserId.ClientID %>').val() == '' ? 0 : 1;
        });

        function generatePassword() {
            $("#" + txtPasswordId).val(randomString(PasswordLength));
        }

        function randomString(length) {
            var chars = DictionaryPasswordChars.split('');

            if (!length) {
                length = Math.floor(Math.random() * chars.length);
            }

            var str = '';
            for (var i = 0; i < length; i++) {
                str += chars[Math.floor(Math.random() * chars.length)];
            }
            return str;
        }

        function textBoxChanged() {
            var lastName = '<%= txtLastName.ClientID %>';
            var firstName = '<%= txtFirstName.ClientID %>';
            var userId = '<%= txtUserId.ClientID %>';
            $('#' + userId).text($('#' + firstName).val().replace(/\s+/gi, '') + $('#' + lastName).val().replace(/\s+/gi, ''));
        }

    </script>
</telerik:RadCodeBlock>
<asp:Table runat="server" ID="tblUserDetails" CssClass="standardTable minimum">
    <asp:TableHeaderRow>
        <asp:TableHeaderCell>User Name</asp:TableHeaderCell>
        <asp:TableHeaderCell>
            <asp:Label runat="server" ID="txtUserId" Width="210px"/>
        </asp:TableHeaderCell>
    </asp:TableHeaderRow>
    <asp:TableRow>
        <asp:TableCell>Last Name</asp:TableCell>
        <asp:TableCell CssClass="blankRight">
            <asp:TextBox ID="txtLastName" runat="server" Width="210px" onchange="textBoxChanged()" />
            <label style="color:Red">*</label>
        </asp:TableCell>
        <asp:TableCell class="blankCell">
            <asp:RequiredFieldValidator ID="requiredLastName" runat="server" ControlToValidate="txtLastName" style="float:left;position:absolute"
                ValidationGroup="userDetails" ErrorMessage="a name is required"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="validLastName" runat="server" ControlToValidate="txtLastName" style="float:left;position:absolute"
                ValidationGroup="userDetails" ErrorMessage="valid name required" ValidationExpression="^[a-zA-Z\s\'\-]{0,100}$"></asp:RegularExpressionValidator>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>First Name</asp:TableCell>
        <asp:TableCell CssClass="blankRight">
            <asp:TextBox ID="txtFirstName" runat="server" Width="210px" onchange="textBoxChanged()" />
            <label style="color:Red">*</label></asp:TableCell>
        <asp:TableCell class="blankCell">
            <asp:RequiredFieldValidator ID="requiredFirstName" runat="server" ControlToValidate="txtFirstName" style="float:left;position:absolute"
                ValidationGroup="userDetails" ErrorMessage="a name is required"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="validFirstName" runat="server" ControlToValidate="txtFirstName" style="float:left;position:absolute"
                ValidationGroup="userDetails" ErrorMessage="valid name required" ValidationExpression="^[a-zA-Z\s\'\-]{0,100}$"></asp:RegularExpressionValidator>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>Position</asp:TableCell>
        <asp:TableCell CssClass="blankRight">
            <asp:TextBox ID="txtPosition" runat="server" Width="210px" /></asp:TableCell>
        <asp:TableCell class="blankCell">
            <asp:RegularExpressionValidator ID="validPosition" runat="server" ControlToValidate="txtPosition" 
                ValidationGroup="userDetails" ErrorMessage="valid position required" ValidationExpression="^[a-zA-Z\s&()\-\']{0,50}$"></asp:RegularExpressionValidator>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>Work Phone</asp:TableCell>
        <asp:TableCell CssClass="blankRight">
            <asp:TextBox ID="txtWorkPhone" runat="server" Width="210px" />
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>Work Mobile</asp:TableCell>
        <asp:TableCell CssClass="blankRight">
            <asp:TextBox ID="txtWorkMobile" runat="server" Width="210px" /></asp:TableCell>
        <asp:TableCell class="blankCell">
            <asp:RegularExpressionValidator ID="validWorkMobile" runat="server" ControlToValidate="txtWorkMobile"
                ValidationGroup="userDetails" ErrorMessage="valid mobile number required" ValidationExpression="^\+?(?:[()]?[\s-]?[0-9]){10,12}$"></asp:RegularExpressionValidator>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>Work Fax</asp:TableCell>
        <asp:TableCell CssClass="blankRight">
            <asp:TextBox ID="txtWorkFax" runat="server" Width="210px" />
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>Work Email</asp:TableCell>
        <asp:TableCell CssClass="blankRight">
            <asp:TextBox ID="txtWorkEmail" runat="server" Width="210px" />
            <label style="color:Red">*</label>
            </asp:TableCell>
        <asp:TableCell class="blankCell">
            <asp:RequiredFieldValidator ID="requiredWorkEmail" runat="server" ControlToValidate="txtWorkEmail" style="float:left;position:absolute"
                ValidationGroup="userDetails" ErrorMessage="an email is required"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="validWorkEmail" runat="server" ControlToValidate="txtWorkEmail" style="float:left;position:absolute"
                ValidationGroup="userDetails" ErrorMessage="valid email address required" ValidationExpression="^([a-zA-Z0-9\.-]+@[a-zA-Z0-9\.-]+[\.][a-zA-Z0-9\.-]+){1,70}$"></asp:RegularExpressionValidator>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>New Password</asp:TableCell>
        <asp:TableCell CssClass="blankRight">
            <asp:TextBox ID="txtPassword" Text="" runat="server"/>
            <asp:Label runat="server" ID="lblAsteriskPwd" Text="*" ForeColor="Red"></asp:Label>
            <asp:Button ID="btnGeneratePassword" runat="server" Text="Random" CssClass="btn"
                UseSubmitBehavior="false" OnClientClick="javascript:generatePassword(); return false;" />
        </asp:TableCell>
        <asp:TableCell class="blankCell">
            <asp:RequiredFieldValidator ID="requiredPassword" runat="server" ControlToValidate="txtPassword"
                ValidationGroup="userDetails" ErrorMessage="a password is required"></asp:RequiredFieldValidator>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
