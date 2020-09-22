<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="UserViewStatus.aspx.cs" Inherits="Communication_UserViewStatus" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/ModalPopup.ascx" TagName="ModalPopup" TagPrefix="uc1" %>
<%@ Register TagPrefix="pd" TagName="PersonalDetails" Src="~/Controls/PersonalDetailsView.ascx" %>
<asp:Content ContentPlaceHolderID="HeadContent" ID="Content1" runat="server" type="text/javascript">
    <script type="text/javascript" language="javascript">
        var pdUser = "<%=pdUser.ClientID %>";
        function showPopup(id) {
            pdUser.Load(id);
            popUserDetails.Show();
        } 
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <h1>
        <asp:Label runat="server" ID="lblAnnouncementName" /></h1>
    <div>
        <div class="annDetails">
            <span><b>Author: </b>
                <asp:Label runat="server" ID="lblAuthor" class="author" /></span> <span><b>Date: </b>
                    <asp:Label runat="server" ID="lblDate" class="authordate" /></span> <span><b>Version:
                    </b>
                        <asp:Label runat="server" ID="lblVersionNumber" class="authordate" /></span>
            <span runat="server" id="spanAck"><b>
                <asp:Label runat="server" ID="lblAck" Text="Acknowledged: " /></b><asp:Label runat="server"
                    ID="lblAckNumber"></asp:Label></span> <span runat="server" id="spanNotAck"><b>
                        <asp:Label runat="server" ID="lblNotAck" Text="Not Acknowledged: " /></b><asp:Label
                            runat="server" ID="lblNotAckNumber"></asp:Label></span>
        </div>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="RadGrid1">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="RadGrid1" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="RadGrid2">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="RadGrid2" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>
        <telerik:RadGrid ID="RadGrid1" runat="server" EnableEmbeddedSkins="true" Skin=""
            AllowFilteringByColumn="True" OnItemDataBound="RadGrid1_ItemDataBound" AlternatingItemStyle-CssClass="alternate"
            OnNeedDataSource="RadGrid1_NeedDataSource" FilterItemStyle-CssClass="radGridFilterRow"
            GroupingSettings-CaseSensitive="false">
            <MasterTableView RetrieveAllDataFields="false" UseAllDataFields="false" AutoGenerateColumns="false"
                AllowSorting="true" AllowPaging="true" CssClass="standardTable" PageSize="10"
                AllowNaturalSort="false" NoMasterRecordsText="No records to display">
                <Columns>
                    <telerik:GridTemplateColumn HeaderText="USER NAME" UniqueName="User.LoginId" DataField="User.Name"
                        SortExpression="User.Name" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                        ShowFilterIcon="false" SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png"
                        SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                        <ItemTemplate>
                            <a href='#' onclick="showPopup('<%# Eval("User.Name") %>');">
                                <%# Eval("User.Name") %></a>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn HeaderText="NAME" DataField="DisplayName" UniqueName="DisplayName"
                        AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                        SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn HeaderText="VIEW STATUS" DataField="TrafficLight" UniqueName="ViewStatus"
                        AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                        SortAscImageUrl="~/App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="~/App_Themes/Default/icons/sortDescending.png" />
                    <telerik:GridBoundColumn HeaderText="DATE VIEWED" DataField="DateViewed" UniqueName="DateViewed"
                        AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                        SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
            <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true" FirstPageImageUrl="../App_Themes/Default/icons/pageFirst.png"
                PrevPageImageUrl="../App_Themes/Default/icons/pagePrevious.png" LastPageImageUrl="../App_Themes/Default/icons/pageLast.png"
                NextPageImageUrl="../App_Themes/Default/icons/pageNext.png" />
        </telerik:RadGrid>
    </div>
    <asp:Button runat="server" Text="Return to Communication" CssClass="btn" ID="btnReturn"
        OnClick="btnReturn_Click" />
    <uc1:modalpopup id="popUserDetails" runat="server" cancelbuttontext="Cancel">
        <FormTemplateContainer>
            <pd:PersonalDetails runat="server" ID="pdUser" />
        </FormTemplateContainer>
    </uc1:modalpopup>
</asp:Content>
