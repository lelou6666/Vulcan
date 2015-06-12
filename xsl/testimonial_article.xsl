<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
	<xsl:output method="html" omit-xml-declaration="yes" indent="yes"/>

	<xsl:template match="/">
			<div style="background-color:#FFF; border:2px solid #898989; width:100px; height:70px; text-align:center; line-height:70px; display:block; float:left; margin:4px 10px 5px 0;">
				<img src="{/root/logo/text()}" style="" />
			</div>
			<xsl:copy-of select="/root/description"/>
	</xsl:template>
</xsl:stylesheet>