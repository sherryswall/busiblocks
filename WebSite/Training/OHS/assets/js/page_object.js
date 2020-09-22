//  This script contains all the properties and methods for a page object running in a SCORM complient LMS
//  Created by Brad Nielsen
//	12/4/08
//  Each page object will be initialised using variables from XML
//************************************************************************************************************

function PageObject()
{
	var filename;
	var type;
	var subindex;
	var bSubIndexed; 
	var bNextButton;
	var bBackButton;
	var time;
	var bScored;
	var objectiveID;
	var bObjectiveComplete;
	var topicheading;
	var comment = ""; //comment to be save on page
	var bBookmark = false;
	var bViewed = false;
	var score;
	var progress;
	var points;
	var id; //the index of the page array
	
	//methods  bool is a boolean to set on or of
	this.SetBackButton = function(boo)
	{
		this.bBackButton = boo;
	}
	
	this.SetNextButton = function(boo)
	{
		this.bNextButton = boo;
	}
	
	//this function will update the display of the page based on variables back/next buttons, heading, and menu and progress
	this.Update = function()
	{
		//update topicheading and breadcrumb
		//window.frames['content_frame'].document.getElementById('topicheading').innerHTML = this.topicheading;
		//window.frames['content_frame'].document.getElementById('breadcrumb').innerHTML = this.topicheading;
		
		//if comment exists for page
		window.frames['content_frame'].document.getElementById("commentbox").value = this.comment;
		
		//if bookmarked, show bookmarked
		//if (this.bBookmark)
		//{
		//	window.frames['content_frame'].document.getElementById('bookmark').style.display = "inline";
		//}
		
		//update back and next buttons
		if (this.bBackButton)
		{
			window.frames['content_frame'].document.getElementById('back_btn').style.display = "inline";
		} else {
			window.frames['content_frame'].document.getElementById('back_btn').style.display = "none";
		}
		
		if (this.bNextButton)
		{
			window.frames['content_frame'].document.getElementById('next_btn').style.display = "inline";
			window.frames['content_frame'].document.getElementById('back_btn').style.marginRight = "0px";
		} else {
			//window.frames['content_frame'].document.getElementById('next_btn').style.display = "none";
			//window.frames['content_frame'].document.getElementById('back_btn').style.marginRight = "30px";
			window.frames['content_frame'].document.getElementById('next_btn').style.backgroundImage = "url('assets/images/nextStatic.jpg')";
			window.frames['content_frame'].document.getElementById('next_btn').style.cursor = "default";
			window.frames['content_frame'].document.getElementById('next_btn').href = "javascript:void()";
		}
		
		//if flash is enabled by diagnostics, show the flash form
		if (bFlash)
		{
			//window.frames['content_frame'].document.getElementById('choice').style.display = "inline";
			if (Format == "flashversion")
			{
			   //window.frames['content_frame'].document.getElementById('radio1').checked=true;
			   //window.frames['content_frame'].document.getElementById('radio2').checked=false;
			} else {
			   //window.frames['content_frame'].document.getElementById('radio1').checked=false;
			   //window.frames['content_frame'].document.getElementById('radio2').checked=true;
			}

			CheckFlash(); //check to see if the user would like flash or not
		} else { 
			//window.frames['content_frame'].document.getElementById('choice').style.display = "none"; 
		}
		
		//update progress bar
		window.frames['content_frame'].document.getElementById('progress').style.width = ''+ this.GetProgress() +'%';
		window.frames['content_frame'].document.getElementById('progress_score').innerHTML = ''+ PageArray[currentpage].GetProgress() +'% Complete';
		
		//update menu
		this.updateMenu();
	}
	
	this.AddComment = function()
	{
		this.comment = window.frames['content_frame'].document.getElementById("commentbox").value;
		window.frames['content_frame'].document.getElementById("notesaved").style.display = "inline";
	}
	
	this.AddBookmark = function()
	{
		this.bBookmark = true;
		window.frames['content_frame'].document.getElementById('bookmark').style.display = "inline";
	}
	
	//this function will load the page into the current frame
	this.Load = function()
	{
		document.getElementById("content_frame").src = '' + this.filename;
		this.bViewed = true;
	}
	
	this.TellMe = function()
	{
		alert(this.filename);
	}
	
	//return a suspend data string data is separated by a |
	this.GetPageSuspendData = function()
	{
		var view = "";
		var booked = "";
		if (this.bViewed) { view = "yes"; } else { view = "no"; }
		if (this.bBookmark) { booked = "yes"; } else { booked = "no"; }
		var datastring = "P:"+ this.id + "|" + "V:" + view + "|" + "B:" + booked + "|" + "C:" + this.comment;
		return datastring;
	}
	
	//read data from suspend data string for this page.
	this.ReadData = function(data)
	{
		var dataobjects = data.split("|");
		for (a=0; a<dataobjects.length; a++)
		{
			//now break down data objects into name and value
			var property = dataobjects[a].split(":");
			switch(property[0])
			{
				case "P":
				  //do nothing
				  break;    
				case "V":
				  if (property[1] == "yes"){ this.bViewed = true; } else { this.bViewed = false; } 
				  
				  break;
				case "B":
				  if (property[1] == "yes"){ this.bBookmark = true; } else { this.bBookmark = false; } 
				  break;
				case "C":
				  this.comment = property[1];
				  break;
				default:
				  break;
			} //end switch
		} //end loop
	}
	
	this.GetProgress = function()
	{
		var progress = Math.round(this.id/(totalPages-1)*100); //stores a progress percentage even before completed
		return progress;
	}
	//UPDATE MENU ITEM - TOPICS
	this.updateMenu = function() {
		window.frames['content_frame'].document.getElementById('menu' + Objectives[Number(this.objectiveID.substring(3))][1]).style.color = '#f58220';
		window.frames['content_frame'].document.getElementById('menu' + Objectives[Number(this.objectiveID.substring(3))][1]).style.fontWeight = 'bold';
	}
}




/**********************************************************************************************/
// Flash Scripts turn flash on or off
//check flash call 
function CheckFlash() {
	if(Format == "textversion") {
		changeFlashCSS("none", "block")
	} else if (Format == "flashversion")
	{
		changeFlashCSS("block", "none")
	}
}

//changes values in the css so dont have to located document elements individually
function changeFlashCSS(flash, text) {
	
	if (!window.frames['content_frame'].document.styleSheets) return;
	var theRules = new Array();
	
	if (window.frames['content_frame'].document.styleSheets[0].cssRules)
		theRules = window.frames['content_frame'].document.styleSheets[0].cssRules;
	else if (window.frames['content_frame'].document.styleSheets[0].rules)
		theRules = window.frames['content_frame'].document.styleSheets[0].rules;
	else return;
	
	//make sure flashdiv is the last style
	theRules[theRules.length-1].style.display = flash;
	//make sure textdiv is the second last sytle
	theRules[theRules.length-2].style.display = text;
}




