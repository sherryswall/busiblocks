
//show the admin columns and hide the default ones on admin bar click.
var ShowAdminClick = function (columnsToHide, columnsToShow) {
    ShowAdminTasks(columnsToHide, columnsToShow, "Show", "Hide");
}
//show the default columns and hide the admin ones on admin bar click.
var HideAdminClick = function (columnsToHide, columnsToShow) {
    HideAdminTasks(columnsToHide, columnsToShow, "Show", "Hide");
}

//showing the admin tasks.set admin state to Zero(true)
var ShowAdminTasks = function (ColumnsToHide, ColumnsToShow, ShowButtonColumn, HideButtonColumn) {
    ToggleAdminTasks(ColumnsToHide, ColumnsToShow, ShowButtonColumn, HideButtonColumn);
    $("#" + adminState).val(1);
}

//hide the admin tasks. set admin state to Zero(false)
var HideAdminTasks = function (ColumnsToHide, ColumnsToShow, ShowButtonColumn, HideButtonColumn) {
    ToggleAdminTasks(ColumnsToHide, ColumnsToShow, HideButtonColumn, ShowButtonColumn);
    $("#" + adminState).val(0);
}

//toggling the display of tasks
var ToggleAdminTasks = function (ColumnsToHide, ColumnsToShow, ButtonColumnToHide, ButtonColumnToShow) {
    ExpandColumn(ButtonColumnToHide);
    for (var i = 0; i < ColumnsToHide.length; i++)
        HideColumn(ColumnsToHide[i]);
    HideColumn(ButtonColumnToHide);
    for (var i = 0; i < ColumnsToShow.length; i++)
        ShowColumn(ColumnsToShow[i]);
    ShowColumn(ButtonColumnToShow);
    ContractColumn(ButtonColumnToShow);
}


function ExpandColumn(ColumnName) {
    var RadGridName = $find(radGrid);
    var masterTable = RadGridName.get_masterTableView();
    for (var rowIndex = 0; rowIndex < masterTable.get_dataItems().length; rowIndex++) {
        var cell = masterTable.getCellByColumnUniqueName(masterTable.get_dataItems()[rowIndex], ColumnName)
        if (cell != null) {
            cell.rowSpan = 1;
            cell.style.display = ''
        }
    }
}

function ContractColumn(ColumnName) {
    var RadGridName = $find(radGrid);
    var masterTable = RadGridName.get_masterTableView();
    for (var rowIndex = 0; rowIndex < masterTable.get_dataItems().length; rowIndex++) {
        var cell = masterTable.getCellByColumnUniqueName(masterTable.get_dataItems()[rowIndex], ColumnName)
        if (cell != null) {
            if (rowIndex == 0) {
                cell.rowSpan = RadGridName.get_masterTableView().get_pageSize();
                cell.style.display = '';
            }
            else {
                cell.rowSpan = 1;
                cell.style.display = 'none'
            }
        }
    }
}

//shows the column
function ShowColumn(columnName) {
    var RadGridName = $find(radGrid);
    var column = RadGridName.get_masterTableView().getColumnByUniqueName(columnName);
    if (column != null) {
        var columnIndex = column.get_element().cellIndex
        RadGridName.get_masterTableView().showColumn(columnIndex);
    }
}
//hides the column
function HideColumn(columnName) {
    var RadGridName = $find(radGrid);
    var column = RadGrid1.get_masterTableView().getColumnByUniqueName(columnName);
    if (column != null) {
        var columnIndex = column.get_element().cellIndex
        RadGridName.get_masterTableView().hideColumn(columnIndex);
    }
}