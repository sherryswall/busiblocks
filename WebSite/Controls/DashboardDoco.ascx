<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DashboardDoco.ascx.cs"
    Inherits="Controls_DashboardDoco" %>
<div class="box" id="docoblock">
    <div class="boxTitle" id="title" runat="server">
        <img src="../app_themes/default/icons/icon_doco_48.png" alt="Doco Icon" />Documents</div>
    <div class="newsBoxContent">
        <p runat="server" id="sectionError" class="errorBox">
            Error has occured</p>
        <asp:Repeater ID="listRepeater" runat="server" OnItemDataBound="ListRepeater_ItemDataBound">
            <ItemTemplate>
                <div class="newsBoxTitle">
                    <a href="<%# GetViewArticleUrl(((BusiBlocks.DocoBlock.Article)Container.DataItem).Name) %>">
                        <asp:Image ID="imgItem" ImageUrl="" runat="server" />
                        <%# Server.HtmlEncode((string)Eval("Title")) %>
                    </a>
                </div>
                <div class="newsBoxDescription">
                    Created by
                    <%# Server.HtmlEncode(((BusiBlocks.DocoBlock.Article)Container.DataItem).Author)%>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Label ID="lblNoResults" runat="server" Text="No unread Documents" Visible="false"
            Font-Italic="True"></asp:Label>
    </div>
</div>
