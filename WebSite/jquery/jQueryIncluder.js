
var jQueryScriptOutputted = false;


function initJQuery(scriptLocation) {

    //if the jQuery object isn't available
    if (typeof (jQuery) == 'undefined') {

        if (!jQueryScriptOutputted) {
            //only output the script once.
            jQueryScriptOutputted = true;

            //output the script (load it from google api)
            document.write("<scr" + "ipt type='text/javascript' src='" + scriptLocation + "'></scr" + "ipt>");
        }
        setTimeout("initJQuery()", 50);
    }
}

