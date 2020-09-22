function toggleEditNameTbx(cName, txtBoxChapterName) {
    $('#changeChapName').click(function () {
        changeChapNameToEditMode(cName, txtBoxChapterName, true);
    });
}

function replaceQuotes(content) {
    var tempName = '';
    var newName = '';

    if (content.indexOf('\'') != -1) {
        tempName = content.split('\'');
        for (var x = 0; x < tempName.length; x++) {
            if (x != tempName.length - 1) {
                if (tempName[x].indexOf('\\') != -1) {
                    newName += replaceCharacter(tempName[x]) + '\\\'';
                }
                else
                    newName += tempName[x] + '\\\'';
            }
            else {
                if (tempName[x].indexOf('\\') != -1) {
                    newName += replaceCharacter(tempName[x]);
                }
                else
                    newName += tempName[x];
            }
        }
    }
    else if (content.indexOf('\\') != -1) {
        newName = replaceCharacter(content);
    }
    else
        newName = content;

    return newName;
}

function replaceCharacter(content) {
    var newName = '';
    var tempName = content.split('\\');

    for (var x = 0; x < tempName.length; x++) {
        if (x != tempName.length - 1) {
            newName += tempName[x] + '\\\\';
        }
        else {
            newName += tempName[x];
        }
    }

    return newName;
}

function changeChapNameToEditMode(lblChapName, txtBxChapName, edit) {
    if (edit == true) {
        //hide the label and the edit button
        $('#' + lblChapName.id).css('display', 'none');
        $('#changeChapName').css('display', 'none');
        //show the text box and the options.
        $('#' + txtBxChapName).css('display', 'inline-block');
        $('#chapterNameChangeOptions').css('display', 'inline-block');
    }
}

function cancelChapNameChange() {
    //show labels
    $('#' + lblChapNameEditor).css('display', 'inline-block');
    $('#changeChapName').css('display', 'inline-block');
    //hide changing options.
    $('#divEditChapName').css('display', 'none');
    $('#chapterNameChangeOptions').css('display', 'none');
    $('#' + txtBoxChapterName).css('display', 'none');
}


function changeChapterName() {

    var tbxChapName = $('#' + txtBoxChapterName).attr('value');
    var tempTbxChapName = tbxChapName;
    tbxChapName = replaceQuotes(tbxChapName);

    var options = {
        type: "POST",
        url: "ViewArticle2.aspx/wmCheckChapterName",
        data: "{'ChapName':'" + tbxChapName + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d == true) {
                showMessage('Chapter Name already in use', 'error');
                cancelChapNameChange();
            }
            else {
                AutoSave.ChangeChapterName(ChapId, tempTbxChapName);

                $('#' + lblChapNameEditor).text(tempTbxChapName);
                var pnl = document.getElementById(pnlChapsList);
                __doPostBack(pnl.id, '');
                cancelChapNameChange();
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert(xhr.status);
            //alert(thrownError);
        }
    };
    $.ajax(options);
}

//Sets the default text for the textboxes.
function InitTxtboxChapName() {
    $('input[type="text"]').each(function () {

        this.value = $(this).attr('title');
        $(this).addClass('defaultTextBoxValue');

        $(this).focus(function () {
            if (this.value == $(this).attr('title')) {
                this.value = '';
                $(this).removeClass('defaultTextBoxValue');
            }
        });

        $(this).blur(function () {
            if (this.value == '' || this.value == 'Add chapter') {
                this.value = $(this).attr('title');
                $(this).addClass('defaultTextBoxValue');
            }
        });
    });
}

//Loads list of draft for the selected chapter.
function loadDraftsList(chapterVersionId) {

    var options = {
        type: "POST",
        url: "ViewArticle2.aspx/wmBindDraftsList",
        data: "{'chapterVersionId':'" + chapterVersionId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var table = '<table>';
            if (msg.d.length > 0) {
                for (var i = 0; i < msg.d.length; i++) {
                    var row = '<tr>';

                    row += '<td><a id=' +
                        msg.d[i].Id + ' name="' + msg.d[i].VersionId + '" onclick=InsertDraft(this.id,this.name);><img id=testImg src=../App_Themes/BusiBlocks/Images/yes.png alt="Select" /></a></td>';
                    row += '<td><i>Saved on - </i>' + msg.d[i].SaveDate + '</td>';

                    row += '</tr>';
                    table += row;
                }
            }
            else {
                var row = '<tr><td>There are no drafts available for this chapter</td></tr>';
                table += row;
            }
            table += '</table>';
            $('#divDraftsList').html(table);

        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert(xhr.status);
            //alert(thrownError);
        }
    };
    $.ajax(options);
}

//Adds a new chapter
function addChapter() {
    var chapterName = $find(tbxChapName).get_value();
    var pnlChaps = document.getElementById(pnlChapsList);
    var radPageView1 = document.getElementById(radPageView1);

    chapterName = replaceQuotes(chapterName);

    if (!chapterName && chapterName.indexOf(' ') == 0) {
        showMessage('Please enter a valid chapter name', 'error');
    }
    else {

        var options = {
            type: "POST",
            url: "ViewArticle2.aspx/wmAddChapter",
            data: "{'chapterName':'" + chapterName + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                if (msg.d != '') {
                    var newChapterId = msg.d;
                    displayEditorDiv(true);

                    showMessage(chapterName + ' has been added', 'update');
                    loadChapter(newChapterId);
                }
                else {
                    showMessage('Chapter name already in use', 'error');
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.status);
                //alert(thrownError);
            }
        };
        $.ajax(options);
    }
    $find(tbxChapName).set_value("");
}

function deleteChapter(id, chVersionId) {
    var confirmDelete = confirm('Are you sure you want to delete this chapter?');
    if (confirmDelete) {

        enableTimer(false);
        IsTextEnteredUI = false;

        enableTimer(IsTextEnteredUI);

        var options = {
            type: "POST",
            url: "ViewArticle2.aspx/wmDeleteChapter",
            data: "{'chapterId':'" + id + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var pnl = document.getElementById(pnlChapsList);
                //if its the current chapter been deleted then deselect everything.
                if (chVersionId == msg.d) {
                    loadChapter('');
                }
                __doPostBack(pnl.id, '');

                //hide the editor div if all the chapters have been deleted.
                var divReorderListItems = document.getElementById(reorderListEditor);

                var childCount = $('#' + reorderListEditor).children().size();

                if (childCount == 1) {
                    displayEditorDiv(false);
                }
                showMessage('Chapter has been deleted', 'update');
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.status);
                //alert(thrownError);
            }
        };
        $.ajax(options);
    }
}

//save draft
function saveDraft() {
    var content = document.getElementById(UltimateEditorId);
    var id = $('#saveDraftPrompt').attr('title');
    var options = {
        type: "POST",
        url: "ViewArticle2.aspx/wmSaveDraft",
        data: "{'ChaptId':'" + id + "','Content':'" + content + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            IsTextEnteredUI == false;
            loadChapter(id);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert(xhr.status);
            //alert(thrownError);
        }
    };
    $.ajax(options);
}

//Loads the chapter when a TOC item is clicked.
function loadChapter(id) {

    stopTimer();
    enableTimer(false);

    var contentArea = document.getElementById(txtEdit);
    var txtChaptName = document.getElementById(txtBoxChapterName);
    var lblChapteNam = document.getElementById(lblChapName);
    var lblChaptNamEditor = document.getElementById(lblChapNameEditor);
    var lblChaptNumb = document.getElementById(lblChapNumb);
    var isDiffChap = false; //will check if the selected chapter is different from the previous one or not. if yes only then load the chapter else no. Performance issue.

    //Save previous chapter if any.
    if (ChapId != '')
        AutoSave.Save(ChapId, txtEditor.get_html(), txtChaptName.value);

    if (ChapId != id) {
        ChapId = id;
        isDiffChap = true;
    }

    txtChaptName.value = replaceQuotes(txtChaptName.value);

    if (isDiffChap == true) {
        if (id != '') {

            var options = {
                type: "POST",
                url: "ViewArticle2.aspx/wmLoadChapter",
                data: "{'Id':'" + id + "','IsEdtorChanged':'" + IsTextEnteredUI + "','ChapName':'" + txtChaptName.value + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {

                    var chapterContent = msg.d["Content"];

                    if (txtChaptName != null) {
                        txtChaptName.value = msg.d["Name"];
                        lblChaptNamEditor.innerHTML = msg.d["Name"];
                    }

                    lblChapteNam.innerHTML = msg.d["Name"];

                    if (isNumbChaps) {
                        lblChaptNumb.innerHTML = msg.d["Sequence"].toString();
                        chapterContent = addChapNumbers(chapterContent, lblChaptNumb.innerHTML);
                        lblChaptNumb.innerHTML = lblChaptNumb.innerHTML + ".&nbsp;";
                    }
                    $('#' + litViewContent).html(chapterContent);

                    showTopLink(id);

                    if (contentArea != null) {
                        var btnSave = document.getElementById(btnSaveContent);
                        btnSave.disabled = false;
                        txtEditor.set_html(chapterContent);
                        txtEditor.set_editable(true);

                        if (SwitchMode == 'Edit' && isNumbChaps == true)
                            removeChapNumbers();
                    }
                    showMessage(txtChaptName.value + ' has been selected', 'update');
                    //enable button
                    setTimeout(function () { hideOverlay(''); }, 500);
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //enable button
                    //alert(xhr.status);
                    //alert(thrownError);
                }

            };
            createOverlay('trans');
            $.ajax(options);

        }
        else {
            lblChapteNam.innerHTML = '';
            txtChaptName.value = '';
            lblChaptNamEditor.innerHTML = '';
            txtEditor.set_html('');
            txtEditor.set_editable(false);
            $('#' + litViewContent).html('');

            var btnSave = document.getElementById(btnSaveContent);
            btnSave.disabled = true;
        }
    }
    IsTextEnteredUI = false;
}
function scrollToAnchor() {
    var uri = document.location.toString();
    var x = uri.split("#");
    var anchor = x[1];
    if (anchor != '') {
        var temp = "<a name=" + anchor + "/>";
        scrollTo(x, $(temp).position().top);
    }
}
//confirm from the user for changes not being saved.
function checkEditorTextChanges() {

    if (IsTextEnteredUI == true) {
        return true;
    }
    else
        return false;
}

function enableTimer(val) {
    IsTimerRunning = val;
    var options = {
        type: "POST",
        url: "ViewArticle2.aspx/wmIsEditorChanged",
        data: "{'EditorChanged':'" + val + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert(xhr.status);
            //alert(thrownError);
        }
    };
    $.ajax(options);
}

function createOverlay(type) {
    if (type == '') {
        $('#overlay').fadeIn('fast');
    }
    else {
        $('#overlay').fadeIn('fast');
    }
}

function displayEditorDiv(show) {
    var editorDivId = document.getElementById(divEditor);
    var previewDivId = document.getElementById(divPreview);

    if (show == true) {
        $('#' + editorDivId.id).show();
        $('#' + previewDivId.id).css('display', 'block');
    }
    else {
        $('#' + editorDivId.id).hide();
        $('#' + previewDivId.id).hide();
    }
}


//Insert link to the Editor area where the mouse is clicked.
function InsertLink(id, name, type, isupload) {
    if (txtEditor == null) {
        // Throw an error because we can't do anything without a text editor.
        return;
    }
    if (type == 'doco') {
        // Insert HTML fragment into the editor
        var encodeURI = name.split(' ').join('+');
        if (isupload == 'True') {
            txtEditor.pasteHtml('<a href=ViewUploadedDoc.aspx?id=' + id + ' target=_blank>' + name + '</a>');
        } else {
            txtEditor.pasteHtml('<a href=ViewArticle2.aspx?name=' + encodeURI + '&cMode=pub&id=' + id + ' target=_blank>' + name + '</a>');
        }
    }
    else if (type == 'chap') {
        txtEditor.pasteHtml('<a href="#" onclick=loadChapter(\'' + id + '\')>' + name + '</a>');
    }
    else if (type == 'subChap') {
        var encodeURI = name.split(' ').join('+');
        txtEditor.pasteHtml('<a href=#' + encodeURI + ' onclick=loadChapter(\'' + id + '\')>' + name + '</a>');
    }
    //close the div as only one can be selected at a time -- note this is only for this release, future feature can include multiple link select.
    hideOverlay('d1');
}
//Insert link to the Editor area where the mouse is clicked.
function InsertDraft(id, name) {

    var ultimateEditorObj = UltimateEditors[UltimateEditorId];
    var options = {
        type: "POST",
        url: "ViewArticle2.aspx/wmLoadDraft",
        data: "{'Id':'" + id + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            var contentArea = document.getElementById(UltimateEditorId);
            var chapterDraftContent = msg.d["Content"];

            if (chapterDraftContent != null) {
                var did = document.getElementById('hfDraftId');
                did.value = msg.d["Id"];
                UltimateEditors[UltimateEditorId].SetEditorHTML(chapterDraftContent);
            } else
                UltimateEditors[UltimateEditorId].SetEditorHTML('');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert(xhr.status);
            //alert(thrownError);
        }
    };
    $.ajax(options);
    //close the div as only one can be selected at a time -- note this is only for this release, future feature can include multiple link select.
    hideOverlay('d2');
}
//check if the person logged in is admin or not.
function isAdminMode() {
    var contents = {
        type: "POST",
        url: "ViewArticle2.aspx/wmAdminMode",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            isAdmin = msg.d;
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert(xhr.status);
            //alert(thrownError);
        }
    };
    $.ajax(contents);
}

//runs first when document is loaded.
function checkNumbChaps() {
    var contents = {
        type: "POST",
        url: "ViewArticle2.aspx/wmGetIsNumbChaps",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            isNumbChaps = msg.d;
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert(xhr.status);
            //alert(thrownError);
        }
    };
    $.ajax(contents);
}

function switchModeClick() {
    if (SwitchMode == 'preview') {
        //display the content in preview mode
        switchModes('preview');
        SwitchMode = 'edit';
    }
    else {
        //display the content in edit mode
        switchModes('edit');
        SwitchMode = 'preview';
    }
}

function switchModes(mode) {
    var contents = {
        type: "POST",
        url: "ViewArticle2.aspx/wmSwitchModes",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            if (mode == 'Preview') {
                var tempContent = null;
                tempContent = txtEditor.get_html();
                tempContent = formatHTML(tempContent, false);

                if (isNumbChaps) {
                    var chapNumb = document.getElementById(lblChapNumb);
                    tempContent = addChapNumbers(tempContent, chapNumb.innerHTML);
                }
                $('#' + litViewContent).html(tempContent);
            }
            else {
                if (isNumbChaps)
                    removeChapNumbers();
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert(xhr.status);
            //alert(thrownError);
        }
    };
    $.ajax(contents);
}

function getChapId() {
    var options = {
        type: "POST",
        url: "ViewArticle2.aspx/wmGetChapId",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d != null) {
                ChapId = msg.d.toString();
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert(xhr.status);
            //alert(thrownError);
        }
    };
    $.ajax(options);
}
// remove tags is boolean parameter which is set to true - used to format/remove tags when they are pasted from external source.
function formatHTML(htmlContent, removeTags) {

    var tags = ['a', 'span', 'font']; //array of tags to be formatted/removed.
    var htmlDiv = $('<div id="tempDiv">' + htmlContent + '</div>');
    
    //if there are any tables then apply the 'standard class to it.
    $(htmlDiv).find('table').each(function (index) {
        $(this).addClass('standardTable');
    });
   
    //remove tags if they are from external source.
    if (removeTags) {
        //remove style attributes from all elements.
        $(htmlDiv).find('*[style]').each(function (index) {
            $(this).removeAttr('style');
        });
        //remove format the tags listed above.
        jQuery.each(tags, function (index) {
            $(htmlDiv).find(tags[index].toString()).each(function (index) {
                $(this).after($(this).html());
                $(this).remove();
            });
        });
    }
    return $(htmlDiv).html();
}

function addChapNumbers(htmlContent, chapNumb) {
    //if(check if numbering is on)

    var newHTML = '';
    if (htmlContent != '' && htmlContent != null) {

        var htmlTags = htmlContent.split('>');
        var h2Numb = 1;
        var h3Numb = 1;
        var h4Numb = 1;
        var h5Numb = 1;

        chapNumb = chapNumb.replace('.', '');
        for (var i = 0; i < htmlTags.length; i++) {

            if (htmlTags[i].toLowerCase().indexOf('/h2') > -1) {

                h3Numb = 1;
                var tag = "<label class=lblChapNumb>" + chapNumb.toString() + "." + h2Numb.toString() + "&nbsp;</label>";
                var tagged = tag + htmlTags[i];

                if (htmlTags[htmlTags.length - 1] != htmlTags[i])
                    newHTML += tagged + '>';
                else
                    newHTML += tagged;

                h2Numb = h2Numb + 1;

            }
            else if (htmlTags[i].toLowerCase().indexOf('/h3') > -1) {

                h4Numb = 1;
                h5Numb = 1;

                var tag = "<label class=lblChapNumb>" + chapNumb.toString() + "." + (h2Numb - 1).toString() + "." + h3Numb.toString() + "&nbsp;</label>";
                var tagged = tag + htmlTags[i];

                if (htmlTags[htmlTags.length - 1] != htmlTags[i])
                    newHTML += tagged + '>';
                else
                    newHTML += tagged;

                h3Numb = h3Numb + 1;
            }
            else if (htmlTags[i].toLowerCase().indexOf('/h4') > -1) {

                h5Numb = 1;

                var tag = "<label class=lblChapNumb>" + chapNumb.toString() + "." + (h2Numb - 1).toString() + "." + (h3Numb - 1).toString() + "." + h4Numb.toString() + "&nbsp;</label>";
                var tagged = tag + htmlTags[i];

                if (htmlTags[htmlTags.length - 1] != htmlTags[i])
                    newHTML += tagged + '>';
                else
                    newHTML += tagged;

                h4Numb = h4Numb + 1;
            }
            else if (htmlTags[i].toLowerCase().indexOf('/h5') > -1) {

                var tag = "<label class=lblChapNumb>" + chapNumb.toString() + "." + (h2Numb - 1).toString() + "." + (h3Numb - 1).toString() + "." + (h4Numb - 1).toString() + "." + h5Numb.toString() + "&nbsp;</label>";
                var tagged = tag + htmlTags[i];

                if (htmlTags[htmlTags.length - 1] != htmlTags[i])
                    newHTML += tagged + '>';
                else
                    newHTML += tagged;

                h5Numb = h5Numb + 1;
            }
            else {
                if (htmlTags[htmlTags.length - 1] != htmlTags[i])
                    newHTML += htmlTags[i] + '>';
                else
                    newHTML += htmlTags[i];
            }
        }
    }
    return newHTML;
}

function removeChapNumbers() {
    $(".lblChapNumb").remove();
}
//a generic method which displays the update message
function showMessage(msg, type) {

    var lblUpdateMessage = document.getElementById(lblLastDraft);
    if (type == 'error')
        lblUpdateMessage.style.color = "Red";
    else
        lblUpdateMessage.style.color = "#568bbd";

    lblUpdateMessage.innerHTML = '<i>' + msg + '</i>';
}

function showUploadRow() {
    $('#rowUploadFile2').css('display', 'block');
}


function uploadImage() {
    var t = document.getElementById('attachments');
}
function ConfirmResize() {
    return confirm('Your image will be resized. Do you wish to proceeed?');
}

//Show the Div used for inserting links to the content
function showSelectionDiv(obj) {
    createOverlay('');
    $('#' + obj).fadeIn("slow");
}

//Hides the over lay div
function hideOverlay(div) {
    $('#overlay').fadeOut('fast');
    if (div != '')
        $('#' + div).fadeOut('fast');
}

function showTopLink(id) {

    var divTopLink = document.getElementById('topLinkDiv');
    var divLinks = document.getElementById('linksDiv');

    if (divTopLink != null) {
        $('#topLinkDiv').remove();
    }
    if (divLinks != null) {
        $('#linksDiv').remove();
    }
    var divContent = document.getElementById(divLitContent);

    var divHeight = $('#' + divContent.id).innerHeight(); //the height of the content.

    var d = document.getElementById(divPrev);

    var linksDiv = '<div id="linksDiv" class="right navLinks">';

    var contents = {
        type: "POST",
        url: "ViewArticle2.aspx/wmGetNextChapter",
        data: "{'ChaptId':'" + id + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            if (msg.d[0] != null) {
                var next = '<a href="#" id="PrevLinkDiv" onclick="loadChapter(\'' + msg.d[0] + '\')"><< Previous Chapter</a>&nbsp;&nbsp;';
                linksDiv += next;
            }

            //add Top link
            if (divHeight > 508) {
                $('#wrapper').prepend('<a name="top"></a>');
                linksDiv += '<a href="#top">[Back to Top]</a>&nbsp;&nbsp;';
            }

            if (msg.d[1] != null) {
                var prev = '<a href="#" id="NextLinkDiv" onclick="loadChapter(\'' + msg.d[1] + '\')">Next Chapter >></a>';
                linksDiv += prev;
            }
            linksDiv += '</div>';
            $('#' + d.id).remove('#linksDiv');
            $('#' + d.id).append(linksDiv);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert(xhr.status);
            //alert(thrownError);
        }
    };
    $.ajax(contents);
}

function addSubChaptersStyle() {
    //adding jquery to show the subchapters.
    var senderID = '';
    var togg = false;

    $('div.chapterReorderList').click(function (event) {
        var id = $(event.target).attr('id');
        if (senderID != id) {
            senderID = id;
            togg = true;
        }
        else {
            togg = false;
        }
    }).toggle(function () {
        if (togg == true) {
            $('div.subChapList').slideUp('normal');
            var x = $(this).next().children();
            //dont slide if children are not present -  prevents the glitch that occurs on click.
            if (x.length != 0) {
                $(this).next().slideDown('normal');
            }
        }
    }, function () {
        if (togg == true) {
            $('div.subChapList').slideUp('normal');
            var x = $(this).next().children();
            if (x.length != 0) {
                $(this).next().slideDown('normal');
            }
        }
    });
}

function hideTOCEditor(visible, type) {
    if (visible == true) {
        $('#' + 'tocDiv' + type).animate({ width: '10px' }, "medium");
        $('#' + 'tocItems' + type).hide("fast");
        $('#' + 'lblTOCHeader' + type).hide("fast");
        $('#' + 'btnHideToc' + type).css('margin-left', '14px');
        $('#' + 'btnHideToc' + type).html('>>');
        if (type == 'Edit') {
            $('#' + document.getElementById(txtEdit).id).animate({ width: '100%' }, "slow");
            $('#' + document.getElementById(divEditor).id).css('margin-left', '72px');
        }
        else {
            $('#' + document.getElementById(divLitPreview).id).animate({ width: '100%' }, "slow");
            $('#' + document.getElementById(divPrev).id).css('margin-left', '73px');
        }
    }
    else {
        //$('#tocDiv' + type).animate({ width: '300px' }, "medium");
        $('#tocItems' + type).show("fast");
        $('#lblTOCHeader' + type).show("fast");
        //$('#btnHideToc' + type).css('margin-left', '120px');
        $('#btnHideToc' + type).html('<<');
        if (type == 'Edit') {
            $('#' + document.getElementById(divEditor).id).css('margin-left', '315px');
        }
        else {
            $('#' + document.getElementById(divLitPreview).id).animate({ width: '100%' }, "slow");
            $('#' + document.getElementById(divPrev).id).css('margin-left', '300px');
        }
    }
}

function toggleTOC() {
    $('#btnHideTocEdit').click().toggle(function () {
        hideTOCEditor(true, 'Edit');
    }, function () {
        hideTOCEditor(false, 'Edit');
    });
    $('#btnHideTocPrev').click().toggle(function () {
        hideTOCEditor(true, 'Prev');
    }, function () {
        hideTOCEditor(false, 'Prev');
    });
}


function addChap_Enter(sender, eventArgs) {
    if (eventArgs.get_keyCode() == 13) {
        addChapter();
    }
}

function setSwitchMode() {
    SwitchMode = getMode();
}

//Editor Functions
function onTabSelected(sender, args) {
    var tabName = args.get_tab().get_text();
    switchModes(tabName);
    SwitchMode = tabName;
    if (tabName == 'Preview') {
        enableTimer(false);
    }
}

function OnClientPasteHtml(sender, args) {

    var commandName = args.get_commandName();
    var value = args.get_value();
    //When a table is inserted. Set it to use the default style.
    if (commandName == "InsertTable") {

        //Set border to the inserted table elements
        var div = document.createElement("DIV");

        //Remove extra spaces from begining and end of the tag
        value = value.trim();

        Telerik.Web.UI.Editor.Utils.setElementInnerHtml(div, value);
        var table = div.firstChild;

        $(table).addClass("standardTable");
        //$(table).append('<br/>');
        //Set new content to be pasted into the editor 
        args.set_value('<br/>' + div.innerHTML + '<br/>');

    }
    else {
        //remove Anchor tags if pasting from external document.
        if (value.toLowerCase().indexOf('viewarticle2') != -1 ||
            value.toLowerCase().indexOf('viewuploaded') != -1)
            value = formatHTML(value, false);
        else if (value.indexOf('loadChapter') != -1)
            value = formatHTML(value, false);
        else
            value = formatHTML(value, true);

        args.set_value(value);
    }
}

function OnClientCommandExecuting(editor, args) {
    //The command name
    var commandName = args.get_commandName();
    //The tool that initiated the command
    var tool = args.get_tool();
    //The selected value [if command is coming from a dropdown]
    var value = args.get_value();
}

//RadGrid Functions
function createCreationRow(grid, catIdUri, type) {
    var g = document.getElementById(grid.id); //the gridis lost on postback so find it again.
    var gridDivChild = $(g).children();

    var colNum = $("#" + grid.id + " tr:not(:has(td))").children().filter(":not(*:hidden)").length;

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
                                $(this).before('<tr class="createRowTR"><td colspan=' + colNum + '><a href=' + catIdUri +
                                    '><img alt="Create ' + type + '" src="../App_Themes/Default/images/add.png"/>Create ' + type + '</a></td></tr>');
                            }
                        }
                    });
                }
            });
        }
    });
}

function checkEditAccessCat(gridDiv, url, typ, catId) {
    url = url + ".aspx";
    var contents = {
        type: "POST",
        url: url + "/wmCheckEditAccess" + "?id=" + catId,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d != null) {
                var m = msg.d["Access"];
                if (m == true) {
                    createCreationRow(gridDiv, msg.d["URL"].toString(), typ);
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert(xhr.status);
            //alert(thrownError);
        }
    };
    $.ajax(contents);
}

function checkEditAccess(gridDiv, url, typ) {
    url = url + ".aspx";
    var contents = {
        type: "POST",
        url: url + "/wmCheckEditAccess",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d != null) {
                var m = msg.d["Access"];
                if (m == true) {
                    var catId = checkCategory();
                    if (catId == '') {
                        createCreationRow(gridDiv, msg.d["URL"].toString(), typ);
                    }
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert(xhr.status);
            //alert(thrownError);
        }
    };
    $.ajax(contents);
}

function checkCategory() {
    var catId = '';
    var w = window.location.href;

    if (w.indexOf('default') != -1) {
        catId = 'found';
    }
    return catId;
}

function getMode() {
    var mode = '';
    var w = window.location.href;
    if (w.indexOf('ViewArticle2') != -1) {
        var queryVars = w.substring(w.indexOf('?') + 1);
        if (queryVars.indexOf('cMode=pub') != -1)
            mode = 'Preview';
        else
            mode = 'Edit';
    }
    return mode;
}