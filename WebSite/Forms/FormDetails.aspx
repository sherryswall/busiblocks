<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="FormDetails.aspx.cs" Inherits="FormDetails" Title="Form Details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <h1>
        Form details</h1>
    <div>
    <label ID="lblFormDefTitle" runat="server" width="100"/>
        <asp:Repeater ID="formPropertyList" runat="server">
            <HeaderTemplate>
                <table border="1" width="100%">
                    
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <%# Server.HtmlEncode((string)Eval("Name")) %>
                    </td>
                    <td><asp:TextBox ID="txtName" runat="server" MaxLength="100"></asp:TextBox></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>

        <p>
            <asp:Button ID="btSave" runat="server" Text="Submit" CssClass="btn" OnClick="btSave_Click" />
            <asp:Button ID="btCancel" runat="server" Text="Cancel" CssClass="btn" OnClick="btCancel_Click" CausesValidation="false" />
        </p>
    </div>
</asp:Content>
