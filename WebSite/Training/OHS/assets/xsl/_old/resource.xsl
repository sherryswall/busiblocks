<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"> 
<!--<xsl:import href="content.xsl"/>-->
<xsl:param name="modnumber" select="document('../../content.xml')/unit/@modulenumber"/> 
<xsl:param name="modname" select="document('../../content.xml')/unit/@modulename"/>
<xsl:param name="package" select="document('../../content.xml')/unit/@package"/>
<xsl:param name="xsl" select="current()/page/@xsl"/>
<xsl:param name="css" select="@style"/>
 
<xsl:include href="utility.xsl"/> 
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
				<!--<h2><span id="topicheading">Topic Heading</span></h2>-->
			</div>
					
			<!-- MENU BAR  ****************************************************************************** -->
			<div id="menu">
				<span id="lms">
					<span id="username">Welcome</span>
					<a class="dropdown" href="javascript:parent.ShowMenu();"></a>
				</span>
			
				<!--<span class="menu2"><a href="javascript:parent.DoNothing();" onmousedown="javascript:parent.ShowMenu();">Topics</a></span>-->
				<span id="menu1">
					   <a class="menu1one" href="javascript:top.window.close();"></a>
						<a class="menu1two" href="javascript:parent.LoadUtility('contacts.xml');"></a>
						<a class="menu1three" href="javascript:parent.LoadUtility('resources.xml');"></a>
						<a class="menu1four" href="javascript:parent.LoadUtility('glossary.xml');"></a>
				</span>
			</div>
		</div>

		<!-- MAIN CONTENT ******************************************************************************* -->
		<div id="mainframe">
			<div id="maincontent">
			<!--
				<div id="print"><span class="breadcrumb"><a href="javascript:parent.GoToCurrentTopic();" ><span id="breadcrumb">Breadcrumb</span></a> ></span><span class="printpage"><a href="#" onclick="print();" accesskey="h">Print page</a></span></div> -->
				<!--  Apply all content templates--> 
				<xsl:apply-imports/>
			</div>
		</div>
		<div id="dropdown" onmouseover="javascript:parent.ShowMenu();" onmouseout="javascript:parent.HideMenu();">
			<xsl:for-each select="document('../../content.xml')/unit/topic">
					<xsl:variable name="heading" select="current()/@heading" />
					<xsl:variable name="indexed" select="current()/@indexed" />
					<xsl:variable name="id" select="current()/@pid" />
					<xsl:if test="$indexed='yes'">
							<a href="javascript:parent.LoadPage('{$id}');">
								 <xsl:attribute name="id">
								 <xsl:value-of select="concat('menu',$id)" />
								 </xsl:attribute>
								 <span class='menutitle'><xsl:value-of select="$heading"/></span>
							</a>
					</xsl:if>
			</xsl:for-each>
		
		</div>
		
		
		<!-- POP-UP for notes******************************************************************************* -->
		<div id="notebox">
			<h4>Notes</h4>
			   <span id="close"><a href="javascript:parent.HideDialogueBox('notebox');" id="close_btn">Close</a></span>
			  <textarea class="comments" name="Comments" cols="42" rows="10" id="commentbox"></textarea>
			  <input type="submit" name="Add Comment" value="Save Notes" onmousedown="javascript:parent.AddNotesToPage();"/>
			<span id="notesaved">Your notes have been saved.<img src="assets/images/check2.gif"/></span>
		</div>
		
		<!-- POP-UP for bookmarks******************************************************************************* -->
		<div id="bookmarkbox">
			<h4>Bookmarks</h4>
			<span id="close"><a href="javascript:parent.HideDialogueBox('bookmarkbox');" id="close_btn">Close</a></span>
			<div id="bookmarks">No Bookmarks saved.</div>
			<input type="submit" name="Submit" value="Bookmark this Page" onmousedown="javascript:parent.BookmarkPage();"/>
			<span id="bookmarksaved">This page has been bookmarked. <img src="assets/images/check2.gif"/></span>
		</div>
		<!-- END MAIN CONTENT ******************************************************************************* -->
		<!-- END MAIN CONTENT ******************************************************************************* -->
		
		<!-- FOOTER ******************************************************************************* -->
		<div id="footer">
			<div id="main">
				<span id="options">
				<div id="complete"><div id="progress"></div></div>
					<div id="choice"><form name='userform'><input type='radio' id='radio1' name='userflash' value='flashversion' checked='checked' onfocus="parent.Format='flashversion'"/>Flash version <input type='radio' id='radio2' name='userflash' value='textversion' onfocus="parent.Format='textversion'"/> HTML version</form></div>
					<div id="trackingNumber"><a href="javascript:parent.ShowDialogueBox();"><xsl:value-of select="current()/@name"/></a></div>
				</span>
				<span id="navbar">
					<a href="javascript:parent.next_navigation();" id="next_btn" class="navigation"></a>
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


<!-- insert other templates-->
</xsl:stylesheet>
