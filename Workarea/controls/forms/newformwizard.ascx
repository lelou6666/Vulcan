<%@ Control Language="vb" AutoEventWireup="false" Inherits="newformwizard" CodeFile="newformwizard.ascx.vb" %>

<style type="text/css">
    #stepsTable
    {
	    padding-top:1px;
	    height:25px;
    }
    #stepsTable td
    {
	    white-space:nowrap;
    }
    #stepsGraphicsTable
    {
	    border: solid 1px #5d8fb4;
	    height:2ex;
    }
    #stepsGraphicsTable td
    {
	    border: solid 1px #5d8fb4;
	    text-align: center;
	    vertical-align: middle;
    }
    td.stepDisabled
    {
	    background-color:#666666;
    }
    td.stepVisited
    {
	    cursor:pointer;
	    background-color:#cccccc;
    }
    td.stepCurrent
    {
	    font-weight:bold;
	    background-color:#F7D673;
    }
    td.stepFuture, td.stepNext
    {
	    background-color:#d6e4f1;
    }
    td.stepNext
    {
	    cursor:pointer;
    }
    td.stepNotUsed
    {
	    display:none;
    }
    div#pleaseWait
    {
        width: 128px;
        height: 128px;
        margin: -64px 0 0 -64px;
        background-color: #fff;
        background-image: url("images/ui/loading_big.gif");
        background-repeat: no-repeat;
        text-indent: -10000px;
        border: none;
        padding: 0;
        top: 50%;
    }

    td#stepsCaption
    {
	    padding: .25em .25em .25em .5em;
    }

    table#stepsGraphicsTable
    {
	    margin: .25em 1em;
	    padding: 0;
	    border-collapse: collapse;
	    background-color: #5d8fb4;
    }

    table#stepsGraphicsTable td
    {
	    padding: .25em 0;
	    width: 2em;
	    font-size: .9em;
	    text-align: center;
    }

    table#stepsGraphicsTable .stepVisited {background: #ccc; color: #666;}
    table#stepsGraphicsTable .stepCurrent {background-color: #F7D673; color: #235478;}

    .ektronToolbar table#stepsTable {margin: -.1em 0;}
    .ektronToolbar table#stepsTable a.button {font-size: .9em; height: auto; margin: auto; width: auto; background-position: .25em center;}
    a.button:active {color: #565656 !important;}
    a.ruleBack {background-color: #BED5EB; border: solid 1px #BED5EB; background-image: url('images/ui/icons/arrowHeadLeft.png');}
    a.ruleNext {background-color: #BED5EB; border: solid 1px #BED5EB; background-image: url('images/ui/icons/arrowHeadRight.png');}
    .ektronToolbar table#stepsTable a.ruleNext {background-position: right center;}
    a.ruleDone {background-color: #BED5EB; border: solid 1px #BED5EB; background-image: url('images/ui/icons/save.png');}
    a.ruleCancel {background-color: #BED5EB; border: solid 1px #BED5EB; background-image: url('images/ui/icons/cancel.png');}

    #stepDescription {padding: .5em}

    .ektronFormWizardToolbar {height: 48px !important;}
    #stepsContainerMsg {position: absolute; top: 62px;}
</style>
<div class="ektronWindow" id="pleaseWait">
    <h3><asp:Literal ID="LoadingImg" runat="server" /></h3>
</div>
<div id="stepsContainerMain" class="ektronPageHeader ektronFormWizardToolbar">
	    <div class="ektronTitlebar" id="stepTitle"><%=m_refmsg.GetMessage("lbl response")%></div>
	    <div class="ektronToolbar">
		    <table id="stepsTable">
			    <tr>
				    <td id="stepsCaption">
				        <%=m_refmsg.GetMessage("lbl step")%>
				        <span id="currentStep">1</span>
				        <%=m_refmsg.GetMessage("lbl of")%>
				        <span id="totalSteps">5</span>
				    </td>
				    <td>
					    <table id="stepsGraphicsTable">
						    <tr>
							    <td id="step1" class="stepCurrent">1</td>
							    <td id="step2" class="stepNext">2</td>
							    <td id="step3" class="stepNext">3</td>
							    <td id="step4" class="stepNext">4</td>
							    <td id="step5" class="stepNext">5</td>
							    <td id="step6" class="stepNotUsed">6</td>
							    <td id="step7" class="stepNotUsed">7</td>
							    <td id="step8" class="stepNotUsed">8</td>
							    <td id="step9" class="stepNotUsed">9</td>
						    </tr>
					    </table>
				    </td>
				    <td valign="middle">
					    <a id="btnBackStep" href="#" onclick="oProgressSteps.back(); return false;" title="Back" class="button buttonInline blueHover ruleBack" disabled="disabled"><%=m_refmsg.GetMessage("lbl back")%></a>
					    <a
					    id="btnNextStep" href="#" class="button buttonRightIcon buttonInline blueHover ruleNext" onclick="oProgressSteps.next(); return false;" title="Next"><%=m_refmsg.GetMessage("next")%></a>
					    <a
					    id="btnDoneSteps" href="#" style="display:none" class="button buttonInline greenHover ruleDone"  onclick="window.removePositionHack(); oProgressSteps.done(); return false;" title="Done"><%=m_refmsg.GetMessage("res_pcm_exit")%></a>
					    <a
					    id="btnCancelSteps" href="#" class="button buttonInline redHover ruleCancel" onclick="oProgressSteps.cancel(); return false;" title="Cancel"><%=m_refmsg.GetMessage("btn cancel")%></a>
				    </td>
				    <td>
					    <span id="helpBtn1" style="display: none;">
						    <asp:Literal ID="HelpButton1" runat="server" />
					    </span>
					    <span id="helpBtn2" style="display: none;">
						    <asp:Literal ID="HelpButton2" runat="server" />
					    </span>
					    <span id="helpBtn3" style="display: none;">
						    <asp:Literal ID="HelpButton3" runat="server" />
					    </span>
					    <span id="helpBtn4" style="display: none;">
						    <asp:Literal ID="HelpButton4" runat="server" />
					    </span>
					    <span id="helpBtn5" style="display: none;">
						    <asp:Literal ID="HelpButton5" runat="server" />
					    </span>
				    </td>
			    </tr>
            </table>
        </div>
    </div>
<div id="stepsContainerMsg">
	<div id="stepDescription"><%= m_refmsg.getmessage("alt msg form data submitted")%>    </div>
</div>
<script type="text/javascript">
<!--//--><![CDATA[//><!--
Ektron.ready( function()
{
    $ektron(".editFormHeader").hide();
    // hide main content editing form's tabs and toolbar so we can show the wizard to the user
    $ektron("#editContentToolbar").hide();
    $ektron("#editContentTabs").hide();
	$ektron("#editContentContainer .ektronTabBackground").hide();
	$ektron(".ektronTabUpperContainer").hide();
    $ektron("#propertiesTabContainer").hide();
	if ( $("#editContentTabs").length > 0 ) {
	    // only hide this space if it's visible
	    $ektron("#editContentPageContainer").css("cssText", "top: 0px !important;"); // jquery workaround for setting !important
	}
	// need to push form area down so it doesn't cover the wizard but only if wizard is showing
	if ( $ektron("#stepsContainerMain").is(':visible') )
	{
	    if ($ektron(".ewebeditproWrapper").length > 0)
	    {
	    $ektron("div.ektronPageTabbed").css("cssText", "top: 75px; width: 99%;"); // jquery workaround for setting !important
	    }
	    else
	    {
	        $ektron("div.ektronPageTabbed").css("cssText", "position: absolute; top: 75px; width: 99%;"); // jquery workaround for setting !important
	    }
	}

	window.removePositionHack = function()
	{
	    $ektron("div.ektronPageTabbed").css("cssText", "");
	}
    //$ektron("#stepsContainer").prependTo("#selectInitialWrapper");
    // PLEASE WAIT MODAL
    $ektron("#pleaseWait").modal(
        {
            trigger: '',
            modal: true,
            toTop: true,
            onShow: function(hash)
            {
                hash.o.fadeIn();
                hash.w.fadeIn();
            },
            onHide: function(hash)
            {
                hash.w.fadeOut("fast");
                hash.o.fadeOut("fast", function()
                    {
                        if (hash.o)
                        {
                            hash.o.remove();
                        }
                    }
                );
            }
        }
    );
})
function ProgressSteps()
{
	// member vars
	this.maxSteps = 9;
	this.currentStepNumber = 0; // 1-based
	this.startStepNumber = 0; // 1-based
	this.nextStepNumber = 0; // 1-based
	this.steps = [];
	this.disabled = false;
	// methods
	this.define = ProgressSteps_define;
	this.select = ProgressSteps_select;
	this.back = ProgressSteps_back;
	this.next = ProgressSteps_next;
	this.done = ProgressSteps_done;
	this.cancel = ProgressSteps_cancel;
	this.close = ProgressSteps_close;
	this.disable = ProgressSteps_disable;
	this.getStep = ProgressSteps_getStep;
	// events
	this.onselect = function(stepNumber) { return true; };
	this.ondone = function() { return true; };
	this.oncancel = function() { return true; };
	// private
	this.setStepState = ProgressSteps_setStepState;
	this.getEnabledStepNumber = ProgressSteps_getEnabledStepNumber;
}

function ProgressSteps_define(steps)
{
	this.steps = steps;
	var n = this.steps.length;
	if (n > this.maxSteps)
	{
		n = this.maxSteps;
		this.steps.length = n;
	}
	this.currentStepNumber = 0;
	this.startStepNumber = 0;
	document.getElementById("totalSteps").innerHTML = n;
	for (var i = 1; i <= this.maxSteps; i++)
	{
		var objElem = document.getElementById("step" + i);
		if (i <= n)
		{
			var objStep = this.getStep(i);
			objElem.title = objStep.title;
			objStep.disabled = (objStep.disabled ? true : false);
			objElem.className = (objStep.disabled ? "stepDisabled" : "stepFuture");
			if (!objStep.disabled)
			{
				if (0 == this.startStepNumber) this.startStepNumber = i;
			}
			objElem.onmouseover = new Function("document.getElementById('stepTitle').innerHTML = this.title");
			objElem.onmouseout = new Function("document.getElementById('stepTitle').innerHTML = oProgressSteps.getStep(oProgressSteps.currentStepNumber).title");
		}
		else
		{
			objElem.title = "";
			objElem.className = "stepNotUsed";
			objElem.onmouseover = null;
			objElem.onmouseout = null;
		}
		objElem.onclick = null;
	}
	this.nextStepNumber = this.startStepNumber;
	this.select(this.startStepNumber);
	document.getElementById("stepsContainerMain").style.display = "block";
	document.getElementById("stepsContainerMsg").style.display = "block";
}

function ProgressSteps_select(stepNumberOrID)
{
	// stepNumberOrID as number: 1-based index
	// stepNumberOrID as ID: string
	if (this.disabled) return this.currentStepNumber;
	var cur = 1;
	if ("number" == typeof stepNumberOrID)
	{
		cur = stepNumberOrID;
	}
	else if ("string" == typeof stepNumberOrID)
	{
		for (var i = 0; i < this.steps.length; i++)
		{
			if (this.steps[i].id == stepNumberOrID)
			{
				cur = i + 1; // 1-based index
				break;
			}
		}
	}
	if (this.startStepNumber <= cur && cur <= this.nextStepNumber && cur <= this.steps.length)
	{
		cur = this.getEnabledStepNumber(cur);
		if (false == this.onselect(cur)) return this.currentStepNumber;
		var objStep = this.getStep(cur);
		document.getElementById("currentStep").innerHTML = cur;
		document.getElementById("stepTitle").innerHTML = objStep.title;
		document.getElementById("stepDescription").innerHTML = objStep.description;
		var old = this.currentStepNumber;
		if (1 <= old)
		{
			if (old > cur && old + 1 == this.nextStepNumber)
			{
				this.setStepState(old, "stepNext");
			}
			else
			{
				this.setStepState(old, "stepVisited");
			}
			if (old < cur - 1)
			{
				this.setStepState(cur - 1, "stepVisited");
			}
		}
		if (cur == this.nextStepNumber)
		{
			var next = this.getEnabledStepNumber(cur + 1);
			this.setStepState(next, "stepNext");
			this.nextStepNumber = next;
		}
		this.setStepState(cur, "stepCurrent");

		var objElem;
		objElem = document.getElementById("btnBackStep");
		var bDisabled = (cur <= this.startStepNumber)
		objElem.disabled = (bDisabled);
		objElem.title = (bDisabled ? "<%= m_refmsg.GetMessage("lbl cannot go back")%>" : "<%= m_refmsg.GetMessage("btn back")%> " + this.getStep(this.getEnabledStepNumber(cur - 1)).title);

		var bAtEnd = (cur == this.steps.length);
		objElem = document.getElementById("btnNextStep");
		objElem.style.display = (bAtEnd ? "none" : "");
		objElem.style.display = (bAtEnd ? "none" : "");
		objElem.title = (bAtEnd ? "<%= m_refmsg.GetMessage("txt linkcheck idle")%>" : "<%= m_refmsg.GetMessage("next")%>: " + this.getStep(this.getEnabledStepNumber(cur + 1)).title);
		document.getElementById("btnDoneSteps").style.display = (bAtEnd ? "" : "none");
		if (bAtEnd || ((cur == 3) && (document.getElementById("numPollChoices") != null))) {
		    // hide the bordered square which is the empty content container during the last Done step
		    // also hide this at the third step of the Poll form (but not the other forms)
		    if ($ektron(".ewebeditproWrapper").length > 0)
	        {
			    $ektron("#editContentContainer").css("cssText", "left: -100000px;");
			}
			else
			{
			    $ektron("#editContentTabContainer").css("cssText", "position: absolute; left: -100000px;");
			}
			// get rid of the giant "busy" indicator
	        $ektron("body").css("cssText", "background-image: url('');");
		} else {
		   	$ektron("#editContentTabContainer").css("cssText", null);
        }
		this.currentStepNumber = cur;
	}
	if (cur == 1) {
	    $ektron("#selectInitialWrapper").css("cssText", null);
	} else {
	    // even when this is hidden, it sits over the form area so people can't type into it,
	    // so we drop the z-index of the form chooser page of the wizard so they can
	    $ektron("#selectInitialWrapper").css("cssText", "z-index:-1");
	}
	if (cur == 3) {
		// get rid of the giant "busy" indicator
        $ektron("body").css("cssText", "background-image: url('');");
	}
	SelectHelpButton(cur);
	return this.currentStepNumber;
}

// private
function ProgressSteps_setStepState(stepNumber, stepState)
{
	var n = stepNumber;
	if (1 <= n && n <= this.steps.length && !this.getStep(n).disabled)
	{
		var objElem = document.getElementById("step" + n);
		objElem.className = stepState;
		if ("stepVisited" == stepState || "stepNext" == stepState)
		{
			objElem.onclick = new Function("oProgressSteps.select(" + n + ")");
		}
		else
		{
			objElem.onclick = null;
		}
	}
}

function ProgressSteps_back()
{
	if (this.disabled) return;
	var cur = this.currentStepNumber - 1;
	while (cur >= 1 && true == this.getStep(cur).disabled)
	{
		cur--;
	}
	this.select(cur);
	return;
}

function ProgressSteps_next()
{
	if (this.disabled) return;
	this.select(this.getEnabledStepNumber(this.currentStepNumber + 1));
	return;
}

function ProgressSteps_done()
{
	if (this.disabled) return;
	if (false == this.ondone()) return;
	this.close();
	$ektron(document).trigger("wizardPanelShown");
	return;
}

function ProgressSteps_cancel()
{
	if (this.disabled) return;
	if (false == this.oncancel()) return;
	return;
}

function ProgressSteps_close()
{
	document.getElementById("stepsContainerMain").style.display = "none";
	document.getElementById("stepsContainerMsg").style.display = "none";

	// show normal edit content form
	$ektron("#editContentToolbar").show();
	$ektron("#editContentTabs").show();
	$ektron("#editContentContainer .ektronTabBackground").show();   // this puts in a background color for the tabs
	$ektron("#editContentPageContainer").css("cssText", null);   // this uses the default from the CSS
	$ektron("#editContentTabContainer").css("cssText", null);
	$ektron(".ektronTabUpperContainer").show();
}

function ProgressSteps_disable()
{
	this.disabled = true;
	document.getElementById("stepsGraphicsTable").disabled = true;
	document.getElementById("btnBackStep").disabled = true;
	document.getElementById("btnNextStep").disabled = true;
	document.getElementById("btnDoneSteps").disabled = true;
	document.getElementById("btnCancelSteps").disabled = true;
}

function ProgressSteps_getStep(stepNumber)
{
	return this.steps[stepNumber - 1];
}

// private
function ProgressSteps_getEnabledStepNumber(stepNumber)
{
	var cur = stepNumber;
	while (cur <= this.steps.length && true == this.getStep(cur).disabled)
	{
		cur++;
	}
	return cur;
}

function SelectHelpButton(cur)
{
	if ((cur >= 1) && (cur <= 5))
	{
		var targId;
		var targObj;
		var idx;
		for (idx = 1; idx <= 5; idx ++)
		{
			targId = "helpBtn" + idx;
			targObj = document.getElementById(targId);
			targObj.style.display = ((cur == idx) ? "" : "none");
		}
	}
}

var oProgressSteps = new ProgressSteps;
//--><!]]>
</script>