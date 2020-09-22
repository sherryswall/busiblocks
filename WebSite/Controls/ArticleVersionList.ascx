<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ArticleVersionList.ascx.cs"
    Inherits="Controls_ArticleVersionList" %>

<p>Versions for article: <span id="lblTitle" runat="server"></span>
</p>

<table  class="standardTable">
    <thead>
        <tr>
            <th>Version</th>
            <th>Author</th>
            <th>Date</th>
        </tr>
    </thead>
    <tbody>
        <asp:Repeater ID="listRepeater" runat="server">
            <ItemTemplate>
                <tr>
                    <td>
                        <a href="<%# GetViewUrl( (BusiBlocks.DocoBlock.ArticleBase)Container.DataItem ) %>">
                            <%# Eval("Version") %>
                        </a>
                    </td>
                    <td>
                        <%# Utilities.GetDisplayUser((string)Eval("UpdateUser"))%>
                    </td>
                    <td>
                        <%# Utilities.GetDateTimeForDisplay((DateTime?)Eval("UpdateDate"))%>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </tbody>
</table>