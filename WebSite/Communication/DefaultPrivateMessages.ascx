<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DefaultPrivateMessages.ascx.cs" Inherits="Communication_DefaultPrivateMessages" %>

<link type="text/css" href="<%=ResolveUrl("~/")%>jquery/jqueryui/css/ui-lightness/jquery-ui-1.8.16.custom.css" rel="Stylesheet" />	

<script type="text/javascript" src="<%=ResolveUrl("~/")%>jquery/jqueryui/js/jquery-ui-1.8.16.custom.min.js"></script>
  
 <script type="text/javascript">

     var txtNewToId = "<%=txtNewTo.ClientID%>";

     $(document).ready(function () {
         $(function () {
             $.ajax({
                 type: 'POST',
                 url: "Default.aspx/GetUsernames",
                 data: '{}',
                 dataType: 'json',
                 contentType: 'application/json; charset=utf-8',
                 success: function (msg) {
                     InitRecipientAutoComplete(msg.d);
                 },
                 error: function (xhr, ajaxOptions, thrownError) {
                     $("#" + txtNewToId).bind("keydown", function (event) {
                         $("#" + txtNewToId).val("Error loading usernames - reload page");
                     });
                 }
             });
         });
     });

     var preventSubmit = function (event) { return (event.keyCode != 13); };

     var split = function (val) { return val.split(/,\s*/); }
     var extractLast = function (term) { return split(term).pop(); }

     var InitRecipientAutoComplete = function (usernames) {
         $("#" + txtNewToId).bind("keydown", function (event) {
             if (event.keyCode === $.ui.keyCode.TAB && $(this).data("autocomplete").menu.active) {
                 event.preventDefault();
             }
             else if (!$(this).data("autocomplete").menu.active) {
                 return (event.keyCode != $.ui.keyCode.ENTER);
             }
         }).autocomplete({
             minLength: 0,
             autoFocus: true,
             source: function (request, response) {
                 response($.ui.autocomplete.filter(usernames, extractLast(request.term)));
             },
             focus: function () {
                 return false;
             },
             select: function (event, ui) {
                 var terms = split(this.value);
                 terms.pop();
                 if (this.value.indexOf(ui.item.value) < 0) {
                     terms.push(ui.item.value);
                 }
                 terms.push("");
                 this.value = terms.join(", ");
                 return false;
             }
         });
     }

     var MessageHover = function (hoverOver, row, isNew) {
         document.body.style.cursor = (hoverOver ? "pointer" : "default");
         row.style.textDecoration = (hoverOver ? "underline" : "");
     };

    var ViewMessage = function (MessageId) { __doPostBack('OpenPrivateMessage', MessageId); };

</script>

<h1 class="sectionhead" id="messages">Private Messages</h1>
<div style="display:inline-block; vertical-align:top; ">
    <asp:LinkButton runat="server" Text="New" ID="lnkNew" onclick="lnkNew_Click"></asp:LinkButton><br />
    <asp:LinkButton runat="server" Text="Inbox" ID="lnkInbox" onclick="lnkInbox_Click"></asp:LinkButton><br />
    <asp:LinkButton runat="server" Text="Sent" ID="lnkSentMessages" onclick="lnkSentMessages_Click"></asp:LinkButton><br />
</div>
<div style="display:inline-block; width:90%;">
    <div id="divNewPrivateMessage" style="width:100%" visible="false" runat="server">
        <table width="100%">
            <tr>
                <td><asp:Label ID="Label4" runat="server" Text="To: " /></td>
                <td width="100%"><asp:TextBox Width="100%" ID="txtNewTo" runat="server" /></td>
            </tr>
            <tr>
                <td><asp:Label ID="Label6" runat="server" Text="Subject: " /></td>
                <td><asp:TextBox Width="100%" ID="txtNewSubject" runat="server" onkeydown="javascript:return preventSubmit(event);" /></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td><asp:TextBox ID="txtNewBody" runat="server" TextMode="MultiLine" Wrap="true" Width="100%" style="resize: none; min-height:250px;" /></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:Button CssClass="btn" runat="server" ID="btnNewSend" Text="Send" onclick="btnNewSend_Click" />
                    <asp:Button CssClass="btn" runat="server" ID="btnNewCancel" Text="Cancel" onclick="btnNewCancel_Click" />
                </td>
            </tr>
        </table>
    </div>
    <div id="divViewPrivateMessage" visible="false" runat="server">
        <table width="100%">
            <tr>
                <td><asp:Label ID="Label1" runat="server" Text="From: " /></td>
                <td width="100%"><asp:Label ID="lblFrom" runat="server" /></td>    
            </tr>
            <tr>
                <td><asp:Label ID="Label5" runat="server" Text="To: " /></td>
                <td width="100%"><asp:Label ID="lblTo" runat="server" /></td>    
            </tr>
            <tr>
                <td><asp:Label ID="Label2" runat="server" Text="Subject: " /></td>
                <td><asp:Label ID="lblSubject" runat="server" /></td>
            </tr>
            <tr>
                <td><asp:Label ID="Label3" runat="server" Text="Date: " /></td>
                <td><asp:Label ID="lblDateSent" runat="server" /></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td><asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine" BorderStyle="None" Wrap="true" BorderWidth="0" ReadOnly="true" Width="100%" style="resize: none; min-height:250px;" /></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:Button CssClass="btn" runat="server" ID="btnReplyOpenedMessage"  Text="Reply" onclick="btnReplyOpenedMessage_Click" />
                    <asp:Button CssClass="btn" runat="server" ID="btnDeleteOpenedMessage" 
                        Text="Delete" onclick="btnDeleteOpenedMessage_Click" />
                    <asp:Button CssClass="btn" runat="server" ID="btnForwardMessage" Text="Forward" 
                        onclick="btnForwardMessage_Click" />
                </td>
            </tr>
        </table>
    </div>
    
    <div id="divList" visible="true" runat="server">
        <asp:GridView Width="100%" ID="grdMessages" runat="server" 
            AutoGenerateColumns="false" AllowPaging="True" AllowSorting="true"
            ondatabound="grdMessages_DataBound" 
            onrowdatabound="grdMessages_RowDataBound" 
            onpageindexchanging="grdMessages_PageIndexChanging" 
            onsorting="grdMessages_Sorting">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="Id" />
                <asp:TemplateField HeaderText="To" >
                    <ItemTemplate>
                        <%# Eval("Recipient.Person.FirstName")%>&nbsp;<%# Eval("Recipient.Person.LastName")%>&nbsp(<%# Eval("Recipient.Name")%>)
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="From">
                    <ItemTemplate>
                        <%# Eval("Sender.Person.FirstName")%>&nbsp;<%# Eval("Sender.Person.LastName")%>&nbsp(<%# Eval("Sender.Name")%>)
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Subject" HeaderText="Subject" SortExpression="Subject" />
                <asp:BoundField DataField="SentDate" HeaderText="Date Sent" SortExpression="SentDate" ItemStyle-Width="250px" />
                <asp:BoundField DataField="ReadDate" HeaderText="ReadDate" SortExpression="ReadDate" NullDisplayText="null" />
                <%--
                <asp:TemplateField ItemStyle-Width="150px">
                    <ItemTemplate>
                        <asp:Button ID="btnOpen" runat="server" CssClass="btn" Text="Open" OnClick="btnOpen_Click" CommandArgument='<%# Eval("Id")%>' />
                        <asp:Button ID="btnDelete" runat="server" CssClass="btn" Text="Delete" OnClick="btnDelete_Click" CommandArgument='<%# Eval("Id")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                --%>
            </Columns>
        </asp:GridView>
    </div>
</div>  


<asp:HiddenField runat="server" ID="hidPrivateMessageId" Value="" Visible="false" />
<asp:HiddenField runat="server" ID="hidFolder" Value="" Visible="false" />