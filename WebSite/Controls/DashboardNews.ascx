<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DashboardNews.ascx.cs"
    Inherits="Controls_DashboardNews" %>
<div id="commsblock" class="box">
    <div class="boxTitle" id="title" runat="server">
        <img src="../app_themes/default/icons/icon_comms_48.png" />Communication (Announcements
        and Messages)</div>
    <div class="newsBoxContent">
        <p runat="server" id="sectionError" class="errorBox">
            Error has occured</p>
        <asp:Repeater ID="listRepeater" runat="server" OnItemDataBound="listRepeater_ItemDataBound">
            <ItemTemplate>
                <div class="newsBoxTitle">
                    <asp:Image ID="imgItem" runat="server" />                       
                    <asp:HyperLink runat="server" id="lnkItem">                        
                    </asp:HyperLink>                    
                </div>
                <div class="newsBoxDescription">
                    Created by
                    <asp:Label runat="server" ID="lblAuthor"></asp:Label>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Label ID="lblNoResults" runat="server" Text="No unread Announcements" Visible="false"
            Font-Italic="True"></asp:Label>
    </div>
</div>
