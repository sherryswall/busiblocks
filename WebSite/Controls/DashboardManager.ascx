<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DashboardManager.ascx.cs"
    Inherits="Controls_DashboardManager" %>
<div class="manbox" id="manager">
    <div class="boxTitle" id="title" runat="server">
        <img src="../app_themes/default/icons/icon_management._48.png" />Management Dash</div>
    <div class="newsBoxContent">
        <div runat="server" id="sectionAnnouncements">
            <h3>
                Communication (Announcements and Messages)</h3>
            <asp:Repeater ID="listNewsAckRepeater" runat="server" OnItemDataBound="listNewsAckRepeater_ItemDataBound">
                <ItemTemplate>
                    <div class="newsBoxTitle">
                        <asp:HyperLink NavigateUrl="" ID="lnkDoc" runat="server" />
                        <asp:Image ID="imgItem" ImageUrl="" runat="server" />
                        <asp:Label Text="" ID="lblDocTitle" runat="server" />
                    </div>
                    <div class="newsBoxDescription">
                        <asp:Label Text="" ID="lblDocMessage" runat="server" />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Repeater ID="listNewsViewedRepeater" runat="server" OnItemDataBound="listNewsViewedRepeater_ItemDataBound">
                <ItemTemplate>
                    <div class="newsBoxTitle">
                        <asp:HyperLink NavigateUrl="" ID="lnkDoc" runat="server" />
                        <asp:Image ID="imgItem" ImageUrl="" runat="server" />
                        <asp:Label Text="" ID="lblDocTitle" runat="server" />
                    </div>
                    <div class="newsBoxDescription">
                        <asp:Label Text="" ID="lblDocMessage" runat="server" />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Label ID="lblNoNews" runat="server" Text="No Announcements" Visible="false"
                Font-Italic="True"></asp:Label>
        </div>
        <div runat="server" id="sectionDocuments">
            <h3>
                Documents</h3>
            <asp:Repeater ID="listDocAckRepeater" runat="server" OnItemDataBound="listDocAckRepeater_ItemDataBound">
                <ItemTemplate>
                    <div class="newsBoxTitle">
                        <asp:HyperLink NavigateUrl="" ID="lnkDoc" runat="server" />
                        <asp:Image ID="imgItem" ImageUrl="" runat="server" />
                        <asp:Label Text="" ID="lblDocTitle" runat="server" />
                    </div>
                    <div class="newsBoxDescription">
                        <asp:Label Text="" ID="lblDocMessage" runat="server" />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Repeater ID="listDocViewedRepeater" runat="server" OnItemDataBound="listDocViewedRepeater_ItemDataBound">
                <ItemTemplate>
                    <div class="newsBoxTitle">
                        <asp:HyperLink NavigateUrl="" ID="lnkDoc" runat="server" />
                        <asp:Image ID="imgItem" ImageUrl="" runat="server" />
                        <asp:Label Text="" ID="lblDocTitle" runat="server" />
                    </div>
                    <div class="newsBoxDescription">
                        <asp:Label Text="" ID="lblDocMessage" runat="server" />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Label ID="lblNoDocs" runat="server" Text="No Documents" Visible="false" Font-Italic="True"></asp:Label>
        </div>

    </div>
</div>
