<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DashboardMessages.ascx.cs" Inherits="Controls_DashboardMessages" %>

<div class="box" id="messageblock">
    <div class="boxTitle" id="title" runat="server"><img src="../app_themes/default/icons/icon_message_48.png" />Messages</div>

    <div class="newsBoxContent">        
        <p runat="server" id="sectionError" class="errorBox">Error has occured</p>
        
        <asp:Repeater ID="listRepeater" runat="server" 
            onitemdatabound="listRepeater_ItemDataBound">
            <ItemTemplate>
                <div class="newsBoxTitle">                    
                    <a href="<%# GetViewUrl( (BusiBlocks.CommsBlock.Forums.Topic)Container.DataItem ) %>">
                       <asp:Image ID="imgItem" ImageUrl="" runat="server" /> <%# Server.HtmlEncode((string)Eval("Title")) %>
                    </a>
                </div>
                <div class="newsBoxDescription">
                     Created by <%# Server.HtmlEncode(((BusiBlocks.CommsBlock.Forums.Topic)Container.DataItem).Owner)%>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Label ID="lblNoResults" runat="server" Text="No unread Messages" Visible="true" Font-Italic="True"></asp:Label>

    </div>
</div>
