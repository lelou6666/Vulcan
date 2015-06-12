<?xml version="1.0" encoding="iso-8859-1"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ext="metadata">
  <xsl:template match="/">
      <xsl:for-each select="Collection/Content">
		  <xsl:if test="position() &lt; 5">
			  <div class="twocolumn" style="padding-bottom:30px;">
				  <table border="0" cellspacing="0" cellpadding="0" width="100%">
					  <tr>
						  <td width="122" valign="top" align="left">
							  <div style="background-color:#FFF; border:2px solid #898989; width:100px; height:70px; text-align:center; line-height:70px;">
								  <a href="{QuickLink}">
									  <img src="{Html/root/logo/node()}" style="width:100%; max-width:100px; max-height:70px; height:auto; vertical-align: top;" border="0" />
								  </a>
							  </div>
						  </td>
						  <td valign="top" align="left">
							  <a href="{QuickLink}" class="events_title">
								  <xsl:value-of select="Title" />
							  </a>
							  <br />
							  <div class="events_copy">
								  <xsl:value-of select="Html/root/link_text" />
							  </div>
						  </td>
						  <td width="10"> </td>
					  </tr>
				  </table>
			  </div>
		  </xsl:if>
      </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>