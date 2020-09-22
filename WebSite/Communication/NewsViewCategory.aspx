<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="NewsViewCategory.aspx.cs"
    Inherits="Communication_NewsViewCategory" Title="View category" %>

<%@ Register TagPrefix="uc" TagName="NewsList" Src="NewsList.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <h1>
        Category: <span id="lblDisplayName" runat="server"></span>
    </h1>
    <!-- Permissions View -->
    <table class="permissionsTable">
        <tr>
            <th>
                <asp:Label Width="150px" runat="server" Text="Parent Category"></asp:Label>
            </th>
            <th>
                <asp:Label Width="150px" Text="Visible to:" runat="server" valign="top" /><br />
            </th>
            <th>
                <asp:Label Width="150px" Text="Editable by:" runat="server" /><br />
            </th>
        </tr>
        <tr>
            <td>
                <asp:Label runat="server" ID="lblParentCategoryName"></asp:Label><br />
            </td>
            <td>
                <asp:Repeater ID="lstSummaryViewing" runat="server">
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
    <uc:NewsList ID="list" runat="server" />
</asp:Content>
