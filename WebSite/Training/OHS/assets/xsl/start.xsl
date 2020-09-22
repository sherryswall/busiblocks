<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:template match="page">
<xsl:param name="modnumber" select="document('../../content.xml')/unit/@modulenumber"/> 
<xsl:param name="modname" select="document('../../content.xml')/unit/@modulename"/>
<xsl:param name="src" select="current()/bannerimage/@src" />
<xsl:param name="css" select="current()/@css" />

<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
<title><xsl:value-of select="start/title" /></title>
<!--<link rel="stylesheet" type="text/css" href="assets/css/content.css"/>-->
<link rel="stylesheet" type="text/css" href="assets/css/start.css"/>
<script language="javascript">AC_FL_RunContent = 0;</script>
<script src="assets/js/AC_RunActiveContent.js" language="javascript"></script>
<script type="text/javascript" language="javascript" src="assets/js/page_interaction.js"></script>
</head>

<body onload="javascript:parent.PageComplete();parent.numberToWords();parent.checkImage();" onunload="">
<div id="container">
	<!--<div id="lmsmenubar">-->
    	<!--<span id="modulename">Module Name</span>
        <span id="username">Username</span> -->
			<!--<span id="modulename">Module <xsl:value-of select="$modnumber"/></span>
			<span id="username">Welcome</span>
	</div>-->
	<div id="startpage">
		
		<div id="startheader">
        	<img src="assets/images/logo.png" alt="Ground Down Coffee" width="250" height="250" />
            <div id="starttitle">
            	<h1>Ground Down Coffee</h1>
                <h2><xsl:value-of select="$modname" /></h2> 
			</div>
		</div>
		<div id="maintext">
        	<!--<div id="startImage">-->
            	<!--<xsl:variable name="imagepath" select="concat('start_M',$modnumber,'.jpg')"/>-->
            	
            <!--</div>-->
			<div id="colmask">
            <div id="topics">
				<h3>Topics</h3>
                <ul>
                	<xsl:for-each select="document('../../content.xml')/unit/topic">
						<xsl:variable name="heading" select="current()/@heading" />
						<xsl:variable name="indexed" select="current()/@indexed" />
						<xsl:if test="$indexed='yes'">
							<li>
								 <xsl:value-of select="$heading"/>
							</li>
						</xsl:if>
					</xsl:for-each>
                    <!--<xsl:for-each select="current()/content/objective">
                      <li><xsl:copy-of select="current()"/></li>
                      </xsl:for-each>-->
				</ul>
				<span id="diagnostic"></span>
			</div>
            <div id="objectives">
            	<h3>Welcome to Module <span id="modnumber"><xsl:value-of select="$modnumber"/></span> - <xsl:value-of select="$modname" /></h3>
				<xsl:for-each select="current()/content/paragraph">
                	<p><xsl:copy-of select="current()"/></p>
                </xsl:for-each>
                <div id="startbutton">
					<a href="javascript:parent.next_navigation();" id="next_btn" class="navigation"></a>
				</div>
            	<div id="disclaimer">
					<h5><a href="www.grounddowncoffee.com.au" target="_blank">www.grounddowncoffee.com.au</a></h5>
				</div>
             </div>
             </div>
		</div>
		

	</div>
	<div id="checkDPI" style="width:1in; height:1in;">|</div>
</div>
<div id="checkImages"><img id="accessibility_square" src="assets/images/accessibility_square.gif" /></div>
</body>
</html>
</xsl:template>
</xsl:stylesheet>