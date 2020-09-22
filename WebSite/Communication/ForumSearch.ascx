<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ForumSearch.ascx.cs" Inherits="Communication_ForumSearch" %>

<%@ Register TagPrefix="uc" TagName="SearchResult" Src="~/Controls/SearchResult.ascx" %>

    <h1>Forum Search</h1>
    <a runat="server" id="lnkBack">Back To Forums</a>
    <h2>Search parameters:</h2>
    <table  class="standardTable">
        <tr>
            <td><label for="txtSearchFor">Search for:</label></td>
            <td>
                <asp:TextBox ID="txtSearchFor" runat="server" Columns="40"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td><label for="txtAuthor">Author:</label></td>
            <td>
                <asp:TextBox ID="txtAuthor" runat="server" Columns="40"></asp:TextBox>
            </td>
        </tr>
    </table>
    
    <h2>Search on these forums:</h2>
    <asp:CheckBoxList ID="listForum" runat="server" DataTextField="Description" DataValueField="Id">
    </asp:CheckBoxList>
    
    <p>
        <asp:Button ID="btSearch" runat="server" Text="Search" CssClass="btn" OnClick="btSearch_Click" />
    </p>

    <h2>Results:</h2>
    <uc:SearchResult ID="searchResult" runat="server" />
