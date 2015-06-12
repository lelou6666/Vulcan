using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Ektron.Cms;
using System.Globalization;
using Ektron.Cms.Content;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using System.Collections.Generic;

namespace Ektron.Workarea.Reports
{
    public class ViewCheckedOutClientData
    {
        #region members

        private long _Id;
        private int _LanguageId;

        #endregion

        #region properties

        public long Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }
        public int LanguageId
        {
            get
            {
                return _LanguageId;
            }
            set
            {
                _LanguageId = value;
            }
        }

        #endregion
    }

    public partial class ViewCheckedOut : System.Web.UI.UserControl
    {
        #region members

        private ContentAPI _ContentApi;
        private ContentData[] _ReportData;
        private CommonApi _CommonApi;

        #endregion

        #region properties

        public ContentData[] ReportData
        {
            get
            {
                return _ReportData;
            }
            set
            {
                _ReportData = value;
            }
        }

        #endregion

        #region events

        public ViewCheckedOut()
        {
            _ContentApi = new ContentAPI();
            _CommonApi = new CommonApi();
        }
        protected void Page_init(object sender, EventArgs e)
        {
            hdnCheckedItems.Value = String.IsNullOrEmpty(Request.Form[hdnCheckedItems.UniqueID]) ? String.Empty : Request.Form[hdnCheckedItems.UniqueID];
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            dgCheckedOut.PageSize = _CommonApi.RequestInformationRef.PagingSize;
            dgCheckedOut.CurrentPageIndex = ucPaging.SelectedPage;
            dgCheckedOut.DataSource = this.ReportData;
            dgCheckedOut.DataBind();
            if (dgCheckedOut.PageCount > 1)
            {
                ucPaging.TotalPages = dgCheckedOut.PageCount;
                ucPaging.CurrentPageIndex = dgCheckedOut.CurrentPageIndex;
            }
            else
            {
                ucPaging.Visible = false;
            }
            
        }
        protected void dgCheckedOut_ItemDataBound(Object sender, DataGridItemEventArgs e)
        {
            switch(e.Item.ItemType)
            {
                case ListItemType.Header:
                    ((Literal)e.Item.Cells[2].FindControl("litTitleHeader")).Text = "Title";
                    ((Literal)e.Item.Cells[3].FindControl("litIdHeader")).Text = "ID";
                    ((Literal)e.Item.Cells[4].FindControl("litSubmittedByHeader")).Text = "Submitted By";
                    ((Literal)e.Item.Cells[5].FindControl("litModifiedDateHeader")).Text = "Modified Date";
                    ((Literal)e.Item.Cells[6].FindControl("litPathHeader")).Text = "Path";
                    break;
                case ListItemType.Item:
                case ListItemType.AlternatingItem:
                    ((HtmlGenericControl)e.Item.Cells[0].FindControl("spanId")).Attributes.Add("data-ektron-id", ((ContentData)e.Item.DataItem).Id.ToString());
                    ((HtmlGenericControl)e.Item.Cells[0].FindControl("spanLanguageId")).Attributes.Add("data-ektron-languageId", ((ContentData)e.Item.DataItem).LanguageId.ToString());

                    ((Image)e.Item.Cells[1].FindControl("imgContentIcon")).AlternateText = ((ContentData)e.Item.DataItem).Title;
                    ((Image)e.Item.Cells[1].FindControl("imgContentIcon")).ImageUrl = GetImagePath(((ContentData)e.Item.DataItem));
                    ((Image)e.Item.Cells[1].FindControl("imgContentIcon")).Attributes.Add("title", ((ContentData)e.Item.DataItem).Title);

                    ((HyperLink)e.Item.Cells[2].FindControl("aTitle")).NavigateUrl = GetTitleHref(((ContentData)e.Item.DataItem));
                    ((HyperLink)e.Item.Cells[2].FindControl("aTitle")).Text = ((ContentData)e.Item.DataItem).Title;
                    ((HyperLink)e.Item.Cells[2].FindControl("aTitle")).Attributes.Add("title", ((ContentData)e.Item.DataItem).Title);

                    ((Literal)e.Item.Cells[3].FindControl("litIdValue")).Text = ((ContentData)e.Item.DataItem).Id.ToString();

                    ((HyperLink)e.Item.Cells[4].FindControl("aSubmittedBy")).NavigateUrl = GetSubmittedByHref(((ContentData)e.Item.DataItem));
                    ((HyperLink)e.Item.Cells[4].FindControl("aSubmittedBy")).Text = ((ContentData)e.Item.DataItem).EditorLastName + ", " + ((ContentData)e.Item.DataItem).EditorFirstName;
                    ((HyperLink)e.Item.Cells[4].FindControl("aSubmittedBy")).Attributes.Add("title", ((ContentData)e.Item.DataItem).EditorLastName + ", " + ((ContentData)e.Item.DataItem).EditorFirstName);
                    
                    ((Literal)e.Item.Cells[3].FindControl("litModifiedDateValue")).Text = ((ContentData)e.Item.DataItem).DisplayLastEditDate;

                    ((HyperLink)e.Item.Cells[2].FindControl("aPathValue")).NavigateUrl = GetPathHref(((ContentData)e.Item.DataItem));
                    ((HyperLink)e.Item.Cells[2].FindControl("aPathValue")).Text = ((ContentData)e.Item.DataItem).Path;
                    ((HyperLink)e.Item.Cells[2].FindControl("aPathValue")).Attributes.Add("title", ((ContentData)e.Item.DataItem).Path);
                    break;
            }
        }
        protected void lbCheckIn_Click(Object sender, EventArgs e)
        {
            EkContent ekContent = _ContentApi.EkContentRef;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<ViewCheckedOutClientData> clientData = serializer.Deserialize<List<ViewCheckedOutClientData>>(hdnCheckedItems.Value);

            foreach (ViewCheckedOutClientData item in clientData)
            {
                _ContentApi.ContentLanguage = item.LanguageId;
                ekContent.CheckIn(item.Id, null);
            }

            string filtertype = Server.HtmlEncode(Request.QueryString["filtertype"]);
            string orderby = Server.HtmlEncode(Request.QueryString["orderby"]);
            string filterid = Request.QueryString["filterid"];
            string action = Request.QueryString["action"];
            Response.Redirect("reports.aspx?action=" + action + "&orderby=" + orderby + "&filtertype=" + filtertype + "&filterid=" + filterid, true);
        }

        #endregion

        #region helpers

        private bool IsChecked(long id, int languageId)
        {
            return false;
        }
        private string GetImagePath(ContentData contentData)
        {
            string path = String.Empty;
            switch (contentData.ContType)
            {
                case 1:
                    path = _ContentApi.AppPath + "images/ui/icons/contentHtml.png";
                    break;
                case 2:
                    path = _ContentApi.AppPath + "images/ui/icons/contentForm.png";
                    break;
                case 3:
                    path = _ContentApi.AppPath + "images/ui/icons/icon_document.png";
                    break;
                case 1111:
                    path = _ContentApi.AppPath + "images/ui/icons/asteriskOrange.png";
                    break;
                case 3333:
                    path = _ContentApi.AppPath + "images/ui/icons/brick.png";
                    break;
                default:
                    path = contentData.AssetData.Icon;
                    break;
            }

            return path;
        }
        private string GetTitleHref(ContentData contentData)
        {
            string href = String.Empty;
            switch (contentData.ContType)
            {
                case 2:
                    href = _ContentApi.AppPath + "cmsform.aspx?action=ViewForm" + "&LangType=" + contentData.LanguageId + "&form_id=" + contentData.Id + "&folder_id=" + contentData.FolderId;
                    break;
                default:
                    href = _ContentApi.AppPath + "content.aspx?action=ViewStaged&LangType=" + contentData.LanguageId + "&id=" + contentData.Id + "&callerpage=reports.aspx" + "&origurl=" + Server.UrlEncode("action=ViewCheckedOut" + "&filtertype=" + Request.QueryString["filtertype"] + "&filterid=" + Request.QueryString["filterid"] + "&orderby=" + Request.QueryString["orderby"] + "&interval=" + Request.QueryString["interval"]);
                    break;
            }
            return href;
        }
        private string GetSubmittedByHref(ContentData contentData)
        {
            return _ContentApi.AppPath + "reports.aspx?action=ViewCheckedOut&interval=" + Request.QueryString["interval"] + "&filtertype=USER&filterId=" + contentData.UserId + "&orderby=" + Request.QueryString["orderby"];
        }

        private string GetPathHref(ContentData contentData)
        {
            return _ContentApi.AppPath + "reports.aspx?action=ViewCheckedOut&interval=" + Request.QueryString["interval"] + "&filtertype=PATH&filterId=" + contentData.FolderId + "&orderby=" + Request.QueryString["orderby"];
        }

        #endregion

        #region js/css

        private void RegisterJs()
        {
        }

        private void RegisterCss()
        {
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronPagingControlCss);
        }

        #endregion
    }
}