// STANDARD JS SCRIPTS FOR A START PAGE***************************************************
// Check that XML importing will work, There is a similar function used in interface.js functions - possible duplication
// x is the name of the file
function ImportXML(x) {
        //Import XML into Opera
    var di = document.implementation;

    if (di && di.createLSParser)
	{
		var lsparser = di.createLSParser(di.MODE_ASYNCHRONOUS, null);
		lsparser.addEventListener('load', function(ev) {
				xmlDoc = ev.newDocument;
				InitXML(x);
				//fnCreateMenu();
			}, false);
		
		lsparser.parseURI(x);
		alert("This eLearning lesson uses a hybrid of XML, CSS and XSL technologies. \n\nUnfortunately, the Opera browser does not fully support the XSL standard and cannot be used to view this lesson.\n\nPlease open this lesson in another browser (such as Firefox or Internet Explorer).");
		
		lsparser = null;
                return;
		
	}

	//Import XML into browsers other than IE
	if (document.implementation && document.implementation.createDocument)
	{
		var BrowserDetect = GetBrowserDetect(); //Gets browser from DIAGNOSTIC.JS
		BrowserDetect.init(); // detect type of browser
		var browser=navigator.appName;
		if(BrowserDetect.browser != "Safari") {
			xmlDoc = document.implementation.createDocument("", "", null);
			xmlDoc.async = false;
			xmlDoc.load(x);
			xmlDoc.onload = InitXML(x);
		} else {
			initSafari();
		}
	}
	//Import XML into IE
	else if (window.ActiveXObject)
	{
		xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
		xmlDoc.async = false;
		xmlDoc.load(x);
		InitXML(x);
 	}
	else
	{
		alert('Your browser can\'t handle this script');
		return;
	}
}  //"../../content/" + x + "/start.xml"

//Load start.XML into frame
function InitXML(filename)
{
	/*document.getElementById('container').innerHTML = '<iframe src="' + filename +'" width="100%" height="600px" frameborder="0" title="Content" name="mrframe" marginheight="0px" marginwidth="0px" scrolling="no"></iframe>';*/
	window.location.assign(filename);
}

//Run Diagnostic Tests
function RunDiagnostics()
{
	//if an error report error and then allow user to click to view diagnostics report
	if (!InitDiagnostics()) //runs diagnostic tests - see diagnostic.js 
	{
		document.getElementById('diagnostic').innerHTML = 'Diagnostics has detected problem/s with your settings. Click <a href="javascript:ViewDiagnostics()">Here</a> to view the diagnostics report.'
	}
}

//View Diagnostics page
function ViewDiagnostics()
{
	//display diagnostics report
	ShowDiagnostics(); //see diagnostics.js
}

//begin the module
function BeginModule()
{
	//writeFlashCookie(FormatChoice); //use diagnostic script to write a cookie for flash version
	//window.top.location.assign("interface_tga.xml");
	openWindow("interface.xml");
}

//Connect to LMS
function ConnectToLMS(){

}




