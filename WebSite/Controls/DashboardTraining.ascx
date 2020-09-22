<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DashboardTraining.ascx.cs"
    Inherits="Controls_DashboardTraining" %>
<div class="box" id="trainingblock">
    <div class="boxTitle" id="title" runat="server">
        <img src="../app_themes/default/icons/icon_training_48.png" />Training</div>
    <div class="newsBoxContent">
        <div class="newsBoxTitle">
            <a href="#" onclick="javascript:window.open('../Training/OHS/index.htm','_blank','width=996,height=688,toolbar=no,titlebar=yes,location=no,menubar=no,resizable=yes,status=no')">
                <img src="../App_Themes/Default/icons/cube_green.png" />
                Occupational Health & Safety </a>
        </div>
        <div class="newsBoxDescription">
            An introduction to Ground Down Coffee Occupational Health & Safety Guidelines
        </div>
        <asp:Label ID="lblNoResults" runat="server" Text="No unread Announcements" Visible="false"
            Font-Italic="True"></asp:Label>
    </div>
</div>
