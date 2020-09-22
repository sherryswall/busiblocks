<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ModalPopup.ascx.cs" Inherits="Controls_ModalPopup" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<style type="text/css">
    .DialogWindowNoBorder table .rwTopLeft, .DialogWindowNoBorder table .rwTopRight, .DialogWindowNoBorder table .rwFooterLeft, .DialogWindowNoBorder table .rwFooterRight, .DialogWindowNoBorder table .rwFooterCenter, .DialogWindowNoBorder table .rwBodyLeft, .DialogWindowNoBorder table .rwBodyRight, .DialogWindowNoBorder table .rwTitlebar, .DialogWindowNoBorder table .rwTopResize, .DialogWindowNoBorder table .rwIcon
    {
        display: none;
    }
</style>
<script type="text/javascript" language="javascript">



    var <%=this.ID%> = new function () {
    
        var KeyCodeEnter = 13;
        var KeyCodeEscape = 27;

        var radWindowId = "<%= DialogWindow.ClientID %>";
        var divCustomContentId = "<%= divCustom.ClientID %>";
        var divInputId = "<%= divInput.ClientID %>";
        var divButtonsId = "<%= divButtons.ClientID %>";
        var refHidId = "<%= refId.ClientID %>";
        var divTitleId = "<%= divTitle.ClientID %>";
        
        var btnAcceptText = "<%= AcceptButtonText %>";
        var btnCancelText = "<%= CancelButtonText %>";

        var windowHeight = "<%= this.Height %>";
        var windowWidth = "<%= this.Width %>";

        var StringType = "string";
        var NumberType = "number";
        var RadWindow = function () {
            return $find(radWindowId);
        }
        
        this.Show = function () {
            if (windowHeight != "") {
                RadWindow().set_minHeight(windowHeight);
                RadWindow().set_height(windowHeight);
            }
            if (windowWidth != "") {
                RadWindow().set_minWidth(windowWidth);
                RadWindow().set_width(windowWidth);
            }
            RadWindow().Show();

            var textbox = $("#" + divInputId + " :input");
            if (textbox != undefined) {
                textbox.focus();
            }
            
            $(document).keypress(function (event) {
                if(RadWindow().isVisible()) {                    
                    switch (event.keyCode) {
                        case (KeyCodeEnter):
                            $("#" + divButtonsId).children('input[value="' + btnAcceptText + '"]').click();
                            break;
                        case (KeyCodeEscape):
                            $("#" + divButtonsId).children('input[value="' + btnCancelText + '"]').click();
                            break;
                    }
                }
            });
        }
        this.Hide = function () {
            RadWindow().Close();
        }
        this.SetRefId = function (val) {
            var one = document.getElementById(refHidId);
            one.value = val;
        }
        this.GetRefId = function () {
            return document.getElementById(refHidId).value;
        }

        this.Value = function () {
            var inputDiv = $("#" + divInputId);
            var inputTextbox = inputDiv.children()[1];
            var inputTextboxValue = $(inputTextbox).val();
            return inputTextboxValue;
        }
        this.SetValue = function (newValue) {
            var inputDiv = $("#" + divInputId);
            var inputTextbox = inputDiv.children()[1];
            $(inputTextbox).val(newValue);
        }

        this.SetContent = function (content) {
            if (content == undefined)
                content = "";
            var divCustomContent = document.getElementById(divCustomContentId);

            if ((typeof content == StringType) || (typeof content == NumberType)) {
                divCustomContent.innerHTML = content;
            }
            else if (content.innerHTML != undefined) {
                divCustomContent.innerHTML = content.innerHTML;
            }
            else {
                divCustomContent.innerHTML = content.html();
            }
        }
        this.ClearContent = function () {
            this.SetContent("");
        }
        
        this.SetTitle = function (title) {
            if (title == undefined) content = "";
            
            var divTitle = document.getElementById(divTitleId);

            if ((typeof title == StringType) || (typeof title == NumberType)) {
                divTitle.innerHTML = title;
            }
            else if (title.innerHTML != undefined) {
                divTitle.innerHTML = title.innerHTML;
            }
            else {
                divTitleId.innerHTML = title.html();
            }
        }
    }
</script>
<telerik:RadWindow ID="DialogWindow" runat="server" Modal="true" VisibleStatusbar="false"
    VisibleTitlebar="true" Behaviors="Close">
    <ContentTemplate>
        <asp:HiddenField ID="refId" runat="server" />
        <div id="divTitle" class="popupModalTitle" runat="server"></div>
        <div id="divCustom" class="popupModalCustom" runat="server"></div>
        <div><asp:PlaceHolder ID="plhCustomContent" runat="server"></asp:PlaceHolder></div>
        <div id="divInput" class="popupModalInput" runat="server"></div>
        <div id="divButtons" class="popupModalButtons center" runat="server"></div>
    </ContentTemplate>
</telerik:RadWindow>
