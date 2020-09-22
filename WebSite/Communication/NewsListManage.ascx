<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NewsListManage.ascx.cs" Inherits="Communication_NewsListManage" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagName="ModalPopup" TagPrefix="uc1" Src="~/Controls/ModalPopup.ascx" %>
<%@ Register TagPrefix="ul" TagName="UsersList" Src="~/Controls/UsersList.ascx" %>



<telerik:RadScriptBlock runat="server" ID="radscriptblock">
<script type="text/javascript" language="javascript">

    var radGridManage = "<%= RadGridManage.ClientID %>";
    var editUrl = "<%= EditLink  %>";
    var radGridManageUniqueId = "<%= RadGridManage.UniqueID %>";
    var showCreationRow = true; 

    var clickedButton;
    var treeViewType = 'Category';
    var treeViewName = 'News';
    var nodeName = '';
    var nodeId = '';
    var isGridOnPage = true;
    var RadGridManage1;


    var EditClick = function (button) {
        var id = $(button).attr('arg');
        window.location = editUrl.replace("IDPLACEHOLDER", id);
    }
    var DeleteClick = function (button) {
        pupDeleteNewsItem.Show();
        clickedButton = button;
    }

    var deleteNewsItem = function () {
        var code = clickedButton.toString();
        code = code.replace("javascript:", "");
        code = decodeURIComponent(code);
        eval(code);
    }

    function GetGridObject(sender, eventArgs) {
        RadGridManage = sender;
    }

    // *** RAD GRID FUNCTIONALITY *************************

    function AddCreationRow(catId) {
        $.ajax({
            type: "POST",
            url: "NewsWS.asmx/wmCheckEditAccess" + "?id=" + catId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                if (msg.d != null) {
                    if (msg.d["Access"] == true) {
                        RenderCreationRow(msg.d["URL"]);
                    }
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.status);
                //alert(thrownError);
            }
        });
    }


    function RenderCreationRow(catIdUrl) {
        var grid = $("#" + radGridManage);
        var gridDivChild = $(grid).children();

        $(gridDivChild).each(function (index) {
            if (index == 0) {
                var x = $(this).children();
                $(x).each(function (index) {
                    if (index == 3) {
                        var tableRows = $(this).children();
                        $(tableRows).each(function (index) {
                            if (index == 0) {
                                var findRow = $('.createRowTR').html();
                                if (findRow == null) {
                                    var htmlString = '';

                                    htmlString += '<tr class="createRowTR">';
                                    htmlString += '<td colspan="7">';
                                    htmlString += '<a href=' + catIdUrl + '>';
                                    htmlString += '<img alt="Create Announcement" src="../App_Themes/Default/images/add.png"/>';
                                    htmlString += '&nbsp;Create Announcement';
                                    htmlString += '</a></td></tr>';

                                    $(this).before(htmlString);
                                }
                            }
                        });
                    }
                });
            }
        });
    }


    // *** TREEVIEW FUNCTIONALITY *************************

</script>
</telerik:RadScriptBlock>


<telerik:RadAjaxManagerProxy ID="RadAjaxProxyManage" runat="server">
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="RadGridManage">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="RadGridManage" />
                <telerik:AjaxUpdatedControl ControlID="Feedback" />
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
</telerik:RadAjaxManagerProxy>


<telerik:RadGrid ID="RadGridManage" runat="server" EnableEmbeddedSkins="true" Skin=""
    AllowFilteringByColumn="True" OnItemCommand="RadGrid1ItemCommand" OnItemDataBound="RadGrid1_ItemDataBound"
    OnNeedDataSource="RadGrid1_NeedDataSource" GroupingSettings-CaseSensitive="false"
    EnableLinqExpressions="false" AlternatingItemStyle-CssClass="alternate">
    <MasterTableView RetrieveAllDataFields="false" UseAllDataFields="false" AutoGenerateColumns="false"
        CssClass="standardTable" AllowSorting="true" AllowPaging="true" FilterItemStyle-CssClass="radGridFilterRow"
        PageSize="10" BorderStyle="none" AllowNaturalSort="false" NoMasterRecordsText="No announcements to display"
        OnPreRender="RadGrid1_PreRender">
        <Columns>
            <telerik:GridBoundColumn DataField="NewsGridItem.NewsItem.Id" UniqueName="Id" Display="false" />
            <telerik:GridBoundColumn DataField="NewsGridItem.NewsItem.Author" UniqueName="Author"
                Display="false" />
            <telerik:GridBoundColumn DataField="NewsGridItem.NewsItem.Category.Id" UniqueName="CategoryId"
                Display="false" />
            <telerik:GridBoundColumn DataField="NewsGridItem.NewsItem.Deleted" UniqueName="Deleted"
                Display="false" />
            <telerik:GridBoundColumn DataField="NewsGridItem.NewsItem.ApprovalStatus.Name" UniqueName="PubStatus"
                Display="false" />
            <telerik:GridBoundColumn DataField="NewsGridItem.NewsItem.ApprovalStatus.Name" UniqueName="ApprovalStatusName"
                Display="false" />
            <telerik:GridBoundColumn DataField="NewsGridItem.NewsItem.ApprovalStatus.Id" UniqueName="ApprovalStatusId"
                Display="false" />
            <telerik:GridBoundColumn DataField="NewsGridItem.NewsItem.RequiresAck" UniqueName="RequiresAck"
                Display="false" />
            <telerik:GridBoundColumn DataField="NewsGridItem.Draft.GroupId" UniqueName="GroupId"
                Display="false" />
            <telerik:GridBoundColumn DataField="NewsGridItem.Draft.VersionNumber" UniqueName="VersionNumber"
                Display="false" />
            <telerik:GridBoundColumn DataField="NewsGridItem.Draft.DraftId" UniqueName="DraftId"
                Display="false" />
                
            <telerik:GridBoundColumn DataField="NewsGridItem.NewsItem.Owner" UniqueName="Owner" Display="false" />
            <telerik:GridBoundColumn HeaderText="TITLE" DataField="NewsGridItem.NewsItem.Title"
                UniqueName="Title" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                ShowFilterIcon="false" SortAscImageUrl="~/App_Themes/Default/icons/sortAscending.png"
                SortDescImageUrl="~/App_Themes/Default/icons/sortDescending.png" />
            <telerik:GridBoundColumn HeaderText="CATEGORY" DataField="NewsGridItem.NewsItem.Category.Name"
                UniqueName="Category" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                ShowFilterIcon="false" SortAscImageUrl="~/App_Themes/Default/icons/sortAscending.png"
                SortDescImageUrl="~/App_Themes/Default/icons/sortDescending.png" />
            <telerik:GridBoundColumn HeaderText="EDIT STATUS" DataField="NewsGridItem.NewsItem.ApprovalStatus.Name"
                UniqueName="EditStatus" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                ShowFilterIcon="false" SortAscImageUrl="~/App_Themes/Default/icons/sortAscending.png"
                SortDescImageUrl="~/App_Themes/Default/icons/sortDescending.png" />
            
             <telerik:GridHyperLinkColumn DataTextField="NewsGridItem.Draft.VersionNumber" AllowFiltering="true"
                HeaderText="VERSION" UniqueName="Version" SortExpression="NewsGridItem.Draft.Version"
                ShowFilterIcon="false" CurrentFilterFunction="Contains" SortAscImageUrl="~/App_Themes/Default/icons/sortAscending.png"
                SortDescImageUrl="~/App_Themes/Default/icons/sortDescending.png" AutoPostBackOnFilter="true" DataType="System.String">                
            </telerik:GridHyperLinkColumn>
            
            <telerik:GridBoundColumn HeaderText="Modified" DataField="NewsGridItem.NewsItem.UpdateDate"
                UniqueName="Modified" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                ShowFilterIcon="false" SortAscImageUrl="~/App_Themes/Default/icons/sortAscending.png"
                SortDescImageUrl="~/App_Themes/Default/icons/sortDescending.png" DataType="System.DateTime"
                DataFormatString="{0:dd/MM/yy - HH:mm}" />
            <telerik:GridTemplateColumn UniqueName="Actions" HeaderText="ACTIONS" AllowFiltering="false">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblCheckedOut"></asp:Label>
                    <asp:LinkButton CssClass="edit" ID="lnkEditItem" runat="server" arg='<%# Eval("NewsGridItem.Draft.ItemId") %>'
                        CommandName="editVersion" CommandArgument='<%# Eval("NewsGridItem.Draft.ItemId") %>'>Edit</asp:LinkButton>
                    <asp:LinkButton CssClass="deleteitem" ID="lnkDeleteClient" runat="server" OnClientClick="javascript:DeleteClick(this); return false;"
                        CommandName="delete" CommandArgument='<%# Eval("NewsGridItem.Draft.ItemId") %>'>Delete</asp:LinkButton>
                    <asp:LinkButton runat="server" ID="lnkBtnCheckIn" CommandName="checkinItem" CssClass="checkIn"
                        CommandArgument='<%#Eval("NewsGridItem.Draft.Id") %>' Visible="false">Check In</asp:LinkButton>
                    <asp:LinkButton runat="server" ID="lnkApproveItem" CommandName="approveItem" CssClass="approve"
                        CommandArgument='<%# Eval("NewsGridItem.Draft.ItemId") %>' Visible="false">Approve</asp:LinkButton>
                    <asp:LinkButton runat="server" ID="lnkViewStatus" CommandName="viewStatus" CssClass="viewStatus"
                        CommandArgument='<%# Eval("NewsGridItem.Draft.ItemId") %>' Visible="true">View Status</asp:LinkButton>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
        </Columns>
    </MasterTableView>
    <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true" FirstPageImageUrl="~/App_Themes/Default/icons/pageFirst.png"
        PrevPageImageUrl="~/App_Themes/Default/icons/pagePrevious.png" LastPageImageUrl="~/App_Themes/Default/icons/pageLast.png"
        NextPageImageUrl="~/App_Themes/Default/icons/pageNext.png" />
    <ClientSettings>
        <ClientEvents OnGridCreated="GetGridObject"></ClientEvents>
    </ClientSettings>
</telerik:RadGrid>
<telerik:RadCodeBlock runat="server" ID="radCodeBlock">
    <script language="javascript" type="text/javascript">
        function ResponseEnd() {
            if (tv.SelectedId() != "" && tv.SelectedId() != null)
                AddCreationRow(tv.SelectedId());
        }
    </script>
</telerik:RadCodeBlock>

<uc1:ModalPopup ID="pupDeleteNewsItem" runat="server" OnClientAcceptClick="deleteNewsItem()"
    AcceptButtonText="Delete" Content="Are you sure you want to delete this item?"
    CancelButtonText="Cancel" Width="275px" Title="Delete?" Height="100px" />    


<uc1:ModalPopup ID="pupUsers" runat="server" AcceptButtonText="Close" Width="275px" Height="100px" > 
    <FormTemplateContainer>
        <ul:UsersList ID="tttt" runat="server" />
    </FormTemplateContainer>
</uc1:ModalPopup>



