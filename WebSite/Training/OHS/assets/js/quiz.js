/*******************************************************************************************************/
//This is Javascript is used for the quiz.xsl to create Multiple Choice / True and False questions


function TestRadioCorrect()
{
	//get answers array
	var answerList = document.getElementsByName("RadioGroup");
	//checkSelected = new Array();
	checkSelected = "";
	correctResponses = new Array();
	//look through answers to find button that is checked
	for (i=0;i<answerList.length;i++)
	{
		var letter = "";
		switch (i)
		{
			case 0:
			{ letter = "a"; break; }
			case 1:
			{ letter = "b"; break; }
			case 2:
			{ letter = "c"; break; }
			case 3:
			{ letter = "d"; break; }
			case 4:
			{ letter = "e"; break; }
			case 5:
			{ letter = "f"; break; }
			case 6:
			{ letter = "g"; break; }
			default: { letter = ""; }
		}
		//set normal background and colour
		setNormal(letter);
		
		if(answerList[i].checked)
		{
			if (answerList[i].value == "correct")
			{
				showCorrect(letter);
				Proceed();
				checkCorrect = "correct";
				checkSelected = letter;
				//parent.right_answer();
				//checkSelected[checkSelected.length] = letter;
			} else
			{
				showIncorrect(letter);
				TryAgain();
				checkCorrect = "wrong";
				checkSelected = letter;
				//parent.wrong_answer();
			}
		}
		if (answerList[i].value == "correct") {
			responseArray = new Array ("a","b","c","d","e","f","g");
			//alert(responseArray[i]);
			correctResponses[correctResponses.length] = responseArray[i];
			/*if (correctResponses == "") {
				correctResponses = responseArray[i];
			} else {
				correctResponses = correctResponses + "," + responseArray[i];
			}*/
		}
		
	}
	lmsAssess(checkCorrect,checkSelected,correctResponses);
}

function TestCheckboxCorrect(total)
{	
	//get answers array
	var answerList = document.getElementsByName("CheckGroup");
	var correct = 0;
	var checked = 0;
	//checkSelected = new Array();
	checkSelected = "";
	correctResponses = new Array();
	checkCorrect = "";
	//look through answers to find button that is checked
	for (i=0;i<answerList.length;i++)
	{
		var letter = "";
		switch (i)
		{
			case 0:
			{ letter = "a"; break; }
			case 1:
			{ letter = "b"; break; }
			case 2:
			{ letter = "c"; break; }
			case 3:
			{ letter = "d"; break; }
			case 4:
			{ letter = "e"; break; }
			case 5:
			{ letter = "f"; break; }
			case 6:
			{ letter = "g"; break; }
			default: { letter = ""; }
		}
		//set normal background and colour
		setNormal(letter);
		
		if(answerList[i].checked)
		{
			checked++;
			if (checkSelected == "") {
				checkSelected = letter;
			} else {
				checkSelected = checkSelected + "," + letter;
			}
			//checkSelected[checkSelected.length] = letter;
			
			if (checked > total)
			{
				break; //too many answers selected
			}
			if (answerList[i].value == "correct")
			{
				showCorrect(letter);
				
				correct++;
			} else
			{
				showIncorrect(letter);
				
			}
		}
		if (answerList[i].value == "correct") {
			responseArray = new Array ("a","b","c","d","e","f","g");
			//alert("Correct response: " + responseArray[i]);
			correctResponses[correctResponses.length] = responseArray[i];
			//alert("Length of Array = " + correctResponses.length);
			/*if (correctResponses == "") {
				correctResponses = responseArray[i];
			} else {
				correctResponses = correctResponses + "," + responseArray[i];
			}*/
		}
	}
	
	if (checked < total)
	{
		document.getElementById("alert").innerHTML  = "<p>You have not checked enough boxes. Please try again.</p>"
		checkCorrect = "wrong";
		TryAgain();
	} else if (checked > total)
	{
		document.getElementById("alert").innerHTML  = "<p>You have checked too many boxes. Please try again.</p>"
		checkCorrect = "wrong";
		TryAgain();
	} else //the correct number has been checked
	if (correct == total)
	{
		Proceed();
		checkCorrect = "correct";
		//parent.right_answer();
	} else
	{
		checkCorrect = "wrong";
		TryAgain();
		
		//parent.wrong_answer();
	}
	lmsAssess(checkCorrect,checkSelected,correctResponses);
}

//Takes data from quiz and writes to LMS
function lmsAssess(checkCorrect,checkSelected,correctResponses) {
	for(i=0;i<parent.quizList.length;i++){
		if(parent.quizList[i] == parent.currentpage) {
			countElements = document.getElementsByTagName("tr");
			countElementsLength = countElements.length - 1;
			//for(j=0;j<parent.correctCheckArray.length;j++) {
				if(parent.correctCheckArray[i] == "" && checkCorrect == "correct") {
					parent.rawScore++;
					parent.correctCheckArray[i] = "correct";
				}
			//}
			
			for(j=0;j<correctResponses.length;j++) {
				//alert(i);
				parent.doLMSSetValue("cmi.interactions."+i+".correct_responses."+j+".pattern",correctResponses[j]);
				//alert(parent.doLMSGetValue("cmi.interactions."+i+".correct_responses._count"));
			}
			
				
			parent.doLMSSetValue("cmi.interactions."+i+".result",checkCorrect);
			parent.doLMSSetValue("cmi.interactions."+i+".student_response",checkSelected);
			varTime = new Date();
			varHours = varTime.getHours();
			if (varHours < 10) {
				varHours = "0" + varHours;
			}
			varMins = varTime.getMinutes();
			if (varMins < 10) {
				varMins = "0" + varMins;
			}
			varSecs = varTime.getSeconds();
			if (varSecs < 10) {
				varSecs = "0" + varSecs;
			}
			speedTime = varTime - parent.startQuizTime;
			
			varTime = varHours+":"+varMins+":"+varSecs;
			parent.doLMSSetValue("cmi.interactions."+i+".time",varTime);
			parent.doLMSSetValue("cmi.interactions."+i+".latency",convertTime(speedTime));
			
			//alert(convertTime(speedTime));
			
		}
	}	
}

function two(x) {return ((x>9)?"":"0")+x}
//function three(x) {return ((x>99)?"":"0")+((x>9)?"":"0")+x}

function convertTime(ms) {
	var sec = Math.floor(ms/1000)
	ms = ms % 1000
	//t = three(ms)

	var min = Math.floor(sec/60)
	sec = sec % 60
	//t = two(sec) + ":" + t
	t = two(sec);

	var hr = Math.floor(min/60)
	min = min % 60
	t = two(min) + ":" + t

	var day = Math.floor(hr/60)
	hr = hr % 60
	t = two(hr) + ":" + t
	//t = day + ":" + t

	return t
}

function setNormal(id)
{
	try {
		document.getElementById(id).style.backgroundColor="white";
		document.getElementById(id).style.color="black";
	} catch (e)
	{
		//cannot find element
	}
}

function showCorrect(id)
{
	try {
	document.getElementById(id).style.backgroundColor="green";
	document.getElementById(id).style.color="white";
		} catch (e)
	{
		//cannot find element
	}
}

function showIncorrect(id)
{
	try {
	document.getElementById(id).style.backgroundColor="red";
	document.getElementById(id).style.color="white";
	} catch (e)
	{
		//cannot find element
	}
}

function Proceed()
{
	try {
	document.getElementById("submitbutton").style.display="none";
	document.getElementById("proceed").style.display="block";
	} catch (e)
	{
		//cannot find element
	}
}


function TryAgain()
{
	try {
	document.getElementById("submitbutton").style.display="none";
	document.getElementById("tryagain").style.display="block";
	} catch (e)
	{
		//cannot find element
	}
}