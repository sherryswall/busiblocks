<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ChangeUserInfo.aspx.cs"
    Inherits="User_ChangeUserInfo" Title="Change user settings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <h1>
        User settings</h1>
    <h2>
        Personal Details</h2>
    <table  class="standardTable">
        <tr>
            <td>
                <label for="ddlTitle">
                    Title:</label>
            </td>
            <td>
                <asp:DropDownList ID="ddlTitle" runat="server">
                    <asp:ListItem>--Select One--</asp:ListItem>
                    <asp:ListItem>Dr</asp:ListItem>
                    <asp:ListItem>Mr</asp:ListItem>
                    <asp:ListItem>Mrs</asp:ListItem>
                    <asp:ListItem>Miss</asp:ListItem>
                    <asp:ListItem>Ms</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <label for="txtLastName">
                    Last Name:</label>
            </td>
            <td>
                <asp:TextBox ID="txtLastName" runat="server" MaxLength="100"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <label for="txtOtherNames">
                    Other Names:</label>
            </td>
            <td>
                <asp:TextBox ID="txtOtherNames" runat="server" MaxLength="100"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <label for="txtPreferredName">
                    Preferred Name:</label>
            </td>
            <td>
                <asp:TextBox ID="txtPreferredName" runat="server" MaxLength="100"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <label for="txtDateOfBirth">
                    Date of Birth:</label>
            </td>
            <td>
                <asp:TextBox ID="txtDateOfBirth" runat="server" MaxLength="100"></asp:TextBox>
            </td>
        </tr>
    </table>
    <h2>
        Location</h2>
    <table  class="standardTable">
        <tr>
            <td>
                <label for="txtPostalAddress">
                    Location(s):</label>
            </td>
            <td>
                <asp:CheckBoxList ID="chkListLocations" runat="server">
                </asp:CheckBoxList>
            </td>
        </tr>
    </table>
    <h2>
        Address</h2>
    <table  class="standardTable">
        <tr>
            <td>
                <label for="txtPostalAddress">
                    Postal Address:</label>
            </td>
            <td>
                <asp:TextBox ID="txtPostalAddress" runat="server" MaxLength="100"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <label for="txtPostalCity">
                    City/Suburb:</label>
            </td>
            <td>
                <asp:TextBox ID="txtPostalCity" runat="server" MaxLength="100"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <label for="ddlPostalState">
                    State:</label>
            </td>
            <td>
                <asp:DropDownList ID="ddlPostalState" runat="server">
                    <asp:ListItem>--Select One--</asp:ListItem>
                    <asp:ListItem>ACT</asp:ListItem>
                    <asp:ListItem>NSW</asp:ListItem>
                    <asp:ListItem>NT</asp:ListItem>
                    <asp:ListItem>QLD</asp:ListItem>
                    <asp:ListItem>SA</asp:ListItem>
                    <asp:ListItem>TAS</asp:ListItem>
                    <asp:ListItem>VIC</asp:ListItem>
                    <asp:ListItem>WA</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <label for="txtPostCode">
                    Post Code:</label>
            </td>
            <td>
                <asp:TextBox ID="txtPostCode" runat="server" MaxLength="100"></asp:TextBox>
            </td>
        </tr>
    </table>
    <h2>
        Contact Details</h2>
    <table  class="standardTable">
        <tr>
            <td>
                <label for="txtPhoneBH">
                    Phone BH:</label>
            </td>
            <td>
                <asp:TextBox ID="txtPhoneBH" runat="server" MaxLength="100"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <label for="txtPhoneAH">
                    Phone AH:</label>
            </td>
            <td>
                <asp:TextBox ID="txtPhoneAH" runat="server" MaxLength="100"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <label for="txtPhoneMobile">
                    Phone Mobile:</label>
            </td>
            <td>
                <asp:TextBox ID="txtPhoneMobile" runat="server" MaxLength="100"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <label for="txtFax">
                    Fax:</label>
            </td>
            <td>
                <asp:TextBox ID="txtFax" runat="server" MaxLength="100"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <label for="txtEMail">
                    E-Mail:</label>
            </td>
            <td>
                <asp:TextBox ID="txtEMail" runat="server" MaxLength="100"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <label for="chkReceiveNotification">
                    Receive notifications:</label>
            </td>
            <td>
                <asp:CheckBox ID="chkReceiveNotification" runat="server"></asp:CheckBox>
            </td>
        </tr>
    </table>
    <asp:Panel ID="pnlManageGroups" runat="server" Visible="false">
        <h2>
            Groups I watch</h2>
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
                        This following groups will be monitored:<br />
                        <asp:ListBox Height="250px" ID="lstSummary" runat="server" Width="238px"></asp:ListBox>
                        <br />
                        <asp:Button Text="Remove" ID="btnRemove" runat="server" CssClass="btn" OnClick="btnRemove_Click" />
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <p>
        <asp:Button ID="btSave" runat="server" Text="Save" CssClass="btn" OnClick="btSave_Click" />
        <asp:Button ID="btCancel" runat="server" Text="Cancel" CssClass="btn" OnClick="btCancel_Click"
            CausesValidation="false" />
    </p>
</asp:Content>
