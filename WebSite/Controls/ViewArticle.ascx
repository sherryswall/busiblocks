<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ViewArticle.ascx.cs" Inherits="Controls_ViewArticle" %>

<p runat="server" id="sectionError" class="errorBox"> Document '<%= ArticleName %>' not found</p>

<script type="text/javascript" language="javascript">

    $(document).ready(function () {

        var toc = $("#ctl00_contentPlaceHolder_viewArticle_sectionTOC");
        var body = $("#ctl00_contentPlaceHolder_viewArticle_sectionBody");

        if (toc.height() >= body.height())
            body.height(toc.height() + 50);
    });

    function displaySection(section) {
        __doPostBack("<%=lnkSection.UniqueID %>", section);
    }

</script>

<div runat="server" id="controlDiv" class="article">
<asp:HiddenField ID="hidReferrer" runat="server" />
    <asp:LinkButton ID="lnkSection" runat="server" Width="0px" OnClick="lnkSection_Click"></asp:LinkButton>
    <div class="annDetails" id="sectionProperties" runat="server">
        <span>
            Title: <span class="articleTitle" id="lblArticleTitle" runat="server"></span>
            <div class="actionButton" id="sectionActions" runat="server">
            <asp:LinkButton OnClientClick="return confirm('You are acknowledging that you have read and understood this document.\nClick OK to confirm.');" ID="btnAcknowledge" runat="server" Text="Acknowledge" OnClick="btnAcknowledge_Click" CssClass="ackitem" />
    
                <a id="linkEdit" runat="server" class="edit">Edit</a> <a id="linkPrint" runat="server"
                    class="printitem" target="_blank">Print</a>
            </div>
        </span>
        <span>
            <a class="user" id="linkAuthor" runat="server">by <span id="lblAuthor" runat="server">
            </span></a>
        </span>
        <span>
            Date modified: <span id="lblDate" runat="server"></span>
        </span>
        <span>
            Version: <span id="lblVersion" runat="server"></span><small>(<a id="linkLatestVersion"
                runat="server">Latest version</a> <a id="linkBrowseVersions" runat="server">Browse versions</a>)</small>
        </span>
        
        <div class="articleDescription">
        <u>Editorial Notes</u><br />
            <span  id="lblArticleDescription" runat="server"></span>
        </div>
    </div>
    <asp:LinkButton ID="lnkBack" runat="server" onclick="lnkBack_Click" style="float:right">< Back</asp:LinkButton>
    <div class="sidebar" runat="server" id="sectionTOC">
    </div>
    <div class="section" runat="server" id="sectionBody">
    </div>
</div>
