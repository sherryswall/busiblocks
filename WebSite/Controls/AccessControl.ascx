<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AccessControl.ascx.cs" Inherits="Controls_AccessControl" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajax" %>

<script type="text/javascript" language="javascript">
    $(document).ready(function () {
        RefreshHid(lstSummaryViewingId, hidAccessViewing);
        RefreshHid(lstSummaryEditingId, hidAccessEditing);
        RefreshHid(lstSummaryContributingId, hidAccessContributing);
        RefreshHid(lstSummaryApprovingId, hidAccessApproving);
    });

    
     var lstGroupsViewingId = "<%=lstGroupsViewing.ClientID%>";
     var lstLocationsViewingId = "<%=lstLocationsViewing.ClientID%>";
     var lstSummaryViewingId = "<%=lstSummaryViewing.ClientID%>";
     var hidAccessViewing = "<%=hidAccessViewing.ClientID%>";

     var lstGroupsEditingId = "<%=lstGroupsEditing.ClientID%>";
     var lstLocationsEditingId = "<%=lstLocationsEditing.ClientID%>";
     var lstSummaryEditingId = "<%=lstSummaryEditing.ClientID%>";
     var hidAccessEditing = "<%=hidAccessEditing.ClientID%>";

     var lstGroupsContributingId = "<%=lstGroupsContributing.ClientID%>";
     var lstLocationsContributingId = "<%=lstLocationsContributing.ClientID%>";
     var lstSummaryContributingId = "<%=lstSummaryContributing.ClientID%>";
     var hidAccessContributing = "<%=hidAccessContributing.ClientID%>";

     var lstGroupsApprovingId = "<%=lstGroupsApproving.ClientID%>";
     var lstLocationsApprovingId = "<%=lstLocationsApproving.ClientID%>";
     var lstSummaryApprovingId = "<%=lstSummaryApproving.ClientID%>";
     var hidAccessApproving = "<%=hidAccessApproving.ClientID%>";


     function AddViewing() { Add(lstGroupsViewingId, lstLocationsViewingId, lstSummaryViewingId, hidAccessViewing); }
     function RemoveViewing() { Remove(lstSummaryViewingId, hidAccessViewing); }

     function AddEditing() { Add(lstGroupsEditingId, lstLocationsEditingId, lstSummaryEditingId, hidAccessEditing); }
     function RemoveEditing() { Remove(lstSummaryEditingId, hidAccessEditing); }

     function AddContributing() { Add(lstGroupsContributingId, lstLocationsContributingId, lstSummaryContributingId, hidAccessContributing); }
     function RemoveContributing() { Remove(lstSummaryContributingId, hidAccessContributing); }

     function AddApproving() { Add(lstGroupsApprovingId, lstLocationsApprovingId, lstSummaryApprovingId, hidAccessApproving); }
     function RemoveApproving() { Remove(lstSummaryApprovingId, hidAccessApproving); }

    function Add(lstGroupsId, lstLocationsId, lstSummaryId, hidAccess) {

        var lstGroup = $("#" + lstGroupsId + " :selected");
        var lstLocation = $("#" + lstLocationsId + " :selected");
        var lstSummary = $("#" + lstSummaryId);
        var lstSummaryOptions = $("#" + lstSummaryId + " option");

        if ((!IsNullOrEmpty(lstGroup.text())) && (!IsNullOrEmpty(lstLocation.text()))
            && (!IsNullOrEmpty(lstGroup.val())) && (!IsNullOrEmpty(lstLocation.val()))) {

            var text = lstGroup.text() + " from " + lstLocation.text();
            var value = lstGroup.val() + "|" + lstLocation.val();

            var alreadyAdded = false;
            lstSummaryOptions.each(function () {
                if ($(this).text() == text && $(this).val() == value)
                    alreadyAdded = true;
            });

            if (!alreadyAdded)
                lstSummary.append("<option value=\"" + value + "\">" + text + "</option>");

            RefreshHid(lstSummaryId, hidAccess);
        }
    };

    function Remove(lstSummaryId, hidAccess) {

        var selectedItem = $("#" + lstSummaryId + " :selected");
        $("#" + lstSummaryId + " option[value='" + selectedItem.val() + "']").remove();
        RefreshHid(lstSummaryId, hidAccess);
    }

    function RefreshHid(lstSummaryId, hidAccess) {
            var ids = "";
            var lstSummaryOptions = $("#" + lstSummaryId + " option");
            lstSummaryOptions.each(function () {
                ids = ids + "," + $(this).val();
            });
            ids = ids.substr(1, ids.length - 1);
            $("#" + hidAccess).val(ids);
    }

    function IsNullOrEmpty(string) {
        return (string == "" || string == null);
    }


</script>
    
<ajax:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0">
    <ajax:TabPanel ID="TabPanelViewingAccess" runat="server" HeaderText="View">      
        <ContentTemplate>
            <table class="formTable">
                <tr valign="top">
                    <td>
                        Select Group:<br />
                        <asp:ListBox ID="lstGroupsViewing" SelectionMode="Single" runat="server" Height="250px" Width="238px" />
                    </td>
                    <td>
                        Select Site:<br />
                        <asp:ListBox ID="lstLocationsViewing" SelectionMode="Single" runat="server" Height="250px" Width="238px" />
                        <br />
                        
                    </td>
                    <td>
                        <br />
                        <asp:Button UseSubmitBehavior="false" Text="Add >>" ID="btnAddViewing" runat="server" CausesValidation="false" OnClientClick="javascript:AddViewing(); return false;" CssClass="btn" Width="80px" /><br />
                        <asp:Button UseSubmitBehavior="false" Text="Remove <<" ID="btnRemoveViewing" runat="server" CssClass="btn"  OnClientClick="javascript:RemoveViewing(); return false;" Width="80px" />
                    </td>
                    <td>
                        Select Viewers:<br />
                        <asp:ListBox Height="250px" ID="lstSummaryViewing" runat="server" Width="238px"></asp:ListBox>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:TabPanel>
    <ajax:TabPanel ID="TabPanelContributingAccess" runat="server" HeaderText="Contribute">      
        <ContentTemplate>
            <table class="formTable">
                <tr valign="top">
                    <td>
                        Select Group:<br />
                        <asp:ListBox ID="lstGroupsContributing" SelectionMode="Single" runat="server" Height="250px" Width="238px" />
                    </td>
                    <td>
                        Select Site:<br />
                        <asp:ListBox ID="lstLocationsContributing" SelectionMode="Single" runat="server" Height="250px" Width="238px" />
                        <br />
                        
                    </td>
                    <td>
                        <br />
                        <asp:Button UseSubmitBehavior="false" Text="Add >>" ID="btnAddContributing" runat="server" CausesValidation="false" OnClientClick="javascript:AddContributing(); return false;" CssClass="btn" Width="80px" /><br />
                        <asp:Button UseSubmitBehavior="false" Text="Remove <<" ID="btnRemoveContributing" runat="server" CssClass="btn"  OnClientClick="javascript:RemoveContributing(); return false;" Width="80px" />
                    </td>
                    <td>
                        Select Contributors:<br />
                        <asp:ListBox Height="250px" ID="lstSummaryContributing" runat="server" Width="238px"></asp:ListBox>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:TabPanel>
    <ajax:TabPanel ID="TabPanelEditingAccess" runat="server" HeaderText="Edit">      
        <ContentTemplate>
            <table class="formTable">
                <tr valign="top">
                    <td>
                        Select Group:<br />
                        <asp:ListBox ID="lstGroupsEditing" SelectionMode="Single" runat="server" Height="250px" Width="238px" />
                    </td>
                    <td>
                        Select Site:<br />
                        <asp:ListBox ID="lstLocationsEditing" SelectionMode="Single" runat="server" Height="250px" Width="238px" />
                        <br />
                        
                    </td>
                    <td>
                        <br />
                        <asp:Button UseSubmitBehavior="false" Text="Add >>" ID="btnAddEditing" runat="server" CausesValidation="false" OnClientClick="javascript:AddEditing(); return false;" CssClass="btn" Width="80px" /><br />
                        <asp:Button UseSubmitBehavior="false" Text="Remove <<" ID="btnRemoveEditing" runat="server" CssClass="btn"  OnClientClick="javascript:RemoveEditing(); return false;" Width="80px" />
                    </td>
                    <td>
                        Select Editors:<br />
                        <asp:ListBox Height="250px" ID="lstSummaryEditing" runat="server" Width="238px"></asp:ListBox>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:TabPanel>

    <ajax:TabPanel ID="TabPanelApprovingAccess" runat="server" HeaderText="Approving" Visible="false">      
        <ContentTemplate>
            <table class="formTable">
                <tr valign="top">
                    <td>
                        Select Group:<br />
                        <asp:ListBox ID="lstGroupsApproving" SelectionMode="Single" runat="server" Height="250px" Width="238px" />
                    </td>
                    <td>
                        Select Site:<br />
                        <asp:ListBox ID="lstLocationsApproving" SelectionMode="Single" runat="server" Height="250px" Width="238px" />
                        <br />
                        
                    </td>
                    <td>
                        <br />
                        <asp:Button UseSubmitBehavior="false" Text="Add >>" ID="btnAddApproving" runat="server" CausesValidation="false" OnClientClick="javascript:AddApproving(); return false;" CssClass="btn" Width="80px" /><br />
                        <asp:Button UseSubmitBehavior="false" Text="Remove <<" ID="btnRemoveApproving" runat="server" CssClass="btn"  OnClientClick="javascript:RemoveApproving(); return false;" Width="80px" />
                    </td>
                    <td>
                        This item will be visible to the following:<br />
                        <asp:ListBox Height="250px" ID="lstSummaryApproving" runat="server" Width="238px"></asp:ListBox>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:TabPanel>
</ajax:TabContainer>

<input type="hidden" id="hidAccessViewing" name="hidAccessViewing" runat="server" />
<input type="hidden" id="hidAccessEditing" name="hidAccessEditing" runat="server" />
<input type="hidden" id="hidAccessContributing" name="hidAccessContributing" runat="server" />
<input type="hidden" id="hidAccessApproving" name="hidAccessApproving" runat="server" />