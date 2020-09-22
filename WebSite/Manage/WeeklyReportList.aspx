<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="WeeklyReportList.aspx.cs" Inherits="Manage_WeeklyReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolder" Runat="Server">
<h1>Weekly Profit Reports</h1>
<p>
        <a class="newitem" id="linkNew" runat="server" href="NewForm.aspx">New Profit Report</a>
        </p>
<table  class="standardTable">
    <thead>
        <tr>
            <th>Week Ending</th>
            <th>Status</th>
            
        </tr>
    </thead>
    <tbody>
    <tr>
    <td><a href="">28/01/2011</a></td><td>Saved</td>
    </tr>
    <tr>
    <td><a href="">21/01/2011</a></td><td>Submitted</td>
    </tr>
    <tr>
    <td><a href="">14/01/2011</a></td><td>Submitted</td>
    </tr>
    <tr>
    <td><a href="">07/01/2011</a></td><td>Submitted</td>
    </tr>
    </tbody>
    </table>

</asp:Content>

