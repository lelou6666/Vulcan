<%@ Control Language="VB" AutoEventWireup="false" CodeFile="adduser.ascx.vb" Inherits="adduser" %>

<asp:Literal ID="PostBackPage" runat="server" />
<div id="FrameContainer" style="display: none; left: 55px; width: 1px; position: absolute;
    top: 48px; height: 1px">
    <iframe id="ChildPage" name="ChildPage" frameborder="yes" marginheight="0" marginwidth="0"
        width="100%" height="100%" scrolling="auto"></iframe>
</div>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer">
    <div id="TR_AddUserDetail" runat="server">
        <div class="ektronPageTabbed">
            <div class="tabContainerWrapper">
                <div class="tabContainer">
                    <ul>
                        <li><a href="#General">General</a></li>
                        <li><a href="#CustomProperties">Custom Properties</a></li>
                    </ul>
                    <div id="General">
                        <table class="ektronGrid">
                            <tbody>
                                <asp:Literal ID="err_msg" EnableViewState="false" runat="server" />
                                <tr>
                                    <td class="label"><span style="color:red;">*</span><%= (m_refMsg.GetMessage("username label")) %></td>
                                    <td class="value"><input type="text" id="username" name="username" maxlength="255" size="25" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="label"><span style="color:red;">*</span><%= (m_refMsg.GetMessage("first name label")) %></td>
                                    <td class="value"><input type="text" id="firstname" name="firstname" maxlength="50" size="25" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="label"><span style="color:red;">*</span><%= (m_refMsg.GetMessage("last name label")) %></td>
                                    <td class="value"><input type="text" id="lastname" name="lastname" maxlength="50" size="25" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="label"><span style="color:red;">*</span><%= (m_refMsg.GetMessage("display name label")) %>:</td>
                                    <td class="value"><input type="text" id="displayname" name="displayname" maxlength="55" size="25" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="label"><span style="color:red;">*</span><%= (m_refMsg.GetMessage("password label")) %></td>
                                    <td class="value"><input type="password" id="pwd" name="pwd" maxlength="255" size="25" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="label"><span style="color:red;">*</span><%= (m_refMsg.GetMessage("confirm pwd label")) %></td>
                                    <td class="value"><input type="password" id="confirmpwd" name="confirmpwd" maxlength="255" size="25" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server"/></td>
                                </tr>
                                <%'If m_intGroupType = 0 Then%>
                                <tr>
                                    <td class="label"><%= (m_refMsg.GetMessage("user language label")) %></td>
                                    <td class="value"><asp:DropDownList ID="language" runat="server" /></td>
                                </tr>
                                <%'End If%>
                                <tr>
                                    <td class="label"><%= (m_refMsg.GetMessage("email address label")) %></td>
                                    <td class="value"><input type="text" maxlength="255" size="25" id="email_addr1" name="email_addr1" onkeypress="javascript:return CheckKeyValue(event,'34,32');" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="label"><%=(m_refMsg.GetMessage("lbl editor"))%>:</td>
                                    <td class="value"><asp:DropDownList ID="drp_editor" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="label"><%=(m_refMsg.GetMessage("lbl avatar"))%>:</td>
                                    <td class="value">http://<input type="text" id="avatar" name="avatar" maxlength="255" size="19" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server" />
									<asp:Literal ID="ltr_upload" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="label">Address:</td>
                                    <td class="value"><input  type="text" id="mapaddress" name="mapadderss" maxlength="100" size="19" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="label">Latitude:</td>
                                    <td class="value"><input type="text" disabled="disabled" id="maplatitude" name="maplatitude" maxlength="100" size="19" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="label">Longitude:</td>
                                    <td class="value"><input type="text" disabled="disabled" id="maplongitude" name="maplongitude" maxlength="100" size="19" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server" /></td>
                                </tr>               
                            </tbody>
                        </table>
                    </div>
                    <div id="CustomProperties">
                        <table class="ektronGrid">
                            <tbody>
                                <%If m_intGroupType = 0 Then%>
                                <tr>
                                    <td class="label">
                                        <asp:Literal ID="litDisableMessage" runat="server" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="disable_msg" runat="server" />
                                        <span style="color:red;"><asp:Literal ID="msg" runat="server" /></span>
                                    </td>
                                </tr>
                                <%End If%>
                                <asp:Literal ID="litUCPUI" runat="server" />
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id="TR_AddLDAPDetail" runat="server">
        <div class="ektronPageInfo">
            <table class="ektronGrid">
                <tr>
                    <td class="label"><%= (m_refMsg.GetMessage("username label")) %></td>
                    <td class="value"><input type="text" id="LDAP_username" name="username" maxlength="255" size="25" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server"/></td>
                </tr>
                <tr>
                    <td class="label"><%=(m_refMsg.GetMessage("generic path"))%>:</td>
                    <td class="value"><input type="text" id="LDAP_ldapdomain" name="LDAP_ldapdomain" maxlength="50" size="25" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server"/></td>
                </tr>
                <tr>
                    <td class="label"><%= (m_refMsg.GetMessage("first name label")) %></td>
                    <td class="value"><input type="text" id="LDAP_firstname" name="firstname" maxlength="50" size="25" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server"/></td>
                </tr>
                <tr>
                    <td class="label"><%= (m_refMsg.GetMessage("last name label")) %></td>
                    <td class="value"><input type="text" id="LDAP_lastname" name="lastname" maxlength="50" size="25" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server"/></td>
                </tr>
                <tr>
                    <td class="label"><%= (m_refMsg.GetMessage("display name label")) %>:</td>
                    <td class="value"><input type="text" id="LDAP_displayname" name="displayname" maxlength="55" size="25" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server"/></td>
                </tr>
                <tr>
                    <td class="label"><%= (m_refMsg.GetMessage("user language label")) %></td>
                    <td class="value"><asp:DropDownList ID="LDAP_language" runat="server" /></td>
                </tr>
                <tr>
                    <td class="label"><%= (m_refMsg.GetMessage("email address label")) %></td>
                    <td class="value"><input type="text" maxlength="255" size="25" id="LDAP_email_addr1" name="email_addr1" onkeypress="javascript:return CheckKeyValue(event,'34,32');" runat="server"/></td>
                </tr>
                <tr>
                    <td class="label"><%=(m_refMsg.GetMessage("lbl editor"))%>:</td>
                    <td class="value"><asp:DropDownList ID="drp_LDAPeditor" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:CheckBox ID="LDAP_disable_msg" runat="server" /><br/>
                        <asp:Literal ID="LDAP_msg" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Literal ID="LDAP_litUCPUI" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="TR_AddUserList" runat="server">
        <div class="ektronPageGrid">
            <asp:DataGrid ID="AddUserGrid"
                runat="server"
                AutoGenerateColumns="False"
                Width="100%"
                EnableViewState="False"
                GridLines="None">
                <HeaderStyle CssClass="title-header" />
            </asp:DataGrid>
        </div>
    </div>

    <input type="hidden" name="netscape" onkeypress="javascript:return CheckKeyValue(event,'34');" />
    <input type="hidden" id="addusercount" name="addusercount" value="0" runat="server" />
</div>