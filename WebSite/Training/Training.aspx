<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="Training.aspx.cs" Inherits="Training_training" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <h1>
        Category: Safety Training</h1>
    <p id="lblDescription" runat="server">
        Staff safety training
    </p>
    <p>
        <a class="newitem" id="linkNew" runat="server" href="#">New Training</a>
    </p>
    <table  class="standardTable">
        <thead>
            <tr>
                <th>
                    Title
                </th>
                <th>
                    Author
                </th>
                <th>
                    Date
                </th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>
                    <a href="#" onclick="javascript:window.open('CleanDesk/DESK.html','_blank','width=964,height=400,toolbar=no,titlebar=yes,location=no,menubar=no,resizable=yes,status=no')">
                        Clean Desk Requirements</a>
                </td>
                <td>
                    admin
                </td>
                <td>
                    20/01/2011
                </td>
            </tr>
            <tr>
                <td>
                    <a href="#" onclick="javascript:window.open('OHSDesk/OHS.html','_blank','width=964,height=400,toolbar=no,titlebar=yes,location=no,menubar=no,resizable=yes,status=no')">
                        OH&S Workstation Setup</a>
                </td>
                <td>
                    admin
                </td>
                <td>
                    20/01/2011
                </td>
            </tr>
        </tbody>
    </table>
</asp:Content>
