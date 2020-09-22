<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="NewForm.aspx.cs" Inherits="Manage_NewForm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolder" runat="Server">

    <h1>Weekly Profit Report</h1>
    <table  class="standardTable">
        <tr>
            <td>
                <label for="txtWeekEnding">
                    Week Ending:</label>
            </td>
            <td colspan="3">
                <asp:TextBox ID="txtWeekEnding" runat="server"></asp:TextBox>
             
            </td>
        </tr>
        <tr>
            <td> <label for="txtTotalSales">
                    (G) Sales:</label></td>
            <td>
                </td>
            <td></td>
            <td>$<asp:TextBox ID="txtTotalSales" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td> <label for="txtFoodCostPerc">
                    (F) Food Cost:</label></td>
            <td>(<asp:TextBox ID="txtFoodCostPerc" runat="server"></asp:TextBox>%)
                </td>
            <td>$<asp:TextBox ID="txtFoodCost" runat="server"></asp:TextBox></td>
            <td></td>
        </tr>
        <tr>
            <td valign="top"> <label for="txtGrossWages">
                    Gross Wages:<br /><small>(Excluding Operator)</small></label></td>
            <td valign="top">(<asp:TextBox ID="txtGrossWagesPrec" runat="server"></asp:TextBox>%)
                </td>
            <td valign="top">$<asp:TextBox ID="txtGrossWages" runat="server"></asp:TextBox></td>
            <td></td>
        </tr>
        <tr>
            <td valign="top"> <label for="txtGrossWages">
                    Rent:<br /><small>(Including Rates)</small></label></td>
            <td valign="top">(<asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>%)
                </td>
            <td valign="top">$<asp:TextBox ID="TextBox2" runat="server"></asp:TextBox></td>
            <td></td>
        </tr>
        <tr>
            <td> <label for="txtGrossWages">
                    Franchisee Service Fee:</label></td>
            <td>(<asp:TextBox ID="TextBox3" runat="server"></asp:TextBox>%)
                </td>
            <td>$<asp:TextBox ID="TextBox4" runat="server"></asp:TextBox></td>
            <td></td>
        </tr>
        <tr>
            <td> <label for="txtGrossWages">
                    Advertising:</label></td>
            <td>(<asp:TextBox ID="TextBox5" runat="server"></asp:TextBox>%)
                </td>
            <td>$<asp:TextBox ID="TextBox6" runat="server"></asp:TextBox></td>
            <td></td>
        </tr>
        <tr>
            <td style="border-bottom: 1px solid black" valign="top"> <label for="txtGrossWages">
            
                    Overheads:<br />
                    Accounting Fees<br />
Bank Fees and Interest<br />
Ceaning<br />
Insurance<br />
Light and Power<br />
Postage<br />
Reg and Licences<br />
Repairs and Maintenance<br />
Telephone<br />
Uniforms</small>

                    </label></td>
            <td style="border-bottom: 1px solid black" valign="top">(<asp:TextBox ID="TextBox7" runat="server"></asp:TextBox>%)
                </td>
            <td style="border-bottom: 1px solid black" valign="top">$<asp:TextBox ID="TextBox8" runat="server"></asp:TextBox></td>
            <td style="border-bottom: 1px solid black"></td>
        </tr>
        <tr>
            <td> <label for="txtGrossWages">
                    Total Overheads PA:</label></td>
            <td>
                </td>
            <td>$<asp:TextBox ID="TextBox10" runat="server"></asp:TextBox></td>
            <td></td>
        </tr>
        <tr>
            <td style="border-bottom: 1px double black"> <label for="txtGrossWages">
                    Divide 52:</label></td>
            <td style="border-bottom: 1px double black">
                </td>
            <td style="border-bottom: 1px double black">$<asp:TextBox ID="TextBox9" runat="server"></asp:TextBox></td>
            <td style="border-bottom: 1px double black"></td>
        </tr>
        <tr>
            <td> <label for="txtGrossWages">
                    (H) Total Expenses:</label></td>
            <td>
                </td>
            <td></td>
            <td>$<asp:TextBox ID="TextBox11" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td> <label for="txtGrossWages">
                    (I) Profit (G-H):</label></td>
            <td>(<asp:TextBox ID="TextBox13" runat="server"></asp:TextBox>%)
                </td>
            <td></td>
            <td>$<asp:TextBox ID="TextBox12" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td> <label for="txtGrossWages">
                    (J) Leasing or Loans:</label></td>
            <td>
                </td>
            <td>$<asp:TextBox ID="TextBox14" runat="server"></asp:TextBox></td>
            <td></td>
        </tr>
        <tr>
            <td> <label for="txtGrossWages">
                    Net Profit (I-J):</label></td>
            <td>(<asp:TextBox ID="TextBox15" runat="server"></asp:TextBox>%)
                </td>
            <td></td>
            <td>$<asp:TextBox ID="TextBox16" runat="server"></asp:TextBox></td>
        </tr>
    </table>
    <asp:Button ID="Button1" runat="server" Text="Clear" CssClass="btn" />
    <asp:Button ID="Button2" runat="server" Text="Save" CssClass="btn" />
    <asp:Button ID="Button3" runat="server" Text="Submit" CssClass="btn" />

</asp:Content>
