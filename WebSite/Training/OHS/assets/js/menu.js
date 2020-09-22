////////////////////////////////////////////////////////////////////////////////////////////////
//		Menu scripts are used in tandem with interface.js as it uses key variables and functions.
//		IT WILL NOT WORK WITHOUT INTERFACE.JS
//		It works by searching for the ids embedded in the menu - "markerM01T02P003" "mainM01T02P003" "subM01T02P003"
//
//////////////////////////////////////////////////////////////////////////////////////////////////
//Note this function must be used with the myMenu, pagelist, currentpage variables in interface.js
var lastHighlightedElement = ""; //previous highlighted element

function updateMenu()
{
	var name = parent.pagelist[currentpage];
	var module = name.substring(0,3);
	var topic = name.substring(3,6);
	//collapseAllBut(topic);
	highlightCurrentPage(name);
	
}


//hides the element based on if is "name" or "sub"
function hideElement(element) 
{
	try {
		document.getElementById(element).style.display = "none";
	} catch (e) { /*element does not exists or isnt indexed*/  }
}

function showElement(element) 
{
	try {
  		document.getElementById(element).style.display = "block";
	} catch (e) { /*element does not exists or isnt indexed*/  }
}

function highlightElement(element) 
{
	try {
		document.getElementById(element).style.color = "#0077B9";
  		document.getElementById(element).style.fontWeight = "bold";
	} catch (e) { /*element does not exists or isnt indexed*/  }
}

function unhighlightElement(element)
{
	try {
		document.getElementById(element).style.fontWeight = "normal";
		//document.getElementById(element).style.color = "#4FB958";
	} catch (e) { /*element does not exists or isnt indexed*/  }
}

function boldMenu(element)
{
	try {
		document.getElementById(element).style.fontWeight = "bold";
		document.getElementById(element).style.color = "#0077B9";
	} catch (e) { /*element does not exists or isnt indexed*/  }
}

function hideAllSublinks()
{
	var elements = document.getElementsByName("submenu");
	for (q=0; q<elements.length; q++)
	{
		elements[q].style.display = "none";
	}
}

function highlightCurrentPage(name)
{
	var page = name.substring(6,10);
	var element = "";
	if (page != "P001")
	{
		element = "sh"+name;
	} else
	{
		element = "main"+name;
	}
	try {
		document.getElementById(element).style.color = "#009933";
		document.getElementById(element).style.fontWeight = "bold";
		lastHighlightedElement = element;
	} catch (e) { /*element does not exists or isnt indexed*/
		document.getElementById(lastHighlightedElement).style.color = "#009933";
		document.getElementById(lastHighlightedElement).style.fontWeight = "bold"; 
		}
}


//Hides all topic sublinks other than current topic
function collapseAllBut(topic)
{
	
	for(g=0; g<parent.pagelist.length; g++)
	{
		name = parent.pagelist[g];
		var top = name.substring(3,6);
		var page = name.substring(6,10);
		
		//bold all topics
		if (page == "P001")
		{
			boldMenu("main"+name); 
		}
		
		if (top != topic) // not the current topic hide all sub tags
		{
			if (page != "P001") //if not index page //SHOULD BE ANOTHER WAY TO DO THIS
			{
				//alert("sh"+name);
				hideElement("sub"+name);
			} 
		} else if (top == topic) //current topic
		{
			if (page != "P001") //if not index
			{
				//alert("sh"+name);
				unhighlightElement("sh"+name); 
				showElement("sub"+name);
			} else //is index so highlight
			{
				//alert("main"+name);
				highlightElement("main"+name);
			}
		}
	}
}


/*
function expandcollapse(style, menuId) {
	if (style=="block") {
		document.getElementById("marker" + menuId).innerHTML = menuCollapseMarker;
	} else {
		document.getElementById("marker" + menuId).innerHTML = menuExpandMarker;
	}
	//document.getElementById(menuId).style.color = "#9D58A7";
	//document.getElementById(menuId).style.fontWeight = "bold";
	
        for(i=0;i<parent.pagelist.length;i++){
		if(parent.pagelist[i] == menuId) {
			for(j=i+1;j<parent.pagelist.length;j++){
				if(document.getElementById(parent.pagelist[j])) {
					for(k=i;k<j;k++){
						if(document.getElementById("sub" + parent.pagelist[k])) {
							document.getElementById("sub" + parent.pagelist[k]).style.display = style;
						}
					}
					return;
				}
			}
		}
	}
}
*/

/*
function UpdateMenu(myMenuType) {
        switch(myMenuType) {
          case("expandcollapsesubmenu"):
               menuType = "expandcollapsesubmenu";
               menuExpandMarker = "+ ";
               menuCollapseMarker = "- ";
               menuTrigger[0] = "marker"
               menuEvent[0] = "onclick"
               break;
          case("dropdownsubmenu"):
               menuType = "dropdownsubmenu";
               menuExpandMarker = "+ ";
               menuCollapseMarker = "- ";
               menuTrigger[0] = "marker"
               menuTrigger[1] = ""
               menuEvent[0] = "onmouseover"
               menuEvent[1] = "onfocus"
               break;
          case("progressivesubmenu"):
		  	   alert("hello");
               menuType = "progressivesubmenu";
               menuExpandMarker = "";
               menuCollapseMarker = "";
               break;
          case("nosubmenu"):
               menuType = "nosubmenu";
               menuExpandMarker = "";
               menuCollapseMarker = "";
               break;
          default:
               menuExpandMarker = "";
               menuCollapseMarker = "";
               break;
         }
		
        //init user interaction to expand menu
        for(i=0;i<menuEvent.length;i++){
             for(j=0;j<parent.pagelist.length;j++){
                  for(k=0;k<menuTrigger.length;k++){
                       if(document.getElementById(menuTrigger[k] + parent.pagelist[j])) {
	                    switch(menuEvent[i]) {
		                 case("onmouseover"):
                                      document.getElementById(menuTrigger[k] + parent.pagelist[j]).onmouseover = function() {menuAction(this.id.substr(this.id.length-10,10),menuType)};
                                      break;
                                 case("onfocus"):
                                      document.getElementById(menuTrigger[k] + parent.pagelist[j]).onfocus = function() {menuAction(this.id.substr(this.id.length-10,10),menuType)};
                                      break;
                                 case("onmousedown"):
                                      document.getElementById(menuTrigger[k] + parent.pagelist[j]).onmousedown = function() {menuAction(this.id.substr(this.id.length-10,10),menuType)};
                                      break;
                                 case("onmouseup"):
                                      document.getElementById(menuTrigger[k] + parent.pagelist[j]).onmouseup = function() {menuAction(this.id.substr(this.id.length-10,10),menuType)};
                                      break;
                                 case("onclick"):
                                      document.getElementById(menuTrigger[k] + parent.pagelist[j]).onclick = function() {menuAction(this.id.substr(this.id.length-10,10),menuType)};
                                      break;
                                 case("ondblclick"):
                                      document.getElementById(menuTrigger[k] + parent.pagelist[j]).ondblclick = function() {menuAction(this.id.substr(this.id.length-10,10),menuType)};
                                      break;
                                 case("onmouseout"):
                                      document.getElementById(menuTrigger[k] + parent.pagelist[j]).onmouseout = function() {menuAction(this.id.substr(this.id.length-10,10),menuType)};
                                      break;
                                 case("onblur"):
                                      document.getElementById(menuTrigger[k] + parent.pagelist[j]).onblur = function() {menuAction(this.id.substr(this.id.length-10,10),menuType)};
                                      break;
                                // default():
                                      //break;
                             }
                        }
                  }
             }
        }
        
        // init page turn menu features
        hideMenu("sub")
        switch (menuType) {
            case("expandcollapsesubmenu"):
                 //add submenu markers
                 for(i=0;i<parent.pagelist.length;i++){
                   if(document.getElementById(parent.pagelist[i])){
                        if(document.getElementById("sub" + parent.pagelist[i])){
                             document.getElementById("marker" + parent.pagelist[i]).innerHTML = menuExpandMarker;
                        }
                   }
                 }

                 //start at current page and count backwards until the previous menu item is found
                 for(i=parent.currentpage;i!=0;i--) {
			        if(document.getElementById(parent.pagelist[i])) {
						document.getElementById(parent.pagelist[i]).style.color = "#9D58A7";
						document.getElementById(parent.pagelist[i]).style.fontWeight = "bold";
						//now find the next menu item
                        for(j=i+1;j<parent.pagelist.length;j++){
                        	if(document.getElementById(parent.pagelist[j])) {
                            //now show each submenu item between i and j
                            	for(k=i;k<j;k++){
                                	if(document.getElementById("sub" + parent.pagelist[k])) {
                                    	document.getElementById("sub" + parent.pagelist[k]).style.display = "block";
			                            document.getElementById("marker" + parent.pagelist[i]).innerHTML = menuCollapseMarker;
			                        }
			                    } return;
			                }
			        	}
			        	return;
					}
                 }


            case("dropdownsubmenu"):
                 //add submenu markers
                 for(i=0;i<parent.pagelist.length;i++){
                   if(document.getElementById(parent.pagelist[i])){
                        if(document.getElementById("sub" + parent.pagelist[i])){
                             document.getElementById("marker" + parent.pagelist[i]).innerHTML = menuExpandMarker;
                        }
                   }
                 }

                 //start at current page and count backwards until the previous menu item is found
                 for(i=parent.currentpage;i!=0;i--) {
			        if(document.getElementById(parent.pagelist[i])) {
						document.getElementById(parent.pagelist[i]).style.color = "#9D58A7";
						document.getElementById(parent.pagelist[i]).style.fontWeight = "bold";
						//now find the next menu item
                        for(j=i+1;j<parent.pagelist.length;j++){
                        	if(document.getElementById(parent.pagelist[j])) {
                            //now show each submenu item between i and j
                            	for(k=i;k<j;k++){
                                	if(document.getElementById("sub" + parent.pagelist[k])) {
                                    	document.getElementById("sub" + parent.pagelist[k]).style.display = "block";
			                            document.getElementById("marker" + parent.pagelist[i]).innerHTML = menuCollapseMarker;
			                        }
			                    } return;
			                }
			        	}
			        	return;
					}
                 }

                 /*if(document.getElementById("sub" + parent.pagelist[parent.currentpage])) {
		for(i=parent.currentpage;i>0;i--) {
			if(document.getElementById(parent.pagelist[i])) {
				expandcollapse("block",parent.pagelist[i]);
				return;
			}
		}
	} else {
		for(i=parent.currentpage;i!=0;i--) {
			if(document.getElementById(parent.pagelist[i])) {
				document.getElementById(parent.pagelist[i]).style.color = "#9D58A7";
				document.getElementById(parent.pagelist[i]).style.fontWeight = "bold";
				
				for(j=0;j<parent.pagelist.length;j++){
					if(parent.pagelist[j] == parent.pagelist[i]) {
						for(k=j+1;k<parent.pagelist.length;k++){
							if(document.getElementById(parent.pagelist[k])) {
								for(l=j;l<k;l++){
									if(document.getElementById("sub" + parent.pagelist[l])) {
										document.getElementById("sub" + parent.pagelist[l]).style.display = "block";
										document.getElementById("marker" + parent.pagelist[i]).innerHTML = "- ";
									        //document.getElementById("sh" + parent.pagelist[l]).style.fontWeight = "normal";
										//document.getElementById("sh" + parent.pagelist[l]).style.color = "#C79DCC";
									}
								}
							return;
							}
						}
					}
				}
				
				for(j=0;j<parent.pagelist.length;j++){
					if(document.getElementById("sub" + parent.pagelist[j])) {
						document.getElementById("sub" + parent.pagelist[j]).style.display = "none";
					}
				}
				return;
			}
			
		}
	} 
                 break;
            case("progressivesubmenu"):
                 //start at current page and count backwards until the previous menu item is found
                 for(i=parent.currentpage;i!=0;i--) {
			if(document.getElementById(parent.pagelist[i])) {
				document.getElementById(parent.pagelist[i]).style.color = "#9D58A7";
				document.getElementById(parent.pagelist[i]).style.fontWeight = "bold";
                                //now find the next menu item
                                for(j=i+1;j<parent.pagelist.length;j++){
                                     if(document.getElementById(parent.pagelist[j])) {
                                          //now show each submenu item between i and j
                                          for(k=i;k<j;k++){
                                               if(document.getElementById("sub" + parent.pagelist[k])) {
			                            document.getElementById("sub" + parent.pagelist[k]).style.display = "block";
			                       }
			                  } return;
			             }
			        }
			        return;
			}
                 }
                 //get page number
                 //for(i=0;i<parent.pagelist.length;i++) {
                 //     if(document.getElementById("pid").title == i) {
                 //          //menuAction(parent.pagelist[i], menuType)
                 //          alert("pid method: " + parent.pagelist[i]);
                 //          alert("current page method: " + parent.pagelist[parent.currentpage]);
                 //     }
                 //}
                 break;
            case("nosubmenu"):

                 break;
            case("nomenu"):

                 break;
        }
}

*/



