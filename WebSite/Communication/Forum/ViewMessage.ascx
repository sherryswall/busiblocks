<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ViewMessage.ascx.cs" Inherits="Communication_ViewMessage"  %>

<div class="message" runat="server" id="controlDiv">
    <div class="messageHeader">
        <span class="messageSubject" id="messageTitle" runat="server"></span>
        <div class="messageAuthor">
            <a class="user" id="linkAuthor" runat="server">by <span id="lblAuthor" runat="server"></span></a>
        </div>
        <div class="messageDate">
            Date: <span id="lblDate" runat="server"></span>
        </div>
        <div class="messageAttach" id="sectionAttachment" runat="server">
            <a class="attachment" id="linkAttach" runat="server"></a>
        </div>
    </div>
    <div class="messageBody" runat="server" id="sectionBody">
    </div>

    <div class="messageActions">
            <a runat="server" id="linkNew" class="mailreply" >Reply</a>
            <a runat="server" id="linkDelete" class="deleteitem" >Delete</a>
    </div>
</div>

