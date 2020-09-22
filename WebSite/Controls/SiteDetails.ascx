<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SiteDetails.ascx.cs" Inherits="Controls_SiteDetails" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="tree" TagName="TreeView" Src="~/Controls/TreeView.ascx" %>
<telerik:RadCodeBlock runat="server" ID="radCodeBlock">
    <script src="../jquery/TreeView.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        var treeViewType = 'Region';
        var treeViewName = '';
        var nodeName = '';
        var nodeId = '';
        var isGridOnPage = false;
        var primarySite = '';
    </script>
</telerik:RadCodeBlock>
<tree:TreeView runat="server" ID="tree1" />
<h3 id="h3Site" class="hideElement">
    Location Access</h3>
<table id="tableSiteAccess" class="standardTable minimum">
    <tr>
        <th>
            REGIONS / SITES
        </th>
        <th>
            ACTIVE
        </th>
        <th class="hideElement">
            MANAGER
        </th>
        <th>
            ADMIN
        </th>
        <th>
            ACTIONS
        </th>
    </tr>
    <tr id="trNoAccesses">
        <td colspan="5">
            No region or site access selected
        </td>
    </tr>
</table>
