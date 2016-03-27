

/*jquery.tinyscrollbar.min.js */
(function(b){function a(D,J){function k(){G.update();z();return G}function U(){var c=H.toLowerCase();L.obj.css(N,Q/B.ratio);I.obj.css(N,-Q);K.start=L.obj.offset()[N];B.obj.css(c,P[J.axis]);P.obj.css(c,P[J.axis]);L.obj.css(c,L[J.axis])}function z(){if(!O){L.obj.bind("mousedown",C);P.obj.bind("mouseup",q)}else{F.obj[0].ontouchstart=function(c){if(1===c.touches.length){C(c.touches[0]);c.stopPropagation()}}}if(J.scroll&&window.addEventListener){M[0].addEventListener("DOMMouseScroll",j,false);M[0].addEventListener("mousewheel",j,false)}else{if(J.scroll){M[0].onmousewheel=j}}}function C(c){b("body").addClass("noSelect");var d=parseInt(L.obj.css(N),10);K.start=R?c.pageX:c.pageY;A.start=d=="auto"?0:d;if(!O){b(document).bind("mousemove",q);b(document).bind("mouseup",e);L.obj.bind("mouseup",e)}else{document.ontouchmove=function(f){f.preventDefault();q(f.touches[0])};document.ontouchend=e}}function j(d){if(I.ratio<1){var f=d||window.event,c=f.wheelDelta?f.wheelDelta/120:-f.detail/3;Q-=c*J.wheel;Q=Math.min(I[J.axis]-F[J.axis],Math.max(0,Q));L.obj.css(N,Q/B.ratio);I.obj.css(N,-Q);if(J.lockscroll||Q!==I[J.axis]-F[J.axis]&&Q!==0){f=b.event.fix(f);f.preventDefault()}}}function q(c){if(I.ratio<1){if(J.invertscroll&&O){A.now=Math.min(P[J.axis]-L[J.axis],Math.max(0,A.start+(K.start-(R?c.pageX:c.pageY))))}else{A.now=Math.min(P[J.axis]-L[J.axis],Math.max(0,A.start+((R?c.pageX:c.pageY)-K.start)))}Q=A.now*B.ratio;I.obj.css(N,-Q);L.obj.css(N,A.now)}}function e(){b("body").removeClass("noSelect");b(document).unbind("mousemove",q);b(document).unbind("mouseup",e);L.obj.unbind("mouseup",e);document.ontouchmove=document.ontouchend=null}var G=this,M=D,F={obj:b(".viewport",D)},I={obj:b(".overview",D)},B={obj:b(".scrollbar",D)},P={obj:b(".track",B.obj)},L={obj:b(".thumb",B.obj)},R=J.axis==="x",N=R?"left":"top",H=R?"Width":"Height",Q=0,A={start:0,now:0},K={},O="ontouchstart"in document.documentElement;this.update=function(c){F[J.axis]=F.obj[0]["offset"+H];I[J.axis]=I.obj[0]["scroll"+H];I.ratio=F[J.axis]/I[J.axis];B.obj.toggleClass("disable",I.ratio>=1);P[J.axis]=J.size==="auto"?F[J.axis]:J.size;L[J.axis]=Math.min(P[J.axis],Math.max(0,J.sizethumb==="auto"?P[J.axis]*I.ratio:J.sizethumb));B.ratio=J.sizethumb==="auto"?I[J.axis]/P[J.axis]:(I[J.axis]-F[J.axis])/(P[J.axis]-L[J.axis]);Q=c==="relative"&&I.ratio<=1?Math.min(I[J.axis]-F[J.axis],Math.max(0,Q)):0;Q=c==="bottom"&&I.ratio<=1?I[J.axis]-F[J.axis]:isNaN(parseInt(c,10))?Q:parseInt(c,10);U()};return k()}b.tiny=b.tiny||{};b.tiny.scrollbar={options:{axis:"y",wheel:40,scroll:true,lockscroll:true,size:"auto",sizethumb:"auto",invertscroll:false}};b.fn.tinyscrollbar=function(d){var c=b.extend({},b.tiny.scrollbar.options,d);this.each(function(){b(this).data("tsb",new a(b(this),c))});return this};b.fn.tinyscrollbar_update=function(c){return b(this).data("tsb").update(c)}})(jQuery);

/*tooltips */
(function(e){e.fn.tooltip=function(t){var n=e("#tooltipWrap"),r="",i=false,s=0,o={x:-13,y:-14},u={x:0,y:0};return this.each(function(){var t=e(this),a=t[0].title,f;if(a==="")return true;t.attr("title","");t.on("mousemove",function(t){var l=function(){i=true;if(!n.length){n=e("<div/>",{id:"tooltipWrap","class":"tooltip-wrap",html:'<div class="tip-content"></div><div class="tip-point tip-point-bottom"></div>'}).hide().appendTo(e("body"));r=n.find(".tip-content")}r.text(a);s=n.height();u.x=t.pageX+o.x;u.y=t.pageY-s+o.y;n.css({top:u.y-15,left:u.x});n.animate({opacity:"show",top:u.y},250)};clearTimeout(f);if(!i)f=setTimeout(l,400)}).on("mouseleave",function(e){if(i)n.animate({opacity:"hide",top:u.y-15},250);i=false;clearTimeout(f)})})}})(jQuery)

/*jquery.jqtransform */
var is_safari=navigator.userAgent.indexOf("Safari")>-1;(function(e){var t={preloadImg:true};var n=false;var r=function(e){e=e.replace(/^url\((.*)\)/,"$1").replace(/^\"(.*)\"$/,"$1");var t=new Image;t.src=e.replace(/\.([a-zA-Z]*)$/,"-hover.$1");var n=new Image;n.src=e.replace(/\.([a-zA-Z]*)$/,"-focus.$1")};var i=function(t){var n=e(t.get(0).form);var r=t.next();if(!r.is("label")){r=t.prev();if(r.is("label")){var i=t.attr("id");if(i){r=n.find('label[for="'+i+'"]')}}}if(r.is("label")){return r.css("cursor","pointer")}return false};var s=function(t){var n=e(".jqTransformSelectWrapper ul:visible");n.each(function(){var n=e(this).parents(".jqTransformSelectWrapper:first").find("select").get(0);if(!(t&&n.oLabel&&n.oLabel.get(0)==t.get(0))){e(this).hide()}})};var o=function(t){if(e(t.target).parents(".jqTransformSelectWrapper").length===0){s(e(t.target))}};var u=function(){e(document).mousedown(o)};var a=function(t){var n;e(".jqTransformSelectWrapper select",t).each(function(){n=this.selectedIndex<0?0:this.selectedIndex;e("ul",e(this).parent()).each(function(){e("a:eq("+n+")",this).click()})});e("a.jqTransformCheckbox, a.jqTransformRadio",t).removeClass("jqTransformChecked");e("input:checkbox, input:radio",t).each(function(){if(this.checked){e("a",e(this).parent()).addClass("jqTransformChecked")}})};e.fn.jqTransInputButton=function(){return this.each(function(){var t=e('<button id="'+this.id+'" name="'+this.name+'" type="'+this.type+'" class="'+this.className+' jqTransformButton"><span><span>'+e(this).attr("value")+"</span></span>").hover(function(){t.addClass("jqTransformButton_hover")},function(){t.removeClass("jqTransformButton_hover")}).mousedown(function(){t.addClass("jqTransformButton_click")}).mouseup(function(){t.removeClass("jqTransformButton_click")});e(this).replaceWith(t)})};e.fn.jqTransInputText=function(){return this.each(function(){var t=e(this);if(t.hasClass("jqtranformdone")||!t.is("input")){return}t.addClass("jqtranformdone");var n=i(e(this));n&&n.bind("click",function(){t.focus()});var r=t.width();if(t.attr("size")){r=t.attr("size")*10;t.css("width",r)}t.addClass("jqTransformInput").wrap('<div class="jqTransformInputWrapper"><div class="jqTransformInputInner"><div></div></div></div>');var s=t.parent().parent().parent();s.css("width",r+10);t.focus(function(){s.addClass("jqTransformInputWrapper_focus")}).blur(function(){s.removeClass("jqTransformInputWrapper_focus")}).hover(function(){s.addClass("jqTransformInputWrapper_hover")},function(){s.removeClass("jqTransformInputWrapper_hover")});is_safari&&s.addClass("jqTransformSafari");is_safari&&t.css("width",s.width()+16);this.wrapper=s})};e.fn.jqTransCheckBox=function(){return this.each(function(){if(e(this).hasClass("jqTransformHidden")){return}var t=e(this);var n=this;var r=i(t);r&&r.click(function(){s.trigger("click")});var s=e('<a href="#" class="jqTransformCheckbox"></a>');t.addClass("jqTransformHidden").wrap('<span class="jqTransformCheckboxWrapper"></span>').parent().prepend(s);t.change(function(){this.checked&&s.addClass("jqTransformChecked")||s.removeClass("jqTransformChecked");return true});s.click(function(){if(t.attr("disabled")){return false}t.trigger("click").trigger("change");return false});this.checked&&s.addClass("jqTransformChecked")})};e.fn.jqTransRadio=function(){return this.each(function(){if(e(this).hasClass("jqTransformHidden")){return}var t=e(this);var n=this;oLabel=i(t);oLabel&&oLabel.click(function(){r.trigger("click")});var r=e('<a href="#" class="jqTransformRadio" rel="'+this.name+'"></a>');t.addClass("jqTransformHidden").wrap('<span class="jqTransformRadioWrapper"></span>').parent().prepend(r);t.change(function(){n.checked&&r.addClass("jqTransformChecked")||r.removeClass("jqTransformChecked");return true});r.click(function(){if(t.attr("disabled")){return false}t.trigger("click").trigger("change");e('input[name="'+t.attr("name")+'"]',n.form).not(t).each(function(){e(this).attr("type")=="radio"&&e(this).trigger("change")});return false});n.checked&&r.addClass("jqTransformChecked")})};e.fn.jqTransTextarea=function(){return this.each(function(){var t=e(this);if(t.hasClass("jqtransformdone")){return}t.addClass("jqtransformdone");oLabel=i(t);oLabel&&oLabel.click(function(){t.focus()});var n='<table cellspacing="0" cellpadding="0" border="0" class="jqTransformTextarea">';n+='<tr><td id="jqTransformTextarea-tl"></td><td id="jqTransformTextarea-tm"></td><td id="jqTransformTextarea-tr"></td></tr>';n+='<tr><td id="jqTransformTextarea-ml">&nbsp;</td><td id="jqTransformTextarea-mm"><div></div></td><td id="jqTransformTextarea-mr">&nbsp;</td></tr>';n+='<tr><td id="jqTransformTextarea-bl"></td><td id="jqTransformTextarea-bm"></td><td id="jqTransformTextarea-br"></td></tr>';n+="</table>";var r=e(n).insertAfter(t).hover(function(){!r.hasClass("jqTransformTextarea-focus")&&r.addClass("jqTransformTextarea-hover")},function(){r.removeClass("jqTransformTextarea-hover")});t.focus(function(){r.removeClass("jqTransformTextarea-hover").addClass("jqTransformTextarea-focus")}).blur(function(){r.removeClass("jqTransformTextarea-focus")}).appendTo(e("#jqTransformTextarea-mm div",r));this.oTable=r;if(is_safari){e("#jqTransformTextarea-mm",r).addClass("jqTransformSafariTextarea").find("div").css("height",t.height()).css("width",t.width())}})};e.fn.jqTransSelect=function(){return this.each(function(t){var n=e(this);if(n.hasClass("jqTransformHidden")){return}if(n.attr("multiple")){return}var r=i(n);var o=n.addClass("jqTransformHidden").wrap('<div class="jqTransformSelectWrapper"></div>').parent().css({zIndex:10-t});o.prepend('<div><span></span><a href="#" class="jqTransformSelectOpen"></a></div><ul></ul>');var u=e("ul",o).css("width",n.width()).hide();e("option",this).each(function(t){var n=e('<li><a href="#" index="'+t+'">'+e(this).html()+"</a></li>");u.append(n)});u.find("a").click(function(){e("a.selected",o).removeClass("selected");e(this).addClass("selected");if(n[0].selectedIndex!=e(this).attr("index")&&n[0].onchange){n[0].selectedIndex=e(this).attr("index");n[0].onchange()}n[0].selectedIndex=e(this).attr("index");e("span:eq(0)",o).html(e(this).html());u.hide();return false});e("a:eq("+this.selectedIndex+")",u).click();e("span:first",o).click(function(){e("a.jqTransformSelectOpen",o).trigger("click")});r&&r.click(function(){e("a.jqTransformSelectOpen",o).trigger("click")});this.oLabel=r;var a=e("a.jqTransformSelectOpen",o).click(function(){if(u.css("display")=="none"){s()}if(n.attr("disabled")){return false}u.slideToggle("fast",function(){var t=e("a.selected",u).offset().top-u.offset().top;u.animate({scrollTop:t})});return false});var f=n.outerWidth();var l=e("span:first",o);var c=f>l.innerWidth()?f+a.outerWidth():o.width();o.css("width",c);u.css("width",c-2);l.css({width:f});u.css({display:"block",visibility:"hidden"});var h=e("li",u).length*e("li:first",u).height();h<u.height()&&u.css({height:h,overflow:"hidden"});u.css({display:"none",visibility:"visible"})})};e.fn.jqTransform=function(n){var r=e.extend({},t,n);return this.each(function(){var t=e(this);if(t.hasClass("jqtransformdone")){return}t.addClass("jqtransformdone");e('input:submit, input:reset, input[type="button"]',this).jqTransInputButton();e("input:text, input:password",this).jqTransInputText();e("input:checkbox",this).jqTransCheckBox();e("input:radio",this).jqTransRadio();e("textarea",this).jqTransTextarea();if(e("select",this).jqTransSelect().length>0){u()}t.bind("reset",function(){var e=function(){a(this)};window.setTimeout(e,10)})})}})(jQuery)

/*From Masterpage */
 function showSites() {
	        $('#othersites').slideToggle("fast");
	    }

	    function showProducts() {
	        $('#product_on_bar').toggle();
	        $('#product_menu').slideToggle("fast");
	    }

	    function showResources() {
	        $('#resource_on_bar').toggle();
	        $('#resources_menu').slideToggle("fast");
	    }


	    /* get querystring values */
	    function getParameterByName(name) {
	        name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
	        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
	        return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
	    }

	    $(document).ready(function() {
	        if ($(window).width() > 900) {
	            $('#resource_menu_link').hover(function() {
	                showResources();
	            });

	            $('#product_menu_link').hover(function() {
	                showProducts();
	            });
	        }
	        else {
	            $('#resource_menu_link_click').click(function() {
	                window.location.replace("/resources/");
	            });
	        }
	
	        if ($(window).width() < 900) {
	            //$('.scrollbar_resource').tinyscrollbar();
	        }

	        $(".defaultText").focus(function(srcc) {
	            if ($(this).val() == $(this)[0].title) {
	                $(this).removeClass("defaultTextActive");
	                $(this).val("");
	            }
	        });

	        $(".defaultText").blur(function() {
	            if ($(this).val() == "") {
	                $(this).addClass("defaultTextActive");
	                $(this).val($(this)[0].title);
	            }
	        });

	        $(".defaultText").blur();

	        var clicked = false;
	        $(".menu_expand").click(function() {

	            if (clicked == false) {
	                clicked = true;
	                $(".nav UL LI").show();
	                $(".nav UL").css("border-bottom", "6px solid #000000");
	                $(".nav UL LI UL").css({ display: "inherit" });  //This will display the sub items
	            }
	            else {
	                clicked = false;
	                $(".nav UL LI").hide();
	                $(".nav UL").css("border-bottom", "none");
	                $(".nav UL LI UL").css({ display: "none" });  //This will hide the sub items
	                if (mobileResourceHidden == false) {
	                    hideMobileResourcesOnly(); //This will hide the resources items if open
	                }
	            }
	        });

	        var closeSearch = "yes";
	        $("#seach_btn").click(function() {
	            if (closeSearch == "yes") {
	                $(".search_expand").animate({
	                    right: "+=220"
	                }, 500, function() {
	                });
	                closeSearch = "no";
	            }
	            else {
	                $(".search_expand").animate({
	                    right: "-=220"
	                }, 500, function() {
	                });
	                closeSearch = "yes";
	            }
	        });
	    });

	    $(function() {
	        // find the div.fade elements and hook the hover event
	        $('div.fade').hover(function() {
	            // on hovering over find the element we want to fade *up*
	            var fade = $('> div', this);
	            fade.stop().fadeIn(250);
	        }, function() {
	            var fade = $('> div', this);
	            fade.stop().fadeOut(250);
	        });
	    });  

	    //Scroll to top js
	    $(document).ready(function() {

	        $(window).scroll(function() {
	            if ($(this).scrollTop() > 700) {
	                $('.scrollup').fadeIn();
	            } else {
	                $('.scrollup').fadeOut();
	            }
	        });

	        $('.scrollup').click(function() {
	            $("html, body").animate({ scrollTop: 0 }, 600);
	            return false;
	        });

	    });