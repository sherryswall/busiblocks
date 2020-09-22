<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Setup.aspx.cs" Inherits="Setup" Theme="" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>BusiBlocks database setup</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>Setup</h1>
            
            <strong><span id="lblStatus" runat="server"></span></strong>
            
            <h2>Database configuration</h2>
            <p>Use this setup to create the required database schema.
            Select the connection string:</p>
            <asp:DropDownList ID="cmbConnections" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbConnections_SelectedIndexChanged"></asp:DropDownList>
            
            <p>Schema sections:</p>
            <asp:CheckBoxList ID="list" runat="server">
            </asp:CheckBoxList>

            <p>Use the 'Check status' button to check the status of the database tables. Use the 'Create schema' button to regenerate the selected tables.</p>

            <p>
                <asp:Button ID="btCheckStatus" runat="server" Text="Check status" OnClick="btCheckStatus_Click" />
                <asp:Button ID="btCreate" runat="server" Text="Create schema" OnClick="btCreate_Click" 
                   OnClientClick="return confirm('Are you sure to generate the schema? WARNING: If a table already exists its content will be deleted.');" />
            </p>

            <h2>Administrator user</h2>
            <p>Click 'Create admin user' to create the administrator user with the following configuration:</p>
            <table  class="standardTable">
                <tr>
                    <td>User: </td>
                    <td><asp:TextBox ID="txtAdminUser" runat="server" Text="admin"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Password:</td>
                    <td><asp:TextBox ID="txtAdminPassword" runat="server" TextMode="Password"></asp:TextBox> (required)</td>
                </tr>
                <tr>
                    <td>EMail:</td>
                    <td><asp:TextBox ID="txtAdminEMail" runat="server"></asp:TextBox> (required if you have configured requiresUniqueEmail=true)</td>
                </tr>
                <tr>
                    <td>Role:</td>
                    <td><asp:TextBox ID="txtAdminRole" runat="server" Text="core:administrator"></asp:TextBox></td>
                </tr>                
            </table>
            <p>
                <asp:Button ID="btCreateAdmin" runat="server" Text="Create admin user" OnClick="btCreateAdmin_Click" />
            </p>

        </div>
    </form>
</body>
</html>
