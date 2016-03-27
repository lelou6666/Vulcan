/* Floatbox v3.53.0 */
Floatbox.prototype.cyclePreload=function(f,d){var c=fb,e=c.cycleDivs[f],b=e.nodes[d].img,a=b.getAttribute("longdesc");if(a){b.src=a;b.setAttribute("longdesc","")}};Floatbox.prototype.cyclePreloadNext=function(){var b=fb,a=b.cycleDivs.length;while(a--){var d=b.cycleDivs[a],c=d.showing+1;if(c>=d.nodes.length){c=0}b.cyclePreload(a,c);if(b.preloadAll&&fb.preloads.count&&d.nodes[d.showing].href){setTimeout(function(){fb.preload(d.nodes[d.showing].href)},200)}}};Floatbox.prototype.cycleShow=function(h,f,i){var j=fb,b=j.cycleDivs[h],d=i?0:j.cycleFadeDuration*2;if((j.ie||j.opera)&&d>2){d-=1}if(f>=b.nodes.length){f=0}if(f<0){f=b.nodes.length-1}if(f===b.showing){return}var c=b.nodes[b.showing],e=b.nodes[f],g=(c.getElementsByTagName("span")||[])[0],l=(e.getElementsByTagName("span")||[])[0],a=c.style,k=e.style;j.setOpacity(e,0,0);if(l){if(false&&j.ie){l.style.visibility="hidden"}else{j.setOpacity(l,100,0)}}k.display="block";if(b.div.offsetHeight<e.offsetHeight){b.div.style.height=e.offsetHeight+"px"}a.zIndex="10";k.zIndex="20";j.setOpacity(e,100,d,function(){a.display="none";if(l){l.style.visibility="visible"}});if(g){if(true||!j.ie){g.style.opacity="1";j.setOpacity(g,0,d*0.9)}}b.showing=f;j.cyclePreloadNext()};Floatbox.prototype.cycleShowNext=function(){var b=fb,c=fb.children.length+(fb.lastChild.fbBox?1:0),a=b.cycleDivs.length;while(a--){if(b.cycleDivs[a].level===c){b.cycleShow(a,b.cycleDivs[a].showing+1)}}};Floatbox.prototype.cycleGo=function(){var a=fb;a.cycleStop();a.cycleTimer=setTimeout(function(){a.cycleShowNext();a.cycleTimer=setInterval(function(){a.cycleShowNext()},a.cycleInterval*1000)},a.cycleInterval*500)};Floatbox.prototype.cycleStop=function(){var a=fb;clearInterval(a.cycleTimer);clearTimeout(a.cycleTimer)};Floatbox.prototype.cycleInit=function(w){var g=fb,d,b=fb.children.length+(fb.lastChild.fbBox?1:0),n=g.getElementsByClassName(g.cycleClass,w),q=n.length;while(q--){var h=n[q],c=h.childNodes,l=[],f=0;for(var p=0;p<c.length;p++){var m=c[p],y=m.nodeName;if(/A|DIV|IMG/.test(y)){if(y==="IMG"){var v=h.ownerDocument.createElement("div");v.style.display=fb.getStyle(m,"display");g.setInnerHTML(v,g.getOuterHTML(m));h.replaceChild(v,m);m=v}var x=(m.getElementsByTagName("img")||[])[0];if(x){x.style.display="inline";if(m.offsetWidth){f=l.length;if(h.offsetHeight<m.offsetHeight){h.style.height=m.offsetHeight+"px"}}m.img=x;l.push(m)}}}if(l.length>1){var p=l.length;while(p--){var m=l[p];if(l[p].nodeName==="A"){var o=g.anchors.length;while(o--){var s=g.anchors[o];if(m===s.anchor){s.options.preloadCycleItem="fb.cyclePreload("+g.cycleDivs.length+", "+p+")";s.options.setCycleItem="fb.cycleShow("+g.cycleDivs.length+", "+p+", true)";break}}}if(g.ie){var e=m.getElementsByTagName("span")||[],o=e.length;while(o--){var u=e[o].currentStyle.backgroundColor.toLowerCase();if(!u||u==="transparent"){if(d===g.undefined){var r=e[o];while((r=r.parentNode)){d=r.currentStyle.backgroundColor.toLowerCase();if(d&&d!=="transparent"){break}d="#fff"}}e[o].style.backgroundColor=d}}}}g.cycleDivs.push({nodes:l,showing:f,level:b,div:h})}}g.cyclePreloadNext();g.cycleGo()};fb.cycleInit(document.getElementsByTagName("body")[0]);
