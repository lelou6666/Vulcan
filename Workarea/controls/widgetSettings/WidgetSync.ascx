<%@ Control Language="VB" AutoEventWireup="false" CodeFile="WidgetSync.ascx.vb" Inherits="Workarea_controls_widgetSettings_WidgetSync" %>

<script type="text/javascript">
    function SyncWidgets()
    {
        if (confirm('<%= string.format(m_refMsg.GetMessage("js confirm sync widgets"), m_refContentApi.RequestInformationRef.WidgetsPath) %>'))
        {
            document.forms[0].submit();
        }

        return false;
    }
    function showEditWidget(id){
        $ektron('#WidgetEditIframe')[0].src='widgetsettings.aspx?action=widgetedit&widgetid=' + id;
        $ektron('#editWidget').modalShow();
    }
</script>
<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageGrid">
    <asp:DataGrid ID="grdWidgets"
        runat="server"
        Width="100%"
        EnableViewState="False"
        AutoGenerateColumns="False"
        CssClass="ektronGrid"
        GridLines="None">
        <Columns>
            <asp:TemplateColumn HeaderText="Widget">
                <ItemTemplate>
                    <a class="widgetedit" title="Edit <%#Container.DataItem.ControlURL%>" href="#" onclick="showEditWidget('<%#Container.DataItem.ID %>');">&nbsp;</a>
                    <asp:Label ID="lblControlURL"
                       Text='<%# DataBinder.Eval(Container.DataItem, "ControlURL") %>'
                       runat="server"/>


                </ItemTemplate>
            </asp:TemplateColumn>
        </Columns>
        <HeaderStyle CssClass="title-header" />
    </asp:DataGrid>
    <asp:Label ID="lblNoWidgets" runat="server" Visible="false" />
</div>
<div id="editWidget" class="ektronWindow ektronModalStandard">
    <div class="ektronModalHeader"><h3><span>Editing Widget</span><a href="#" class="ektronModalClose">Close</a></h3></div>
    <div class="ektronModalBody">
        <iframe style="z-index:-1; width:100%; height:400px; border:0px;" id="WidgetEditIframe" src="">
        </iframe>
    </div>
</div>

<script type="text/javascript">
    Ektron.ready(function(){

        // EDIT WIDGET MODAL
        $ektron("#editWidget").modal(
        {
            trigger: '',
            modal: true,
            toTop: true,
            onShow: function(hash)
            {
                hash.w.css("margin-top", -1 * Math.round(hash.w.outerHeight()/2)).css("top", "50%");
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
                });
            }
        });
    });
</script>
