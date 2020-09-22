<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ForumViewForum.ascx.cs" Inherits="Communication_ForumViewForum" %>

<%@ Register TagPrefix="uc" TagName="TopicList" Src="~/Communication/Forum/TopicList.ascx" %>


    <h1>Forum: <span id="lblForumName" runat="server"></span></h1>
    <p id="lblDescription" runat="server">
    </p>
    <p>
        <a runat="server" id="lnkBack">Back To Forums</a>
        <a class="newitem" id="linkNewTopic" runat="server" >New topic</a>
        <a class="rss" id="linkRss" runat="server">RSS News</a>    
        <a class="search" id="linkSearch" runat="server">Search</a>    
    </p>
    
    <uc:TopicList ID="topicList" runat="server" />
 
