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
using System.Collections.Generic;
using Ektron.Cms.Commerce;
using Ektron.Cms.Common;
using Ektron.Cms;
using Ektron.Newtonsoft.Json;
using Ektron.Cms.API;
using Ektron.Cms.Commerce.Workarea.Coupons.List.ClientData;

namespace Ektron.Cms.Commerce.Workarea.Coupons.List.ClientData
{
    public class CouponListClientData
    {
        private long _Id;
        private bool _IsEnabled;
        private string _Code;
        private int _CurrencyId;
        private string _CurrencyName;
        private string _Description;
        private string _Count;
        private string _StartDate;
        private string _ExpirationDate;
        private bool _MarkedForDelete;

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
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }
            set
            {
                _IsEnabled = value;
            }
        }
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                _Code = value;
            }
        }
        public int CurrencyId
        {
            get
            {
                return _CurrencyId;
            }
            set
            {
                _CurrencyId = value;
            }
        }
        public string CurrencyName
        {
            get
            {
                return _CurrencyName;
            }
            set
            {
                _CurrencyName = value;
            }
        }
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }
        public string Count {
            get {
                return _Count;
            }
            set {
                _Count = value;
            }
        }
        public string StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                _StartDate = value;
            }
        }
        public string ExpirationDate
        {
            get
            {
                return _ExpirationDate;
            }
            set
            {
                _ExpirationDate = value;
            }
        }
        public bool MarkedForDelete
        {
            get
            {
                return _MarkedForDelete;
            }
            set
            {
                _MarkedForDelete = value;
            }
        }
    }
}

namespace Ektron.Cms.Commerce.Workarea.Coupons.List
{
    public partial class List : CouponBase
    {
        #region Member Variables


        private List<CouponListClientData> _ClientData;
        private int _CurrentPage;
        private int _TotalPages;
        private string _SearchText;
        private Ektron.Cms.Common.EkMessageHelper _MessageHelper;

        #endregion

        #region Properties

        public List<CouponListClientData> ClientData
        {
            get
            {
                return _ClientData;
            }
        }

        #endregion

        #region Events

        #region Page Events

        /// <summary>
        /// Set member variable defaults
        /// </summary>
        protected List()
        {
            _SearchText = String.Empty;
            _MessageHelper = _ContentApi.EkMsgRef;
        }

        /// <summary>
        /// Get coupons marked for delete.  Deserialize JSON string in hidden input field "CouponListClientData"
        /// into "List<CouponListClientData>".  Expose deserialized value as public property "ClientData"
        /// </summary>
        protected void Page_PreInit(object sender, EventArgs e)
        {
            string couponListClientData = Request.Form["CouponListClientData"] ?? String.Empty;
            if (couponListClientData != String.Empty)
            {
                _ClientData = (List<CouponListClientData>)JsonConvert.DeserializeObject(couponListClientData, typeof(List<CouponListClientData>));
            }
        }

        /// <summary>
        /// Register JS & CSS, set default CSS classes for page elements
        /// </summary>
        protected void Page_Init(object sender, EventArgs e)
        {
            //check permissions - both admin and commerce admin can edit
            mvPermissions.SetActiveView((this.IsCommerceAdmin == true || this.IsAdmin == true) ? vwValidPermissions : vwInavlidPersmissions);
            this.IsEditable = (this.IsCommerceAdmin == true || this.IsAdmin == true) ? true : false;

            //set help link
            aHelp.HRef = "#Help";
            aHelp.Attributes.Add("onclick", "window.open('" + _ContentApi.fetchhelpLink("coupons_main") + "', 'SitePreview', 'width=800,height=500,resizable=yes,toolbar=no,location=no,directories=no,status=no,menubar=no,copyhistory=no');return false;");
            imgHelp.Src = this.GetListImagesPath() + "/help.png";

            //register page components
            this.RegisterCSS();
            this.RegisterJS();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //set search text
            _SearchText = txtSearch.Text != String.Empty ? txtSearch.Text : String.Empty;
            searchLabelText.Text = _MessageHelper.GetMessage("generic search");

            //Check if user is logged in
            Utilities.ValidateUserLogin();
        }

        /// <summary>
        /// Retreive coupon list data, bind data to repeater.
        /// </summary>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            List<CouponData> couponList;
            Criteria<CouponProperty> couponCriteria = new Criteria<CouponProperty>();

            //set max records per page
            couponCriteria.PagingInfo.RecordsPerPage = _ContentApi.RequestInformationRef.PagingSize;

            //set page, sort, and search
            this.SetSearch(ref couponCriteria);
            this.SetPage(ref couponCriteria);
            this.SetSort(ref couponCriteria);

            //retreive coupon list data
            couponList = _CouponApi.GetList(couponCriteria);

            //if no coupons show no coupons message
            View activeView = couponList.Count > 0 ? vwCoupons : vwNoCoupons;
            mvPublishedCoupons.SetActiveView(activeView);

            //hide search if no coupons
            phSearch.Visible = (_SearchText != String.Empty || couponList.Count > 0) ? true : false;

            //if user is not eCom admin or admin, hide all action buttons, mark for delete, modals
            if (this.IsEditable == false)
            {
                phActions.Visible = false;
                phMarkForDeleteHeader.Visible = false;
                phModal.Visible = false;
            }

            //set ui
            this.SetSearchUi();
            this.SetPageUi(ref couponCriteria);
            this.SetSortUi(ref couponCriteria);

            //bind coupon list data to repeater
            rptCouponList.DataSource = couponList;
            rptCouponList.DataBind();

            //set localized strings
            this.SetLocalizedControlText();
        }

        protected void rptCouponList_DataBound(object sender, RepeaterItemEventArgs e)
        {
            ((PlaceHolder)e.Item.FindControl("phMarkForDeleteTableCell")).Visible = this.IsEditable;
        }

        #endregion

        #region User Events

        /// <summary>
        /// Handle user paging requests. 
        /// </summary>
        protected void Paging_Click(object sender, CommandEventArgs e)
        {
            int requestedPage = Convert.ToInt32(txtPageNumber.Text);
            int totalPages = Convert.ToInt32(hdnTotalPages.Value);
            switch (e.CommandName)
            {
                case "FirstPage":
                    _CurrentPage = 1;
                    break;
                case "PreviousPage":
                    _CurrentPage = requestedPage - 1 <= 0 ? 1 : requestedPage - 1;
                    break;
                case "NextPage":
                    _CurrentPage = requestedPage + 1 >= totalPages ? totalPages : requestedPage + 1;
                    break;
                case "LastPage":
                    _CurrentPage = totalPages;
                    break;
                case "AdHocPage":
                    _CurrentPage = requestedPage >= totalPages ? totalPages : requestedPage;
                    break;
            }

            //reset current page text field
            txtPageNumber.Text = _CurrentPage.ToString();
        }

        /// <summary>
        /// Handle user sorting request.  Can sort on any column - both ascending and descending.
        /// </summary>
        protected void Sorting_Click(object sender, CommandEventArgs e)
        {
            string sortedColumn = hdnOrderByField.Value;
            string selectedColumn = String.Empty;
            switch (e.CommandName)
            {
                case "Id":
                    selectedColumn = "Id";
                    hdnOrderByField.Value = Enum.GetName(typeof(CouponProperty), CouponProperty.Id);
                    break;
                case "IsActive":
                    selectedColumn = "IsActive";
                    hdnOrderByField.Value = Enum.GetName(typeof(CouponProperty), CouponProperty.IsActive);
                    break;
                case "IsRedeemable":
                    selectedColumn = "IsRedeemable";
                    hdnOrderByField.Value = Enum.GetName(typeof(CouponProperty), CouponProperty.IsRedeemable);
                    break;
                case "Code":
                    selectedColumn = "Code";
                    hdnOrderByField.Value = Enum.GetName(typeof(CouponProperty), CouponProperty.Code);
                    break;
                case "CurrencyId":
                    selectedColumn = "CurrencyId";
                    hdnOrderByField.Value = Enum.GetName(typeof(CouponProperty), CouponProperty.CurrencyId);
                    break;
                case "Description":
                    selectedColumn = "Description";
                    hdnOrderByField.Value = Enum.GetName(typeof(CouponProperty), CouponProperty.Description);
                    break;
                case "Count":
                    selectedColumn = "UseCount";
                    hdnOrderByField.Value = Enum.GetName(typeof(CouponProperty), CouponProperty.UseCount);
                    break;
                case "StartDate":
                    selectedColumn = "StartDate";
                    hdnOrderByField.Value = Enum.GetName(typeof(CouponProperty), CouponProperty.StartDate);
                    break;
                case "ExpirationDate":
                    selectedColumn = "ExpirationDate";
                    hdnOrderByField.Value = Enum.GetName(typeof(CouponProperty), CouponProperty.ExpirationDate);
                    break;
            }

            //set order direction hidden input
            hdnOrderDirection.Value = sortedColumn == selectedColumn ? ((hdnOrderDirection.Value.ToLower() == "ascending") ? "descending" : "ascending") : "ascending";
        }

        /// <summary>
        /// Handle user searching request.  Can search on "Code" and "Description"
        /// </summary>
        protected void Search_Click(object sender, CommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Search":
                    _SearchText = txtSearch.Text;
                    break;
                case "Clear":
                    txtSearch.Text = String.Empty;  //clear search box
                    _SearchText = String.Empty;
                    break;
            }
        }

        /// <summary>
        /// Handle user request to delete coupons (if any).  Coupons to be deleted are captured in Pre_Init.
        /// </summary>
        protected void Save_Click(object sender, CommandEventArgs e)
        {
            if (this.ClientData != null)
            {
                foreach (CouponListClientData couponMarkedForDelete in this.ClientData)
                {
                    _CouponApi.Delete(couponMarkedForDelete.Id);
                }
            }
        }

        #endregion

        #endregion

        #region Helpers

        private void SetPage(ref Criteria<CouponProperty> couponCriteria)
        {
            //current page is 1 by default or set to value via Paging_Click
            _CurrentPage = smCouponList.IsInAsyncPostBack == false ? 1 : Convert.ToInt32(txtPageNumber.Text);

            //set couponCriteria to current page value
            couponCriteria.PagingInfo.CurrentPage = _SearchText != String.Empty ? 1 : _CurrentPage;
        }

        private void SetPageUi(ref Criteria<CouponProperty> couponCriteria)
        {
            //set total pages in result set
            _TotalPages = couponCriteria.PagingInfo.TotalPages;

            //set total pages hidden field - this is required by Paging_Click
            //Paging_Click fires BEFORE Page_PreRender, so this value is only available
            //in Paging_Click via a hidden field
            hdnTotalPages.Value = _TotalPages.ToString();

            //only display paging if more than 1 page
            phPaging.Visible = _TotalPages > 1 ? true : false;
        }

        private void SetSort(ref Criteria<CouponProperty> couponCriteria)
        {
            //OrderByField is Id if default or set to value via Sorting_Click
            couponCriteria.OrderByField = smCouponList.IsInAsyncPostBack == false ? CouponProperty.Id : (CouponProperty)Enum.Parse(typeof(CouponProperty), hdnOrderByField.Value.ToLower(), true);

            //OrderByDirection is Ascending if default or set to value via Sorting_Click
            couponCriteria.OrderByDirection = smCouponList.IsInAsyncPostBack == false ? EkEnumeration.OrderByDirection.Ascending : (EkEnumeration.OrderByDirection)Enum.Parse(typeof(EkEnumeration.OrderByDirection), hdnOrderDirection.Value.ToLower(), true);
        }

        private void SetSortUi(ref Criteria<CouponProperty> couponCriteria)
        {
            //reset all CssClasses of table headers to empty
            lbId.CssClass = String.Empty;
            lbEnabled.CssClass = String.Empty;
            lbCode.CssClass = String.Empty;
            lbCurrency.CssClass = String.Empty;
            lbDescription.CssClass = String.Empty;
            lbCount.CssClass = String.Empty;
            lbStartDate.CssClass = String.Empty;
            lbEndDate.CssClass = String.Empty;

            switch (couponCriteria.OrderByField)
            {
                case CouponProperty.Id:
                    lbId.CssClass = hdnOrderDirection.Value.ToLower() == "ascending" ? "ascending" : "descending";
                    break;
                case CouponProperty.IsActive:
                    lbEnabled.CssClass = hdnOrderDirection.Value.ToLower() == "ascending" ? "ascending" : "descending";
                    break;
                case CouponProperty.IsRedeemable:
                    lbRedeemable.CssClass = hdnOrderDirection.Value.ToLower() == "ascending" ? "ascending" : "descending";
                    break;
                case CouponProperty.Code:
                    lbCode.CssClass = hdnOrderDirection.Value.ToLower() == "ascending" ? "ascending" : "descending";
                    break;
                case CouponProperty.CurrencyId:
                    lbCurrency.CssClass = hdnOrderDirection.Value.ToLower() == "ascending" ? "ascending" : "descending";
                    break;
                case CouponProperty.Description:
                    lbDescription.CssClass = hdnOrderDirection.Value.ToLower() == "ascending" ? "ascending" : "descending";
                    break;
                case CouponProperty.UseCount:
                    lbCount.CssClass = hdnOrderDirection.Value.ToLower() == "ascending" ? "ascending" : "descending";
                    break;
                case CouponProperty.StartDate:
                    lbStartDate.CssClass = hdnOrderDirection.Value.ToLower() == "ascending" ? "ascending" : "descending";
                    break;
                case CouponProperty.ExpirationDate:
                    lbEndDate.CssClass = hdnOrderDirection.Value.ToLower() == "ascending" ? "ascending" : "descending";
                    break;
            }
        }

        private void SetSearch(ref Criteria<CouponProperty> couponCriteria)
        {
            couponCriteria.AddFilter(CouponProperty.Code, CriteriaFilterOperator.Contains, _SearchText);
            couponCriteria.Condition = LogicalOperation.Or;
            couponCriteria.AddFilter(CouponProperty.Description, CriteriaFilterOperator.Contains, _SearchText);
        }

        private void SetSearchUi()
        {
            //set CssClass to show or hide clear search button
            ibClearSearch.CssClass = _SearchText == String.Empty ? "clearSearch hide" : "clearSearch";
        }

        public string GetFooterColspan()
        {
            return this.IsEditable == true ? "8" : "7";
        }

        public string GetBooleanClass(bool value)
        {
            //Boolean isActive = ((CouponData)e).IsActive;
            return value == true ? "enabled" : "disabled";
        }

        public string GetBooleanFriendlyName(bool value)
        {
            //Boolean isActive = ((CouponData)e).IsActive;
            return value == true ? "Yes" : "No";
        }


        public string GetCurrencyFriendlyName(Object e)
        {
            string currencyName = String.Empty;
            EkEnumeration.CouponDiscountType discountType = ((CouponData)e).DiscountType;
            if (discountType == EkEnumeration.CouponDiscountType.Percent)
            {
                currencyName = GetLocalizedStringAllCurrencies();
            }
            else
            {
                int currencyId = ((CouponData)e).CurrencyId;
                CurrencyData currencyData = _CurrencyApi.GetItem(currencyId);
                currencyName = currencyData.Name;
            }
            return currencyName;
        }

        public string GetDeleteImage()
        {
            string deleteImage = String.Empty;
            System.Web.HttpBrowserCapabilities browser = Request.Browser;
            string name = browser.Browser;
            float version = (float)(browser.MajorVersion + browser.MinorVersion);
            if (name == "IE" && version < 7)
            {
                deleteImage = "delete.gif";
            }
            else
            {
                deleteImage = "delete.png";
            }
            return deleteImage;
        }

        public string GetRestoreImage()
        {
            string deleteImage = String.Empty;
            System.Web.HttpBrowserCapabilities browser = Request.Browser;
            string name = browser.Browser;
            float version = (float)(browser.MajorVersion + browser.MinorVersion);
            if (name == "IE" && version < 7)
            {
                deleteImage = "restore.gif";
            }
            else
            {
                deleteImage = "restore.png";
            }
            return deleteImage;
        }

        #endregion

        #region Localized Strings

        private void SetLocalizedControlText()
        {
            litNoCoupons.Text = _SearchText != String.Empty ? "No coupons match your search terms." : "No coupons have been created.";
            litCouponHeader.Text = "Coupons";
            lbSave.Text = "Save";

            //searching
            btnSearch.Text = _MessageHelper.GetMessage("generic search");
            btnSearch.ToolTip = btnSearch.Text;
            ibClearSearch.ImageUrl = _ContentApi.AppPath + "images/ui/icons/cancel.png";
            ibClearSearch.ToolTip = GetLocalizedStringCancelSearch();
            ibFirstPage.AlternateText = "Clear Search";
            ibFirstPage.ToolTip = "Clear Search";

            //sorting
            lbId.Text = this.GetLocalizedStringIdHeader();
            lbEnabled.Text = this.GetLocalizedStringEnabledHeader();
            lbCode.Text = this.GetLocalizedStringCodeHeader();
            lbCurrency.Text = this.GetLocalizedStringCurrencyHeader();
            lbDescription.Text = this.GetLocalizedStringDescriptionHeader();
            lbCount.Text = this.GetLocalizedStringCountHeader();
            lbStartDate.Text = this.GetLocalizedStringStartDateHeader();
            lbEndDate.Text = this.GetLocalizedStringEndDateHeader();
            lbRedeemable.Text = "Redeemable";

            //paging
            litPage.Text = "Page";
            txtPageNumber.Text = _CurrentPage.ToString();
            litOf.Text = "of";
            litTotalPages.Text = _TotalPages.ToString();

            ibFirstPage.ImageUrl = base.GetImagesPath("list") + "/first.gif";
            ibFirstPage.AlternateText = "First Page";
            ibFirstPage.ToolTip = "First Page";

            ibPreviousPage.ImageUrl = base.GetImagesPath("list") + "/previous.gif";
            ibPreviousPage.AlternateText = "Previous Page";
            ibPreviousPage.ToolTip = "Previous Page";

            ibNextPage.ImageUrl = base.GetImagesPath("list") + "/next.gif";
            ibNextPage.AlternateText = "Next Page";
            ibNextPage.ToolTip = "Next Page";

            ibLastPage.ImageUrl = base.GetImagesPath("list") + "/last.gif";
            ibLastPage.AlternateText = "Last Page";
            ibLastPage.ToolTip = "Last Page";

            ibPageGo.ImageUrl = base.GetImagesPath("list") + "/go.gif";
            ibPageGo.AlternateText = "Go To Page";
            ibPageGo.ToolTip = "Go To Page";

            // modal - cancel
            litConfirmCancelHeader.Text = "Confirm Cancel";
            litCancelNo.Text = "No";
            litCancelYes.Text = "Yes";
            litConfirmCancelMessage.Text = "Restore all coupons that are marked for delete?";

            //modal - confirm save
            litConfirmSaveHeader.Text = "Confirm Save";
            litConfirmSaveMessage.Text = "Are you sure you want to delete the coupons listed in the table below?";

            //help link
            aHelp.Title = "Help";
            imgHelp.Alt = "Help";
            imgHelp.Attributes.Add("title", "Help");

            //set invalid persmissions text
            litInvalidPermissions.Text = "You either do not have permissions to add a coupon or are not logged in.";
        }

        #region Localized Strings

        public string GetLocalizedStringAllCurrencies()
        {
            return "All Currencies";
        }

        public string GetLocalizedStringViewCouponDetail()
        {
            return "Yes, Leave Page";
        }

        public string GetLocalizedStringAddCoupon()
        {
            return "Add Coupon";
        }

        public string GetLocalizedStringIdHeader()
        {
            return "Id";
        }

        public string GetLocalizedStringEnabledHeader()
        {
            return "Enabled";
        }

        public string GetLocalizedStringCodeHeader()
        {
            return "Code";
        }

        public string GetLocalizedStringCurrencyHeader()
        {
            return "Currency";
        }

        public string GetLocalizedStringDescriptionHeader()
        {
            return "Description";
        }

        public string GetLocalizedStringCountHeader() {
            return "Count";
        }

        public string GetLocalizedStringStartDateHeader()
        {
            return "Start";
        }

        public string GetLocalizedStringEndDateHeader()
        {
            return "End";
        }

        public string GetLocalizedStringHeaderRowTitle()
        {
            return "Click to sort";
        }

        public string GetLocalizedStringCouponRowTitle()
        {
            return "Click to view coupon properties";
        }

        public string GetLocalizedStringNo()
        {
            return "No";
        }

        public string GetLocalizedStringYes()
        {
            return "Yes";
        }

        public string GetLocalizedStringCancelSearch()
        {
            return "Click to cancel the search";
        }

        #endregion

        #endregion

        #region JS, CSS

        protected override void RegisterCSS()
        {
            base.RegisterCSS();
            Css.RegisterCss(this, this.ApplicationPath + @"/Commerce/Coupons/List/css/list.css", @"EktronCommerceCouponListCss");
		Css.RegisterCss(this, this.ApplicationPath + @"/Commerce/Coupons/List/css/list.ie.css", @"EktronCommerceCouponListIeCss", Css.BrowserTarget.LessThanEqualToIE7);
        }

        protected override void RegisterJS()
        {
            base.RegisterJS();
            JS.RegisterJS(this, this.ApplicationPath + @"/Commerce/Coupons/List/js/list.js", @"EktronCommerceCouponListJs");
            JS.RegisterJS(this, JS.ManagedScript.EktronInputLabelJS);
        }

        private string GetListImagesPath()
        {
            return this.ApplicationPath + @"/Commerce/Coupons/List/css/images";
        }

        #endregion

    }
}