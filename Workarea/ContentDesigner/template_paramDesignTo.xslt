<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:param name="rootXPath">
	<xsl:choose>
		<xsl:when test="//*[@ektdesignns_role='root' and not(ancestor::*/@ektdesignns_name)]"></xsl:when>
		<xsl:otherwise>/root</xsl:otherwise>
	</xsl:choose>
</xsl:param>

</xsl:stylesheet>