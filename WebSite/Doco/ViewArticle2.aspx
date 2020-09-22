<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewArticle2.aspx.cs" Inherits="ViewArticle2"
    MasterPageFile="~/Site.master" EnableEventValidation="false" EnableViewState="true" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register Src="~/Controls/ModalPopup.ascx" TagName="ModalPopup" TagPrefix="uc1" %>
<%--this will handle the back/forward button request so the page does not refer to cache but instead requests new data--%>
<%@ OutputCache Location="None" NoStore="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <script src="../jquery/DocoBlock.js" type="text/javascript"></script>
    <telerik:RadCodeBlock runat="server" ID="radCodeBlock">
        <script type="text/javascript" language="javascript">

            var txtEdit = "<%=txtEditor.ClientID %>";
            var litViewContent = "<%=litViewContent.ClientID%>"; // literal used to display content when in non-admin mode
            var txtBoxChapterName = "<%=txtBoxChapterName.ClientID%>"; //text box displayed when changing chapter name
            var tbxChapName = "<%=tbxChapName.ClientID %>"; // text box used to adding a new  chapter 
            var lblChapName = "<%=lblChapterName.ClientID%>"; // label above editor displaying chapter's name
            var lblChaptName = "<%=lblChapterName.ClientID %>";
            var lblChapNameEditor = "<%=lblChapterNameEditor.ClientID %>";
            var lblLastDraft = "<%=lblLastDraft.ClientID %>"; // label displayed real time updates on when draft was saved.
            var divEditor = "<%=divEditor.ClientID %>"; //  div containing the editor
            var divPreview = "<%=previewDiv.ClientID %>"; // preview container in edit mode
            var divLitPreview = "<%=divLitPreview.ClientID %>"; //preview container
            var divTocPrev = "<%=tocDivPrev.ClientID %>";
            var divPrev = "<%=divPrev.ClientID %>";
            var divLitContent = "<%=litViewContent.ClientID %>";
            var reorderListEditor = "<%=ReorderList1.ClientID %>";
            var reorderListPrev = "<%=ReorderList2.ClientID %>";
            var pnlChapsList = "<%=pnlChapsList.ClientID %>"; // panel covering the the TOC.
            var SwitchMode = 'Edit'; // a variable used to switch b/w edit mode and preview mode; for use by admin.
            var IsTextEnteredUI = false; // a boolean variable used to check whether any text was entered in the editor; used to determine when navigating away from chapter's content without saving it.
            var IsNameChanged = false;
            var isAdmin = false; // used to determine whether logged in person is admin or not.
            var IsTimerRunning = false;
            var btnSaveContent = "<%=btnSaveContent.ClientID %>";
            var lblChapNumb = "<%=lblChapNumb.ClientID %>";
            var isNumbChaps = false;
            var ChapId = ''; //also used as reference to previous chapter ID
            var IsPostBack = true;

            function OnClientLoad(editor, args) {
                if (editor != null) {
                    txtEditor = editor;
                    editor.attachEventHandler("onkeydown", function (e) {
                        if (!IsTextEnteredUI) {
                            IsTextEnteredUI = true; //means you have started editing content. 
                        }
                        if (!IsTimerRunning) {
                            enableTimer(true);
                            startTimer();
                        }
                    });

                    //Telerik Editor Commands
                    Telerik.Web.UI.Editor.CommandList["customInsertLink"] = function (commandName, editor, args) {
                        showSelectionDiv('d1');
                    };
                    Telerik.Web.UI.Editor.CommandList["customDraftLink"] = function (commandName, editor, args) {
                    };
                }
            }

            function pageLoad() {
                tbxChapName = $find("<%=tbxChapName.ClientID %>");
            }

            function startTimer() {
                var timer = $find("<%=Timer1.ClientID%>");
                timer._startTimer();
            }
            function stopTimer() {
                var timer = $find("<%=Timer1.ClientID%>");
                timer._stopTimer();
            }

            $(document).ready(function () {
                var cName = document.getElementById('<%=lblChapterNameEditor.ClientID%>');
                setSwitchMode(); //set the default mode for switching
                toggleEditNameTbx(cName, txtBoxChapterName); // toggle functionality for the textbox used to change chapter name.
                getChapId(); //Gets the ChapterID;   
                toggleTOC(); //script for button to hide/show the TOC 
                showTopLink(''); //shows the bottom navigation links for  'Back to top, previous and next chapters.
                addSubChaptersStyle(); // adding animation to chapter sub menus -hides/show using toggle.      
                checkNumbChaps();

                //disabling all textboxes to prevent default behaviour.
                $(function () {
                    $(':text').bind('keydown', function (e) { //on keydown for all textboxes 
                        if (e.keyCode == 13 || e.which == 13) //if this is enter key  
                            e.preventDefault();
                    });
                });
            });

            var AcknowledgeClick = function () {
                popAck.Show();
            }
                  
        </script>
    <uc1:ModalPopup ID="popAck" runat="server" Content="Do you confirm that you have read and understood this document?"
        Title="Acknowledge Document" AcceptButtonText="Acknowledge" CancelButtonText="Cancel" OnAcceptClick="btnAcknowledge_Click"/>
    </telerik:RadCodeBlock>
    <div id="ackText" class="ackText" runat="server">
        <div id="divAckButton" class="divAckButton" runat="server">
            I acknowledge I have read and understood. <asp:Button ID="btnAcknowledge" runat="server" OnClientClick="javascript: AcknowledgeClick(); return false;"/>
        </div>
    </div>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="inline">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="Timer1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lblLastDraft" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <asp:ListBox runat="server" Visible="false" ID="lstChangedChaps"></asp:ListBox>
    <!--Div used for overlay-->
    <div class="overlayDiv" id="overlay">
    </div>
    <!--Image Upload panel-->
    <div id="d3" class="overlayInsertChapter">
        <a href="#" onclick="hideOverlay('d3');">
            <img src="../App_Themes/Default/icons/no.png" /></a><br />
        <asp:FileUpload runat="server" ID="fuImg" /><br />
        Width:<asp:TextBox runat="server" ID="txtWidth" Title=""></asp:TextBox><br />
        Height:<asp:TextBox runat="server" ID="txtHeight" Title=""></asp:TextBox>
        <input type="file" id="attachments" />
        <input type="button" onclick="uploadImage();" class="btn" value="Insert Image" />
        <br />
        <asp:Button runat="server" ID="btnUpload" Text="Insert Image" OnClick="btnImgUpload_Click"
            CssClass="btn" Enabled="false" />
        <!--OnClientClick="return ConfirmResize();" USE THIS WHEN RESTORING IMAGE UPLOAD-->
    </div>
    <!--Drafts List-->
    <div id="d2" class="overlayInsertDraft">
        <a href="#" onclick="hideOverlay('d2');" style="margin-left: 413px;">
            <img src="../App_Themes/Default/icons/no.png" /></a>
        <div id="divDraftsList" runat="server">
        </div>
        <input type="hidden" id="hfDraftId" value="" name="hfDraftId" />
    </div>
    <!--Docus/Chaps List-->
    <div id="d1" class="overlayInsertChapter">
        <a href="#" onclick="hideOverlay('d1');">
            <img src="../App_Themes/Default/icons/no.png" /></a>
        <ajax:TabContainer ID="selectLink" runat="server" ActiveTabIndex="0" BackColor="Gray">
            <ajax:TabPanel ID="selectChapters" runat="server" HeaderText="Documents" BackColor="Gray"
                Height="300px">
                <ContentTemplate>
                    <div style="height: 250px; overflow: auto;">
                        <asp:Repeater runat="server" ID="rptDocusList">
                            <ItemTemplate>
                                <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
                                    <p>
                                        <a title='<%#Eval("Name")%>' id='<%#Eval("Id") %>' catid='<%#Eval("Category.Id")%>' isupload='<%#Eval("IsUpload")%>'
                                            name='<%#Eval("Name")%>' onclick="InsertLink(this.id,this.name,'doco',this.isupload);" style="cursor: inherit">
                                            <img id="imgSelect" src="../App_Themes/Default/icons/yes.png" alt='Select' runat="server" /></a>
                                        <%#Eval("Name")%>
                                    </p>
                                </telerik:RadCodeBlock>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </ContentTemplate>
            </ajax:TabPanel>
            <ajax:TabPanel ID="selectDocumests" runat="server" HeaderText="Chapters" BackColor="Gray"
                Height="300px">
                <ContentTemplate>
                    <div style="height: 250px; overflow: auto;">
                        <asp:Repeater runat="server" ID="rptChapsList" OnItemDataBound="rptChapsList_ItemDataBound">
                            <ItemTemplate>
                                <telerik:RadCodeBlock ID="RadCodeBlock2" runat="server">
                                    <p>
                                        <a title='<%#Eval("Name")%>' id='<%#Eval("Id") %>' name='<%#Eval("Name")%>' onclick="InsertLink(this.id,this.name,'chap','');"
                                            style="cursor: inherit">
                                            <img id="imgSelect" src="../App_Themes/Default/icons/yes.png" alt='Select' runat="server" /></a>
                                        <%#Eval("Name")%>
                                        <asp:HiddenField runat="server" ID="hdnChapId" Value='<%#Eval("Id") %>' />
                                    </p>
                                    <div class="chapSection">
                                        <div class="right">
                                            <label style="text-align: right; color: white; font-size: x-small;">
                                                Sub Sections</label><br />
                                        </div>
                                        <asp:Repeater runat="server" ID="rptChapSecList">
                                            <ItemTemplate>
                                                <img runat="server" id="imgSel" title='<%#Eval("ChapId") %>' alt='<%#Eval("AnchorTag") %>'
                                                    src="../App_Themes/Default/icons/tick.png" onclick="InsertLink(this.title,this.alt,'subChap','');" />
                                                <%#Eval("AnchorTag")%><br />
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </telerik:RadCodeBlock>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </ContentTemplate>
            </ajax:TabPanel>
        </ajax:TabContainer>
    </div>
    <!--Article Information-->
    <div id="articleHeader">
        <telerik:RadCodeBlock ID="RadCodeBlock3" runat="server">
            <div class="articleHeaderLeft">
                <h1>
                    <%=ArticleName%>
                    <div class="tabStripSmall">
                        <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1"
                            SelectedIndex="0" OnClientTabSelected="onTabSelected" CssClass="inlinelist" EnableEmbeddedSkins="False"
                            EnableTheming="False" BackColor="Transparent" EnableEmbeddedBaseStylesheet="false">
                            <Tabs>
                                <telerik:RadTab Text="Preview" runat="server" ImageUrl="~/App_Themes/Default/icons/viewdoc_24.png"
                                    SelectedImageUrl="~/App_Themes/Default/icons/viewdoc_24.png" SelectedCssClass="current" />
                                <telerik:RadTab Text="Edit" ImageUrl="~/App_Themes/Default/icons/editdoc_24.png"
                                    SelectedImageUrl="~/App_Themes/Default/icons/editdoc_24.png" SelectedCssClass="current" />
                            </Tabs>
                        </telerik:RadTabStrip>
                    </div>
                </h1>
            </div>
        </telerik:RadCodeBlock>
        <asp:UpdatePanel runat="server" ID="upPnlTimer" UpdateMode="Conditional" ChildrenAsTriggers="false">
            <ContentTemplate>
                <asp:Timer ID="Timer1" runat="server" Interval="10000" OnTick="Timer1_Tick" Enabled="true">
                </asp:Timer>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div class="articleHeaderRight">
            <asp:Label runat="server" ID="lblLastDraft" Text="" Visible="true"></asp:Label>
        </div>
        <!--<div class="articleProperties">-->
        <div class="annDetails">
            <span>
                <%--<a class="user" id="linkAuthor" runat="server">by <span id="lblAuthor" runat="server">
                </span></a>--%>
                <b>Owner: </b>
                <asp:Label ID="lblAuthor" runat="server"></asp:Label>
            </span>
            <span>
                <b>Date:</b> <span id="lblDate" runat="server"></span>
            </span>
            <span runat="server" id="spanAcked">                
                <b><asp:Label id="ackLabel" runat="server" for="lblAcknowledged">
                    Acknowledged:</asp:Label></b>
                <asp:Image ID="imgAck" runat="server" />
            </span>
            <span>
                <b>Version:</b> <span id="lblVersion" runat="server"></span>
            </span>
        </div>
    </div>
    <telerik:RadMultiPage ID="RadMultiPage1" runat="server">
        <telerik:RadPageView ID="RadPageView1" runat="server">
            <div id="previewDiv" runat="server">
                <div class="tocDiv" id="tocDivPrev" runat="server">
                    <h2 id="lblTOCHeaderPrev">
                        Table of Contents</h2>
                    <button type="button" class="btnTOCPrev btn" id="btnHideTocPrev">
                        <<</button>
                    <div class="tocItemsPrev" id="tocItemsPrev">
                        <div class="reorderItemsDivPrev" id="reorderItemsDivPrev">
                            <asp:UpdatePanel runat="server" ID="UpdatePanel2" OnLoad="pnlChapsList_Load">
                                <ContentTemplate>
                                    <ajax:ReorderList ID="ReorderList2" runat="server" AllowReorder="false" Visible="true"
                                        EnableViewState="true" DataKeyField="Id" CssClass="reorderListPrev" OnItemDataBound="rlTemplate2_ItemDataBound">
                                        <ItemTemplate>
                                            <div id="chapterReorderList" runat="server" class="chapterReorderList">
                                                <asp:Label runat="server" ID="lblChapNumbTOC" Visible="false"><%#Int32.Parse(Eval("Sequence").ToString())+1 %>.</asp:Label>
                                                <asp:LinkButton runat="server" Text='<%#Eval("Name")%>' OnClientClick="loadChapter(this.name);"
                                                    ID="lnkBtnChapPrev" Name='<%#Eval("Id")%>' Title='<%#Eval("Name")%>' OnClick="btnSaveContent_ChapClick"></asp:LinkButton>
                                                <asp:HiddenField runat="server" ID="hdnId" Value='<%#Eval("Id")%>' />
                                            </div>
                                            <div id="subChapList" class="subChapList" runat="server">
                                                <asp:Repeater runat="server" ID="rptSubChapters" Visible="false" OnItemDataBound="rptSubChaps_ItemDataBound">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblSubChapNumbTOC"></asp:Label>
                                                        <asp:HyperLink runat="server" ID="subChapAnchor"></asp:HyperLink><br />
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </div>
                                        </ItemTemplate>
                                    </ajax:ReorderList>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
                <div id="divPrev" class="divPrev" runat="server">
                    <h1>
                        <asp:Label runat="server" ID="lblChapNumb"></asp:Label><asp:Label runat="server"
                            ID="lblChapterName"></asp:Label></h1>
                    <div class="tocItemsPrevContent" id="tocItemsPrevContent">
                        <div id="divLitPreview" runat="server">
                            <div id="litViewContent" runat="server" class="litViewContnt" />
                        </div>
                    </div>
                </div>
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView ID="RadPageView2" runat="server">
            <!--TOC-->
            <div class="tocDiv" id="tocDivEdit" runat="server">
                <h2 id="lblTOCHeaderEdit">
                    Table of Contents</h2>
                <button type="button" class="btnTOC btn" id="btnHideTocEdit">
                    <<
                </button>
                <br />
                <div class="tocItems" id="tocItemsEdit">
                    <asp:Panel runat="server" ID="pnlAddChapter">
                        <telerik:RadTextBox ID="tbxChapName" runat="server" Width="90%" EmptyMessage="Add chapter"
                            EmptyMessageStyle-CssClass="defaultTextBoxValue" MaxLength="100">
                            <%--<ClientEvents OnKeyPress="addChap_Enter" />--%>
                        </telerik:RadTextBox>
                        <asp:UpdatePanel runat="server" ID="pnlAdd">
                            <ContentTemplate>
                                <triggers>
                                    <asp:PostBackTrigger ControlID="btnAdd" />
                                </triggers>
                                <asp:ImageButton ID="btnAdd" ImageUrl="~/App_Themes/Default/icons/yes.png" runat="server"
                                    ValidationGroup="valChapGroup" OnClientClick="addChapter();" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>
                    <div class="reorderItemsDiv" id="reorderItemsDiv">
                        <asp:UpdatePanel runat="server" ID="pnlChapsList" OnLoad="pnlChapsList_Load">
                            <ContentTemplate>
                                <ajax:ReorderList ID="ReorderList1" runat="server" AllowReorder="true" Visible="true"
                                    EnableViewState="true" DragHandleAlignment="Left" SortOrderField="Name" DataKeyField="Id"
                                    OnItemReorder="rlTemplate_ItemReorder" PostBackOnReorder="true" CssClass="reorderList">
                                    <DragHandleTemplate>
                                        <div class="dragHandle">
                                        </div>
                                    </DragHandleTemplate>
                                    <ItemTemplate>
                                        <div id="chapterReorderList" runat="server" class="chapterReorderList">
                                            <asp:LinkButton runat="server" Text='<%#Eval("Name")%>' OnClientClick="loadChapter(this.name)"
                                                ID="lnkBtnChap" CommandName='<%#Eval("Id")%>' name='<%#Eval("Id")%>' title='<%#Eval("Name")%>'></asp:LinkButton>
                                            <asp:HiddenField runat="server" ID="hdnId" Value='<%#Eval("Id")%>' />
                                        </div>
                                        <div class="delChapIcon">
                                            <a href="#">
                                                <img src="../App_Themes/Default/icons/delete_TOC.png" onclick="deleteChapter(this.alt,this.id);"
                                                    alt='<%#Eval("ChapterId")%>' id='<%#Eval("Id")%>' title="delete" /></a>
                                        </div>
                                    </ItemTemplate>
                                    <ReorderTemplate>
                                        <div class="dragTemplate">
                                        </div>
                                    </ReorderTemplate>
                                </ajax:ReorderList>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
            <!--Editor-->
            <div id="divEditor" class="divEditor" runat="server">
                <div class="docTitle">
                    <div class="left">
                        <h1>
                            <label runat="server" id="lblChapterNameEditor">
                            </label>
                            <asp:TextBox runat="server" ID="txtBoxChapterName" Title="" Style="display: none;"></asp:TextBox>
                            <a href="#">
                                <img src="../App_Themes/Default/icons/editdoc_24.png" id="changeChapName" /></a>
                            <div id="chapterNameChangeOptions" class="" style="display: none;">
                                <img src="../App_Themes/Default/icons/yes.png" id="acceptChapName" onclick="changeChapterName()" />
                                <img src="../App_Themes/Default/icons/delete_TOC.png" id="cancelChapName" onclick="cancelChapNameChange()" />
                            </div>
                        </h1>
                    </div>
                    <!--div class="chapName">
                        <h2 class="header">Edit Mode</h2>
                    </div-->
                </div>
                <asp:Panel runat="server" ID="pnlEditor">
                    <telerik:RadEditor runat="server" ID="txtEditor" OnClientLoad="OnClientLoad" Width="100%"
                        EditModes="Design" OnClientPasteHtml="OnClientPasteHtml" SpellCheckSettings-SpellCheckProvider="MicrosoftWordProvider"
                        ContentAreaMode="Div" NewLineMode="P" EnableResize="false" OnClientCommandExecuting="OnClientCommandExecuting">
                        <ImageManager ViewPaths="~/Doco/Files/" UploadPaths="~/Doco/Files/" />
                        <Paragraphs>
                            <telerik:EditorParagraph Title="<h3>Heading X</h3>" Tag="<h3>" />
                        </Paragraphs>
                    </telerik:RadEditor>
                    <%--<h3>
                Draft Options</h3>
            <asp:CheckBox runat="server" Text="Save draft on submit" ID="cbxSaveSubmit" />
            - (Check this option if you want to keep a draft after submitting.)<br />
            <asp:CheckBox ID="cbxDelAllDraft" runat="server" Text="Delete previous drafts" />--%>
                </asp:Panel>
            </div>
                <div class="alignLeftSubmitBtn">
                        <%--<asp:Button runat="server" Text="Save Draft" ID="btnSaveDraft" OnClick="btnSaveDraft_Click"
                    CssClass="btn" />--%>
                        <asp:Button runat="server" Text="Save" ID="btnSaveContent" OnClick="btnSaveContent_Click"
                            CssClass="btn" />
                        <asp:Button runat="server" Text="Finish Editing" ID="btnManage" OnClick="btnManage_Click"
                            CssClass="btn" />
                    </div>
        </telerik:RadPageView>
    </telerik:RadMultiPage>
    <telerik:RadCodeBlock runat="server" ID="radCodeBlock4">
        <script type="text/javascript" language="javascript">
            function pageLoad() {
                if (IsPostBack == true) {
                    stopTimer();
                    IsPostBack = false;
                }
            } 
        </script>
    </telerik:RadCodeBlock>
        <asp:Button runat="server" Text="Return to Documents" CssClass="btn" ID="btnReturn" OnClick="btnReturn_Click" />
</asp:Content>
