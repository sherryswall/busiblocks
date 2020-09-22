<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DefaultForums.ascx.cs" Inherits="Communication_DefaultForums" %>

<h1 class="sectionhead" id="forums">Forums</h1>
<h2>Forum List</h2>
<div>
    <p>
        <a class="newitem" id="lnkAddNew" runat="server">New Forum</a>    
        <a class="search" id="linkSearch" runat="server">Search</a>    
    </p>
    
    <asp:Repeater ID="listRepeater" runat="server">
        <HeaderTemplate>
            <ul>
        </HeaderTemplate>
        <ItemTemplate>
            <li>
                <a href="<%# GetForumLink((string)Eval("Name")) %>"><%# Server.HtmlEncode((string)Eval("DisplayName")) %></a> <%# ((string)Eval("Description")).Length > 0 ? "-": string.Empty %> <%# Server.HtmlEncode((string)Eval("Description")) %>
            </li>
        </ItemTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>
    </asp:Repeater>
</div>