/*
Crossbrowser HTMLElement Prototyping
Copyright (C) 2005  Jason Davis, http://www.browserland.org
Additional thanks to Brothercake, http://www.brothercake.com

This code is licensed under the LGPL:
	http://www.gnu.org/licenses/lgpl.html
*/

if (navigator.vendor == "Apple Computer, Inc." || navigator.vendor == "KDE") { // WebCore/KHTML
	(function(c) {
		for (var i in c)
			window["HTML" + i + "Element"] = document.createElement(c[ i ]).constructor;
	})({
		Html: "html", Head: "head", Link: "link", Title: "title", Meta: "meta",
		Base: "base", IsIndex: "isindex", Style: "style", Body: "body", Form: "form",
		Select: "select", OptGroup: "optgroup", Option: "option", Input: "input",
		TextArea: "textarea", Button: "button", Label: "label", FieldSet: "fieldset",
		Legend: "legend", UList: "ul", OList: "ol", DList: "dl", Directory: "dir",
		Menu: "menu", LI: "li", Div: "div", Paragraph: "p", Heading: "h1", Quote: "q",
		Pre: "pre", BR: "br", BaseFont: "basefont", Font: "font", HR: "hr", Mod: "ins",
		Anchor: "a", Image: "img", Object: "object", Param: "param", Applet: "applet",
		Map: "map", Area: "area", Script: "script", Table: "table", TableCaption: "caption",
		TableCol: "col", TableSection: "tbody", TableRow: "tr", TableCell: "td",
		FrameSet: "frameset", Frame: "frame", IFrame: "iframe"
	});

	function HTMLElement() {}
	HTMLElement.prototype     = HTMLHtmlElement.__proto__.__proto__;	
	var HTMLDocument          = document.constructor;
	var HTMLCollection        = document.links.constructor;
	var HTMLOptionsCollection = document.createElement("select").options.constructor;
	var Text                  = document.createTextNode("").constructor;
	var Node                  = Text;
}

function ekxbrowserCheck(domClassName)
{
	if (null == domClassName) return false;
	if ("undefined" == typeof domClassName.prototype) return false;
	if (null == domClassName.prototype) return false;
	if (typeof domClassName.prototype.__defineGetter__ != "function") return false;
	return true;
}

//source code extracted from http://www.codeproject.com/jscript/crossbrowserjavascript.asp?df=100&forumid=245519&exp=0&select=1712237

// Safari 3, HTMLElement is a function w/o prototype
if (typeof HTMLElement != "undefined" && ekxbrowserCheck(HTMLElement)) 
{
    
    // This definition needs to stay.  "parentElement" would still exist in the content.
    // see DesignToEntryXSLT.xslt, it converts the $ektron(document).parent().get(0)
    HTMLElement.prototype.__defineGetter__("parentElement",function()
    {
        if (this.parentNode == this.ownerDocument)
        {
            return null;
        }
        return this.parentNode;
    });
    
}

if (typeof Element != "undefined" && ekxbrowserCheck(Element))
{
    Element.prototype.__defineGetter__("text",function()
    {
        return this.textContent;
    });
}

// Safari does not have Event.prototype
if (typeof Event != "undefined" && ekxbrowserCheck(Event)) 
{
    Event.prototype.__defineGetter__("srcElement",function()
    {
        return this.target;
    });
    Event.prototype.__defineGetter__("offsetX",function()
    {
		var offset = 0;
		var objElem = this.target;
		while (objElem != null && objElem.tagName != "BODY")
		{
			offset += objElem.offsetLeft;
			objElem = objElem.offsetParent;
		}
        return this.clientX - offset;
    });
    Event.prototype.__defineGetter__("offsetY",function()
    {
		var offset = 0;
		var objElem = this.target;
		while (objElem != null && objElem.tagName != "BODY")
		{
			offset += objElem.offsetTop;
			objElem = objElem.offsetParent;
		}
        return this.clientY - offset;
    });
    // reference: http://www.reloco.com.ar/mozilla/compat.html
    Event.prototype.__defineSetter__("cancelBubble",function(value)
    {
        if (true == value) this.stopPropagation();
    });
    Event.prototype.__defineSetter__("returnValue",function(value)
    {
        if (false == value) this.preventDefault();
    });
}

function ekCanHaveChildren(oElem)
{
    if (oElem && /*Node.ELEMENT_NODE*/1 == oElem.nodeType)
    {
        switch(oElem.tagName.toLowerCase())
        {
            case "area":
            case "base":
            case "basefont":
            case "col":
            case "frame":
            case "hr":
            case "img":
            case "br":
            case "input":
            case "isindex":
            case "link":
            case "meta":
            case "param":
                return false;
        }
        return true;
    }
    else
    {
        return false;
    }
}

function ekHasChildren(oElem)
{
	return (ekCanHaveChildren(oElem) && $ektron(oElem).children().length > 0);
}

function ekCreateRange(sel)
{
    var rng = null;
    try
    {
        rng = sel.createRange();
    }
    catch(ex)
    {
        try
        {
            sel.clear();
            rng = sel.createRange();
        }
        catch (ex) {}
    }
    return rng;
}

function getSelectionElement(doc)
{
	var targetElement = null;
	if (doc.selection) // IE
	{
		var sel = doc.selection;
		var rng = ekCreateRange(sel);
	    if (rng)
	    {
	        if ("Control" == sel.type) // .type is undefined in Opera
	        {
			    targetElement = rng.commonParentElement();
	        }
	        else
	        {
			    targetElement = rng.parentElement();
	        }
	    }
	}
	else if (doc.defaultView.getSelection) // Mozilla
	{
		var sel = doc.defaultView.getSelection();
		if (sel.rangeCount > 0) {
			var rng = sel.getRangeAt(0);
			targetElement = rng.commonAncestorContainer;
			try 
			{
				// May throw "Permission denied to access property 'nodeType' from a non-chrome context
				// Seen in FF 3.5 when cursor is moved INTO input text field using arrow key.
				if (targetElement.nodeType != 1 /*Node.ELEMENT_NODE*/)
				{
					targetElement = targetElement.parentNode;
				}
			}
			catch (ex)
			{
				return null;
			}
		}
	}
	else // Safari, et others
	{
	}
	return targetElement;
}

function ekIsMac() 
{
    var userAgent = navigator.userAgent.toLowerCase();
    if (userAgent.indexOf('mac') != -1) 
    {
        return true;
    }
    return false;
}

// DO NOT CHANGE THIS CODE
// Copyright 2000-2007, Ektron, Inc.

// static, not exposed as method in this class, use queryArgs[]
function EkUtil_parseQuery()
{
	var objQuery = new Object();
	var strQuery = location.search.substring(1);
	// escape() encodes space as "%20".
	// If space is encoded as "+", then use the following line
	// in your customized function.
	// strQuery = strQuery.replace(/\+/g, " ");
	var aryQuery = strQuery.split("&");
	var pair = [];
	for (var i = 0; i < aryQuery.length; i++)
	{
		pair = aryQuery[i].split("=");
		if (2 == pair.length)
		{
			objQuery[unescape(pair[0])] = unescape(pair[1]);
		}
	}
	return objQuery;
}
