<?xml version="1.0" encoding="iso-8859-1"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ext="metadata">
  <xsl:template match="/">
      <xsl:for-each select="Collection/Content">
	
			  <div class="row_padding">
				  <div class="twocolumn">
					  <div class="events_title" style="padding-right:10px;">
						  <ul>
							  <li><xsl:copy-of select="Html" /></li>
						  </ul>
					  </div>
				  </div>
			  </div>

      </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>