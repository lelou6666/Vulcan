<?xml version="1.0" encoding="iso-8859-1"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ext="metadata">
  <xsl:template match="/">
      <xsl:for-each select="Collection/Content">
		  <div class="twocolumn">
			  <table border="0" cellspacing="0" cellpadding="0" height="170px">
				  <tr>
					  <td width="122" valign="top" align="left">
						  <div style="background-color:#FFF; border:2px solid #898989; width:100px; height:70px; text-align:center; line-height:70px; padding:4px;">
							  <a href="{Html/root/url/text()}" target="_blank">
								<img src="{Html/root/logo/node()}" style="width:100%; max-width:100px; max-height:70px; height:auto; vertical-align: middle;" border="0" />
							  </a>
						  </div>
					  </td>
					  <td valign="top" align="left">
						  <a href="{Html/root/url/text()}" target="_blank" class="events_title">
							  <xsl:value-of select="Html/root/link_text" />
						  </a>
						  <br />
						  <div class="events_copy"><xsl:copy-of select="Html/root/description" /></div>
					  </td>
					  <td width="10"> </td>
				  </tr>
			  </table>
		  </div>
      </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>