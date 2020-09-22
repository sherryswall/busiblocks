<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"> 
<!--<xsl:import href="content.xsl"/>-->
<xsl:param name="modnumber" select="document('../../content.xml')/unit/@modulenumber"/> 
<xsl:param name="modname" select="document('../../content.xml')/unit/@modulename"/>
<xsl:param name="package" select="document('../../content.xml')/unit/@package"/>
<xsl:param name="xsl" select="current()/page/@xsl"/>
<xsl:param name="css" select="current()/page/@style"/>
<xsl:param name="pagename" select="current()/page/@name" />
 
<!--<xsl:include href="content.xsl"/> -->
<xsl:template match="page">
	<html>
	<meta http-equiv="Pragma" content="no-cache" />
	<meta http-equiv="Expires" content="-1" />
	<meta http-equiv="Cache-Control" content=" no-store, no-cache, must-revalidate, pre-check=0, post-check=0, max-age=0" />
	<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
	<title>Welcome to Medicare Australia</title>
	<head>
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
		<link rel="stylesheet" type="text/css" href="assets/css/content.css" media="screen" />
		<link rel="stylesheet" type="text/css" href="assets/css/page.css" media="screen" /> 
        <link rel="stylesheet" type="text/css" href="assets/css/interaction.css" media="screen" /> 
        <link rel="stylesheet" type="text/css" href="{$css}" media="screen" /> <!-- ensure this css page is linked last -->
		<script language="javascript">AC_FL_RunContent = 0;</script>
		<script src="assets/js/AC_RunActiveContent.js" language="javascript"></script>
		<script type="text/javascript" language="javascript" src="assets/js/page_interaction.js"></script>
	</head>
	<body onload="javascript:parent.PageComplete();javascript:parent.UpdatePage();" onunload="">
		
        
        
        
        
        <!--LMS Options  **************************************************************************** -->
		<!--<div id="lmsmenubar">
			<span id="modulename">Module <xsl:value-of select="$modnumber"/></span>
			<span id="username2">Welcome</span>
		</div>-->
		
        
        <!-- BANNER ******************************************************************************* -->
		<div id="banner">
			<div id="bg_top_left"></div>
            <div id="bg_top_right"></div>
            <div id="headingbackground">
				<div id="title"> 
					<h1 id="heading_title"><img src="assets/images/title_background.jpg" align="absmiddle" /><xsl:value-of select="$modname"/></h1>
					<!--<h2><span id="topicheading">Topic Heading</span></h2>-->
				</div>
			</div>
					

		</div>

		<!-- MAIN CONTENT ******************************************************************************* -->
		<!--<div id="main_content">-->
        <div id="bg_left"></div>
        <div id="bg_right"></div>
        <div id="mainframe">
			<div id="top_frame">
				<div id="topicheading">
                	<a id="close_btn" href="javascript:top.window.close();"></a>
					<a id="print_btn" href="content.xml" target="_blank"></a>
                    <a class="wide" id="acronyms_btn" href="javascript:parent.LoadUtility('acronyms');"></a>
                    <a class="wide" id="contact_btn" href="javascript:parent.LoadUtility('contacts');"></a>
					<a class="wide" id="modules_btn" href="javascript:parent.ShowMenu();"></a>
                    <span id="heading"><h1><xsl:value-of select="current()/content/topicheading"/></h1></span>
                </div>
                
                <div id="dropdown" onmouseover="javascript:parent.ShowMenu();" onmouseout="javascript:parent.HideMenu();">
					<xsl:for-each select="document('../../listOfModules.xml')/modules/module">
						<xsl:variable name="heading" select="current()/@modulename" />
						<xsl:variable name="id" select="current()/@modulenumber" />
							<a>
                            <!--<a href="javascript:parent.LoadPage('{$id}');">-->
								 <xsl:attribute name="id">
								 <xsl:value-of select="concat('menu',$id)" />
								 </xsl:attribute>
                                 <xsl:attribute name="href">
								 <xsl:value-of select="concat('javascript:window.top.location.replace(&quot;../',$id,'/index.htm&quot;)')" />
								 </xsl:attribute>
								 
								 <span class='menutitle'><xsl:value-of select="$heading"/></span>
							</a>
					</xsl:for-each>
				</div>
            </div>
            <div id="mid_frame">
            	<div id="topic_menu">
					<h3>Topics</h3>
            		<ul>
            		<xsl:for-each select="document('../../content.xml')/unit/topic">
						<xsl:variable name="heading" select="current()/@heading" />
						<xsl:variable name="indexed" select="current()/@indexed" />
						<xsl:variable name="id" select="current()/@pid" />
						<xsl:if test="$indexed='yes'">
							<li><a href="javascript:parent.LoadPage('{$id}');">
								 <xsl:attribute name="id">
								 <xsl:value-of select="concat('menu',$id)" />
								 </xsl:attribute>
								 <span class='menutitle'><xsl:value-of select="$heading"/></span>
							</a></li>
						</xsl:if>
					</xsl:for-each>
            		</ul>
				</div>
            	<div id="maincontent">
					<!--<xsl:apply-imports/>-->
                    <xsl:apply-templates/>
				</div>
        	</div>
            
            <div id="bottom_frame">
            	<div id="complete">
                	<div id="progress"></div>
                </div>
                <div id="progress_score">0% complete</div>
                <span id="navbar">
					<a href="javascript:parent.next_navigation();" id="next_btn" class="navigation"></a>
					<a href="javascript:parent.back_navigation();" id="back_btn" class="navigation"></a>
				</span>
                
            </div>
		</div>
		<!--</div>-->
		
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
		<!--</div>-->
        <!-- END MAIN CONTENT ******************************************************************************* -->


		
		<!-- FOOTER ******************************************************************************* -->
		<div id="footer">
			<div id="bg_bottom_left"></div>
            <div id="bg_bottom_right"></div>
            <div id="bg_bottom"></div>
            
            <!--<div id="main">
				<span id="options">
				
					<div id="choice"><form name='userform'><input type='radio' id='radio1' name='userflash' value='flashversion' checked='checked' onfocus="parent.Format='flashversion'"/>Flash version <input type='radio' id='radio2' name='userflash' value='textversion' onfocus="parent.Format='textversion'"/> HTML version</form></div>
					<div id="trackingNumber"><xsl:value-of select="current()/@name"/></div>
				</span>
				
			</div>-->
		</div>
		
        
        <div class="popUp" id="acronymsBox">
        	<h4>Acronyms</h4>
            <div id='acronymsList'><dl>
            <xsl:for-each select="document('../reference/acronyms.xml')/acronyms/acronym">
            	<dt><xsl:value-of select="current()/term" /></dt>
                <dd><xsl:value-of select="current()/description" /></dd>
            </xsl:for-each>
            </dl></div>
            <div class="popUpClose">
            	<a id='closeAcronymsBtn' href='#' onclick='document.getElementById("acronymsBox").style.display="none"'></a>
            </div>
        </div>
        
        <div class="popUp" id="contactsBox">
        	<h4>Contacts</h4>
            <div id='contactsList'><dl>
            <xsl:for-each select="document('../reference/contacts.xml')/page/content/modcontact">
            	<h5><xsl:value-of select="current()/name" /></h5>
                <p><xsl:value-of select="current()/position" /></p>
                <p><b>Phone:</b> <xsl:value-of select="current()/number" /></p>
                <p><b>Email: </b> <a> 
                	<xsl:attribute name="href">
					<xsl:value-of select="concat('mailto://',current()/email)" />
					</xsl:attribute>
					<xsl:value-of select="current()/email" /></a></p>
            </xsl:for-each>
            </dl></div>
            <div class="popUpClose">
            	<a id='closeContactsBtn' href='#' onclick='document.getElementById("contactsBox").style.display="none"'></a>
            </div>
        </div>
        
        
        
        
        
		<!-- PAGE DIAGNOSTICS ************************************************************************* -->
		<div id="diagnostics">
		</div>
		</body>
	</html>
</xsl:template>
<!--END PAGE TEMPLATE-->


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
		<!--<xsl:param name="width" select="current()/@width"/>
		<p style="width:{$width};"><xsl:copy-of select="current()"/></p>-->
        <p><xsl:copy-of select="current()"/></p>
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

<!--Rollover*********************************************************************************-->
<xsl:template match="rollover">
	<xsl:param name="rid" select="current()/@id"/>
	<xsl:param name="type" select="current()/@type"/>
	<div id="rollover" name="rollover">
		<div id="rolloverobject">
			<a href="javascript:ClickShow('{$rid}','{$type}')" onmouseover="javascript:Show('{$rid}','{$type}')" onmouseout="javascript:Hide('{$rid}','{$type}');">
				<!--<xsl:copy-of select="current()/rolloverobject"/>-->
				<xsl:apply-templates select="current()/rolloverobject"/>
			</a>
		</div>
		<div id="rollovertarget">
		<xsl:choose>
			<!--if type is equal to "rollover" insert rollover div tags-->
			<xsl:when test="$type='rollover' ">
				<div id="{$rid}"  style="display:none;" class="rollovertarget" name="rollovertarget">
					<xsl:apply-templates select="current()/rollovertarget"/>
				</div>
			</xsl:when>
			<!--if type is equal to "click" insert div tags-->
			<xsl:when test="$type='click'">
				<div name="clicktarget" class="click">
					<xsl:attribute name="id">
                    	<xsl:copy-of select="concat('c',$rid)"/>
						<!--<xsl:value-of select="concat('c',$rid)"/>-->
					</xsl:attribute>
					<xsl:apply-templates select="current()/clicktarget"/>
				</div>
			</xsl:when>
			<!--if type is equal to "both" insert rollover and click div tags-->
			<xsl:when test="$type='both'">
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
	</div> <!--Close rollover div-->
	<div id="line"></div>
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
		   <xsl:apply-templates/> <!--otherwise display normal-->
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
		<p class="instruction"><xsl:copy-of select="current()"/></p>
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


<!-- insert other templates-->
</xsl:stylesheet>
