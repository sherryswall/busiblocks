<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform"> 
<!--ROOT NODE TEMPLATE for CONTENT TAGS ******************************************************************************* -->
<xsl:template match="content"> 
	<div id="topicheading">
		<span id="heading"><h1><xsl:value-of select="current()/topicheading"/></h1></span>
		<span id="printpage">
			<a id="notes" href="javascript:parent.ShowDialogueBox('notebox');" title="Add Notes"></a>
			<a id="print" href="content.xml" target="_blank" title="Print Version"></a>
		</span>
	</div>
	<div id="mainsection">
		<div id="graphicbox">
			<xsl:apply-templates select="current()/graphicbox/image"/>
			<xsl:apply-templates select="current()/graphicbox/linkbox"/>
		</div>
		<div id="maintext">
			<h1>Resources for Module <xsl:value-of select="$modnumber"/></h1>
			<xsl:for-each select="current()/module">
					<xsl:if test="current()/@id=$modnumber">
						<ul><xsl:apply-templates select="resource"/></ul>
					</xsl:if>
			</xsl:for-each>
		</div>
	</div>
</xsl:template>

<!-- ModuleHeading and Topicheading show nothing ******************************************************************************* -->
<xsl:template match="moduleheading"></xsl:template>
<xsl:template match="topicheading"></xsl:template>

<!-- Template=Resources -->
<xsl:template match="resource">
		<li><a class='link'>
				<xsl:attribute name="href">
					<xsl:value-of select="current()/@href" />
				</xsl:attribute>
				<xsl:value-of select="name"/>
			</a></li>
</xsl:template>
</xsl:stylesheet>
