<?xml version="1.0" encoding="iso-8859-1"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ext="metadata">
  <xsl:template match="/">
      <xsl:for-each select="Collection/Content">
		  <xsl:if test="position() &lt; 9">
			  <div class="twocolumn" style="padding-bottom:15px;">
			  <table border="0" cellspacing="0" cellpadding="0" width="97%">
				  <tr>
					  <td width="145" valign="top" align="left">
						  <div style="background-color:#FFF; border:2px solid #898989; width:135px; height:70px; text-align:center; line-height:70px;">
							  <a href="{Html/root/url/text()}" target="_blank">
								  <img src="{Html/root/logo/node()}" style="width:100%; max-width:125px; max-height:70px; height:auto; vertical-align: middle;" border="0" />
							  </a>
						  </div>
					  </td>
					  <td valign="top">
						  <table border="0" cellspacing="0" cellpadding="0" width="100%">
							  <tr>
								  <td width="105" style="text-align:right; padding-right:15px;" valign="top">
									  <div class="article_month">
										  <xsl:call-template name="FormatMonth">
											  <xsl:with-param name="DateTime" select="Html/root/date/node()"/>
										  </xsl:call-template>
										  &#160;
									  </div>
									  <div class="article_year">
										  <xsl:call-template name="FormatYear">
											  <xsl:with-param name="DateTime" select="Html/root/date"/>
										  </xsl:call-template>
									  </div>
								  </td>
							  </tr>
							  <tr>
								  <td valign="top" align="left" class="release_title">
									  <p style="padding-bottom:0px;">
										  <a href="{Html/root/url/text()}" target="_blank" class="events_title">
											  <xsl:value-of select="Html/root/link_text" />
										  </a>
									  </p>
								  </td>
							  </tr>
							  <tr>
								  <td valign="top" align="left" class="article_source">
										<xsl:value-of select="Html/root/source" />
								  </td>
							  </tr>
						  </table>
					  </td>
				  </tr>
			  </table>
		  </div>
		  </xsl:if>
      </xsl:for-each>
  </xsl:template>
	
	<xsl:template name="FormatYear">
		<xsl:param name="DateTime" />
		<!-- build the out put string -->
		<xsl:variable name="year">
			<xsl:value-of select="substring($DateTime,1,4)" />
		</xsl:variable>
		<xsl:value-of select="$year"/>
	</xsl:template>

	<xsl:template name="FormatMonth">
		<xsl:param name="DateTime" />
		<xsl:variable name="mo">
			<xsl:value-of select="substring($DateTime,6,2)" />
		</xsl:variable>
		<xsl:choose>
			<xsl:when test="$mo = '01'">JANUARY </xsl:when>
			<xsl:when test="$mo = '02'">FEBRUARY </xsl:when>
			<xsl:when test="$mo = '03'">MARCH </xsl:when>
			<xsl:when test="$mo = '04'">APRIL </xsl:when>
			<xsl:when test="$mo = '05'">MAY </xsl:when>
			<xsl:when test="$mo = '06'">JUNE </xsl:when>
			<xsl:when test="$mo = '07'">JULY </xsl:when>
			<xsl:when test="$mo = '08'">AUGUST </xsl:when>
			<xsl:when test="$mo = '09'">SEPTEMBER </xsl:when>
			<xsl:when test="$mo = '10'">OCTOBER </xsl:when>
			<xsl:when test="$mo = '11'">NOVEMBER </xsl:when>
			<xsl:when test="$mo = '12'">DECEMBER </xsl:when>
		</xsl:choose>
	</xsl:template>


</xsl:stylesheet>