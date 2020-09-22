//******************************************************************************
//  This script is used to initalise the the interface frame
//  Using content.XML is will create an array of page_objects each object representing a page in the course
//  It will also calculate the time for each session
//******************************************************************************
// Global variables
//var activateScorm = true;
var activateScorm = false;
var totalScore=0; // percentage score during the course
var totalPages=0;
var totalObjectives=0;
var currentpage = 0; // Variable Used to track user progress through pages
var currenttopicpage = 0;
var xmlDoc = null; //XMLDOC object stores tags content.xml
var bFlash = true; //global flash switch is enabled if diagnostics reports true
var Format= "flashversion"; //individual page flash switch by flash form
var currenttag = "";
var bComplete = false;
var bScored = false;
var PageArray = new Array(); //page object array 
var currentobjective = 0; //track user progress through objectives array
var Objectives = new Array(); //this contains the objectives for the module and the page number for the start of each topic
var modulenumber = 0;
var modulename = "";
var package = "";
var username = "";
var accessibility = false;

// Initialise interface
function LoadInterface(content) {
	username = StartScorm(); 	//Begins SCORM SESSION
	ImportXML(content);	//imports content.xml
	//bFlash = DetectFlash();  //set format flash or textversion - Diagnostic.js
}

// Function imports from content.xml
function ImportXML(content,loadContent) {
	//Import XML into browsers other than IE
	if (document.implementation && document.implementation.createDocument)
	{
		var BrowserDetect = GetBrowserDetect(); //Gets browser from DIAGNOSTIC.JS
		BrowserDetect.init(); // detect type of browser
		var browser=navigator.appName;
		if(BrowserDetect.browser != "Safari" && BrowserDetect.browser != "Chrome") {
			xmlDoc = document.implementation.createDocument("", "", null);
			xmlDoc.async = false;
			xmlDoc.load(content);
			if(loadContent != "false") {
				xmlDoc.onload = initXML();
			}
		} else { // Code for Safari
			//initSafari();
			var xmlRequest = new XMLHttpRequest();
			xmlRequest.open("GET",content,false);
			xmlRequest.send(null);
			xmlDoc = xmlRequest.responseXML;
			if(loadContent != "false") {
				initXML();
			}
		}
	}
	//Import XML into IE
	else if (window.ActiveXObject)
	{
		xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
		xmlDoc.async = false;
		xmlDoc.load(content);
		if(loadContent != "false") {
			initXML();
		}
 	}
	else
	{
		alert('Your browser can\'t handle this script');
		return;
	}
}


//  Initialise XML and create page objects
function initXML() {
	
	var getUnit = xmlDoc.getElementsByTagName("unit");
	modulename = getUnit[0].getAttribute("modulename");
	modulenumber = getUnit[0].getAttribute("modulenumber");
	package = getUnit[0].getAttribute("client");
	
	//get previous data from LMS
	var datastring = GetSuspendDataLMS();
	var SuspendDataObjects;  //this array contains object data from previous course attempts
	if (datastring != "")
	{
		var data = datastring.substr(1,datastring.length);
		SuspendDataObjects = data.split("&");
	} else SuspendDataObjects = "nodata";
	
	//set page total
	totalPages = 0;
	totalObjectives = 0;
	
	var getTopic = getUnit[0].getElementsByTagName("topic");
	
	//search through topics
	for (i=0; i<getTopic.length; i++)
	{
		var objective = getTopic[i].getAttribute("objective");
		var topicheading = getTopic[i].getAttribute("heading");
		var objectiveID = "obj"+totalObjectives;
		
		var getPage = getTopic[i].getElementsByTagName("page");
		
		if (objective != null)
		{
			SaveObjectiveLMS(objectiveID, totalObjectives);
			Objectives[totalObjectives] = new Array();
			Objectives[totalObjectives][0] = objective; //save the text of the current object
			Objectives[totalObjectives][1] = totalPages; //save the page position of the start of each objective
			Objectives[totalObjectives][2] = topicheading; //save the heading of the current object
			totalObjectives++;
		}
		
	    //now search through each page and initialise page object
		for (j=0; j<getPage.length; j++)
		{
			//create a page object variables
			var filename = "";
			var type = "";
			var subindex = "";
			var bSubIndexed = false;
			var bNextButton = true;
			var bBackButton = true;
			var time = 0;
			var bScore = false;
			var score = 0;
			var points = 0;
			var comment = "";
			var bViewed = false;
			
			//if page is last page for objective, write objective complete into LMS, by default objective is incomplete
			var bObjectiveComplete = (j > getPage.length-2);
			
			//for each node inside page
			for (a=0; a<getPage[j].childNodes.length; a++)
			{
				if (getPage[j].childNodes[a].nodeName == "name")
				{
					filename = getPage[j].childNodes[a].firstChild.nodeValue;
				}
				
				if (getPage[j].childNodes[a].nodeName == "type")
				{
					type = getPage[j].childNodes[a].firstChild.nodeValue;
				}
				
				if (getPage[j].childNodes[a].nodeName == "subindextitle")
				{
					subindex = getPage[j].childNodes[a].firstChild.nodeValue;
				}
				
				if (getPage[j].childNodes[a].nodeName == "subindex")
				{
					var bsubindex = getPage[j].childNodes[a].firstChild.nodeValue;
					if (bsubindex == "yes") { bSubIndexed = true; }
				}		
			}
			
			//Create a page object and store variables found
			PageArray[totalPages] = new PageObject();
			PageArray[totalPages].filename = filename + ".xml";
			PageArray[totalPages].type = type;
			PageArray[totalPages].subindex = subindex;
			PageArray[totalPages].bSubIndexed = bSubIndexed; 
			PageArray[totalPages].bNextButton = bNextButton;
			PageArray[totalPages].bBackButton = bBackButton;
			PageArray[totalPages].time = time;
			PageArray[totalPages].bScored = bScored;
			PageArray[totalPages].objectiveID = objectiveID;
			PageArray[totalPages].bObjectiveComplete =  bObjectiveComplete; //indicated if page should save completion of "objN"
			PageArray[totalPages].comment = comment;
			PageArray[totalPages].score = score;
			PageArray[totalPages].bViewed = bViewed;
			PageArray[totalPages].points = points;
			PageArray[totalPages].topicheading = topicheading;
			PageArray[totalPages].id = totalPages;
			
			//initialise objkects with previous data held by LMS
			if (SuspendDataObjects != "nodata")
			{
				PageArray[totalPages].ReadData(SuspendDataObjects[totalPages]);
			}
			totalPages++;
		}
	}

	//hide back and next button
	PageArray[0].SetBackButton(false);
	PageArray[totalPages-1].SetNextButton(false);
	
	if (bFirstTime)
	{
		PageArray[currentpage].Load();
	} else
	{
		var lastlocation = parseInt(GetLastLocationLMS());
		currentpage = lastlocation;
		PageArray[currentpage].Load();
	}
	
	
	//save all objectives in LMS
	SaveLMS();

	
	
	//clear memory of XML
	xmlDoc = null;
}

// Unload Interface and save data to LMS
function UnloadInterface()
{ 
	//set score in LMS
	if (bScored)
	{
		SaveScoreLMS(totalScore);
	}
	//set time of session
	ComputeTimeLMS();
	//get suspend data
	datastring = "";
	for (i=0; i<PageArray.length; i++)
	{
		datastring = datastring + "&" + PageArray[i].GetPageSuspendData();
	}
	//close LMS
	CloseLMS(datastring);
}

//NAVIGATION SCRIPTS-------------------------------------------------------------------------------------------//
//next button
function next_navigation() {
	if (currentpage < totalPages)
	{
		currentpage++;
		currentobjective = parseInt(PageArray[currentpage].objectiveID.substring(3,4));
		PageArray[currentpage].Load();
	}
}

//back button
function back_navigation() {
	if (currentpage > 0)
	{
		currentpage--;
		currentobjective = parseInt(PageArray[currentpage].objectiveID.substring(3,4));
		PageArray[currentpage].Load();
	}
}

//find the current page
function LoadPage(id)
{
	currentpage = id;
	currentobjective = parseInt(PageArray[currentpage].objectiveID.substring(3,4));
	PageArray[currentpage].Load();
}

//updates the page, including next and back buttons, and flash
// Could move all getElementById code to page object
function UpdatePage()
{
	//write and data to LMS menu bar
	/*window.frames['content_frame'].document.getElementById("username").innerHTML = "Welcome " + username;*/
	PageArray[currentpage].Update();
	if (accessibility == true) {
		window.frames['content_frame'].document.getElementById("close_btn").innerHTML = "Close";
		window.frames['content_frame'].document.getElementById("acronyms_btn").innerHTML = "Acronyms";
		window.frames['content_frame'].document.getElementById("contact_btn").innerHTML = "Contact";
		window.frames['content_frame'].document.getElementById("modules_btn").innerHTML = "Modules";
		window.frames['content_frame'].document.getElementById("next_btn").innerHTML = "Next";
		window.frames['content_frame'].document.getElementById("back_btn").innerHTML = "Back";
		window.frames['content_frame'].document.getElementById("headingbackground").style.backgroundColor= "#5E86BA";
		window.frames['content_frame'].document.getElementById("heading_title").style.textIndent = "0";
	}
}

//updates the currentpage count so when back button is triggered will go back to normal page
function LoadUtility(utility) {
	//var contentFrame = window.frames.length;
	window.frames['content_frame'].document.getElementById(utility + "Box").style.display = "block";
}



function UpdateUtility()
{
	window.frames['content_frame'].document.getElementById('next_btn').style.display = "none";
	window.frames['content_frame'].document.getElementById('progress').style.width = ''+ PageArray[currentpage].GetProgress() +'%';
	window.frames['content_frame'].document.getElementById('progress_score').innerHTML = ''+ PageArray[currentpage].GetProgress() +'%';
	currentpage++;
}

function ShowDialogueBox(x)
{
	if (x == "bookmarkbox")
	{
		WriteBookmarks();
	}
	window.frames['content_frame'].document.getElementById(x).style.display = "inline";
}
	
function HideDialogueBox(x)
{
	window.frames['content_frame'].document.getElementById(x).style.display = "none";
}

//adds a comment to the page
function AddNotesToPage()
{
	PageArray[currentpage].AddComment();
}

function WriteBookmarks()
{
	htmltext = "";
	for (i=0; i<PageArray.length; i++)
	{
		var string = PageArray[i].GetBookmark();
		if (string != null)
		{
			htmltext = htmltext + PageArray[i].GetBookmark() + "<br/>" ;
		}
	}
	if (htmltext == "") { htmltext = "No Bookmarks saved."; }
	window.frames['content_frame'].document.getElementById('bookmarks').innerHTML = htmltext;
}

//book marks the page
function BookmarkPage()
{
	PageArray[currentpage].AddBookmark();
}

//load bookmark
function LoadBookmark()
{
	
}

//save page completing into SCORM
function PageComplete()
{
	SavePageCompletionLMS(currentpage, currentobjective, PageArray[currentpage].bObjectiveComplete);
}

function DoNothing()
{
	
}

function GoToCurrentTopic()
{
	LoadPage(Objectives[currentobjective][1]);
}

//show menu
function ShowMenu()
{
	window.frames['content_frame'].document.getElementById("dropdown").style.display = "block";
	window.frames['content_frame'].document.getElementById("dropdown").style.left = window.frames['content_frame'].document.getElementById("modules_btn").offsetLeft;

}
	
//hide menu
function HideMenu()
{
	window.frames['content_frame'].document.getElementById("dropdown").style.display = "none";
}


// Convert Number to Word
function numberToWords(){
	var num = window.frames['content_frame'].document.getElementById("modnumber").innerHTML;
	var words = ['Zero','One','Two','Three','Four', 'Five','Six','Seven','Eight','Nine','Ten']; 
	num = parseInt(num);
	if (num <11) {
		num = words[num]
	}
	window.frames['content_frame'].document.getElementById("modnumber").innerHTML = '' + num;
}

function checkImage() {
	/*alert(window.frames['content_frame'].document.getElementById("accessibility_square").offsetWidth)*/
	if (window.frames['content_frame'].document.getElementById("accessibility_square").offsetWidth < 50) {
		if (confirm("This is an image intensive lesson.\n\nIf you wish to view a text only version of this lesson, click on OK.\n\nYou may experience playback problems if you continue to view this lesson with images disabled.")) { 
			top.window.location.assign("content.xml");
		} else {
			window.frames['content_frame'].document.getElementById("next_btn").innerHTML = "Click here to begin.";
			accessibility = true;
		}
	}
}

//run Flash using script in AC-RunActiveContent
//this script is called when a flash object is embedded
function runFlash(x,w,h,tag) {
	currenttag = tag;
	if (!bFlash || (Format == 'textversion')) { return; } //return if flash turned off
	var loc = 'xmlfile='+PageArray[currentpage].filename;
	AC_FL_RunContent(
	'codebase', 'https://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=7,0,0,0',
	'width', w,
	'height', h,
	'src', x,
	'FlashVars', loc,
	'quality', 'high',
	'pluginspage', 'https://www.macromedia.com/go/getflashplayer',
	'align', 'middle',
	'play', 'true',
	'loop', 'false',
	'scale', 'noscale',
	'wmode', 'transparent',
	'devicefont', 'false',
	'id', x,
	'bgcolor', '#ffffff',
	'name', x,
	'menu', 'false',
	'allowScriptAccess','sameDomain',
	'movie', x,
	'salign', ''
	);
}
