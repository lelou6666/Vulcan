<?xml version="1.0" encoding="iso-8859-1"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ext="metadata">
  <xsl:template match="/">
      <xsl:for-each select="Collection/Content">
		  <xsl:if test="position() &lt; 4">
			  <div class="threecolumn" style="padding-bottom:30px;">
				  <table border="0" cellspacing="0" cellpadding="0" width="100%">
					  <tr>
						  <td>
							  <p style="color:#9f9e9e; padding-top:10px;">
								  <xsl:copy-of select="Html" />
							  </p>
						  </td>
					  </tr>
				  </table>
			  </div>
		  </xsl:if>
      </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>