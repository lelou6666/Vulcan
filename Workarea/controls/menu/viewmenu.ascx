﻿<%@ Control Language="VB" AutoEventWireup="false" CodeFile="viewmenu.ascx.vb" Inherits="viewmenu" %>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageGrid">

    <table class="ektronForm">
        <tr>
            <td class="label"><%=m_refMsg.GetMessage("generic title label")%></td>
            <td class="readOnlyValue"><%=m_strTitle%></td>
        </tr>
        <tr>
            <td class="label"><%=m_refMsg.GetMessage("lbl Image Link")%>:</td>
            <td class="readOnlyValue"><%=m_strImage%><br />
                <asp:CheckBox id="chkOverrideImage" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label"><%=m_refMsg.GetMessage("generic URL Link")%>:</td>
            <td class="readOnlyValue"><%=m_strLink%><br />
            </td>
        </tr>
        <tr>
            <td class="label"><%=m_refMsg.GetMessage("lbl template link")%>:</td>
            <td class="readOnlyValue"><%=m_strTemplate%>&nbsp;</td>
        </tr>
        <tr>
            <td class="label"><%=m_refMsg.GetMessage("description label")%></td>
            <td class="readOnlyValue"><%=m_strDescription%>&nbsp;</td>
        </tr>
        <tr>
            <td class="label"><%=m_refMsg.GetMessage("lbl folder associations")%>:</td>
            <td class="readOnlyValue"><%=m_strFolderAssociations%>&nbsp;
                <table cellspacing="0" cellpadding="0" border="1" width="100%" 
                    id="EnhancedMetadataMultiContainer1" style="border-color: rgb(216, 230, 255);">
                </table>
            </td>
        </tr>
        <tr>
            <td class="label"><%=m_refMsg.GetMessage("lbl template associations")%>:</td>
            <td class="readOnlyValue"><%=m_strTemplateAssociations%>&nbsp;</td>
        </tr>
    </table>
</div>
