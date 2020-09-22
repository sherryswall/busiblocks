<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CategoryPermissionMatrix.ascx.cs"
    Inherits="Controls_CategoryPermissionMatrix" %>
<!-- Permissions View -->
<table class="permissionsTable">
    <tr class="primary">
        <th>
            <asp:Label ID="Label1" runat="server" Text="Category"></asp:Label>:
        </th>
        <td colspan="2">
            <asp:Label runat="server" ID="lblCategoryName"></asp:Label><br />
        </td>
    </tr>
    <tr>
        <th>
            <asp:Label ID="Label2" Text="Visible to:" runat="server" valign="top" /><br />
        </th>
        <th>
            <asp:Label ID="Label4" Text="Contributable by:" runat="server" valign="top" /><br />
        </th>
        <th>
            <asp:Label ID="Label3" Text="Editable by:" runat="server" /><br />
        </th>
    </tr>
    <tr>
        <td>
            <asp:Repeater ID="lstSummaryViewing" runat="server">
                <ItemTemplate>
                    <%# Container.DataItem %>
                    <br />
                </ItemTemplate>
            </asp:Repeater>
        </td>
        <td>
            <asp:Repeater ID="lstSummaryContributing" runat="server">
                <ItemTemplate>
                    <%# Container.DataItem %>
                    <br />
                </ItemTemplate>
            </asp:Repeater>
        </td>
        <td>
            <asp:Repeater ID="lstSummaryEditing" runat="server">
                <ItemTemplate>
                    <%# Container.DataItem %>
                    <br />
                </ItemTemplate>
            </asp:Repeater>
        </td>
    </tr>
</table>
