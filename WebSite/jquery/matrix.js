

var ImagePath = "../App_Themes/Default/icons/";
var ImageButtonYes = "yes24.png";
var ImageButtonNo = "no24.png";
var ImageButtonYesGrey = "yesGrey24.png";

var ClassButtonYes = "btnYes";
var ClassButtonNo = "btnNo";

var CreateButton = function (yes, columnId, rowId, enabled, UrlDeleteData, UrlCreateData) {
    var button = $('<input>')
                .attr('id', columnId + rowId)
                .attr('type', 'image')
                .attr('class', ((!yes) ? ClassButtonNo : (enabled ? ClassButtonYes : "")))
                .attr('style', 'border-width:0px;')
                .attr('src', ImagePath + ((!yes) ? ImageButtonNo : (enabled ? ImageButtonYes : ImageButtonYesGrey)));
    if (!enabled) {
        button.attr("disabled", "disabled");
        button.attr("style", "cursor: no-drop;");
    }
    if (yes) {
        button.attr("onclick", YesOnClickString(columnId, rowId, UrlDeleteData, UrlCreateData));
    }
    else {
        button.attr("onclick", NoOnClickString(columnId, rowId, UrlDeleteData, UrlCreateData));
    }

    return button;
};

var CreateYesButton = function (columnId, rowId, enabled, UrlDeleteData, UrlCreateData) {
    return CreateButton(true, columnId, rowId, enabled, UrlDeleteData, UrlCreateData);
};

var CreateNoButton = function (columnId, rowId, enabled, UrlDeleteData, UrlCreateData) {
    return CreateButton(false, columnId, rowId, enabled, UrlDeleteData, UrlCreateData);
};

var ToggleButton = function (columnId, rowId, yes, UrlDeleteData, UrlCreateData) {

    var button = $("#" + columnId + rowId);

    if (yes) {
        button.attr('class', ClassButtonYes).attr('src', ImagePath + ImageButtonYes);
        button.attr("onclick", YesOnClickString(columnId, rowId, UrlDeleteData, UrlCreateData));
    }
    else {
        button.attr('class', ClassButtonNo).attr('src', ImagePath + ImageButtonNo);
        button.attr("onclick", NoOnClickString(columnId, rowId, UrlDeleteData, UrlCreateData));
    }
}

var YesOnClickString = function (columnId, rowId, UrlDeleteData, UrlCreateData) {

    return "javascript:RemoveData(\"" + columnId + "\", \"" + rowId + "\", \"" + UrlDeleteData + "\", \"" + UrlCreateData + "\"); return false;";
}

var NoOnClickString = function (columnId, rowId, UrlDeleteData, UrlCreateData) {

    return "javascript:CreateData(\"" + columnId + "\", \"" + rowId + "\", \"" + UrlDeleteData + "\", \"" + UrlCreateData + "\"); return false;";
}

var DisableButton = function (buttonId) {
    var button = $("#" + buttonId);
    button.attr("disabled", "disabled");
    /* disabled cursor change - IE has a known bug and wont change cursor until mouse is moved
    button.attr("style", "cursor: no-drop;"); */
}

var EnableButton = function (buttonId) {
    var button = $("#" + buttonId);
    button.removeAttr("disabled");
    /* disabled cursor change - IE has a known bug and wont change cursor until mouse is moved
    button.attr("style", "cursor: hand"); */
}


var CreateData = function (columnId, rowId, UrlDeleteData, UrlCreateData) {

    DisableButton(columnId + rowId)

    var ids = { "columnId": columnId, "rowId": rowId };
    ids = JSON.stringify(ids);

    $.ajax({
        type: 'POST',
        url: UrlCreateData,
        data: ids,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            HandleSuccess(columnId, rowId, msg.d, UrlDeleteData, UrlCreateData);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert(xhr.status);
            //alert(thrownError);
        }
    });
};

var RemoveData = function (columnId, rowId, UrlDeleteData, UrlCreateData) {

    DisableButton(columnId + rowId)

    var ids = { "columnId": columnId, "rowId": rowId };

    ids = JSON.stringify(ids);

    $.ajax({
        type: 'POST',
        url: UrlDeleteData,
        data: ids,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            var dataExists = (msg.d["exists"] == "true");
            HandleSuccess(columnId, rowId, dataExists, UrlDeleteData, UrlCreateData);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert(xhr.status);
            //alert(thrownError);
        }
    });
}

var HandleSuccess = function (columnId, rowId, yes, UrlDeleteData, UrlCreateData) {
    ToggleButton(columnId, rowId, yes, UrlDeleteData, UrlCreateData);
    EnableButton(columnId + rowId)
}

var BuildDataRow = function (rowData, columnData, data, tableContainerId, UrlDeleteData, UrlCreateData, disabledCollection, actionList) {

    var rowDataId = rowData["Id"];
    var rowDataName = rowData["Name"];

    var firstColumn = $("<td>").attr("class", "datafirst").attr("id", rowDataId);

    firstColumn.append($("<span>").html(rowDataName).attr("title", rowDataName));

    var newRow = $("<tr>").append(firstColumn);
    $.each(columnData, function (key1, columnDataItem) {

        var columnId = columnDataItem["Id"];
        var columnName = columnDataItem["Name"];
        var dataExists = false;

        $.each(data, function (key2, dataItem) {

            var tempRow = dataItem["rowData"];
            var tempColumn = dataItem["columnData"];

            if (tempColumn["Id"] == columnId && tempRow["Id"] == rowDataId) {
                dataExists = true;
            }
        });

        var disabled = (disabledCollection["columnName"].toUpperCase() == columnName.toUpperCase() && disabledCollection["rowName"].toUpperCase() == rowDataName.toUpperCase());

        if (dataExists) {
            button = CreateYesButton(columnId, rowDataId, !disabled, UrlDeleteData, UrlCreateData);
        }
        else {
            button = CreateNoButton(columnId, rowDataId, !disabled, UrlDeleteData, UrlCreateData);
        }

        newRow.append($("<td class='center'>").append(button));
    });

    var actionhtml = "";

    $.each(actionList, function (text, action) {

        var actionlink = "<a ";

        $.each(action, function (attribute, value) {
            actionlink += attribute + "='" + value + "' ";
        });

        actionlink = actionlink.replace("[id]", rowDataId);

        actionlink += ">";
        actionlink += text;
        actionlink += "</a>&nbsp;&nbsp;";

        actionhtml += actionlink;

    });

    newRow.append($("<td>").append(actionhtml));

    $("#" + tableContainerId).append(newRow);
};


var BuildHeaderRow = function (columnData, TableContainerId, TopLeftText) {

    var firstHeaderCell = $('<th>').html(TopLeftText).attr("class", "headfirst");

    $("#" + TableContainerId).append($('<tr id=\"trFirstRow\">').append(firstHeaderCell));

    $.each(columnData, function (key, columnDataItem) {

        var name = columnDataItem["Name"];
        var id = columnDataItem["Id"];

        var headerCell = ($('<th>').html(name).attr("title", name.toUpperCase()).attr("id", id));

        $("#trFirstRow").append(headerCell);
    });
    //add actions column.
    var headerActionsCell = ($('<th>').html("Action").attr("title", "Actions"));
    $("#trFirstRow").append(headerActionsCell);
};


var BuildCreationRow = function (TableContainerId) {
    var colNum = $("#" + TableContainerId + " tr:not(:has(td))").children().filter(":not(*:hidden)").length;
    $("#" + TableContainerId).append('<tr class="createRowTR" id=\"trCreationRow\">');
    $("#trCreationRow").append('<td colspan=' + colNum + '><a href=\"CreateGroup.aspx\"><img alt=\"Create Group\" src=\"../App_Themes/Default/images/add.png\"/>Create Group</a></td>');
};


var BuildMatrix = function (UrlGetData, UrlDeleteData, UrlCreateData, TableContainerId, TopLeftText, disabledCollection, actionList) {
    $.ajax({
        type: 'POST',
        url: UrlGetData,
        data: '{}',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',

        success: function (msg) {
            var columnData = msg.d["columns"];
            var rowData = msg.d["rows"];
            var data = msg.d["data"];

            BuildHeaderRow(columnData, TableContainerId, TopLeftText);
            BuildCreationRow(TableContainerId);

            $.each(rowData, function (key, row) {
                BuildDataRow(row, columnData, data, TableContainerId, UrlDeleteData, UrlCreateData, disabledCollection, actionList);
            });
        },

        error: function (xhr, ajaxOptions, thrownError) {
            //alert(xhr.status);
            //alert(thrownError);
        }
    });
};