<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"> 
<!--<xsl:import href="content.xsl"/>-->
<xsl:param name="modnumber" select="document('../../content.xml')/unit/@modulenumber"/> 
<xsl:param name="modname" select="document('../../content.xml')/unit/@modulename"/>
<xsl:param name="package" select="document('../../content.xml')/unit/@package"/>
 
<xsl:template match="page">
	<html>
	<meta http-equiv="Pragma" content="no-cache" />
	<meta http-equiv="Expires" content="-1" />
	<meta http-equiv="Cache-Control" content=" no-store, no-cache, must-revalidate, pre-check=0, post-check=0, max-age=0" />
	<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
	<title>Welcome to the Medicare Australia</title>
	<head>
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
		<link rel="stylesheet" type="text/css" href="assets/css/content.css" media="screen" />
		<link rel="stylesheet" type="text/css" href="assets/css/page.css" media="screen" /> <!-- ensure this css page is linked last -->
		<script language="javascript">AC_FL_RunContent = 0;</script>
		<script src="assets/js/AC_RunActiveContent.js" language="javascript"></script>
		<script type="text/javascript" language="javascript" src="assets/js/page_interaction.js"></script>
	</head>
	<body onload="javascript:parent.UpdateUtility();" onunload="">
		<!--LMS Options  **************************************************************************** -->
		<div id="lmsmenubar">
			<span id="modulename">Module <xsl:value-of select="$modnumber"/></span>
			<span id="username">Welcome</span>
		</div>
		<!-- BANNER ******************************************************************************* -->
		<div id="banner">
			<div id="title"> 
				<h1><xsl:value-of select="$modname"/></h1>
				<h2>Course Map</h2>
			</div>
		</div>
		
		<!-- MENU BAR  ****************************************************************************** -->
		<div id="menu">
			<span class="menu2"><a href="javascript:" onmousedown="javascript:parent.ShowMenu();">Topics</a></span>
			<span class="menu1">
				<ul>
					<li><a class="menu1one" href="javascript:window.close();">Close</a></li>
					<li><a class="menu1two" href="content.xml" target="_blank">Print module</a></li>
					<li><a class="menu1three" href="#nogo">Contacts</a></li>
					<li><a class="menu1four" href="#nogo">Glossary</a></li>
					<li><a class="menu1five" href="resources.xml">Resources</a></li>
				</ul>
			</span>
		</div>
		
		<!-- MAIN CONTENT ******************************************************************************* -->
		<div id="maincontent">
			<div id="print"><span class="printpage"><a href="#" onclick="print();" accesskey="h">Print page</a></span></div>
			<div id="maintext">
				<h1>Course Map</h1>
				<xsl:apply-templates select="document('../../content.xml')/unit/topic"/>
			</div>
		</div>
		
		<div id="dropdown" onmouseover="javascript:parent.ShowMenu();" onmouseout="javascript:parent.HideMenu();">
			<ul><xsl:for-each select="document('../../content.xml')/unit/topic">
					<xsl:variable name="heading" select="current()/@heading" />
					<xsl:variable name="indexed" select="current()/@indexed" />
					<xsl:variable name="id" select="current()/@pid" />
					<xsl:if test="$indexed='yes'">
						<li>
							<a class='menutitle' href="javascript:parent.LoadPage('{$id}');">
								 <xsl:attribute name="id">
								 <xsl:value-of select="concat('menu',$id)" />
								 </xsl:attribute>
								 <xsl:value-of select="$heading"/>
							</a>
						</li>
					</xsl:if>
			</xsl:for-each>
		</ul>
		</div>
	
		<!-- END MAIN CONTENT ******************************************************************************* -->
		
		<!-- FOOTER ******************************************************************************* -->
		<div id="footer">
			<div id="main">
				<span id="options">
					<span id="choice"><form name='userform'><input type='radio' id='radio1' name='userflash' value='flashversion' checked='checked' onfocus='parent.Format="flashversion"' />Flash version <input type='radio' id='radio2' name='userflash' value='textversion' onfocus='parent.Format="textversion"' /> HTML version</form>
					</span>
					<div id="complete"><div id="progress"></div></div>
					<span id="trackingNumber"><xsl:value-of select="current()/@name"/></span>
				</span>
				<span id="navbar">
					<a href="javascript:parent.back_navigation();" id="back_btn" class="navigation"></a>
				</span>
			</div>
		</div>
		
		<!-- END FOOTER ******************************************************************************* -->
		<span id="logo"><a href="http://www.familycourt.gov.au/" target="_blank"></a></span>
		
		<!-- PAGE DIAGNOSTICS ************************************************************************* -->
		<div id="diagnostics">
		</div>
		</body>
	</html>
</xsl:template>
<!--END PAGE TEMPLATE-->

<!-- for each topic -->
<xsl:template match="topic">
	<h2>Topic: <xsl:value-of select="current()/@heading"/></h2>
	<ul><xsl:for-each select="current()/page">
			<xsl:variable name="id" select="current()/@pid"/>
			<li>
				<a href="javascript:parent.LoadPage('{$id}');"><xsl:value-of select="current()/subindextitle"/></a>
			</li>
		</xsl:for-each>
		<hr />
	</ul>		
</xsl:template>
<!-- insert other templates-->
</xsl:stylesheet>
