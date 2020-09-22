<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NewsDefault.ascx.cs" Inherits="Communication_NewsDefault" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="NewsList" Src="NewsList.ascx" %>
<%@ Register TagPrefix="uc" TagName="NewsListRead" Src="NewsListRead.ascx" %>
<%@ Register TagPrefix="uc" TagName="NewsListManage" Src="NewsListManage.ascx" %>

<%@ Register TagPrefix="ctrl" TagName="TreeView" Src="~/Controls/TreeView/NewsCategoryTreeView.ascx" %>
<telerik:RadCodeBlock runat="server" ID="qwerqwer">

<script language="javascript" type="text/javascript">

    var radAjaxManager = "<%= RadAjaxManager1.ClientID %>";

    var FilterNewsGrid = function () {
        FilterNewsGrid1();
    }

    var FilterNewsGrid1 = function () {
        if (tv.SelectedId() != "" && tv.SelectedId() != null) {
            var dataraw = { "id": tv.SelectedId() };
            var data = JSON.stringify(dataraw);
            $.ajax({
                type: "POST",
                url: "NewsWS.asmx/GetNewsItemsByCategoryId",
                data: data,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (msg) {
                    var filterExpression = '';
                    for (var i = 0; i < msg.d.length; i++) {
                        if (i != (msg.d.length - 1))
                            filterExpression += msg.d[i].Value + ',';
                        else
                            filterExpression += msg.d[i].Value;
                    }
                    var masterTableViewRead = $find(radGridRead).get_masterTableView();
                    masterTableViewRead.filter('Category', filterExpression, 17);
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }

    var FilterNewsGrid2 = function () {
        if (tv.SelectedId() != "" && tv.SelectedId() != null) {
            var dataraw = { "id": tv.SelectedId() };
            var data = JSON.stringify(dataraw);
            $.ajax({
                type: "POST",
                url: "NewsWS.asmx/GetNewsItemsByCategoryId",
                data: data,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (msg) {
                    var filterExpression = '';
                    for (var i = 0; i < msg.d.length; i++) {
                        if (i != (msg.d.length - 1))
                            filterExpression += msg.d[i].Value + ',';
                        else
                            filterExpression += msg.d[i].Value;
                    }
                    var masterTableViewManage = $find(radGridManage).get_masterTableView();
                    masterTableViewManage.filter('Category', filterExpression, 17);
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }


    function FilterResponseEnd(sender, args) {
        var eventArgs = sender["__EVENTARGUMENT"]

        if (eventArgs.indexOf("FireCommand:ctl00$contentPlaceHolder$NewsDefault$newsListRead$RadGridRead$ctl00;Filter;Category") >= 0) {
            FilterNewsGrid2();
        }
    }
</script>
</telerik:RadCodeBlock>

<telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" >
    <ClientEvents OnResponseEnd="FilterResponseEnd" />    
</telerik:RadAjaxManager>

<h1 class="sectionhead" id="announce">
    Announcements</h1>
<h2>
    Categories</h2>
<ctrl:TreeView ID="tv" runat="server" OnClientNodeClicked="FilterNewsGrid" MenuMode="Browse"
    ShowItemCount="true" />
<h2>
    Your Announcements to read</h2>
<uc:NewsListRead runat="server" ID="newsListRead" />
<h2>
    Your Announcements to manage</h2>
<uc:NewsListManage runat="server" ID="newsListManage" />
