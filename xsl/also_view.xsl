<?xml version="1.0" encoding="iso-8859-1"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ext="metadata">
  <xsl:template match="/">
      <xsl:for-each select="Collection/Content">
		  <div class="fourcolumn" style="height:210px;">
			  <div class="alsoview_image">
				  <a href="{Html/root/url/text()}" onClick="ga('send', 'event', 'Also Viewed', '{Html/root/title}');">
					  <div style="position: absolute; top:10px; left:2px;">
							<img src="{Html/root/img/node()}" style="width:100%; max-width:204px; height:auto; vertical-align: middle;" border="0" />
					  </div>
					  <div class="alsoview_image_overlay" style="position: relative;">
						  <img class="alsoview_overlay" src="/images/products/also_view_aqua.png" border="0" />
					  </div>
				  </a>
			  </div>
			  <div class="alsoview_title_wrapper">
				  <a href="{Html/root/url/text()}" class="alsoview_title" onClick="ga('send', 'event', 'Also Viewed', '{Html/root/title}');">
					  <xsl:value-of select="Html/root/title" />
				  </a>
			  </div>
		  </div>
      </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>