!function(e){"use strict";function t(t,n){this.options=this.mergeOptions(n);this.$select=e(t);this.originalOptions=this.$select.clone()[0].options;this.query="";this.searchTimeout=null;this.options.multiple=this.$select.attr("multiple")==="multiple";this.options.onChange=e.proxy(this.options.onChange,this);this.options.onDropdownShow=e.proxy(this.options.onDropdownShow,this);this.options.onDropdownHide=e.proxy(this.options.onDropdownHide,this);this.buildContainer();this.buildButton();this.buildSelectAll();this.buildDropdown();this.buildDropdownOptions();this.buildFilter();this.updateButtonText();this.$select.hide().after(this.$container)}if(typeof ko!=="undefined"&&ko.bindingHandlers&&!ko.bindingHandlers.multiselect){ko.bindingHandlers.multiselect={init:function(e,t,n,r,i){},update:function(t,n,r,i,s){var o=ko.utils.unwrapObservable(n());var u=r().options;var a=e(t).data("multiselect");if(!a){e(t).multiselect(o)}else{a.updateOriginalOptions();if(u&&u().length!==a.originalOptions.length){e(t).multiselect("rebuild")}}}}}t.prototype={defaults:{buttonText:function(t,n){if(t.length===0){return this.nonSelectedText+' <b class="caret"></b>'}else{if(t.length>this.numberDisplayed){return t.length+" "+this.nSelectedText+' <b class="caret"></b>'}else{var r="";t.each(function(){var t=e(this).attr("label")!==undefined?e(this).attr("label"):e(this).html();r+=t+", "});return r.substr(0,r.length-2)+' <b class="caret"></b>'}}},buttonTitle:function(t,n){if(t.length===0){return this.nonSelectedText}else{var r="";t.each(function(){r+=e(this).text()+", "});return r.substr(0,r.length-2)}},label:function(t){return e(t).attr("label")||e(t).html()},onChange:function(e,t){},onDropdownShow:function(e){},onDropdownHide:function(e){},buttonClass:"btn btn-default",dropRight:false,selectedClass:"active",buttonWidth:"auto",buttonContainer:'<div class="btn-group" />',maxHeight:false,includeSelectAllOption:false,selectAllText:" Select all",selectAllValue:"multiselect-all",enableFiltering:false,enableCaseInsensitiveFiltering:false,filterPlaceholder:"Search",filterBehavior:"text",preventInputChangeEvent:false,nonSelectedText:"None selected",nSelectedText:"selected",numberDisplayed:3},templates:{button:'<button type="button" class="multiselect dropdown-toggle" data-toggle="dropdown"></button>',ul:'<ul class="multiselect-container dropdown-menu"></ul>',filter:'<div class="input-group"><span class="input-group-addon"><i class="glyphicon glyphicon-search"></i></span><input class="form-control multiselect-search" type="text"></div>',li:'<li><a href="javascript:void(0);"><label></label></a></li>',liGroup:'<li><label class="multiselect-group"></label></li>'},constructor:t,buildContainer:function(){this.$container=e(this.options.buttonContainer);this.$container.on("show.bs.dropdown",this.options.onDropdownShow);this.$container.on("hide.bs.dropdown",this.options.onDropdownHide)},buildButton:function(){this.$button=e(this.templates.button).addClass(this.options.buttonClass);if(this.$select.prop("disabled")){this.disable()}else{this.enable()}if(this.options.buttonWidth){this.$button.css({width:this.options.buttonWidth})}var t=this.$select.attr("tabindex");if(t){this.$button.attr("tabindex",t)}this.$container.prepend(this.$button)},buildDropdown:function(){this.$ul=e(this.templates.ul);if(this.options.dropRight){this.$ul.addClass("pull-right")}if(this.options.maxHeight){this.$ul.css({"max-height":this.options.maxHeight+"px","overflow-y":"auto","overflow-x":"hidden"})}this.$container.append(this.$ul)},buildDropdownOptions:function(){this.$select.children().each(e.proxy(function(t,n){var r=e(n).prop("tagName").toLowerCase();if(r==="optgroup"){this.createOptgroup(n)}else if(r==="option"){this.createOptionValue(n)}},this));e("li input",this.$ul).on("change",e.proxy(function(t){var n=e(t.target).prop("checked")||false;var r=e(t.target).val()===this.options.selectAllValue;if(this.options.selectedClass){if(n){e(t.target).parents("li").addClass(this.options.selectedClass)}else{e(t.target).parents("li").removeClass(this.options.selectedClass)}}var i=e(t.target).val();var s=this.getOptionByValue(i);var o=e("option",this.$select).not(s);var u=e("input",this.$container).not(e(t.target));if(r){if(this.$select[0][0].value===this.options.selectAllValue){var a=[];var f=e('option[value!="'+this.options.selectAllValue+'"]',this.$select);for(var l=0;l<f.length;l++){if(f[l].value!==this.options.selectAllValue&&this.getInputByValue(f[l].value).is(":visible")){a.push(f[l].value)}}if(n){this.select(a)}else{this.deselect(a)}}}if(n){s.prop("selected",true);if(this.options.multiple){s.prop("selected",true)}else{if(this.options.selectedClass){e(u).parents("li").removeClass(this.options.selectedClass)}e(u).prop("checked",false);o.prop("selected",false);this.$button.click()}if(this.options.selectedClass==="active"){o.parents("a").css("outline","")}}else{s.prop("selected",false)}this.$select.change();this.options.onChange(s,n);this.updateButtonText();if(this.options.preventInputChangeEvent){return false}},this));e("li a",this.$ul).on("touchstart click",function(t){t.stopPropagation();if(t.shiftKey){var n=e(t.target).prop("checked")||false;if(n){var r=e(t.target).parents("li:last").siblings('li[class="active"]:first');var i=e(t.target).parents("li").index();var s=r.index();if(i>s){e(t.target).parents("li:last").prevUntil(r).each(function(){e(this).find("input:first").prop("checked",true).trigger("change")})}else{e(t.target).parents("li:last").nextUntil(r).each(function(){e(this).find("input:first").prop("checked",true).trigger("change")})}}}e(t.target).blur()});this.$container.on("keydown",e.proxy(function(t){if(e('input[type="text"]',this.$container).is(":focus")){return}if((t.keyCode===9||t.keyCode===27)&&this.$container.hasClass("open")){this.$button.click()}else{var n=e(this.$container).find("li:not(.divider):visible a");if(!n.length){return}var r=n.index(n.filter(":focus"));if(t.keyCode===38&&r>0){r--}else if(t.keyCode===40&&r<n.length-1){r++}else if(!~r){r=0}var i=n.eq(r);i.focus();if(t.keyCode===32||t.keyCode===13){var s=i.find("input");s.prop("checked",!s.prop("checked"));s.change()}t.stopPropagation();t.preventDefault()}},this))},createOptionValue:function(t){if(e(t).is(":selected")){e(t).prop("selected",true)}var n=this.options.label(t);var r=e(t).val();var i=this.options.multiple?"checkbox":"radio";var s=e(this.templates.li);e("label",s).addClass(i);e("label",s).append('<input type="'+i+'" />');var o=e(t).prop("selected")||false;var u=e("input",s);u.val(r);if(r===this.options.selectAllValue){u.parent().parent().addClass("multiselect-all")}e("label",s).append(" "+n);this.$ul.append(s);if(e(t).is(":disabled")){u.attr("disabled","disabled").prop("disabled",true).parents("li").addClass("disabled")}u.prop("checked",o);if(o&&this.options.selectedClass){u.parents("li").addClass(this.options.selectedClass)}},createOptgroup:function(t){var n=e(t).prop("label");var r=e(this.templates.liGroup);e("label",r).text(n);this.$ul.append(r);e("option",t).each(e.proxy(function(e,t){this.createOptionValue(t)},this))},buildSelectAll:function(){var e=this.$select[0][0]?this.$select[0][0].value===this.options.selectAllValue:false;if(this.options.includeSelectAllOption&&this.options.multiple&&!e){this.$select.prepend('<option value="'+this.options.selectAllValue+'">'+this.options.selectAllText+"</option>")}},buildFilter:function(){if(this.options.enableFiltering||this.options.enableCaseInsensitiveFiltering){var t=Math.max(this.options.enableFiltering,this.options.enableCaseInsensitiveFiltering);if(this.$select.find("option").length>=t){this.$filter=e(this.templates.filter);e("input",this.$filter).attr("placeholder",this.options.filterPlaceholder);this.$ul.prepend(this.$filter);this.$filter.val(this.query).on("click",function(e){e.stopPropagation()}).on("keydown",e.proxy(function(t){clearTimeout(this.searchTimeout);this.searchTimeout=this.asyncFunction(e.proxy(function(){if(this.query!==t.target.value){this.query=t.target.value;e.each(e("li",this.$ul),e.proxy(function(t,n){var r=e("input",n).val();var i=e("label",n).text();if(r!==this.options.selectAllValue&&i){var s=false;var o="";if(this.options.filterBehavior==="text"||this.options.filterBehavior==="both"){o=i}if(this.options.filterBehavior==="value"||this.options.filterBehavior==="both"){o=r}if(this.options.enableCaseInsensitiveFiltering&&o.toLowerCase().indexOf(this.query.toLowerCase())>-1){s=true}else if(o.indexOf(this.query)>-1){s=true}if(s){e(n).show()}else{e(n).hide()}}},this))}},this),300,this)},this))}}},destroy:function(){this.$container.remove();this.$select.show()},refresh:function(){e("option",this.$select).each(e.proxy(function(t,n){var r=e("li input",this.$ul).filter(function(){return e(this).val()===e(n).val()});if(e(n).is(":selected")){r.prop("checked",true);if(this.options.selectedClass){r.parents("li").addClass(this.options.selectedClass)}}else{r.prop("checked",false);if(this.options.selectedClass){r.parents("li").removeClass(this.options.selectedClass)}}if(e(n).is(":disabled")){r.attr("disabled","disabled").prop("disabled",true).parents("li").addClass("disabled")}else{r.prop("disabled",false).parents("li").removeClass("disabled")}},this));this.updateButtonText()},select:function(t){if(t&&!e.isArray(t)){t=[t]}for(var n=0;n<t.length;n++){var r=t[n];var i=this.getOptionByValue(r);var s=this.getInputByValue(r);if(this.options.selectedClass){s.parents("li").addClass(this.options.selectedClass)}s.prop("checked",true);i.prop("selected",true);this.options.onChange(i,true)}this.updateButtonText()},deselect:function(t){if(t&&!e.isArray(t)){t=[t]}for(var n=0;n<t.length;n++){var r=t[n];var i=this.getOptionByValue(r);var s=this.getInputByValue(r);if(this.options.selectedClass){s.parents("li").removeClass(this.options.selectedClass)}s.prop("checked",false);i.prop("selected",false);this.options.onChange(i,false)}this.updateButtonText()},rebuild:function(){this.$ul.html("");e('option[value="'+this.options.selectAllValue+'"]',this.$select).remove();this.options.multiple=this.$select.attr("multiple")==="multiple";this.buildSelectAll();this.buildDropdownOptions();this.updateButtonText();this.buildFilter()},dataprovider:function(e){var t="";e.forEach(function(e){t+='<option value="'+e.value+'">'+e.label+"</option>"});this.$select.html(t);this.rebuild()},enable:function(){this.$select.prop("disabled",false);this.$button.prop("disabled",false).removeClass("disabled")},disable:function(){this.$select.prop("disabled",true);this.$button.prop("disabled",true).addClass("disabled")},setOptions:function(e){this.options=this.mergeOptions(e)},mergeOptions:function(t){return e.extend({},this.defaults,t)},updateButtonText:function(){var t=this.getSelected();e("button",this.$container).html(this.options.buttonText(t,this.$select));e("button",this.$container).attr("title",this.options.buttonTitle(t,this.$select))},getSelected:function(){return e('option[value!="'+this.options.selectAllValue+'"]:selected',this.$select).filter(function(){return e(this).prop("selected")})},getOptionByValue:function(t){return e("option",this.$select).filter(function(){return e(this).val()===t})},getInputByValue:function(t){return e("li input",this.$ul).filter(function(){return e(this).val()===t})},updateOriginalOptions:function(){this.originalOptions=this.$select.clone()[0].options},asyncFunction:function(e,t,n){var r=Array.prototype.slice.call(arguments,3);return setTimeout(function(){e.apply(n||window,r)},t)}};e.fn.multiselect=function(n,r){return this.each(function(){var i=e(this).data("multiselect");var s=typeof n==="object"&&n;if(!i){e(this).data("multiselect",i=new t(this,s))}if(typeof n==="string"){i[n](r)}})};e.fn.multiselect.Constructor=t;e(function(){e("select[data-role=multiselect]").multiselect()})}(window.jQuery)