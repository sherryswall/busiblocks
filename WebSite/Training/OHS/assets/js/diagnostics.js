// JavaScript Document
var minwidth = 1024;
var minheight = 768;
var dpi = 96;
var colorDepth = 16;
var requiredFlashVersion = 7;
var requiredPlugins = new Array;
requiredPlugins[0] = "Shockwave Flash";
requiredPlugins[1] = "Adobe Acrobat";

var fontOffset = 15;
var countIssues = 0;
var questionCounter = 0;
var issueCounter = 0;
var accessibilityArray = new Array;
var accessSolutions = new Array;
accessSolutions[0] = "";
accessSolutions[1] = "There is a problem with your <b>Color</b> settings"
accessSolutions[2] = "There is a problem with your <b>Font Style</b> settings"
accessSolutions[3] = "There is a problem with your <b>Font Size</b> settings"

var resultText;
resultText = "";

var actualPlugins = new Array;
var pluginCounter = 0;
var getFlashVersion = 0;

var resolutionFix = new Array;
resolutionFix[0] = "From the <b>Start</b> menu, select <b>Settings > Control Panel</b>.";
resolutionFix[1] = "Ensure <b>Classic View</b> is selected.";
resolutionFix[2] = "Double click on the <b>Display</b> icon.";
resolutionFix[3] = "The <b>Display Properties</b> dialogue box will display. Select the <b>Settings</b> tab.";
resolutionFix[4] = "Drag the <b>Screen Resolution</b> slider to <b>" + minwidth + "</b> x <b>" + minheight + "</b>.";
resolutionFix[5] = "If you are unable to drag the Screen Resolution slider to " + minwidth + " x " + minheight + " or higher then this lesson will not fit on your monitor.";
resolutionFix[6] = "Click on <b>Apply</b> then click on <b>OK</b>";
resolutionFix[7] = "After adjusting your Screen Resolution, click <a href='javascript:opener.window.location.reload();window.close();'>here</a> to refresh this page."

var dpiFix = new Array;
dpiFix[0] = "From the <b>Start</b> menu, select <b>Control Panel</b>.";
dpiFix[1] = "Ensure <b>Classic View</b> is selected.";
dpiFix[2] = "Double click on the <b>Display</b> icon.";
dpiFix[3] = "The <b>Display Properties</b> dialogue box will display. Select the <b>Settings</b> tab.";
dpiFix[4] = "Click on <b>Advanced</b>.";
dpiFix[5] = "From the <b>DPI setting</b> drop-down list, select <b>Normal size (96 DPI)</b>.";
dpiFix[6] = "Click on <b>OK</b>.";
dpiFix[7] = "Click on <b>OK</b>.";
dpiFix[8] = "The <b>System Settings Change</b> dialogue box will display. Click on <b>Yes</b>."

var colorFix = new Array;
colorFix[0] = "From the <b>Start</b> menu, select <b>Control Panel</b>.";
colorFix[1] = "Ensure <b>Classic View</b> is selected.";
colorFix[2] = "Double click on the <b>Display</b> icon.";
colorFix[3] = "The <b>Display Properties</b> dialogue box will display. Select the <b>Settings</b> tab.";
colorFix[4] = "From the <b>Color quality</b> drop-down list, select <b>Highest (32 bit)</b>.";
colorFix[5] = "Click on <b>OK</b>.";
colorFix[6] = "After adjusting your Color Depth, click <a href='javascript:opener.window.location.reload();window.close();'>here</a> to refresh this page.";

var fontFix = new Array;
fontFix[0] = "From the <b>View</b> menu, select <b>Text Size</b>.";
fontFix[1] = "Select <b>Medium</b>.";
fontFix[2] = "After adjusting your Font Size, click <a href='javascript:opener.window.location.reload();window.close();'>here</a> to refresh this page.";
fontFix[3] = "<b>Note:</b> Font size my also be effected by your accessibility settings. Accessibility settings can be accessed from the following path: <br /><b>Tools &#62; Internet Options &#62; General Tab &#62; Accessibility</b>.";

var firefoxfontFix = new Array;
firefoxfontFix[0] = "From the <b>View</b> menu, select <b>Text Size</b>.";
firefoxfontFix[1] = "Select <b>Normal</b>.";
firefoxfontFix[2] = "After adjusting your Font Size, click <a href='javascript:opener.window.location.reload();window.close();'>here</a> to refresh this page.";
firefoxfontFix[3] = "If the above steps have not fixed this problem, from the <b>Tools</b> menu, select <b>Options...</b>";
firefoxfontFix[4] = "Select the <b>Content</b> tab.";
firefoxfontFix[5] = "In the <b>Fonts & Colors</b> sections, select <b>16</b> from the <b>Size</b> drop-down list.";
firefoxfontFix[6] = "Click on <b>OK</b>";
firefoxfontFix[7] = "After adjusting your Font Size, click <a href='javascript:opener.window.location.reload();window.close();'>here</a> to refresh this page.";
/* ****************************************** */

var testWin = false; //Is the diagnostics window open
//where were errors reported
var writeFont = false;  
var writeDepth = false;
var writeDPI = false;
var writeResolution = false;
var writeFlash = false;
var writeBrowser = false;
var writePlugin = false;

// getVersion - is used to determine if flash should be used to display a page
var getFormat = "flashversion";

///////////RUN DIAGNOSTIC TESTS/////////////////////////////////////////////////////////////////////
//Runs Diagnostics and if an error is reported return false else returns true as being successful
function InitDiagnostics() {
	checkBrowser();  //checks browser compatibilty and the version of the flash plugin
	checkScreenResolution();
	checkDPI();
	checkDepth();
	//checkFontSize();
	if (countIssues == 0)
	{ 
		return true;
	} else { return false; }
}

//called to open up a diagnostic window
function ShowDiagnostics()
{
	openDWin(); //creates error window
	if (testWin==true) {
		//reportDiagnostics(); //inputs data into error window
		//errWin.focus();
		timer();
	}
}

function timer() {
	setTimeout("reportDiagnostics()",1000)
}

function IsFlashWorking()
{
	if (getFormat=="flashversion")
	{
		return true;
	} else { return false; }
}

function GetFlashFormat()
{
	return getFormat;
}

//DETECT TYPE OF BROWSER////////////////////////////////////////////////////////////
var BrowserDetect = {
	init: function () {
		this.browser = this.searchString(this.dataBrowser) || "An unknown browser";
		this.version = this.searchVersion(navigator.userAgent)
			|| this.searchVersion(navigator.appVersion)
			|| "an unknown version";
		this.OS = this.searchString(this.dataOS) || "an unknown OS";
	},
	searchString: function (data) {
		for (var i=0;i<data.length;i++)	{
			var dataString = data[i].string;
			var dataProp = data[i].prop;
			this.versionSearchString = data[i].versionSearch || data[i].identity;
			if (dataString) {
				if (dataString.indexOf(data[i].subString) != -1)
					return data[i].identity;
			}
			else if (dataProp)
				return data[i].identity;
		}
	},
	searchVersion: function (dataString) {
		var index = dataString.indexOf(this.versionSearchString);
		if (index == -1) return;
		return parseFloat(dataString.substring(index+this.versionSearchString.length+1));
	},
	dataBrowser: [
		{ 	string: navigator.userAgent,
			subString: "Chrome",
			identity: "Chrome"
		},
		{ 	string: navigator.userAgent,
			subString: "OmniWeb",
			versionSearch: "OmniWeb/",
			identity: "OmniWeb"
		},
		{
			string: navigator.vendor,
			subString: "Apple",
			identity: "Safari"
		},
		{
			prop: window.opera,
			identity: "Opera"
		},
		{
			string: navigator.vendor,
			subString: "iCab",
			identity: "iCab"
		},
		{
			string: navigator.vendor,
			subString: "KDE",
			identity: "Konqueror"
		},
		{
			string: navigator.userAgent,
			subString: "Firefox",
			identity: "Firefox"
		},
		{
			string: navigator.vendor,
			subString: "Camino",
			identity: "Camino"
		},
		{		// for newer Netscapes (6+)
			string: navigator.userAgent,
			subString: "Netscape",
			identity: "Netscape"
		},
		{
			string: navigator.userAgent,
			subString: "MSIE",
			identity: "Explorer",
			versionSearch: "MSIE"
		},
		{
			string: navigator.userAgent,
			subString: "Gecko",
			identity: "Mozilla",
			versionSearch: "rv"
		},
		{ 		// for older Netscapes (4-)
			string: navigator.userAgent,
			subString: "Mozilla",
			identity: "Netscape",
			versionSearch: "Mozilla"
		}
	],
	dataOS : [
		{
			string: navigator.platform,
			subString: "Win",
			identity: "Windows"
		},
		{
			string: navigator.platform,
			subString: "Mac",
			identity: "Mac"
		},
		{
			string: navigator.platform,
			subString: "Linux",
			identity: "Linux"
		}
	]

};
BrowserDetect.init();

//for use outside diagnostics
function GetBrowserDetect()
{
	return BrowserDetect;
}

/////////////////Find Browser Plugins//////////////////////////////////////////////////////////////////////
function checkBrowser() {
	var browser=navigator.appName;
	var version=navigator.appVersion;
	if(BrowserDetect.browser == "Explorer" && BrowserDetect.version >= 6 && BrowserDetect.OS == "Windows") {
		checkIEPlugins();
		//working
	} else if (BrowserDetect.browser == "Firefox" && BrowserDetect.version >= 1.5 && BrowserDetect.OS == "Windows") {
		checkOtherPlugins();
	} else {
		countIssues++;
		writeBrowser = true;
		if(BrowserDetect.browser == "Explorer") {
			checkIEPlugins();
		} else {
			checkOtherPlugins();
		}
	}
}

//Check other Browser plugins
function checkOtherPlugins() {
	for(i=0;i<navigator.plugins.length;i++) {
		for(j=0;j<requiredPlugins.length;j++) {
			if(navigator.plugins[i].name == requiredPlugins[j]) {
				actualPlugins[pluginCounter] = navigator.plugins[i].name;
				requiredPlugins.splice(j,1);
				pluginCounter++;
			}
		}
	}
	for(k=0;k<requiredPlugins.length;k++) {
		//alert("For following plugins are missing: " + requiredPlugins[k]);
		countIssues++;
		writePlugin = true;
	}
	//get flash version
	for(l=0;l<actualPlugins.length;l++) {
		if(actualPlugins[l].indexOf("Flash")) {
			getFlashVersion = navigator.plugins["Shockwave Flash"].description;
			getFlashVersion = getFlashVersion.charAt(getFlashVersion.indexOf('.')-1);
		}
	}
	
	if (IsCorrectFlashVersion())
	{
		getFormat = "flashversion";
	} else
	{
		getFormat = "textversion";
		writeFlash = true;
		countIssues++;
	}
	
	//need to check that flash is enabled????
}
	
//Check IE plugins
function checkIEPlugins() {
		for(k=0;k<requiredPlugins.length;k++) {
			if(requiredPlugins[k] == "Adobe Acrobat") {
				for(var i=1; i<15; i++){
					try {
						acro = eval("new ActiveXObject('PDF.PdfCtrl."+i+"');");
						if(acro){
							//alert('Acrobat installed' + i);
						}
					}
					catch(e) {
					}
				}
			}
		}
	
	//get flash version, check flash version
	for(var i=9; i>0; i--){
		try{
			var flash = new ActiveXObject("ShockwaveFlash.ShockwaveFlash." + i);
			getFlashVersion = i;
			//alert("Your flash version is: " + getFlashVersion); 
			if (IsCorrectFlashVersion())
			{
				getFormat = "flashversion";
			} else
			{
				getFormat = "textversion";
				writeFlash = true;
				countIssues++;
			}
			return;
		}
		catch(e){ //cant find flash plugin or not enabled
		}
		//if gets to here no version of flash been found
		getFormat = "textversion";
	}
}

/////////////////////////////////////////////////////////////////////////////////////////////////////
function checkScreenResolution() {
	//alert("Your screen resolution is " + screen.width + " x " + screen.height + ". \n\nPlease write this resolution down in your pilot survey.");
	if ((screen.width>=minwidth) && (screen.height>=minheight)) {
		//working
	} else {
		countIssues++;
		writeResolution = true;
	}	
}

function checkDPI() {
	if (document.getElementById("checkDPI").offsetWidth == dpi) {
		//working
	} else {
		countIssues++;
		writeDPI = true;
	}	
}

function checkDepth() {
	if(screen.colorDepth>=colorDepth){
		//working
	} else {
		countIssues++;
		writeDepth = true;
	}
}

function checkFontSize(){
		var intOffsetHeight = document.getElementById("fontsize").offsetHeight;		
		if (intOffsetHeight == fontOffset) {
			//alert("Working - Font Size is: " + intOffsetHeight + " (Line 358)");
			
			//working
		} else {
			countIssues++;
			writeFont = true;
		}	
}


///////FLASH DETECTION////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////
function IsCorrectFlashVersion() {
	if (getFlashVersion < requiredFlashVersion) {
		return false;
	} else {
		return true;
	}
}

function checkLocalFlashVersion() {
	if (getLocalFlashVersion < requiredLocalFlashVersion) {
		var r=confirm("You need to update your version of Flash player to view this lesson.\n\nClick on OK to download the latest version of Flash. After downloading Flash, you will need to open this lesson again.\n\nClick on Cancel to view an HTML version of this lesson");
		if (r==true) {
			window.open("http://www.adobe.com/products/flashplayer/","_top","","true");
		} else {
			getFormat = "textversion"
		}
	} else {
		getFormat = "flashversion"
	}
}

/////////////////////DetectFLASH /////////////////////////////////////////////////////////
// Interface calls this to detect flash cookie, if there is no cookie or alternatively no flash form exists in the frame that do diagnostics testing again
function DetectFlash() {
	checkBrowser(); //Check browser plugins and ensure flash is working
	return getFormat;
}

//currently not is use
function writeFlashCookie(v){
	window.top.document.cookie="version"+"="+escape(v); //writes cookie into frame object
}

function readFlashCookie(cookieName) 
{
	 var theCookie=""+document.cookie;
	 var ind=theCookie.indexOf(cookieName);
	 if (ind==-1 || cookieName=="") return ""; 
	 var ind1=theCookie.indexOf(';',ind);
	 if (ind1==-1) ind1=theCookie.length; 
	 return unescape(theCookie.substring(ind+cookieName.length+1,ind1));
}

/****** WRITE DIAGNOSTICS REPORT ************/
//Generates Diagnostic Report Window
function openDWin() {
	if(testWin == false) {
		testWin = true;
		errWin = window.open('diagnostic.htm','errWin','toolbars=no,address=no,status=no,scrollbars=yes,resizable=yes,width=500,height=580');
	    
	} else {
	}
}


function reportDiagnostics() {
		if(countIssues != 0) {
			if (countIssues == 1) {
				errWin.document.getElementById("diagnostics").innerHTML="<p>The Diagnostics Test has detected <b>" + countIssues + "</b> issue that <b>may</b> directly impact your experience in using this course. If you would like to resolve this issue, follow the instructions below to change your settings, otherwise click <a href='javascript:window.close();'>Close</a> to continue with your current settings.</p>"
			}
			if (countIssues > 1) {
				errWin.document.getElementById("diagnostics").innerHTML="<p>The Diagnostics Test has detected <b>" + countIssues + "</b> issues that <b>may</b> directly impact your experience in using this course. If you would like to resolve these issues, follow the instructions below to change your settings, otherwise click <a href='javascript:window.close();'>Close</a> to continue with your current settings.</p>"
			}
		
			if (writeResolution == true) { 
				resolutionText = "<h2>You may have problems viewing the programme with your current Screen Resolution</h2><p>The optimal screen resolution is a mimimum of <b>" + minwidth + "</b> x <b>" + minheight + "</b> pixels. Please note that you can still view the programme at lower resolutions (e.g. 800 x 600) however you may need to use scroll bars to view the content.</p>" + "<h3>Procedure</h3><ol>";
				for(i=0;i<resolutionFix.length;i++){
					resolutionText=resolutionText+"<li>"+resolutionFix[i]+"</li>";
				}
				errWin.document.getElementById("screenResolution").innerHTML=resolutionText+"</ol>";
			}
			if (writeDPI == true) {
				dpiText = "<h2>There is problem with your DPI Settings</h2><p>You need to adjust your DPI settings to <b>" + dpi + "</b>.</p>" + "<h3>Procedure</h3><ol>";
				for(i=0;i<dpiFix.length;i++){
					dpiText=dpiText+"<li>"+dpiFix[i]+"</li>";
				}
				errWin.document.getElementById("screenDPI").innerHTML=dpiText+"</ol>";
			}		


			if (writeDepth == true) {
				depthText = "<h2>There is problem with your Colour Depth</h2><p>You need to adjust your Colour Depth settings to <b>" + colorDepth + "</b> bit.</p>" + "<h3>Procedure</h3><ol>";
				for(i=0;i<colorFix.length;i++){
					depthText = depthText+"<li>"+colorFix[i]+"</li>";
				}
				depthText = depthText+"</ol>";
				errWin.document.getElementById("screenColor").innerHTML=depthText;
			}

			if (writeFont == true) {
				fontText = "<h2>There may be an issue with your Font Size settings</h2>" + "<p>Large font sizes may cause information to be cropped from the screen. If you are unsure of you font sizes, an alternative print version is available from the <b>Print</b> menu.</p>" + "<h3>Procedure</h3><ol>";
				if(BrowserDetect.browser == "Explorer") {
					for(i=0;i<fontFix.length;i++){
						fontText=fontText+"<li>"+fontFix[i]+"</li>";
					}
				}
				else if(BrowserDetect.browser == "Firefox") {
					for(i=0;i<firefoxfontFix.length;i++){
						fontText=fontText+"<li>"+firefoxfontFix[i]+"</li>";
					}
				} else {
					fontText=fontText+"<li>Please review the " + BrowserDetect.browser + " documentation for methods on resetting your accessibility settings. This documentation can be accessed by pressing the <b>F1</b> key</li>";
				}
				errWin.document.getElementById("screenFont").innerHTML=fontText+"</ol>";
			}
			if (writePlugin == true) {
				for(k=0;k<requiredPlugins.length;k++) {
					errWin.document.getElementById("screenPlugin").innerHTML = "<h2>There is a problem with your Internet Browser Plugins</h2><p>This lesson requires " + requiredPlugins[k] + ".</p><p>Certain content main not display during the playback of this lesson.</p><p>Click <a href='http://www.google.com.au/search?hl=en&q="+ requiredPlugins[k] +"+plugin+download&meta=' target='_blank'>here</a> to download the " + requiredPlugins[k] + " plugin.</p>";
				}
			}
			if (writeFlash == true) {
				errWin.document.getElementById("screenPlugin").innerHTML=errWin.document.getElementById("screenPlugin").innerHTML+"<h2>There is a problem with your Flash plugin</h2><p>You have Flash version " + getFlashVersion + ". Flash version " + requiredFlashVersion + " is required to view this lesson.</p><p>Click <a href='http://www.adobe.com/products/flashplayer/' target='_blank' onclick='opener.turnOnFlash();' >here</a> to download the latest version of Flash.</p><p>Otherwise, to continue without flash, click <a href='javascript:window.close();'>here</a>.</p>"
			}
			if (writeBrowser == true) {
				errWin.document.getElementById("screenBrowser").innerHTML = "<h2>There is a problem with your Internet Browser or Operating System</h2><p>This lesson has been optimised for either Microsoft Internet Explorer 6+ or Mozilla Firefox 1.5+ on a Windows operating system.</p><p>You are currently using " + BrowserDetect.browser + " " + BrowserDetect.version + " on the " + BrowserDetect.OS + " operating system.</p><p>The may produce unexpected results in the playback of this lesson.</p>";	
			}
			
		} else {
			errWin.close();
		}
}


// MISCELLANEOUS HELPER FUNCTIONS ************************************************************************************************
var xholder;
var refreshStopper = true;

function refreshMe(){
	if(refreshStopper == false) {
		xholder = xholder.substring(12);
		newWindow.frames['mrframe'].open(xholder, '_self');
		refreshStopper = true;
		getVariables(); //now changed to flash detection
	}
}

function openWindow2(x) {
	toppos = 0;
	leftpos = 0;
	if(screen.height == 600 && screen.width == 800) {
		toppos = (screen.height-535)/5;
		leftpos = (screen.width-790)/2;
		var r=confirm("Your screen resolution is currently set at 800 x 600. \n\n1024 x 768 is the recommended screen resolution for viewing this lesson.\n\nHowever, the lesson may be viewed in full screen mode.\n\nWould you like to view the lesson in full screen mode?");
		if (r==true) {
			newWindow = window.open(x,"window2","toolbar=no,location=no,status=no,resizable=no,fullscreen=yes,scrollbars=yes,width=800,height=600");
		} else {
			newWindow = window.open(x,"window2","toolbar=no,location=no,fullscreen=no,scrollbars=yes,status=no,resizable=no,width=800,height=600,left="+leftpos+",top="+ toppos +"");
		}
	} else {
		newWindow = window.open(x,"window2","toolbar=yes,location=no,scrollbars=yes,status=no,resizable=yes,width=800,height=600,left="+leftpos+",top="+ toppos +"");
	}
}

