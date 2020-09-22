<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="CreateUser.aspx.cs" Inherits="Admin_CreateUser" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="sd" TagName="SiteDetails" Src="~/Controls/SiteDetails.ascx" %>
<%@ Register TagPrefix="gd" TagName="GroupDetails" Src="~/Controls/GroupDetails.ascx" %>
<%@ Register TagPrefix="ad" TagName="AdminDetails" Src="~/Controls/AdminDetails.ascx" %>
<%@ Register TagPrefix="ud" TagName="UserDetails" Src="~/Controls/UserDetails.ascx" %>
<%@ Register TagPrefix="pd" TagName="PersonalDetails" Src="~/Controls/PersonalDetails.ascx" %>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <telerik:RadCodeBlock runat="server" ID="radCodeBlock">
        <script type="text/javascript" language="javascript">
            var radTabStrip = "<%= tabStripSteps.ClientID%>";
            var multiPage = "<%= mpCreateUserStep.ClientID %>";
            var manageUserType = 'Create';
            var btnNextServer = "<%=btnNextServer.ClientID %>";
            var btnCreate = "<%=btnCreate.ClientID %>";
            var tabHitCount = 0; // tab hit count is 0<->1 counter which stops loss of site access table and stops from recreating.
            var btnBack = "<%=btnBack.ClientID%>";
            var btnNext = "<%=btnNext.ClientID %>";
            var listSites = new Array();
            var listRegions = new Array();
            var listGroups = new Array();

            var listAllSitesDetails = "<%=listAllSitesDetails.ClientID %>";
            var listRegionAccessDetails = "<%=listRegionAccessDetails.ClientID %>";
            var listRegionSitesDetails = "<%=listRegionSitesDetails.ClientID %>";
            var listGroupsDetails = "<%=listGroupsDetails.ClientID %>";

            function tabSelected() {
                var selectedTabIndex = $find(radTabStrip).get_selectedIndex();
                checkButtons(selectedTabIndex);

                //recreate site access table only once when switching tabs.
                if (selectedTabIndex == 1) {
                    if (tabHitCount == 0) {
                        getSites();
                        getRegions();
                    }
                    tabHitCount = 1;
                }

                if (selectedTabIndex == 3) {
                    $('#' + btnNextServer).click();
                }
            }

            function getSites() {
                var msg = eval($('#' + listAllSitesDetails).val());
                if (!(typeof msg === 'undefined')) {
                    for (var x = 0; x < msg.length; x++) {
                        updateSites(msg[x].personId, msg[x].locationId, msg[x].name, msg[x].isView, msg[x].isAdmin, msg[x].isManager, msg[x].isPrimary, 'Site');                        
                    }
                }
            }
            function getRegions() {
                var msg = eval($('#' + listRegionAccessDetails).val());
                if (!(typeof msg === 'undefined')) {
                    for (var x = 0; x < msg.length; x++) {
                        updateSites(msg[x].personId, msg[x].locationId, msg[x].name, msg[x].isView, msg[x].isAdmin, msg[x].isManager, false, 'Region');
                    }
                }
            }
            function next() {
                var isPageValid = true;
                isPageValid = Page_ClientValidate('userDetails');

                if (isPageValid) {
                    var tabs = $find(radTabStrip);
                    var pageViews = $find(multiPage);
                    var selectedTabIndex = tabs.get_selectedIndex();
                    selectedTabIndex = selectedTabIndex + 1;
                    tabs.trackChanges();
                    tabs.set_selectedIndex(selectedTabIndex);
                    tabs.get_selectedTab().enable();
                    pageViews.get_pageViews().getPageView(selectedTabIndex).set_selected(true);
                    tabs.commitChanges();
                    checkButtons(selectedTabIndex);
                }
            }

            function back() {
                var tabs = $find(radTabStrip);
                var pageViews = $find(multiPage);
                var selectedTabIndex = tabs.get_selectedIndex();
                selectedTabIndex = selectedTabIndex - 1;
                tabs.set_selectedIndex(selectedTabIndex);
                pageViews.get_pageViews().getPageView(selectedTabIndex).set_selected(true);
                checkButtons(selectedTabIndex);
            }
            function checkButtons(selectedTabIndex) {
                //hide/show back button
                if (selectedTabIndex == 0) {
                    $('#' + btnBack).hide();
                    $('#' + btnNext).show();
                }
                else {
                    $('#' + btnBack).show();
                    $('#' + btnNext).show();
                }

                //hide/show create button
                if (selectedTabIndex == 3) {
                    $('#' + btnNext).hide();
                    $('#' + btnCreate).show();
                }
                else {
                    $('#' + btnNext).show();
                    $('#' + btnCreate).hide();
                }

                if (selectedTabIndex == 2) {
                    $('#' + btnNext).hide();
                    $('#' + btnNextServer).show();
                }
                else {
                    $('#' + btnNextServer).hide();
                }
            }
            $(document).keypress(function (event) {
                if (event.which == 13) {
                    next();
                    event.preventDefault();
                }
                
            });  
        </script>
    </telerik:RadCodeBlock>
    <h1 id="managereg" class="sectionhead">
        Create User</h1>
    <!--div class="createUser"-->
    <div class="createUser">
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="btn">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="mpCreateUserStep" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>
        <telerik:RadTabStrip runat="server" EnableEmbeddedSkins="true" ID="tabStripSteps"
            SelectedIndex="0" MultiPageID="mpCreateUserStep" OnClientTabSelected="tabSelected"
            CssClass="progressBar">
            <Tabs>
                <telerik:RadTab Text="1. Personal Details" CssClass="divPersonalDetails">
                </telerik:RadTab>
                <telerik:RadTab Text="2. Location Access" Enabled="false">
                </telerik:RadTab>
                <telerik:RadTab Text="3. Groups" Enabled="false">
                </telerik:RadTab>
                <telerik:RadTab Text="4. Create User" Enabled="false">
                </telerik:RadTab>
            </Tabs>
        </telerik:RadTabStrip>
        <telerik:RadMultiPage ID="mpCreateUserStep" runat="server" SelectedIndex="0">
            <telerik:RadPageView ID="pvPersonalDetails" runat="server">
                <pd:PersonalDetails ID="ctrlPD" runat="server" />
            </telerik:RadPageView>
            <telerik:RadPageView ID="pvSites" runat="server">
                <asp:HiddenField runat="server" ID="listAllSitesDetails"></asp:HiddenField>
                <asp:HiddenField runat="server" ID="listRegionAccessDetails"></asp:HiddenField>
                <asp:HiddenField runat="server" ID="listRegionSitesDetails"></asp:HiddenField>
                <sd:SiteDetails ID="ctrlSD" runat="server" />
            </telerik:RadPageView>
            <telerik:RadPageView ID="pvGroups" runat="server">
                <asp:HiddenField runat="server" ID="listGroupsDetails"></asp:HiddenField>
                <gd:GroupDetails ID="ctrlGD" runat="server" />
            </telerik:RadPageView>
            <telerik:RadPageView ID="pvCreateUser" runat="server" Enabled="true">
                <ud:UserDetails ID="ctrlUD" runat="server" />
            </telerik:RadPageView>
        </telerik:RadMultiPage>
    </div>
    <button type="button" id="btnBack" class="btn hideElement" onclick="back();" runat="server">
        < Back</button>
    <button type="button" id="btnNext" class="btn" onclick="next();" runat="server" validationgroup="userDetails"
        causesvalidation="true">Next ></button>
    <asp:Button CssClass="btn hideElement" runat="server" ID="btnNextServer" Text="Next >"
        OnClick="btnNext_Click" />
    <asp:Button CssClass="btn" runat="server" ID="btnCancel" Text="Cancel" OnClick="btnCancel_Click" />
    <asp:Button CssClass="btn" runat="server" ID="btnCreate" Text="Create" OnClick="btnCreateUser_Click"
        Visible="false" />
</asp:Content>
