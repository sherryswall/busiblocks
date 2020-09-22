<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<!--<xsl:include href="content.xsl"/> 
<xsl:include href="quiz.xsl"/>-->
<xsl:param name="modnumber" select="document('../../content.xml')/unit/@modulenumber"/> 
<xsl:param name="modname" select="document('../../content.xml')/unit/@modulename"/>
<xsl:param name="package" select="document('../../content.xml')/unit/@package"/>

<xsl:template match="unit">
<html>
<head>
	<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
	<script type="text/javascript" language="javascript" src="assets/js/print.js"></script>
	<link rel="stylesheet" type="text/css" href="assets/css/content.css" media="screen" />
	<link rel="stylesheet" type="text/css" href="assets/css/print.css" />
	<title><xsl:value-of select="$modname"/></title>
<script type="text/javascript">
function runFlash()
{
 //do nothing
}
function CheckFlash()
{
 //do nothing
}
function Show()
{
 //do nothing
}
function Hide()
{
 //do nothing
}
</script>
</head>

<body onload="showPrint();">
<div id="printmodule">
	<h4>Print Friendly Version</h4>
	<p>Click <a href="#" onclick="print();" accesskey="h">here</a> to print this page.</p>
	<h1>Module <xsl:value-of select="$modnumber"/> : <xsl:value-of select="$modname"/></h1>
		<xsl:apply-templates select="current()/topic"/>
</div>
<span id="logo"></span>
</body>
</html>
</xsl:template>

<!-- for each topic -->
<xsl:template match="topic">
	<h2>Topic: <xsl:value-of select="current()/@heading"/></h2>
		<xsl:for-each select="current()/page">
		<div id="page">
			<xsl:variable name="file" select="concat('../../',current()/name,'.xml')"/>
			<xsl:apply-templates select="document($file)"/>
		</div>
		</xsl:for-each>
		<hr />	
</xsl:template>

<!-- for each page -->
<xsl:template match="page">
		<!--<xsl:apply-imports/>-->
        <xsl:apply-templates/>
</xsl:template>


<!--ROOT NODE TEMPLATE for CONTENT TAGS ******************************************************************************* -->
<xsl:template match="content"> 
	<!--<div id="topicheading">
		<span id="heading"><h1><xsl:value-of select="current()/topicheading"/></h1></span>
		<span id="printpage">
			<a id="notes" href="javascript:parent.ShowDialogueBox('notebox');" title="Add Notes"></a>
			<a id="print" href="content.xml" target="_blank" title="Print Version"></a>
		</span>
	</div>-->
	<div id="mainsection">
		<div id="graphicbox">
			<xsl:apply-templates select="current()/graphicbox/image"/>
			<xsl:apply-templates select="current()/graphicbox/linkbox"/>
		</div>
		<div id="maintext">
			<!-- Apply all templates-->
			<xsl:apply-templates/>
		</div>
	</div>
</xsl:template>

<!-- ModuleHeading and Topicheading show nothing ******************************************************************************* -->
<xsl:template match="moduleheading"></xsl:template>

<xsl:template match="topicheading"></xsl:template>

<!-- HEADING  ******************************************************************************* -->
<xsl:template match="pageheading">
	<h1><xsl:value-of select="current()"/></h1>
</xsl:template>

<!-- SUBHEADING  ******************************************************************************* -->
<xsl:template match="subheading">
		<h2><xsl:value-of select="current()"/></h2>
</xsl:template>

<!-- PARAGRAPH  ******************************************************************************* -->
<xsl:template match="paragraph">
		<xsl:param name="width" select="current()/@width"/>
		<p style="width:{$width};"><xsl:copy-of select="current()"/></p>
</xsl:template>

<!-- SKILLS  ******************************************************************************* -->
<xsl:template match="skills">
		<div id="bluebox"><p><xsl:copy-of select="current()"/></p></div>
</xsl:template>

<!-- ACTIVITIES  ******************************************************************************* -->
<xsl:template match="activities">
		<div id="bluebox"><p><xsl:copy-of select="current()"/></p></div>
</xsl:template>

<!-- QUOTE  ******************************************************************************* -->
<xsl:template match="quote">
		<p class="quote"><xsl:copy-of select="current()"/></p>
</xsl:template>

<!-- HTML  ******************************************************************************* -->
<xsl:template match="plainhtml">
		<xsl:copy-of select="current()"/>
</xsl:template>

<!-- HIGHLIGHT  ******************************************************************************* -->
<xsl:template match="highlight">
		<table class="highlight">
			<xsl:attribute name="width">
				<xsl:value-of select="current()/@width"/>
			</xsl:attribute>
		  <tr>
			<td><xsl:copy-of select="current()"/></td>
		  </tr>
		</table>
</xsl:template>

<!-- TEXTBOX ******************************************************************************* -->
<xsl:template match="textbox">
	<xsl:param name="rows" select="current()/@rows"/>
	<xsl:param name="length" select="current()/@length"/>
	<xsl:param name="id" select="current()/@id"/>
	<xsl:param name="name" select="current()/@name"/>
	<textarea id="{$id}" name="{$name}" cols="{$length}" rows="{$rows}" class="textbox"></textarea>
</xsl:template>

<!-- LIST  ******************************************************************************* -->
<xsl:template match="list">
<p class="listheading"><xsl:copy-of select="current()/listheading"/></p>
	<ul>
		<xsl:apply-templates select="point"/> <!--create a point tag and look for templates within-->
	</ul>
</xsl:template>

<!-- NUMLIST  ******************************************************************************* -->
<xsl:template match="numlist">
<p class="listheading"><xsl:copy-of select="current()/listheading"/></p>
	<ol>
		<xsl:apply-templates select="point"/> <!--create a point tag and look for templates within-->
	</ol>
</xsl:template>

<!--Point *********************************************************************************-->
<xsl:template match="point">
	<li>
		<xsl:copy-of select="current()"/>
	</li>
</xsl:template>


<!--Option *********************************************************************************-->
<xsl:template match="option">
    <dl>
    <dt> <xsl:apply-templates select="current()/fixeditem"/> </dt>
    <dd> <xsl:apply-templates select="current()/linkeditem"/> </dd>
    </dl>
    <p> <xsl:apply-templates select="current()/correct"/> </p>
</xsl:template>



<!--Rollover*********************************************************************************-->
<xsl:template match="rollover">
	<!--<xsl:param name="rid" select="current()/@id"/>
	<xsl:param name="type" select="current()/@type"/>-->
	
    <dl>
    <dt> <xsl:apply-templates select="current()/rolloverobject"/> </dt>
    <dd> <xsl:apply-templates select="current()/rollovertarget"/> </dd>
    </dl>
    
<!--    <div id="rollover" name="rollover">
		<div id="rolloverobject">
			<a href="javascript:ClickShow('{$rid}','{$type}')" onmouseover="javascript:Show('{$rid}','{$type}')" onmouseout="javascript:Hide('{$rid}','{$type}');">
-->				<!--<xsl:copy-of select="current()/rolloverobject"/>-->
<!--				<xsl:apply-templates select="current()/rolloverobject"/>
			</a>
		</div>
		<div id="rollovertarget">
		<xsl:choose>
-->			<!--if type is equal to "rollover" insert rollover div tags-->
<!--			<xsl:when test="$type='rollover' ">
				<div id="{$rid}"  style="display:none;" class="rollovertarget" name="rollovertarget">
					<xsl:apply-templates select="current()/rollovertarget"/>
				</div>
			</xsl:when>
-->			<!--if type is equal to "click" insert div tags-->
<!--			<xsl:when test="$type='click'">
				<div name="clicktarget" class="click">
					<xsl:attribute name="id">
                    	<xsl:copy-of select="concat('c',$rid)"/>
-->						<!--<xsl:value-of select="concat('c',$rid)"/>-->
<!--					</xsl:attribute>
					<xsl:apply-templates select="current()/clicktarget"/>
				</div>
			</xsl:when>
-->			<!--if type is equal to "both" insert rollover and click div tags-->
<!--			<xsl:when test="$type='both'">
				<div id="{$rid}" class="rollovertarget" name="rollovertarget">
						<xsl:apply-templates select="current()/rollovertarget"/>
				</div>
				<div name="clicktarget" class="click">
						<xsl:attribute name="id">
							<xsl:value-of select="concat('c',$rid)"/>
						</xsl:attribute>
						<xsl:apply-templates select="current()/clicktarget"/>
				</div>
			</xsl:when>
		</xsl:choose>
		</div>
	</div> 
-->    <!--Close rollover div-->
	<div id="line"></div>
</xsl:template>



<!--Sorting Exercise *********************************************************************************-->
<xsl:template match="category">
    <xsl:variable name="category" select="current()/@title" />
    <h4><xsl:value-of select="$category" /></h4>
    <ul>
    	<xsl:for-each select="current()/rollover">
        	<li><xsl:copy-of select="current()" /></li>
        </xsl:for-each>
    </ul>
</xsl:template>

<xsl:template match="corr_feedback">

</xsl:template>

<xsl:template match="incorr_feedback">

</xsl:template>

<!-- FLASH ******************************************************************************* -->
<xsl:template match="flash">
<!--get attributes for flash-->
	<xsl:param name="src" select="current()/@src"/>
	<xsl:param name="width" select="current()/@width"/>
	<xsl:param name="height" select="current()/@height"/>
	<xsl:param name="title" select="current()/@title"/>
	<div id="flashversion">
		<!--Remember also that the flash requireds the filename of the XML doc to feed off XML tags-->
		<script language="javascript">parent.runFlash('<xsl:value-of select="$src"/>','<xsl:value-of select="$width"/>','<xsl:value-of select="$height"/>','flashversion');</script>
		<xsl:apply-templates select="current()/instruction"/>
	</div>
	
	<div id="textversion">
		       <xsl:choose>
    				<xsl:when test="current()/feedback">
    					<!-- Branch for:
                        	* Multiple Choice Knowledge Check (Multiple answer)
                            * Drag and Drop Knowledge Check -->
                        
                        <xsl:choose>
                        	<!-- FIND Drag and Drop -->
                            <xsl:when test="current()/option/fixeditem">
                            	<xsl:apply-templates/>
                            </xsl:when>
                            <!-- FIND Multiple Answer Multiple Choice -->
                            <xsl:otherwise>
                        		<p><b>Correct Answers: </b></p>
                                <ul>
                                <xsl:for-each select="current()/option">
                                    <xsl:if test="correct = 'True'">
                                         <li><xsl:value-of select="answer" /></li>
                                    </xsl:if>
                                </xsl:for-each>
                                </ul>
                                <p><b>Incorrect Answers: </b></p>
                                <ul>
                                <xsl:for-each select="current()/option">
                                    <xsl:if test="correct = 'False'">
                                         <li><xsl:value-of select="answer" /></li>
                                    </xsl:if>
                                </xsl:for-each>
                                </ul>
                                <p> <xsl:copy-of select="current()/feedback"/> </p>
                            </xsl:otherwise>
                        </xsl:choose>  
    				</xsl:when>
    				<xsl:otherwise>
    					 <xsl:choose>
                        	<!-- FIND SORTING EXERCISE -->
                            <xsl:when test="current()/category">
                        		<xsl:apply-templates/>
                            </xsl:when>
                            <!-- END SORTING EXERCISE -->
                            <xsl:otherwise>
                        		<xsl:choose>
                                	<!-- FIND SINGLE ANSWER MULTIPLE CHOICE-->
                                    <xsl:when test="current()/option/feedback">
                                    	<xsl:for-each select="current()/option">
                                        	<xsl:if test="correct = 'True'">
                                        		<p><b>Correct Answer: </b><xsl:value-of select="answer" /></p>
                                            </xsl:if>
                                        </xsl:for-each>
                                        <p><b>Incorrect Answers: </b></p>
                                		<ul>
                                			<xsl:for-each select="current()/option">
                                    			<xsl:if test="correct = 'False'">
                                         			<li><xsl:value-of select="answer" /></li>
                                    			</xsl:if>
                                			</xsl:for-each>
                                		</ul>
                                	<p> <xsl:copy-of select="current()/feedback"/> </p>
                                    </xsl:when>
                                    <!-- END SINGLE ANSWER MULTIPLE CHOICE -->
                                    <xsl:otherwise>
                                		<xsl:choose>
                                        <!-- FIND TRUE/FALSE QUESTION -->
                                        <xsl:when test="current()/option/answer">
                                        	<xsl:for-each select="current()/option">
                                    			<xsl:if test="correct != ''">
                                         			<p>The correct answer is <b> <xsl:value-of select="answer"/></b>.</p>
                                                    <p><xsl:value-of select="correct" /></p>
                                    			</xsl:if>
                                			</xsl:for-each>
                                        </xsl:when>
                                        <xsl:otherwise>
                                        	<xsl:apply-templates/>
                                        </xsl:otherwise>
                                     	</xsl:choose>
                                    </xsl:otherwise>
                                </xsl:choose>
                            </xsl:otherwise>
                        </xsl:choose>
                        
    				</xsl:otherwise>
    			</xsl:choose>
			
            <!--otherwise display normal-->
	</div>
</xsl:template>
<!-- FLASH FLOAT RIGHT-->
<xsl:template match="flashright">
<!--get attributes for flash-->
	<xsl:param name="src" select="current()/@src"/>
	<xsl:param name="width" select="current()/@width"/>
	<xsl:param name="height" select="current()/@height"/>
	<xsl:param name="title" select="current()/@title"/>
	<div id="flashright">
		<!--Remember also that the flash requireds the filename of the XML doc to feed off XML tags-->
		<script language="javascript">parent.runFlash('<xsl:value-of select="$src"/>','<xsl:value-of select="$width"/>','<xsl:value-of select="$height"/>','flashright');</script>
		<xsl:apply-templates select="current()/instruction"/>
	</div>
	
	<div id="textversion">
		   <xsl:apply-templates/> <!--otherwise display normal-->
	</div>
</xsl:template>
<!-- Instruction ******************************************************************************* -->
<xsl:template match="instruction">
		<!--<p class="instruction"><xsl:copy-of select="current()"/></p>-->
</xsl:template>

<!-- TURN FEEDBACK OFF ******************************************************************************* -->
<xsl:template match="feedback">
		<!--<p class="instruction"><xsl:copy-of select="current()"/></p>-->
</xsl:template>


<!-- TextInstruction ******************************************************************************* -->
<xsl:template match="textinstruction">
		<p class="textinstruction"><xsl:copy-of select="."/></p>
</xsl:template>

<!-- Infoquest ******************************************************************************* -->
<xsl:template match="infoquest">
		<p class="infoquest"><xsl:copy-of select="."/></p>
</xsl:template>

<!-- GRAPHIC BOX  ******************************************************************************* -->
<xsl:template match="graphicbox">
	<!--<div id="graphicbox"><xsl:apply-templates/></div>-->
</xsl:template>

<!-- Image ********************************************************************************* -->
<xsl:template match="image">
<!-- if bounded inside graphic box use graphic box tags -->
		<xsl:param name="src" select="current()/@src"/>
		<xsl:param name="width" select="current()/@width"/>
		<xsl:param name="height" select="current()/@height"/>
		<xsl:param name="title" select="current()/@alt"/>
		<xsl:param name="type" select="current()/@type"/>
		<div id="graphic">
			<img src="{$src}" width="{$width}" height="{$height}" alt="{$title}" align="top"/>
		</div>
</xsl:template>

<!-- Review box  ********INSERTS A LINKBOX WITH RESOURCES FROM RESOURCE.********************************************** -->
<xsl:template match="reviewbox">
	<xsl:param name="width" select="current()/@width"/>
    <xsl:param name="href" select="current()/@url"/>    
	<div id="linkbox" style="width:{$width};">
		<h2>Need assistance?</h2> <!-- for each resource in table, look for the matching resource in resource.xml matching resource serach id to resource id -->
		<ul><li><a href="javascript:parent.LoadReview('{$href}');"><xsl:value-of select="current()"/></a></li></ul>
	</div>
</xsl:template>

<!-- LINKBOX  ********INSERTS A LINKBOX WITH RESOURCES FROM RESOURCE.********************************************** -->
<xsl:template match="linkbox">
	<xsl:variable name="width" select="current()/@width"/>
	<div id="linkbox" style="width:{$width};">
		<h2>Additional information</h2> <!-- for each resource in table, look for the matching resource in resource.xml matching resource serach id to resource id -->
		<ul>
    <xsl:for-each select="document('../../resources.xml')/page/content/modresource">
        <xsl:variable name="pagename" select="current()/page/@name" />
        <xsl:variable name="pageid" select="current()/@id"/>
        <xsl:if test="$pageid=$pagename">
        <li><a target="_blank">
            <xsl:attribute name="href">
                <xsl:value-of select="current()/url" />
            </xsl:attribute>
            <xsl:value-of select="name"/>
            </a></li>	
        </xsl:if> 
    </xsl:for-each>
		</ul>
	</div>
</xsl:template>
<!--  Module resource *************************************************************   --> 
<xsl:template match="modresource">
 <div id="utility">
    <li>
        <a target="_blank"><xsl:attribute name="href"><xsl:value-of select="current()/url" /></xsl:attribute><xsl:value-of select="name" /></a>
    </li>
 </div>
</xsl:template>
 
<!--  Module Glossary *************************************************************   --> 
<xsl:template match="modglossary">
 <div id="utility">
        <div id="utilityHead"><xsl:value-of select="current()/name" /></div><br />
        <xsl:value-of select="current()/description" /><br /><br />
 </div>
</xsl:template>
 
<!--  Module contacts *************************************************************   --> 
<xsl:template match="modcontact">
 <div id="utility">
            <div id="utilityHead"><xsl:value-of select="current()/name" /></div><br />
            <!----><xsl:value-of select="current()/position" /><br />
            <b>Phone: </b> <xsl:value-of select="current()/number" /><br />
            <b>Email: </b> <xsl:value-of select="current()/email" /><br /><br />
 </div> 
</xsl:template>



<!-- LINK  ******************************************************************************* -->
<xsl:template match="link">
	<xsl:param name="href" select="current()/@href"/>
	<xsl:param name="target" select="current()/@target"/>
	<a href="{$href}" target="{$target}"><xsl:apply-templates/><br/></a> <!--create a link and then look for content inside-->
</xsl:template>


<!-- EMAIL FORM  ******************************************************************************* -->
<xsl:template match="emailform">
<!--
<div id="emailform">
	<form name="form" aid="form" action="inductionemail.asp" method="post" onSubmit="" >
	<input name="title" type="hidden" id="title" value="{$title}"/>
	  <xsl:apply-templates/>
	  <p><div id="submitbutton"><input type="submit" value="Send"/></div>
	  </p>
	</form>
</div>
<div id="feedback">
  <p><xsl:copy-of select="current()/feedback"/></p>
</div>
-->
</xsl:template>

<!--  Module resource ************************************************************* 
  --> 
<xsl:template match="modresource">
    <li>
        <a target="_blank">
        <xsl:attribute name="href">
          <xsl:value-of select="current()/url" /> 
          </xsl:attribute>
          <xsl:value-of select="name" /> 
        </a>
    </li>
  </xsl:template>
  













<!--ROOT NODE TEMPLATE for QUIZ TAGS ******************************************************************************* -->
<!--ROOT NODE TEMPLATE = PAGE ******************************************************************************* -->
<xsl:template match="/"> 
<!-- Read Style attribute for page -->
<html>
<head>
	<script type="text/javascript" language="javascript" src="assets/js/page.js"></script>
	<script type="text/javascript" language="javascript" src="assets/js/quiz.js"></script>
	<script language="javascript">AC_FL_RunContent = 0;</script>
	<script src="assets/js/AC_RunActiveContent.js" language="javascript"></script>
	<link rel="stylesheet" type="text/css" href="assets/css/page.css"/>
	<link rel="stylesheet" type="text/css" href="assets/css/quiz.css" media="screen" />
	<!-- DO NOT CHANGE THE ORDER OF THESE CSS LINKS BECAUSE THE FLASH FUNCTION LOOKS FOR THE LAST TWO STYLES OF THE CSS SHEET -->
</head>
<body onLoad="javascript:CheckFlash();">	
	<div id="maintext">
		<!-- Apply all templates-->
		<xsl:apply-templates/>
	</div>
</body>
</html>
</xsl:template>

<!--MODULE HEADING TEMPLATE ******************************************************************************* -->
<xsl:template match="moduleheading">
</xsl:template>

<!--TOPIC TEMPLATE ******************************************************************************* -->
<xsl:template match="topicheading">
</xsl:template>

<!--PAGEHEADING TEMPLATE ******************************************************************************* -->
<xsl:template match="pageheading">
	<h1><xsl:value-of select="current()"/></h1>
</xsl:template>

<!--CONTENT TEMPLATE ******************************************************************************* -->
<xsl:template match="content">
	<xsl:apply-templates/> <!--apply templates to all nodes with content-->
</xsl:template>

<!-- HEADING  ******************************************************************************* -->
<xsl:template match="heading">
	<h2><xsl:value-of select="."/></h2>
</xsl:template>

<!-- SUBHEADING  ******************************************************************************* -->
<xsl:template match="subheading">
		<h3><xsl:value-of select="."/></h3>
</xsl:template>

<!-- PARAGRAPH  ******************************************************************************* -->
<xsl:template match="paragraph">
		<p><xsl:value-of select="."/></p>
</xsl:template>

<!-- MULTIPLE CHOICE QUESTION ******************************************************************************* -->
<xsl:template match="question">

<xsl:variable name="questiontype" select="current()/@type" />

<div id="question">
<div id="questionheading"><xsl:copy-of select="current()/questiontext"/></div>

<xsl:if test="$questiontype='radio'">
	<table>
		<tr><td colspan="2"><span class="instruction">Select the correct answer and click Submit.</span></td></tr>
		<xsl:for-each select="current()/answer">
			<tr><td>
			<input>
				<xsl:attribute name="type">radio</xsl:attribute>
				<xsl:attribute name="name">RadioGroup</xsl:attribute>
				<xsl:attribute name="value">
					<xsl:value-of select="@value" />
				</xsl:attribute>
			</input>
			</td><td width="95%"><xsl:attribute name="id"><xsl:value-of select="@id" /></xsl:attribute><b><xsl:value-of select="@id" />) </b><xsl:value-of select="current()"/></td></tr>
		</xsl:for-each>
	</table>
	<div id="submitbutton"><input type="button" value="Submit" onclick="TestRadioCorrect();" /></div>
</xsl:if>

<xsl:if test="$questiontype='check'">
	<xsl:variable name="total" select="current()/@total" />
	<table>
		<tr><td colspan="2"><span class="instruction">Select the correct answers and click Submit.</span></td></tr>
		<xsl:for-each select="current()/answer">
			<tr><td>
			<input>
				<xsl:attribute name="type">checkbox</xsl:attribute>
				<xsl:attribute name="name">CheckGroup</xsl:attribute>
				<xsl:attribute name="value">
					<xsl:value-of select="@value" />
				</xsl:attribute>
			</input>
			</td><td width="95%"><xsl:attribute name="id"><xsl:value-of select="@id" /></xsl:attribute><b><xsl:value-of select="@id" />) </b><xsl:value-of select="current()"/></td></tr>
		</xsl:for-each>
	</table>
	<div id="submitbutton"><input type="button" value="Submit" onclick="TestCheckboxCorrect('{$total}');"/></div>
</xsl:if>



<div id="proceed" style="display:none; text-align:left"><p>Well done, that is correct. <xsl:copy-of select="current()/feedback" /> Click OK to continue.</p>
<input type="button" value="OK" onclick="parent.next_navigation();" /></div>
<div id="tryagain" style="display:none; text-align:left"><span id="alert"><p>Sorry that was incorrect. Please try again.</p></span>
<input type="button" value="Try Again" onclick="javascript:window.location.reload();" /></div>
</div>
</xsl:template>

<!--Point *********************************************************************************-->
<xsl:template match="point">
	<li>
		<xsl:value-of select="current()"/>
	</li>
</xsl:template>

<!-- Image ********************************************************************************* -->
<xsl:template match="image">
	<!--get attributes for flash-->
	<xsl:param name="src" select="current()/@src"/>
	<xsl:param name="width" select="current()/@width"/>
	<xsl:param name="height" select="current()/@height"/>
	<xsl:param name="title" select="current()/@title"/>
	<div id="graphicbox">
			<img src="{$src}" width="{$width}" height="{$height}" alt="{$title}" />
	</div>
</xsl:template>

<!-- LINK  ******************************************************************************* -->
<xsl:template match="link">
			<a><xsl:apply-templates/></a> <!--create a link and then look for content inside-->
</xsl:template> 


</xsl:stylesheet>