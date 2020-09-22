/** 
** Filename: SCOFunctions.js
**
** File Description: This file contains several JavaScript functions that are 
**                   used by the Sample SCOs contained in the Sample Course.
*/
var startDate;
var exitPageStatus;
var bFirstTime = false;

//begin SCORM
function StartScorm()
{
	if (activateScorm)
	{
	
		var result = doLMSInitialize();
		var resume = doLMSGetValue( "cmi.core.entry" ); //has the course already been attempted
		var user = doLMSGetValue( "cmi.core.student_name" );

	   //ab-initio represents first time
	   if (resume == "ab-initio")
	   {
		  doLMSSetValue( "cmi.core.lesson_status", "incomplete" );
		  bFirstTime = true;
		  doLMSCommit();
	   } else if (resume == "resume") 
	   {
		   //go to last page	//set by LMS after entry   
	   }
	
	   startTimer()
	   
	   //maybe should return student name and current course time and last page
	   
	   return user;
	} else return "";
}

//write the objectives into SCORM if objectives have not already been written
function SaveObjectiveLMS(objective, number)
{
	if (activateScorm)
	{
		if (bFirstTime)
		{
			 //TODDdoLMSSetValue( "cmi.objectives."+number+".id", objective);
		}
	}
}

function GetLastLocationLMS()
{
	if (activateScorm)
	{
		return parseInt(doLMSGetValue( "cmi.core.lesson_location" ));
	} else return 0;
}

//set score if course is scored
function SaveScoreLMS(score)
{
	if (activateScorm)
	{
		doLMSSetValue( "cmi.core.score.raw", score);
		var mode = doLMSGetValue( "cmi.core.lesson_mode" );
		if ( mode != "review"  &&  mode != "browse") 
		{
			if (score < 80 )
			{
				doLMSSetValue( "cmi.core.lesson_status", "failed" );
			}
			else
			{
				doLMSSetValue( "cmi.core.lesson_status", "passed" );
			}
		}
		
		//save changes in LMS
		doLMSCommit();
	}
}

//save completion
function SavePageCompletionLMS(pagenumber, objectivenumber, bObjectiveComplete)
{
	if (activateScorm)
	{
		var bComplete = false; //assume course hasnt been completed but then find out if complete
		bComplete = (doLMSGetValue("cmi.core.lesson_status" ) == "completed");
		bObjComplete = true;
		//TODDbObjComplete = (doLMSGetValue("cmi.objectives."+objectivenumber+".status") == "completed");
		
		//if completed skip
		if (!bComplete)
		{
			doLMSSetValue( "cmi.core.lesson_location", "" + pagenumber );


var completionCounter = 0;
for (var i=0;i<totalPages;i++) {
	if (PageArray[i].bViewed == true) {
		completionCounter++
	}
}
if (completionCounter == totalPages) {			
	bComplete = true;
	doLMSSetValue( "cmi.core.lesson_status", "completed" );
} else {
	doLMSSetValue( "cmi.core.lesson_status", "incomplete" );
}




/*TODD
			//if objective completed skip else save as completed or incomplete
			if (!bObjComplete)
			{
				if (bObjectiveComplete)
				{
					//objective as completed
					//TODDdoLMSSetValue( "cmi.objectives."+objectivenumber+".status", "completed");
					
					//now check if all objectives are completed
					numberOfObjectives = doLMSGetValue("cmi.objectives._count");
					var numComplete = 0;
					for (e=0; e<numberOfObjectives; e++)
					{
						//TODDif (doLMSGetValue("cmi.objectives."+e+".status") == "completed")
						//TODD{
						//TODD	numComplete++;
						//TODD}
					}
					if (numComplete == numberOfObjectives)
					{
						bComplete = true;
						doLMSSetValue( "cmi.core.lesson_status", "completed" );
					} else
					{
						doLMSSetValue( "cmi.core.lesson_status", "incomplete" );
					}
				} else
				{
					//TODDdoLMSSetValue( "cmi.objectives."+objectivenumber+".status", "incomplete");
				}
			}
END TODD */
		}
		
		//save changes in LMS
		doLMSCommit();
	}
}

//gets suspend data
function GetSuspendDataLMS()
{
	var datastring = "";
	if (activateScorm)
	{
		var datastring = doLMSGetValue("cmi.suspend_data");
	}
	return datastring;
}


//close LMS
function CloseLMS(datastring)
{
	if (activateScorm)
	{
		//set the last session time
		ComputeTimeLMS();
		
		doLMSSetValue( "cmi.suspend_data", datastring );
		doLMSSetValue( "cmi.core.exit", "suspend" );
		doLMSCommit();
		doLMSFinish();
	}

}

function SaveLMS()
{
	if (activateScorm)
	{
		doLMSCommit();
	}
}

//start timer
function startTimer()
{
   startDate = new Date().getTime();
}

//computeTimeLMS, done every time an objective is completed
function ComputeTimeLMS()
{
   if ( startDate != 0 )
   {
      var currentDate = new Date().getTime();
      var elapsedSeconds = ( (currentDate - startDate) / 1000 );
      var formattedTime = convertTotalSeconds( elapsedSeconds );
   }
   else
   {
      formattedTime = "00:00:00.0";
   }
   
   if (activateScorm)
   {
   		doLMSSetValue( "cmi.core.session_time", formattedTime );
   }
}

/*******************************************************************************
** this function will convert seconds into hours, minutes, and seconds in
** CMITimespan type format - HHHH:MM:SS.SS (Hours has a max of 4 digits &
** Min of 2 digits
*******************************************************************************/
function convertTotalSeconds(ts)
{
   var sec = (ts % 60);

   ts -= sec;
   var tmp = (ts % 3600);  //# of seconds in the total # of minutes
   ts -= tmp;              //# of seconds in the total # of hours

   // convert seconds to conform to CMITimespan type (e.g. SS.00)
   sec = Math.round(sec*100)/100;
   
   var strSec = new String(sec);
   var strWholeSec = strSec;
   var strFractionSec = "";

   if (strSec.indexOf(".") != -1)
   {
      strWholeSec =  strSec.substring(0, strSec.indexOf("."));
      strFractionSec = strSec.substring(strSec.indexOf(".")+1, strSec.length);
   }
   
   if (strWholeSec.length < 2)
   {
      strWholeSec = "0" + strWholeSec;
   }
   strSec = strWholeSec;
   
   if (strFractionSec.length)
   {
      strSec = strSec+ "." + strFractionSec;
   }


   if ((ts % 3600) != 0 )
      var hour = 0;
   else var hour = (ts / 3600);
   if ( (tmp % 60) != 0 )
      var min = 0;
   else var min = (tmp / 60);

   if ((new String(hour)).length < 2)
      hour = "0"+hour;
   if ((new String(min)).length < 2)
      min = "0"+min;

   var rtnVal = hour+":"+min+":"+strSec;

   return rtnVal;
}

