<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PersonalDetailsView.ascx.cs"
    Inherits="Controls_PersonalDetailsView" %>
<script type="text/javascript" language="javascript">
    
    var lblName="<%=txtUserId.ClientID %>";
    var lblLastName="<%=txtLastName.ClientID %>";
    var lblFirstName="<%=txtFirstName.ClientID %>";
    var lblPosition="<%=txtPosition.ClientID %>";
    var lblWorkPhone="<%=txtWorkPhone.ClientID %>";
    var lblWorkFax="<%=txtWorkFax.ClientID %>";
    var lblWorkEmail="<%=txtWorkEmail.ClientID %>";
    var lblWorkMobile="<%=txtWorkMobile.ClientID %>";
    
    var <%=this.ID%> = new function () {
        this.Load = function(id) {                    
            var options = {
            type: "POST",
            url:  "SearchUsers.aspx/wmGetPersonalDetails",
            data: "{'userName':'" + id + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                if (msg.d != null) {
                   $('#'+lblName).html(id);
                   $('#'+lblLastName).html(msg.d["LastName"]);
                   $('#'+lblFirstName).html(msg.d["FirstName"]);
                   $('#'+lblPosition).html(msg.d["Position"]);
                   $('#'+lblWorkPhone).html(msg.d["WorkPhone"]);
                   $('#'+lblWorkFax).html(msg.d["WorkFax"]);
                   $('#'+lblWorkEmail).html(msg.d["Email"]);
                   $('#'+lblWorkMobile).html(msg.d["WorkMobile"]);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.status);
                //alert(thrownError);
            }
         };
          $.ajax(options);
       } 
    }
</script>
<div>
    <asp:HiddenField runat="server" ID="hfPersonId" />
    <table class="standardTable minimum">
        <tr class="primary">
            <td>
                User Name
            </td>
            <td class="details">
                <asp:Label ID="txtUserId" runat="server" Width="210px" />
            </td>
        </tr>
        <tr>
            <td>
                Last Name
            </td>
            <td class="details">
                <asp:Label ID="txtLastName" runat="server" Width="210px" />
            </td>
        </tr>
        <tr>
            <td>
                First Name
            </td>
            <td class="details">
                <asp:Label ID="txtFirstName" runat="server" Width="210px" />
            </td>
        </tr>
        <tr>
            <td>
                Position
            </td>
            <td class="details">
                <asp:Label ID="txtPosition" runat="server" Width="210px" />
            </td>
        </tr>
        <tr>
            <td>
                Work Phone
            </td>
            <td class="details">
                <asp:Label ID="txtWorkPhone" runat="server" Width="210px" />
            </td>
        </tr>
        <tr>
            <td>
                Work Fax
            </td>
            <td class="details">
                <asp:Label ID="txtWorkFax" runat="server" Width="210px" />
            </td>
        </tr>
        <tr>
            <td>
                Work Email
            </td>
            <td class="details">
                <asp:Label ID="txtWorkEmail" runat="server" Width="210px" />
            </td>
        </tr>
        <tr>
            <td>
                Work Mobile
            </td>
            <td class="details">
                <asp:Label ID="txtWorkMobile" runat="server" Width="210px" />
            </td>
        </tr>
    </table>
</div>
