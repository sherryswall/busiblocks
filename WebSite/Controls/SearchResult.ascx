<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SearchResult.ascx.cs" Inherits="Controls_SearchResult" %>

<div class="pageNavigation">
    <span id="lblCurrentPage" runat="server" /> of 
        <span id="lblTotalPage" runat="server" />
    <asp:LinkButton ID="linkPrev" runat="server" CssClass="previous" OnClick="linkPrev_Click">Prev</asp:LinkButton>
    <asp:LinkButton ID="linkNext" runat="server" CssClass="next" OnClick="linkNext_Click">Next</asp:LinkButton>
</div>

<asp:Repeater ID="listRepeater" runat="server">
    <ItemTemplate>
        <div class="searchBox">
            <a href="<%# GetViewUrl((BusiBlocks.ISearchResult)Container.DataItem ) %>">
                <h3 class="searchTitle"><%# HttpUtility.HtmlEncode(((BusiBlocks.ISearchResult)Container.DataItem).Title)%></h3>
            </a>
            
            <div class="searchBody"><%# HttpUtility.HtmlEncode(((BusiBlocks.ISearchResult)Container.DataItem).Description)%></div>
            
            <small class="searchFooter"><%# HttpUtility.HtmlEncode(((BusiBlocks.ISearchResult)Container.DataItem).Category)%> -
             <%# Utilities.GetDateTimeForDisplay(((BusiBlocks.ISearchResult)Container.DataItem).Date)%></small>
        </div>
    </ItemTemplate>
</asp:Repeater>
