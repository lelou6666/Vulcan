<?xml version="1.0" encoding="iso-8859-1"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ext="metadata">
  <xsl:template match="/">
      <xsl:for-each select="Collection/Content">
		  <xsl:if test="position() &gt; 8">
			  <div class="twocolumn" style="padding-bottom:30px;">
			  <table border="0" cellspacing="0" cellpadding="0" width="100%">
				  <tr>
					  <td width="105" style="text-align:right; padding-right:15px;" valign="top">
						  <div class="release_month">
							  <xsl:call-template name="FormatMonth">
								  <xsl:with-param name="DateTime" select="StartDate"/>
							  </xsl:call-template>
						  </div>
						  <div class="release_year">
							  <xsl:call-template name="FormatYear">
								  <xsl:with-param name="DateTime" select="StartDate"/>
							  </xsl:call-template>
						  </div>
					  </td>
					  <td valign="top" align="left" class="release_title">
						  <p style="padding-bottom:5px;">
							  <a>
								  <xsl:attribute name="href">
									  <xsl:value-of select="QuickLink"/>
								  </xsl:attribute>
								  <xsl:value-of select="Title"/>
							  </a>&#160;
						  </p>
					  </td>
				  </tr>
			  </table>
		  </div>
		  </xsl:if>
      </xsl:for-each>
  </xsl:template>

  <xsl:template name="FormatYear">
    <xsl:param name="DateTime" />
    <xsl:variable name="day-temp">
      <xsl:value-of select="substring-after($DateTime,'/')" />
    </xsl:variable>
    <xsl:variable name="year-temp">
      <xsl:value-of select="substring-after($day-temp,'/')" />
    </xsl:variable>
    <!-- build the out put string -->
    <xsl:variable name="year">
      <xsl:value-of select="substring($year-temp,1,4)" />
    </xsl:variable>
    <xsl:value-of select="$year"/>
  </xsl:template>

  <xsl:template name="FormatMonth">
    <xsl:param name="DateTime" />
    <xsl:variable name="mo">
      <xsl:value-of select="substring-before($DateTime,'/')" />
    </xsl:variable>
    <xsl:choose>
      <xsl:when test="$mo = '1'">JANUARY</xsl:when>
      <xsl:when test="$mo = '2'">FEBRUARY</xsl:when>
      <xsl:when test="$mo = '3'">MARCH</xsl:when>
      <xsl:when test="$mo = '4'">APRIL</xsl:when>
      <xsl:when test="$mo = '5'">MAY</xsl:when>
      <xsl:when test="$mo = '6'">JUNE</xsl:when>
      <xsl:when test="$mo = '7'">JULY</xsl:when>
      <xsl:when test="$mo = '8'">AUGUST</xsl:when>
      <xsl:when test="$mo = '9'">SEPTEMBER</xsl:when>
      <xsl:when test="$mo = '10'">OCTOBER</xsl:when>
      <xsl:when test="$mo = '11'">NOVEMBER</xsl:when>
      <xsl:when test="$mo = '12'">DECEMBER</xsl:when>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="FormatDay">
    <xsl:param name="DateTime" />
    <xsl:variable name="day-temp">
      <xsl:value-of select="substring-after($DateTime,'/')" />
    </xsl:variable>
    <xsl:variable name="day">
      <xsl:value-of select="substring-before($day-temp,'/')" />
    </xsl:variable>
    <xsl:value-of select="$day"/>
  </xsl:template>
</xsl:stylesheet>