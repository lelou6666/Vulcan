<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ucCollectionTree.ascx.cs" Inherits="Ektron.ContentDesigner.Dialogs.CollectionTree" %>
<asp:Repeater ID="collections" runat="server">
    <ItemTemplate>
        <div class="treecontainer">
        <span class="folder" data-ektron-resid="<%#DataBinder.Eval(Container.DataItem, "Id")%>">
        <%#DataBinder.Eval(Container.DataItem, "Title")%></span>
        </div>
    </ItemTemplate>
</asp:Repeater>
<script type="text/javascript">
<!--

function  openToSelectedCollection(tid)
{
    //now use tid to select the collection 
    var clicktarget = $ektron("div.treecontainer span.folder[data-ektron-resid='" + tid + "']");
    if(clicktarget.length > 0)
    {
        clicktarget.click();
    }
    //now scroll to the folder
    $ektron("#Pageview2").scrollTo("div.treecontainer span.folder[data-ektron-resid='" + tid + "']");
}

function configCollectionClickAction()
{
    $ektron("div.treecontainer span[data-ektron-resid]").unbind("click").click(function(){
        $ektron("div.treecontainer .selected").removeClass("selected");
        $ektron(this).addClass("selected");
        var objectID = $ektron(this).attr("data-ektron-resid");
        var pageNum = 0;
        var action = "getcollectioncontent";
        var objecttype = "collection";
        getResults(action, objectID, pageNum, objecttype, "", this, "collection");
    });
}

Ektron.ready(function(event, eventName)
{
    var iconSrc = "<%= new Ektron.Cms.SiteAPI().ApplicationPath %>images/UI/Icons/collection.png";
    var eIcon = $ektron("<img />").attr("style", "padding: 0 2px 0 0; vertical-align: bottom;").attr("src", iconSrc).attr("alt", ResourceText.CollectionItems);
    $ektron("div#PageViewCollection div.treecontainer").prepend(eIcon);
});
//-->
</script>