
function confirmMoveItem(source, dest, type) {
    $('#overlay').fadeIn('fast');
    $('#divMove' + type).fadeIn('fast');

    $('#lblSourceNode').html(source);
    $('#lblDestNode').html(dest);

    if (source != dest) {
        $('#btnMove' + type).click(function () {
            if (type == 'Region') {
                WSTreeView.MoveRegion(source, dest);
                hideOverlay('divMove' + type);
                var treeView = $find(radTreeView);
                var node = treeView.findNodeByText(dest);
                alert(node.get_text());
                node.expand();
            }
            if (type == 'Category') {
                var treeView = $find(radTreeView);
                treeView.trackChanges();
                WSTreeView.MoveCategory(source, dest);
                hideOverlay('divMove' + type);
                var node = treeView.findNodeByText(dest);
                treeView.commitChanges();
                node.expand();
            }
        });
    }
    return false;
}

function expandTree() {
    var treeView = $find(radTreeView);
    var nodes = treeView.get_allNodes();
    for (var i = 0; i < nodes.length; i++) {
        if (nodes[i].get_nodes() != null) {
            nodes[i].expand();
        }
    }
}

function collapseTree() {
    var treeView = $find(radTreeView);
    var nodes = treeView.get_allNodes();
    for (var i = 0; i < nodes.length; i++) {
        if (nodes[i].get_nodes() != null) {
            nodes[i].collapse();
        }
    }
}

function nodeClicked(sender, args) {

    nodeName = args.get_node().get_text();
    nodeId = args.get_node().get_value();

    var selectedNode = args.get_node();

    var childrenNodes = selectedNode.get_allNodes();
    childrenNodes.splice(0, 0, selectedNode);

    if (!(typeof curSelNode === 'undefined')) {
        $('#' + curSelNodeVal).val(nodeId);
        $('#' + curSelNodeName).val(nodeName);
    }
    if (prevNodeId != '')
        $('.' + prevNodeId + '_tvContext').addClass('hideElement');

    prevNodeId = nodeId;
    $('.' + nodeId + '_tvContext').removeClass('hideElement');

    if (isGridOnPage == true) {
        var grid = $find(radGrid);
        if (!(typeof grid === 'undefined')) {
            var masterTableView = grid.get_masterTableView();
        }

        //filtering of grid on node click - only if grid is on page
        var filterExpression = '';
        if (!(typeof treeViewName === 'undefined')) {
            for (var i = 0; i < childrenNodes.length; i++) {
                if (i != (childrenNodes.length - 1)) {
                    filterExpression = filterExpression + childrenNodes[i].get_text() + ',';
                }
                else
                    filterExpression = filterExpression + childrenNodes[i].get_text();
            }

            if (!(typeof masterTableView === 'undefined')) {
                //invoke filter command. The first parameter is the unique name of the column. second is the custom filter value and third is the radgrid equivalent for filter function. 
                switch (treeViewType) {
                    case 'Region':
                        masterTableView.filter('RegionName', filterExpression, 17);
                        break;
                    case 'Category':
                        masterTableView.filter('Category', filterExpression, 17);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

function nodeDblClicked(sender, args) {
    nodeName = args.get_node().get_text();
    nodeId = args.get_node().get_value();

    if (!(typeof curSelNode === 'undefined')) {
        $('#' + curSelNodeVal).val(nodeId);
        $('#' + curSelNodeName).val(nodeName);
    }
    if (prevNodeId != '')
        $('#' + prevNodeId).fadeOut();

    prevNodeId = nodeId;
    $('#' + nodeId).show();

    args.get_node().expand();

    if (isGridOnPage == true) {
        var grid = $find(radGrid);
        if (!(typeof grid === 'undefined')) {
            var masterTableView = grid.get_masterTableView();
        }

        //filtering of grid on node click - only if grid is on page
        var filterExpression = '';
        if (!(typeof treeViewName === 'undefined')) {
            filterExpression = nodeName;
            if (!(typeof masterTableView === 'undefined')) {
                //invoke filter command. The first parameter is the unique name of the column. second is the custom filter value and third is the radgrid equivalent for filter function. 
                switch (treeViewType) {
                    case 'Region':
                        masterTableView.filter('RegionName', filterExpression, 17);
                        break;
                    case 'Category':
                        masterTableView.filter('Category', filterExpression, 17);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

function setFilter(filterValue) {
    var masterTable = $find(radGrid).get_masterTableView();
    if (treeViewType == 'Region') {
        masterTable.filter("RegionName", filterValue, Telerik.Web.UI.GridFilterFunction.Contains, true);
    }
    else if (treeViewType == 'Category') {
        if (treeViewName == 'Doco') {
            masterTable.filter("Category", filterValue, Telerik.Web.UI.GridFilterFunction.Contains, true);
        }
        else if (treeViewName == 'News') {
            masterTable.filter("Category.DisplayName", filterValue, Telerik.Web.UI.GridFilterFunction.Contains, true);
        }
    }
}

function clearFilter() {
    var masterTable = $find(radGrid).get_masterTableView();
    masterTable.filter("RegionName", null, Telerik.Web.UI.GridFilterFunction.NoFilter, false);
}

function hideOverlay(div) {
    $('#overlay').fadeOut('fast');
    if (div != '')
        $('#' + div).fadeOut('fast');
}

function addSites(personId, locationId, name, type) {
    if (type == 'Region') {
        addRegionInfo(personId, locationId, unescape(name), type);
    }
    else {
        addSingleSite(personId, locationId, unescape(name), type);
    }
}

function addRegionInfo(personId, locationId, name, locationType) {
    var rowLength = $('#tr' + locationId.toString()).length;
    if (rowLength == 0) {
        var isPrimary = false;
        var rowCount = $('#tableSiteAccess tr').length;
        var classType = (locationType.toString() == "Region") ? "accessRegion" : "accessSite";
        var row = '<tr id=tr' + locationId.toString() + ' class=' + classType.toString() + ' name="trLocation"><td class="accessTitle">' + name;

        row += '<input type="hidden" name="hfIsPrimary" id="hfIsPrimary' + locationId.toString() + '" value="false" /></td>';

        //append default access info.
        row += '<td><img class="center" id="viewAccess' + locationId.toString() + '" src=../App_Themes/Default/icons/noDisabled24.png /></td>' +
        '<td class="hideElement"><img class="btnNo center"  id="viewManager' + locationId.toString() + '" onclick="changeStatus(\'#viewManager' + locationId.toString() + '\',\'' + personId.toString() + '\',\'' + locationId.toString() + '\',\'' + name + '\',\'manager\',\'' + manageUserType + '\',\'' + locationType + '\');" src=../App_Themes/Default/icons/no24.png /></td>' +
        '<td><img class="btnNo center"  id="viewAdmin' + locationId.toString() + '" onclick="changeStatus(\'#viewAdmin' + locationId.toString() + '\',\'' + personId.toString() + '\',\'' + locationId.toString() + '\',\'' + name + '\',\'admin\',\'' + manageUserType + '\',\'' + locationType + '\');" src=../App_Themes/Default/icons/no24.png /></td>';

        if (rowCount == 1)
            row += '<td><a href=# class="deleteitem" onclick=removeLocation(\'' + personId + '\',\'tr' + locationId.toString() + '\',\'' + manageUserType + '\',\'' + locationType + '\');>Remove</a></td></tr>';
        else
            row += '<td><a href=# class="deleteitem" onclick=removeLocation(\'' + personId + '\',\'tr' + locationId.toString() + '\',\'' + manageUserType + '\',\'' + locationType + '\');>Remove</a></td></tr>';

        $('#tableSiteAccess').append(row);
        $('#tableSiteAccess').fadeIn("slow");

        addLocationAccessDetails(personId, locationId, name, true, false, false, isPrimary, 'Add', manageUserType, locationType);
    }
}

function addSingleSite(personId, locationId, name, locationType) {
    //only add if the site is not there
    var rowLength = $('#tr' + locationId.toString()).length;

    if (rowLength == 0) {

        var isPrimary = false;
        var classType = (locationType.toString() == "Region") ? "accessRegion" : "accessSite";
        var row = '<tr id=tr' + locationId.toString() + ' class=' + classType.toString() + ' name="trLocation"><td class="accessTitle">' + name;

        //append primary site info
        if (primarySite == '') {
            row += '<img src="../App_Themes/Default/icons/star_16.png" id="imgPrimary' + locationId.toString() + '" name="imgPrimary" /><input type="hidden" name="hfIsPrimary" value="true" id="hfIsPrimary' + locationId.toString() + '" /></td>';
            isPrimary = true;
            primarySite = 'primary' + locationId.toString();
        }
        else {
            row += '<img src="../App_Themes/Default/icons/star_16.png" id="imgPrimary' + locationId.toString() + '" class="hideElement" /><input type="hidden" name="hfIsPrimary" id="hfIsPrimary' + locationId.toString() + '" value="false" /></td>';
            isPrimary = false;
        }
        //append default access info.
        row += '<td><img class="btnYes center" id="viewAccess' + locationId.toString() + '" onclick="changeStatus(\'#viewAccess' + locationId.toString() + '\',\'' + personId.toString() + '\',\'' + locationId.toString() + '\',\'' + name + '\',\'access\',\'' + manageUserType + '\',\'' + locationType + '\');" src=../App_Themes/Default/icons/yes24.png /></td>' +
        '<td class="hideElement"><img class="btnNo center" id="viewManager' + locationId.toString() + '" onclick="changeStatus(\'#viewManager' + locationId.toString() + '\',\'' + personId.toString() + '\',\'' + locationId.toString() + '\',\'' + name + '\',\'manager\',\'' + manageUserType + '\',\'' + locationType + '\');" src=../App_Themes/Default/icons/no24.png /></td>' +
        '<td><img class="btnNo center" id="viewAdmin' + locationId.toString() + '" onclick="changeStatus(\'#viewAdmin' + locationId.toString() + '\',\'' + personId.toString() + '\',\'' + locationId.toString() + '\',\'' + name + '\',\'admin\',\'' + manageUserType + '\',\'' + locationType + '\');" src=../App_Themes/Default/icons/no24.png /></td>';

        if (primarySite == 'primary' + locationId.toString())
            row += '<td><a href=# onclick="makeSitePrimary(\'' + locationId.toString() + '\',\'' + manageUserType + '\');" id="lnk' + locationId.toString() +
                '" style="display:none;" ><img src="../App_Themes/Default/icons/star_16.png" />Make Primary</a>&nbsp;<a href=# class="deleteitem" onclick=removeLocation(\'' + personId + '\',\'tr' +
                locationId.toString() + '\',\'' + manageUserType + '\',\'' + locationType + '\');>Remove</a></td></tr>';
        else
            row += '<td><a href=# onclick="makeSitePrimary(\'' + locationId.toString() + '\',\'' + manageUserType + '\');" id="lnk' + locationId.toString() +
            '" ><img src="../App_Themes/Default/icons/star_16.png" />Make Primary</a>&nbsp;<a href=# class="deleteitem" onclick=removeLocation(\'' + personId + '\',\'tr' + locationId.toString() +
            '\',\'' + manageUserType + '\',\'' + locationType + '\');>Remove</a></td></tr>';

        $('#tableSiteAccess').append(row);
        $('#tableSiteAccess').fadeIn("slow");
        addLocationAccessDetails(personId, locationId, name, true, false, false, isPrimary, 'Add', manageUserType, locationType);
    }
}

function updateSites(personId, locationId, name, isView, isAdmin, isManager, isPrimary, locationType) {
    var rowLength = $('#tr' + locationId.toString()).length;
    name = unescape(name);
    //if not existing then add
    if (rowLength == 0) {
        var classType = (locationType.toString() == "Region") ? "accessRegion" : "accessSite";
        var row = '<tr id=tr' + locationId.toString() + ' class=' + classType.toString() + ' name="trLocation"><td class="accessTitle">' + name;

        var viewClass = (locationType.toString() == "Region") ? '' : ((isView == "true") ? 'btnYes' : 'btnNo');
        var viewClassImg = (locationType.toString() == "Region") ? 'noDisabled24.png' : ((isView == "true") ? 'yes24.png' : 'no24.png');
        var viewOnClick = (locationType.toString() == "Region") ? '' : 'onclick="changeStatus(\'#viewAccess' + locationId + '\',\'' + personId + '\',\'' + locationId + '\',\'' + name + '\',\'access\',\'' + manageUserType + '\',\'' + locationType + '\');';

        var adminClass = (isAdmin == "true") ? 'btnYes' : 'btnNo';
        var adminClassImg = (isAdmin == "true") ? 'yes24.png' : 'no24.png';

        var managerClass = (isManager == "true") ? 'btnYes' : 'btnNo';
        var managerClassImg = (isManager == "true") ? 'yes24.png' : 'no24.png';

        if (isPrimary == "true") {
            row += '<img src="../App_Themes/Default/icons/star_16.png" id="imgPrimary' + locationId.toString() + '" name="imgPrimary" /><input type="hidden" name="hfIsPrimary" value="' + true + '" id="hfIsPrimary' + locationId.toString() + '" /></td>';
            primarySite = 'primary' + locationId.toString();
        }
        else
            row += '<img src="../App_Themes/Default/icons/star_16.png" id="imgPrimary' + locationId.toString() + '" style="display:none;" /><input type="hidden" name="hfIsPrimary" id="hfIsPrimary' + locationId.toString() + '" value="' + false + '" /></td>';

        row += '<td><img class="' + viewClass + ' center" id="viewAccess' + locationId.toString() + '" ' + viewOnClick + '" src=../App_Themes/Default/icons/' + viewClassImg + ' /></td>' +
        '<td class="hideElement"><img class="' + managerClass + ' center"  id="viewManager' + locationId.toString() + '" onclick="changeStatus(\'#viewManager' + locationId.toString() + '\',\'' + personId.toString() + '\',\'' + locationId.toString() + '\',\'' + name + '\',\'manager\',\'' + manageUserType + '\',\'' + locationType + '\');" src=../App_Themes/Default/icons/' + managerClassImg + ' /></td>' +
        '<td><img class="' + adminClass + ' center"  id="viewAdmin' + locationId.toString() + '" onclick="changeStatus(\'#viewAdmin' + locationId.toString() + '\',\'' + personId.toString() + '\',\'' + locationId.toString() + '\',\'' + name + '\',\'admin\',\'' + manageUserType + '\',\'' + locationType + '\');" src=../App_Themes/Default/icons/' + adminClassImg + ' /></td>';

        var makePrimaryHTML = '';
        if (isPrimary == "true") {
            if (locationType == 'Site' && primarySite == 'primary' + locationId.toString()) {
                makePrimaryHTML = '<a href=# onclick="makeSitePrimary(\'' + locationId.toString() + '\',\'' + manageUserType + '\');" id="lnk' + locationId.toString() + '" class="hideElement" ><img src="../App_Themes/Default/icons/star_16.png" />Make Primary</a>&nbsp;';
            }
            row += '<td>' + makePrimaryHTML + '<a href=# class="deleteitem" onclick=removeLocation(\'' + personId + '\',\'tr' + locationId.toString() + '\',\'' + manageUserType + '\',\'' + locationType + '\');>Remove</a></td></tr>';
        } else {
            if (locationType == 'Site')
                makePrimaryHTML = '<a href=# onclick="makeSitePrimary(\'' + locationId.toString() + '\',\'' + manageUserType + '\');" id="lnk' + locationId.toString() + '" ><img src="../App_Themes/Default/icons/star_16.png" />Make Primary</a>&nbsp;';
            row += '<td>' + makePrimaryHTML + '<a href=# class="deleteitem" onclick=removeLocation(\'' + personId + '\',\'tr' + locationId.toString() + '\',\'' + manageUserType + '\',\'' + locationType + '\');>Remove</a></td></tr>';
        }
        $('#tableSiteAccess').append(row);
        $('#tableSiteAccess').fadeIn("slow");
        $('#tableSiteAccess').removeClass('hideElement');

        addLocationAccessDetails(personId, locationId, name, isView, isAdmin, isManager, isPrimary, 'Update', manageUserType, locationType);
    }
}

function changeStatus(obj, personId, locationId, name, accessType, manageType, locationType) {
    if ($(obj).hasClass('btnYes')) {
        $(obj).removeClass('btnYes').addClass('btnNo');
        $(obj).removeAttr('src').attr('src', '../App_Themes/Default/icons/no24.png');
        switchAccessType(personId, locationId, name, false, accessType, manageType, locationType);
    }
    else {
        $(obj).removeClass('btnNo').addClass('btnYes');
        $(obj).removeAttr('src').attr('src', '../App_Themes/Default/icons/yes24.png');
        switchAccessType(personId, locationId, name, true, accessType, manageType, locationType);
    }
}

function switchAccessType(personId, locationId, name, value, accessType, manageType, locationType) {
    var isPrimary = $('#hfIsPrimary' + locationId).val();
    switch (accessType) {
        case "access":
            addLocationAccessDetails(personId, locationId, name, value, getCurrentStatus('viewAdmin' + locationId.toString()), getCurrentStatus('viewManager' + locationId.toString()), isPrimary, 'Update', manageType, locationType);
            break;
        case "admin":
            addLocationAccessDetails(personId, locationId, name, getCurrentStatus('viewAccess' + locationId.toString()), value, getCurrentStatus('viewManager' + locationId.toString()), isPrimary, 'Update', manageType, locationType);
            break;
        case "manager":
            addLocationAccessDetails(personId, locationId, name, getCurrentStatus('viewAccess' + locationId.toString()), getCurrentStatus('viewAdmin' + locationId.toString()), value, isPrimary, 'Update', manageType, locationType);
            break;
        default:
    }
}

function getCurrentStatus(obj) {
    var className = $('#' + obj).attr('class').split(' ');
    for (var i = 0; i < className.length; i++) {
        if (className[i] == 'btnYes') {
            return true;
        } else {
            return false;
        }
    }
}

function removeLocation(personId, locationId, type, locationType) {
    var rowId = locationId;
    //remove the tr prefix from id as that is only used for removing row on clientside.
    locationId = locationId.toString().replace('tr', '');

    //set primarySite variable to empty so it can check primary site.
    var primaryValue = $('#hfIsPrimary' + locationId).attr("value");
    if (primaryValue == 'true')
        primarySite = '';

    $('#' + rowId).remove();
    removeLocationAccessDetails(personId, locationId, type, locationType);
    updateNoLocationRow();   
}

function makeSitePrimary(locationId, type) {
    var lnkPrimary = '';
    var rows = $('#tableSiteAccess tr:gt(0)');
    var primID = '';
    var tempID = '';

    //go through the rows. Find primary site, hide the primary icon and show the makePrimary link
    rows.each(function (index) {
        var siteID = ($(this).attr("id").substring(2, $(this).attr("id").length));
        var hfPrimId = $(this).children("td").find("input").attr("value");
        if (hfPrimId == 'true') {
            $('#lnk' + siteID).show();
            $('#imgPrimary' + siteID).hide();
            $('#hfIsPrimary' + siteID).val(false);
        }
    });

    //remove the make primary link and show the primary site star for the site.
    $('#lnk' + locationId).hide();
    $('#imgPrimary' + locationId).fadeIn();
    $('#hfIsPrimary' + locationId).val(true);

    //set the primary site for validation purpose.
    primarySite = 'primary' + locationId;

    var msg = eval($('#' + listAllSitesDetails).val());
    listSites = new Array();
    for (var x = 0; x < msg.length; x++) {
        if (msg[x].locationId != locationId) {
            msg[x].isPrimary = false;
        }
        else {
            msg[x].isPrimary = true;
        }
        var data = "{'personId':'" + msg[x].personId + "','locationId':'" + msg[x].locationId + "','name':'" + msg[x].name + "','isView':'" + msg[x].isView + "','isAdmin':'" + msg[x].isAdmin + "','isManager':'" + msg[x].isManager + "','isPrimary':'" + msg[x].isPrimary + "','locationType':'Site'}";
        listSites.push(data);
    }
    var site = '[' + listSites.toString() + ']';
    $('#' + listAllSitesDetails).val(site);
}

function addLocationAccessDetails(personId, locationId, name, isView, isAdmin, isManager, isPrimary, action, manageType, locationType) {
    var data = "{'personId':'" + personId + "','locationId':'" + locationId + "','name':'" + escape(name) + "','isView':'" + isView + "','isAdmin':'" + isAdmin + "','isManager':'" + isManager + "','isPrimary':'" + isPrimary + "','locationType':'" + locationType + "'}";
    if (locationType == 'Site') {
        if (action == 'Add') {
            listSites.push(data);
        }
        else {
            removeLocationAccessDetails(personId, locationId, manageUserType, locationType);
            addLocationAccessDetails(personId, locationId, name, isView, isAdmin, isManager, isPrimary, 'Add', manageUserType, locationType);
        }
        var site = '[' + listSites.toString() + ']';
        $('#' + listAllSitesDetails).val(site);
    }
    else {
        listRegions.push(data);
        var region = '[' + listRegions.toString() + ']';
        $('#' + listRegionAccessDetails).val(region);
    }
    updateNoLocationRow();
}

function removeLocationAccessDetails(personId, locationId, type, locationType) {
    if (locationType == 'Site') {
        var msg = eval($('#' + listAllSitesDetails).val());

        for (var x = 0; x < msg.length; x++) {
            if (msg[x].locationId == locationId)
                listSites.splice(x, 1);
        }

        var site = '[' + listSites.toString() + ']';
        $('#' + listAllSitesDetails).val(site);
    }
    else {
        var msg = eval($('#' + listRegionAccessDetails).val());

        for (var x = 0; x < msg.length; x++) {
            if (msg[x].locationId == locationId)
                listRegions.splice(x, 1);
        }

        var site = '[' + listRegions.toString() + ']';
        $('#' + listRegionAccessDetails).val(site);
    }
}

function updateNoLocationRow() {
    if ($('#tableSiteAccess tr[name="trLocation"]').length == 0) {
        $('#trNoAccesses').show();
    }
    else {
        $('#trNoAccesses').hide();
    }
}
