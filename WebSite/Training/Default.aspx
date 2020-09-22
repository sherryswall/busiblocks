<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Training_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolder" Runat="Server">
<h1 class="sectionhead" id="training">Training</h1>
<h2>Training Categories</h2>
    <div>
        <p>
            <a class="newitem" id="lnkAddNew" runat="server">New Category</a>    
            <a class="search" id="linkSearch" runat="server">Search</a>    
        </p>
    
                <ul>
                <li>
                    <a href="#">Staff Induction</a> - Training for new staff
                    </li>
                    <li>
                    <a href="Training.aspx">Safety Training</a> - Staff safety training
                </li>
                </ul>
    </div>
</asp:Content>

