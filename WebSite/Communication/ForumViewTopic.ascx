<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ForumViewTopic.ascx.cs" Inherits="Communication_ForumViewTopic" %>

<%@ Register  TagPrefix="uc" TagName="ViewMessage" Src="~/Communication/Forum/ViewMessage.ascx" %>

<%@ Register TagPrefix="uc" TagName="ViewTopic" Src="~/Communication/Forum/ViewTopic.ascx" %>



<h1>Topic: <span id="lblTopic" runat="server"></span></h1>

<p>
    <a id="lnkForum" runat="server" >Back to forum</a>
</p>
    
<uc:ViewTopic ID="viewTopic" runat="server" />
