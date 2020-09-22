<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddVisibility.ascx.cs"
    Inherits="Controls_AddVisibility" %>
<h4>
    Visibililty</h4>
<table  class="standardTable">
    <tr>
        <td valign="top">
            <div style="border: 1px solid black">
                Select Group:
                <br />
                <asp:CheckBox ID="chkAllGroups" Text="All Groups" runat="server" AutoPostBack="True"
                    OnCheckedChanged="chkAllGroups_CheckedChanged" />
                <asp:RadioButtonList ID="rdoListGroups" runat="server">
                </asp:RadioButtonList>
                Select Location:<br />
                <asp:CheckBox ID="chkAllLocations" Text="All Sites" runat="server" AutoPostBack="True"
                    OnCheckedChanged="chkAllLocations_CheckedChanged" />
                <asp:RadioButtonList ID="rdoListLocations" runat="server">
                </asp:RadioButtonList>
                <br />
                <asp:Button Text="Add Group/Location" ID="btnAddGroupLocation" runat="server" CssClass="btn"
                    OnClick="btnAddGroupLocation_Click" CausesValidation="false" />
            </div>
            <div style="border: 1px solid black">
                Select Users:<br />
                <asp:CheckBox Text="All Users" ID="chkAllUsers" runat="server" AutoPostBack="True"
                    OnCheckedChanged="chkAllUsers_CheckedChanged" />
                <asp:CheckBoxList ID="chkListUsers" runat="server">
                </asp:CheckBoxList>
                <br />
                <asp:Button Text="Add User" ID="btnAddUser" runat="server" CssClass="btn" OnClick="btnAddUser_Click"
                    CausesValidation="false" />
            </div>
        </td>
        <td valign="top">
            <div style="border: 1px solid black">
                This item will be visible to the following:<br />
                <asp:ListBox Height="250px" ID="lstSummary" runat="server" Width="238px"></asp:ListBox>
                <br />
                <asp:Button Text="Remove" ID="btnRemove" runat="server" CssClass="btn" OnClick="btnRemove_Click" />
            </div>
        </td>
    </tr>
</table>
<hr />
<h4>
    Editing</h4>
<table  class="standardTable">
    <tr>
        <td valign="top">
            <div style="border: 1px solid black">
                Select Group:
                <br />
                <asp:CheckBox ID="chkAllGroups2" Text="All Groups" runat="server" AutoPostBack="True"
                    OnCheckedChanged="chkAllGroups2_CheckedChanged" />
                <asp:RadioButtonList ID="rdoListGroups2" runat="server">
                </asp:RadioButtonList>
                Select Location:<br />
                <asp:CheckBox ID="chkAllLocations2" Text="All Sites" runat="server" AutoPostBack="True"
                    OnCheckedChanged="chkAllLocations2_CheckedChanged" />
                <asp:RadioButtonList ID="rdoListLocations2" runat="server">
                </asp:RadioButtonList>
                <br />
                <asp:Button Text="Add Group/Location" ID="btnAddGroupLocation2" runat="server" CssClass="btn"
                    OnClick="btnAddGroupLocation2_Click" CausesValidation="false" />
            </div>
            <div style="border: 1px solid black">
                Select Users:<br />
                <asp:CheckBox Text="All Users" ID="chkAllUsers2" runat="server" AutoPostBack="True"
                    OnCheckedChanged="chkAllUsers2_CheckedChanged" />
                <asp:CheckBoxList ID="chkListUsers2" runat="server">
                </asp:CheckBoxList>
                <br />
                <asp:Button Text="Add User" ID="Button2" runat="server" CssClass="btn" OnClick="btnAddUser2_Click"
                    CausesValidation="false" />
            </div>
        </td>
        <td valign="top">
            <div style="border: 1px solid black">
                This item will be editable by the following:<br />
                <asp:ListBox Height="250px" ID="lstSummary2" runat="server" Width="238px"></asp:ListBox>
                <br />
                <asp:Button Text="Remove" ID="btnRemove2" runat="server" CssClass="btn" OnClick="btnRemove2_Click" />
            </div>
        </td>
    </tr>
</table>
