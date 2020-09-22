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
        <div id="watermark">
        <div id="maintext" style="width:600px;">
        
        
        <!--<h1>Summary</h1>-->
        <p>Congratulations, you have now completed the <b><xsl:value-of select="$modname" /></b> module.</p>
			<!-- Apply all templates-->
			<xsl:apply-templates/>
            </div>
		</div>
	</div>
    
</xsl:template>

<!-- ModuleHeading and Topicheading show nothing ******************************************************************************* -->
<xsl:template match="moduleheading"></xsl:template>

<xsl:template match="topicheading"></xsl:template>

<!-- HEADING  ******************************************************************************* -->
<xsl:template match="pageheading">
	<h1>Summary</h1>
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
<xsl:template match="lookback">
		
		<div id="bluebox">
        <h2>Looking Back</h2>
        <p><xsl:copy-of select="current()"/></p>
        </div>
</xsl:template>

<!-- ACTIVITIES  ******************************************************************************* -->
<xsl:template match="lookforward">
		<div id="bluebox">
        <h2>Looking Forward</h2>
        <p><xsl:copy-of select="current()"/></p>
        </div>
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


</xsl:stylesheet>
