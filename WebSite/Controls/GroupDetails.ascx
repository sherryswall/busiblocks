<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GroupDetails.ascx.cs"
    Inherits="Controls_GroupDetails" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<telerik:RadCodeBlock runat="server" ID="radCodeBlock">
    <script type="text/javascript" language="javascript">
        function setPersonTypes(id, groupName, pageType) {

            listGroups = new Array();

            var msg = eval($('#' + listGroupsDetails).val());
            if (!(typeof msg === 'undefined')) {
                for (var i = 0; i < msg.length; i++) {
                    var data = "{'Id':'" + msg[i].Id + "','Name':'" + msg[i].Name + "'}";
                    listGroups.push(data);
                }
            }

            if (id.checked == true) {
                var data2 = "{'Id':'" + id.id + "','Name':'" + groupName + "'}";
                listGroups.push(data2);
            }
            else {
                for (var x = 0; x < msg.length; x++) {
                    if (msg[x].Id == id.id)
                        listGroups.splice(x, 1);
                }
            }
            var groups = '[' + listGroups.toString() + ']';
            $('#' + listGroupsDetails).val(groups);
        }        
    </script>
</telerik:RadCodeBlock>
<asp:Table runat="server" ID="tablePersonType" CssClass="standardTable minimum">
    <asp:TableHeaderRow>
        <asp:TableHeaderCell>ASSIGNED TO</asp:TableHeaderCell>
        <asp:TableHeaderCell>GROUP NAME</asp:TableHeaderCell>
    </asp:TableHeaderRow>
</asp:Table>
