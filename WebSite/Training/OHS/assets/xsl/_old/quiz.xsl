<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<!--global variables lie outside templates-->
<xsl:variable name="filename" select="//page/@name" /> <!--get filename-->
<xsl:variable name="folder" select="substring($filename,1,3)"/> <!--get folder for assets management-->
<xsl:variable name="location" select="concat('',$filename,'.xml')"/>
<xsl:param name="css" select="//page/@style" /> <!--get css-->

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
