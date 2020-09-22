<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="EditUser.aspx.cs" Inherits="Admin_EditUser" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="sd" TagName="SiteDetails" Src="~/Controls/SiteDetails.ascx" %>
<%@ Register TagPrefix="gd" TagName="GroupDetails" Src="~/Controls/GroupDetails.ascx" %>
<%@ Register TagPrefix="ad" TagName="AdminDetails" Src="~/Controls/AdminDetails.ascx" %>
<%@ Register TagPrefix="ud" TagName="UserDetails" Src="~/Controls/UserDetails.ascx" %>
<%@ Register TagPrefix="pd" TagName="PersonalDetails" Src="~/Controls/PersonalDetails.ascx" %>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <telerik:RadCodeBlock runat="server" ID="radCodeBlock">
        <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1/jquery.min.js"></script>
        <script type="text/javascript" language="javascript">
            var radTabStrip = "<%= tabStripSteps.ClientID%>";
            var multiPage = "<%= mpCreateUserStep.ClientID %>";
            var tabHitCount = 0;
            var manageUserType = 'Edit';
            var isGridOnPage = false;
            var listSites = new Array();
            var listRegions = new Array();
            var listGroups = new Array();
            var listAllSitesDetails = "<%=listAllSitesDetails.ClientID %>";
            var listRegionAccessDetails = "<%=listRegionAccessDetails.ClientID %>";
            var listRegionSitesDetails = "<%=listRegionSitesDetails.ClientID %>";
            var listGroupsDetails = "<%=listGroupsDetails.ClientID %>";
            var btnNextClient = "<%=btnNextClient.ClientID %>";
            var btnBack = "<%=btnBack.ClientID %>";

            function onTabSelected(sender, args) {
                var selectedTabIndex = $find(radTabStrip).get_selectedIndex();
                checkButtons(selectedTabIndex);

                //recreate site access table only once when switching tabs.
                if (selectedTabIndex == 1) {
                    if (tabHitCount == 0) {
                        // Populate the regions and sites synchronously.
                        getRegionsAndSites();
                    }
                    tabHitCount = 1;
                }
            }

            function getRegionsAndSites() {
                getSites();
                getRegions();
            }
            function getSites() {
                var msg = eval($('#' + listAllSitesDetails).val());
                for (var x = 0; x < msg.length; x++) {
                    updateSites(msg[x].PersonId, msg[x].LocationId, msg[x].Name, msg[x].IsView.toString(), msg[x].IsAdmin.toString(), msg[x].IsManager.toString(), msg[x].IsPrimary.toString(), 'Site');
                }
            }
            function getRegions() {
                var msg = eval($('#' + listRegionAccessDetails).val());
                if (msg.length > 0) {
                    for (var x = 0; x < msg.length; x++) {
                        updateSites(msg[x].PersonId, msg[x].LocationId, msg[x].Name, msg[x].IsView.toString(), msg[x].IsAdmin.toString(), msg[x].IsManager.toString(), false, 'Region');
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
                }
                else {
                    $('#' + btnBack).show();
                    $('#' + btnNextClient).show();
                }
                if (selectedTabIndex == 2) {
                    $('#' + btnNextClient).hide();
                }
            }

            function checkPrimarySite() {
                //commenting out this piece of code since the primary site is no longer a required field (sprint 5 meeting review) - 2/05/2012.
                //if (primarySite == '') {
                //  alert('No primary site selected');
                //return false;
                //}
                return true;
            }

            function checkValidation() {
                var isPageValid = true;
                return isPageValid = Page_ClientValidate('userDetails');
            }
        </script>
    </telerik:RadCodeBlock>
    <h1 id="managereg" class="sectionhead">
        Edit User</h1>
    <div class="createUser">
        <telerik:RadAjaxManager runat="server" ID="radAjaxManager">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="tabStripSteps">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="tabStripSteps" />
                        <telerik:AjaxUpdatedControl ControlID="mpCreateUserStep" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>
        <telerik:RadTabStrip runat="server" EnableEmbeddedSkins="true" ID="tabStripSteps"
            SelectedIndex="0" MultiPageID="mpCreateUserStep" OnClientTabSelected="onTabSelected"
            CssClass="progressBar">
            <Tabs>
                <telerik:RadTab Text="Personal Details" CssClass="divPersonalDetails">
                </telerik:RadTab>
                <telerik:RadTab Text="Location Access">
                </telerik:RadTab>
                <telerik:RadTab Text="Groups">
                </telerik:RadTab>
            </Tabs>
        </telerik:RadTabStrip>
        <telerik:RadMultiPage ID="mpCreateUserStep" runat="server" SelectedIndex="0" CssClass="userTabs">
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
        </telerik:RadMultiPage>
    </div>
    <button type="button" id="btnBack" class="btn hideElement" onclick="back();" runat="server">
        < Back</button>
    <button type="button" id="btnNextClient" class="btn" onclick="next();" runat="server"
        validationgroup="userDetails" causesvalidation="true">
        Next ></button>
    <asp:Button CssClass="btn" runat="server" ID="btnNext" Text="Save" OnClick="btnSave_Click"
        OnClientClick="checkValidation();" CausesValidation="true" ValidationGroup="userDetails" />
    <asp:Button CssClass="btn" runat="server" ID="btnCancel" Text="Cancel" OnClick="btnCancel_Click" />
</asp:Content>
