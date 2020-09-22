// JavaScript Document
// STANDARD JS SCRIPTS FOR ADDING Interaction to a PAGE***************************************************
//******Interaction JS********************************/
//this function is used for a basic rollover
//id is the element being show or hidden, bHide is whether all other rollovers disappear
function HideElements(name)
{
	try 
	{
		var elements = document.getElementByName(name);
	
		for (i=0; i<elements.length; i++)
		{
			//alert(i);
			elements[i].style.display = "none";
		}
	} catch (e)
	{
		//no elements by that name
	}
}


function Show(id, type)
{
	//alert(type);
	if (type == "rollover" || type == "both")
	{
		try {
			document.getElementById(id).style.display = "block";
			
		} catch (e)
		{
			//no element found
		}
	}
}

function ClickShow(id, type)
{
	//if (isHideAll)
	//{
	HideAllClicks();
	//}
	
	
	if (type == "click" || type == "both")
	{
		id = "c" + id;
		try {
			if (document.getElementById(id).style.display == "block")
			{
				document.getElementById(id).style.display = "none";
			} else
			{
				document.getElementById(id).style.display = "block";
			}
		} catch (e)
		{
			//no element found
		}
	}
}

function GoNext()
{
	var answerList = document.getElementsByName("CheckBox");
	if (answerList[0].checked)
	{
		parent.next_navigation();
	}
}

function ClickHide(id, type)
{
	if (type == "click" || type == "both")
	{
		id = "c" + id;
		try {
			document.getElementById(id).style.display = "none";
		} catch (e)
		{
			//no element found
		}
	}
}

function Hide(id, type)
{
	if (type == "rollover" || type == "both")
	try {
		document.getElementById(id).style.display = "none";
	} catch (e)
	{
		//no element found
	}
}

function HideAllRollovers()
{
	var rolloverTargets = document.getElementsByName("rollovertarget");
		
	for(i=1; i<rolloverTargets.length+1; i++) 
	{
		try {
			document.getElementById("r"+i).style.display = "none";
		} catch (e)
		{
			//no element found
		}
	}
}

function HideAllClicks()
{
	var clickObjects = document.getElementsByName("clicktarget");
		
	for(i=1; i<clickObjects.length+1; i++) 
	{
		try {
			document.getElementById("cr"+i).style.display = "none";
		} catch (e)
		{
			//no element found
		}
	}
}


