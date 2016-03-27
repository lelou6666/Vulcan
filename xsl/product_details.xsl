<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
	<xsl:output method="html" omit-xml-declaration="yes" indent="yes"/>

	<xsl:template match="/">
		<div class="forty">
			<table celllpadding="0" cellspacing="0">
				<tr>
					<td width="82" valign="top" style="text-align:center;">
						<div id="best_in_class_container">
							<img class="detail_imgs" id="best_in_class" src='/images/products/bestinclass.png' alt='best in class' style='padding-bottom:10px;' />
						</div>
						<div id="energy_star_container">
							<img src='/images/products/energy_star.jpg' alt='energy star' style='padding-bottom:10px;' class="detail_imgs" /></div>
						<div id='powerfry_container'>
							<img src='/images/products/ki_award.jpg' alt='Kitchen Innovations Award' title='Kitchen Innovations Award' style='padding-bottom:10px;' class="detail_imgs" />
							<img src='/images/products/gfen.jpg' alt='Gas Foodservice Equipement Network' title='Gas Foodservice Equipement Network' style='padding-bottom:10px;' class="detail_imgs" />
						</div>
					</td>
					<td valign="top" style="text-align:center;">
						<xsl:if test="string-length(/root/image1/node()) &gt; 0">
                            <xsl:if test="string-length(/root/zoom_image1/node()) &gt; 0">
                                <a href="{/root/zoom_image1/node()}" class="zoom" rel='gal1'>  
                                <img src="{/root/image1/node()}" alt="Product Image 1" style="text-align:center; width:100%; height:auto; padding-bottom:5px; margin:0 auto;" border="0" id="main_product_image" />
                                </a>
                            </xsl:if>
                            <xsl:if test="string-length(/root/zoom_image1/node()) &lt; 1">
                                <a href="{/root/image1/node()}" class="zoom" rel='gal1'>  
                                <img src="{/root/image1/node()}" alt="Product Image 1" style="text-align:center; width:100%; height:auto; padding-bottom:5px; margin:0 auto;" border="0" id="main_product_image" />
                                </a>
                            </xsl:if>
						</xsl:if>
					</td>
				</tr>
				<tr>
					<td colspan="2" align="center">
                    	<ul id="thumblist" class="clearfix">
                            <xsl:if test="string-length(/root/image1/node()) &gt; 0">
                            	<li>
                                    <div class="product_thumb" id="product_img1">
                                    	<xsl:if test="string-length(/root/zoom_image1/node()) &gt; 0">
                                            <a class="zoomThumbActive zoomthumb" href="javascript:void(0);" rel="[gallery: 'gal1', smallimage: '{/root/image1/node()}',largeimage: '{/root/zoom_image1/node()}']">  
                                        <img id="product_image1" src="{/root/image1/node()}" style="width:100%; max-width:55px; max-height:55px; height:auto; vertical-align: middle; padding:5px;" border="0" alt="Product Image 1" />
                                        </a>
                                        </xsl:if>
                                        <xsl:if test="string-length(/root/zoom_image1/node()) &lt; 1">
                                            <a class="zoomThumbActive zoomthumb" href="javascript:void(0);" rel="[gallery: 'gal1', smallimage: '{/root/image1/node()}',largeimage: '{/root/image1/node()}']">  
                                        <img id="product_image1" src="{/root/image1/node()}" style="width:100%; max-width:55px; max-height:55px; height:auto; vertical-align: middle; padding:5px;" border="0" alt="Product Image 1" />
                                        </a>
                                        </xsl:if>
                                    </div>
                                </li>
                            </xsl:if>
                            <xsl:if test="string-length(/root/image2/node()) &gt; 0">
                            	<li>
                                    <div class="product_thumb" id="product_img2">
                                    	<xsl:if test="string-length(/root/zoom_image1/node()) &gt; 0">
                                            <a class="zoomthumb" href="javascript:void(0);" rel="[gallery: 'gal1', smallimage: '{/root/image2/node()}',largeimage: '{/root/zoom_image2/node()}']">  
                                        <img id="product_image2" src="{/root/image2/node()}" style="width:100%; max-width:55px; max-height:55px; height:auto; vertical-align: middle; padding:5px;" border="0" alt="Product Image 2" />
                                        </a>
                                            </xsl:if>
                                            <xsl:if test="string-length(/root/zoom_image1/node()) &lt; 1">
                                                <a class="zoomthumb" href="javascript:void(0);" rel="[gallery: 'gal1', smallimage: '{/root/image2/node()}',largeimage: '{/root/image2/node()}']">  
                                        <img id="product_image2" src="{/root/image2/node()}" style="width:100%; max-width:55px; max-height:55px; height:auto; vertical-align: middle; padding:5px;" border="0" alt="Product Image 2" />
                                        </a>
                                        </xsl:if>
                                    </div>
                                </li>
							</xsl:if>
                            <xsl:if test="string-length(/root/image3/node()) &gt; 0">
                            	<li>
                                    <div class="product_thumb" id="product_img3">
                                    	<xsl:if test="string-length(/root/zoom_image1/node()) &gt; 0">
                                            <a class="zoomthumb" href="javascript:void(0);" rel="[gallery: 'gal1', smallimage: '{/root/image3/node()}',largeimage: '{/root/zoom_image3/node()}']">  
                                        <img id="product_image3" src="{/root/image3/node()}" style="width:100%; max-width:55px; max-height:55px; height:auto; vertical-align: middle; padding:5px;" border="0" />
                                        </a>
                                            </xsl:if>
                                            <xsl:if test="string-length(/root/zoom_image1/node()) &lt; 1">
                                                <a class="zoomthumb" href="javascript:void(0);" rel="[gallery: 'gal1', smallimage: '{/root/image3/node()}',largeimage: '{/root/image3/node()}']">  
                                        <img id="product_image3" src="{/root/image3/node()}" style="width:100%; max-width:55px; max-height:55px; height:auto; vertical-align: middle; padding:5px;" border="0" />
                                        </a>
                                        </xsl:if>
                                    </div>
                                </li>
                            </xsl:if>
                            <xsl:if test="string-length(/root/video/node()) &gt; 0">
                            	<li>
                                    <div class="product_thumb" id="video_thm" style="border: 1px solid #434343;">
                                        <img id="video_thumb" src="/images/products/video_tiny_thumb.jpg" alt="{/root/video/node()}" style="width:100%; height:auto; vertical-align: middle;" border="0" />
                                    </div>
                                </li>
                            </xsl:if>
                        </ul>
					</td>
				</tr>
			</table>
		</div>
		<div class="sixty">
			<!-- tabs  -->
			<div class="row">
				<div class="tabs">
					<xsl:if test="string-length(/root/description/node()) &gt; 0">
						<a href="#" class="article_tab active" style="margin: 0 10px;">
							features and benefits<img src="/images/current_tab_arrow.png" class="tab_arrow1" />
						</a>
					</xsl:if>
					<xsl:if test="string-length(/root/description/node()) &lt; 1">
						<a href="#" class="article_tab active" style="margin: 0 10px;">
							features and benefits<img src="/images/current_tab_arrow.png" class="tab_arrow3" />
						</a>
					</xsl:if>
					<xsl:if test="string-length(/root/description/node()) &gt; 0">
						<a href="#">
							about<img src="/images/current_tab_arrow.png" class="tab_arrow2" />
						</a>
					</xsl:if>
				</div>
			</div>
			<br />
			<br />
			<!-- end tabs  -->
			<div class="swiper-container">
				<div class="swiper-wrapper">
					<!-- features  -->
					<div class="swiper-slide">
						<div class="content-slide" style="overflow:auto;">
							<div class="row">
								<div class="detail_padding">
									<div class="scrollbar1">
										<div class="scrollbar">
											<div class="track">
												<div class="thumb">
													<div class="end"></div>
												</div>
											</div>
										</div>
										<div class="viewport">
											<div class="overview">
												<p>
													<xsl:copy-of select="/root/features/node()" />
												</p>
											</div>
										</div>
									</div>
								</div>
							</div>
						</div>
					</div>
					<!-- Description  -->
					<xsl:if test="string-length(/root/description/node()) &gt; 0">
					<div class="swiper-slide">
						<div class="content-slide" style="overflow:auto;">
							<div class="row">
								<div class="detail_padding">
									<div class="scrollbar1">
										<div class="scrollbar">
											<div class="track">
												<div class="thumb">
													<div class="end"></div>
												</div>
											</div>
										</div>
										<div class="viewport">
											<div class="overview">
												<p>
													<xsl:copy-of select="/root/description/node()" />
												</p>
											</div>
										</div>
									</div>
								</div>
							</div>
						</div>
					</div>
					</xsl:if>
				</div>
			</div>
			<br />
		</div>
		
	</xsl:template>
</xsl:stylesheet>