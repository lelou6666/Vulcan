<%@ Control Language="vb" AutoEventWireup="false" Inherits="addcustomproperty" CodeFile="addcustomproperty.ascx.vb" %>
<asp:Literal ID="ltrJS" runat="server"></asp:Literal>

<script type="text/javascript" language="javascript">
    /***********************************************
    * Contractible Headers script- © Dynamic Drive (www.dynamicdrive.com)
    * This notice must stay intact for legal use. Last updated Oct 21st, 2003.
    * Visit http://www.dynamicdrive.com/ for full source code
    ***********************************************/

    var enablepersist = "off" //Enable saving state of content structure using session cookies? (on/off)
    var collapseprevious = "no" //Collapse previously open content when opening present? (yes/no)

    function getElementbyClass(classname)
    {
        ccollect = new Array()
        var inc = 0
        var alltags = document.all ? document.all : document.getElementsByTagName("*")
        for (i = 0; i < alltags.length; i++)
        {
            if (alltags[i].className == classname)
                ccollect[inc++] = alltags[i]
        }
    }

    function contractcontent(omit)
    {
        var inc = 0
        while (ccollect[inc])
        {
            if (ccollect[inc].id != omit)
                ccollect[inc].style.display = "none"
            inc++
        }
    }


    function expandcontent(cid)
    {
        if (typeof ccollect != "undefined")
        {
            if (collapseprevious == "yes")
                contractcontent(cid)
            document.getElementById(cid).style.display = (document.getElementById(cid).style.display != "block") ? "block" : "none"
        }
    }
    function revivecontent()
    {
        contractcontent("omitnothing")
        selectedItem = getselectedItem()
        selectedComponents = selectedItem.split("|")
        for (i = 0; i < selectedComponents.length - 1; i++)
            document.getElementById(selectedComponents[i]).style.display = "block"
    }
    function get_cookie(Name)
    {
        var search = Name + "="
        var returnvalue = "";
        if (document.cookie.length > 0)
        {
            offset = document.cookie.indexOf(search)
            if (offset != -1)
            {
                offset += search.length
                end = document.cookie.indexOf(";", offset);
                if (end == -1) end = document.cookie.length;
                returnvalue = unescape(document.cookie.substring(offset, end))
            }
        }
        return returnvalue;
    }

    function getselectedItem()
    {
        if (get_cookie(window.location.pathname) != "")
        {
            selectedItem = get_cookie(window.location.pathname)
            return selectedItem
        }
        else
            return ""
    }

    function saveswitchstate()
    {
        var inc = 0, selectedItem = ""
        while (ccollect[inc])
        {
            if (ccollect[inc].style.display == "block")
                selectedItem += ccollect[inc].id + "|"
            inc++
        }

        document.cookie = window.location.pathname + "=" + selectedItem
    }
    function do_onload()
    {
        getElementbyClass("switchcontent")
        if (enablepersist == "on" && typeof ccollect != "undefined")
            revivecontent()
    }


    if (window.addEventListener)
        window.addEventListener("load", do_onload, false)
    else if (window.attachEvent)
        window.attachEvent("onload", do_onload)
    else if (document.getElementById)
        window.onload = do_onload

    if (enablepersist == "on" && document.getElementById)
        window.onunload = saveswitchstate
    function trim(s)
    {
        while (s.substring(0, 1) == ' ')
        {
            s = s.substring(1, s.length);
        }
        while (s.substring(s.length - 1, s.length) == ' ')
        {
            s = s.substring(0, s.length - 1);
        }
        return s;
    }
    function expand_it(cid)
    {
        if (typeof ccollect != "undefined")
        {
            document.getElementById(cid).className = "";
        }
    }

    function collapse_it(cid)
    {
        if (typeof ccollect != "undefined")
        {
            document.getElementById(cid).className = "switchcontent";
        }
    }
    function show_range2(Min, Max, Obj)
    {
        var myindex
        var bMin
        var bMax
        var arMin = Min.split(',');
        var arMax = Max.split(',');
        bMin = false;
        bMax = false;
        if (typeof Obj == "undefined") return;
        if (Obj.selectedIndex == -1) return;
        myindex = Obj[Obj.selectedIndex].value;
        for (var i = 0; i < arMin.length; i++)
        {
            //alert(myindex + ' == ' + trim(arMin[i]));
            if (myindex == trim(arMin[i]))
            {
                bMin = true;
                break;
            }
        }
        for (var i = 0; i < arMax.length; i++)
        {
            if (myindex == trim(arMax[i]))
            {
                bMax = true;
                break;
            }
        }
        if (bMin || bMax)
        {
            expand_it('addCustomProp_TR_Min');
            expand_it('addCustomProp_TR_Max');
            if (myindex == 4)
            {
                expand_it('sc2');
                expand_it('sc4');
            }
            else
            {
                collapse_it('sc2');
                collapse_it('sc4');
            }
        }
        else
        {
            if (typeof addCustomProp_TR_Min != "undefined")
            {
                collapse_it('addCustomProp_TR_Min');
            }
            if (typeof addCustomProp_TR_Max != "undefined")
            {
                collapse_it('addCustomProp_TR_Max');
            }            
            collapse_it('sc2');
        }
    }
</script>

<style type="text/css">
    .switchcontent
    {
        display: none;
    }
</style>
<div id="dhtmltooltip">
</div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server">
    </div>
    <div class="ektronToolbar" id="htmToolBar" runat="server">
    </div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    <table class="ektronForm">
        <tr>
            <td class="label">
                <%=m_refMsg.GetMessage("lbl Label")%>
            </td>
            <td class="value">
                <asp:TextBox ID="txtLabel" EnableViewState="True" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label">
                <asp:Label ID="lblType" runat="server"><%=m_refMsg.GetMessage("type label")%></asp:Label>
            </td>
            <td class="value">
                <asp:DropDownList ID="ddTypes" EnableViewState="True" runat="server" AutoPostBack="True" />
            </td>
        </tr>
        <tr id="TR_inputType" runat="server">
            <td class="label">
                <asp:Label ID="lblInputType" runat="server"><%=m_refMsg.GetMessage("lbl Input Type")%></asp:Label>
            </td>
            <td class="value">
                <asp:DropDownList ID="ddInputType" EnableViewState="True" runat="server" AutoPostBack="True" />
            </td>
        </tr>
        <tr id="TR_Validation" runat="server">
            <td class="label">
                <asp:Label ID="lblValidation" runat="server"><%=m_refMsg.GetMessage("lbl Validation")%></asp:Label>
            </td>
            <td class="value">
                <asp:DropDownList ID="ddValidationType" EnableViewState="True" runat="server" />
            </td>
        </tr>
        <tr id="TR_Min" class="switchcontent" runat="server">
            <td class="label">
                <asp:Label ID="lblMinVal" runat="server">Min Value:</asp:Label>
            </td>
            <td class="value">
                <asp:TextBox ID="txtMinValue" EnableViewState="True" Rows="20" runat="server" />
                <span class="switchcontent" id="sc2">
                    <asp:Literal ID="dtStart" runat="server" />
                </span>
            </td>
        </tr>
        <tr id="TR_Max" class="switchcontent" runat="server">
            <td class="label">
                <asp:Label ID="lblMaxVal" runat="server">Max Value:</asp:Label>
            </td>
            <td class="value">
                <asp:TextBox ID="txtMaxValue" EnableViewState="True" Rows="20" runat="server" />
                <span class="switchcontent" id="sc4">
                    <asp:Literal ID="dtEnd" runat="server" />
                </span>
            </td>
        </tr>
        <tr id="TR_Message" runat="server">
            <td class="label">
                <asp:Label ID="lblMessage" runat="server"><%=m_refMsg.GetMessage("lbl desc")%>:</asp:Label>
            </td>
            <td class="value">
                <asp:TextBox ID="txtMessage" EnableViewState="True" runat="server" />
            </td>
        </tr>
    </table>
    <%  If DisplaySelect Then%>
        <div class="ektronTopSpace"></div>
        <!--#include file="../../common/selectlist.inc" -->

        <script type="text/javascript" language="javascript">
            function InitializeSelect(){
                populateSelectedList("availableItems","<%=m_strSelectedValue%>");
                document.getElementById("selectedvalues").value ="<%=m_strSelectedValue%>";
                <%if m_intValidationType > 0 Then %>
                    document.getElementById("chkValidation").checked=true;
                <%End If%>
            }
            setTimeout('InitializeSelect()',50);
        </script>
    <%  End If%>
</div>

<script type="text/javascript" language="javascript">
<% If ddValidationType.Enabled %>
setTimeout('myBodyLoad()', 50);
<% End If %>
</script>

