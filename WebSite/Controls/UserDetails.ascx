<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UserDetails.ascx.cs" Inherits="Controls_UserDetails" %>
<asp:Table runat="server" ID="tblUserDetails" CssClass="standardTable minimum">
    <asp:TableHeaderRow>
        <asp:TableHeaderCell>User Name</asp:TableHeaderCell>
        <asp:TableHeaderCell>
            <asp:Label runat="server" ID="lblUserId" Width="210px"></asp:Label>
        </asp:TableHeaderCell>
    </asp:TableHeaderRow>
    <asp:TableRow>
        <asp:TableCell>Last Name</asp:TableCell>
        <asp:TableCell CssClass="blankRight">
            <asp:Label ID="lblLastName" runat="server" Width="210px" />
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>First Name</asp:TableCell>
        <asp:TableCell CssClass="blankRight">
            <asp:Label ID="lblFirstName" runat="server" Width="210px" />
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>Position</asp:TableCell>
        <asp:TableCell CssClass="blankRight">
            <asp:Label ID="lblPosition" runat="server" Width="210px" />
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>Work Phone</asp:TableCell>
        <asp:TableCell CssClass="blankRight">
            <asp:Label ID="lblWorkPhone" runat="server" Width="210px" />
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>Work Mobile</asp:TableCell>
        <asp:TableCell CssClass="blankRight">
            <asp:Label ID="lblWorkMobile" runat="server" Width="210px" />
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>Work Fax</asp:TableCell>
        <asp:TableCell CssClass="blankRight">
            <asp:Label ID="lblWorkFax" runat="server" Width="210px" />
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>Work Emails</asp:TableCell>
        <asp:TableCell CssClass="blankRight">
            <asp:Label ID="lblWorkEmail" runat="server" Width="210px" />
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow CssClass="">
        <asp:TableCell>Sites</asp:TableCell>
        <asp:TableCell CssClass="blankRight">
            <asp:Label runat="server" ID="lblSites"></asp:Label>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow CssClass="">
        <asp:TableCell>Regions</asp:TableCell>
        <asp:TableCell CssClass="blankRight">
            <asp:Label runat="server" ID="lblRegions"></asp:Label>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow CssClass="">
        <asp:TableCell>Groups</asp:TableCell>
        <asp:TableCell CssClass="blankRight">
            <asp:Label runat="server" ID="lblGroups"></asp:Label>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow CssClass="">
        <asp:TableCell>Admin</asp:TableCell>
        <asp:TableCell CssClass="blankRight">
            <asp:Label runat="server" ID="lblIsAdmin"></asp:Label>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
