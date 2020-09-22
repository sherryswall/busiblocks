<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ForumTopicList.ascx.cs" Inherits="Controls_TopicList" %>

<div class="pageNavigation">
    <span id="lblCurrentPage" runat="server" /> of 
        <span id="lblTotalPage" runat="server" />
    <asp:LinkButton ID="linkPrev" runat="server" CssClass="previous" OnClick="linkPrev_Click">Prev</asp:LinkButton>
    <asp:LinkButton ID="linkNext" runat="server" CssClass="next" OnClick="linkNext_Click">Next</asp:LinkButton>
</div>

<table width="100%">
    <thead>
        <tr>
            <th>Subject</th>
            <th>Replies</th>
            <th>Last Post</th>
        </tr>
    </thead>
    <tbody>
        <asp:Repeater ID="listRepeater" runat="server">
            <ItemTemplate>
                <tr>
                    <td>
                        <a href="<%# GetViewTopicUrl( (string)Eval("Id") ) %>">
                            <%# Server.HtmlEncode((string)Eval("Title")) %>
                        </a><br />
                        &nbsp;&nbsp;by <%# Utilities.GetDisplayUser((string)Eval("Owner"))%>
                    </td>
                    <td align="center">
                        <%# GetRepliesCount((BusiBlocks.CommsBlock.Forums.Topic)Container.DataItem)%>
                    </td>
                    <td>
                        <small>
                            <%# GetLastPost((BusiBlocks.CommsBlock.Forums.Topic)Container.DataItem)%>
                        </small>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </tbody>
</table>