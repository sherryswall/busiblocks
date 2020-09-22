<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Manage_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolder" Runat="Server">
    <h1 class="sectionhead" id="management">Manage</h1>
    <h2>Report Categories</h2>
    <div>
        <p>
            <a class="newitem" id="lnkAddNew" runat="server">New Category</a>    
            <a class="search" id="linkSearch" runat="server">Search</a>    
        </p>
    
                <ul>
                <li>
                    <a href="WeeklyReportList.aspx">Weekly Profit Report</a> - Weekly  Reports
                </li>
                </ul>
    </div>
</asp:Content>

