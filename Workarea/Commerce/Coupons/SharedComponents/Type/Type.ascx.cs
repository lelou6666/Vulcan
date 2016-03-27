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
using Ektron.Newtonsoft.Json;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Commerce.Workarea.Coupons;
using Ektron.Cms.Commerce.Workarea.Coupons.Type.ClientData;
using System.Collections.Specialized;

namespace Ektron.Cms.Commerce.Workarea.Coupons.Type
{
    public partial class Type : CouponUserControlBase, ICallbackEventHandler, ICouponUserControl
    {
        #region Member Variables

        private string _CallbackResult;
        //private NameValueCollection _PostBackData;
        private bool _PostBackResult;
        private Criteria<CurrencyProperty> _CurrencyCriteria;
        private List<CurrencyData> _CurrencyData;


        #endregion

        #region Events

        #region Page Events

        protected Type()
        {
            _CurrencyCriteria = new Criteria<CurrencyProperty>();
            _CurrencyCriteria.AddFilter(CurrencyProperty.Enabled, CriteriaFilterOperator.EqualTo, true);
            _CurrencyData = _CurrencyApi.GetList(_CurrencyCriteria);

        }
        
        protected void Page_Init(object sender, EventArgs e)
        {
            //set mode
            mvType.SetActiveView(this.IsEditable ? vwEditType : vwViewType);
            mvCode.SetActiveView(this.IsEditable ? vwEditCode : vwViewCode);
            mvDescription.SetActiveView(this.IsEditable ? vwEditDescription : vwViewDescription);
            vwCurrency.SetActiveView(this.IsEditable ? vwEditCurrency : vwViewCurrency);
            vwStatus.SetActiveView(this.IsEditable ? vwEditStatus : vwViewStatus);

            // only display the required fields message fields can be changed
            litRequiredFieldMessage.Visible = this.IsEditable;

            //register page components
            this.RegisterCSS();
            this.RegisterJS();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // skip the following initialization code if already done
            if (IsInitialized(this.UniqueID))
                return;

            //set values if data exists
            if (null != this.CouponPublishedData)
                this.SetViewValues();

            // initialize the original coupon-code hidden field (used for validation)
            hdnOriginalCouponCode.Value = (null != this.CouponPublishedData) ? this.CouponPublishedData.Code : "";

            //set hidden field indicating published coupon type
            string couponTypePublished = String.Empty;
            if (this.CouponPublishedData != null)
            {
                couponTypePublished = (this.CouponPublishedData.DiscountType == EkEnumeration.CouponDiscountType.Amount)
               ? GetLocalizedStringAmount()
               : GetLocalizedStringPercent();
            }
            hdnCouponTypePublished.Value = couponTypePublished;

            //set localized strings
            this.SetLocalizedControlText();

            SetInitialized(this.UniqueID);
        }

        #endregion

        #region Control Events

        protected void ddlCurrency_Init(object sender, EventArgs e)
        {
            //load currency drop-down list
            foreach (CurrencyData currency in _CurrencyData)
            {
                if (currency.Enabled)
                {
                    ddlCurrency.Items.Add(new ListItem(currency.Name, currency.Id.ToString(), true));
                    ddlCurrency.Items[ddlCurrency.Items.Count - 1].Selected
                        = !this.IsPostBack 
                            && _CurrencyApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId == currency.Id;
                }
            }
        }
        protected void ddlStatus_Init(object sender, EventArgs e)
        {
            //load status drop-down list
            ddlStatus.Items.Add(new ListItem(GetEnabledTrueFriendlyName(), "true", true));
            ddlStatus.Items.Add(new ListItem(GetEnabledFalseFriendlyName(), "false", true));
            ddlStatus.Items[0].Selected = true;
        }

        #endregion

        #endregion

        #region Helpers

        private void SetViewValues()
        {
            if (this.IsEditable)
            {
                // set the discount type radio buttons:
                rbAmount.Checked 
                    = (this.CouponPublishedData.DiscountType 
                        == EkEnumeration.CouponDiscountType.Amount);
                rbPercent.Checked = !rbAmount.Checked;

                // initialize the text boxes
                txtCode.Text = this.CouponPublishedData.Code;
                txtDescription.Text = this.CouponPublishedData.Description;

                // set the currency dropdown
                foreach (ListItem item in ddlCurrency.Items)
                    item.Selected = (this.CouponPublishedData.CurrencyId.ToString() == item.Value);

                // set the status drop down
                ddlStatus.Items.FindByValue("true").Selected = (this.CouponPublishedData.IsActive);
                ddlStatus.Items.FindByValue("false").Selected = !(this.CouponPublishedData.IsActive);
            }
            else
            {
                litViewStatusValue.Text = this.CouponPublishedData.IsActive ? GetEnabledTrueFriendlyName() : GetEnabledFalseFriendlyName();
                litViewDescriptionValue.Text = this.CouponPublishedData.Description;
                litViewCodeValue.Text = this.CouponPublishedData.Code;
                litViewTypeValue.Text = 
                    (this.CouponPublishedData.DiscountType == EkEnumeration.CouponDiscountType.Amount) 
                    ? GetLocalizedStringAmount() 
                    : GetLocalizedStringPercent();

                //get currency name from currency id
                Criteria<CurrencyProperty> currencyCriteria = new Criteria<CurrencyProperty>();
                CurrencyData currencyData = _CurrencyApi.GetItem(this.CouponPublishedData.CurrencyId);
                litViewCurrencyValue.Text = currencyData.Name;
            }
        }

        public string GetControlId()
        {
            return this.UniqueID;
        }

        #endregion

        #region ClientData

        public Object GetClientData()
        {
            TypeClientData clientData = new TypeClientData();
            TypeClientData.CouponTypes couponType;
            couponType = rbAmount.Checked ? TypeClientData.CouponTypes.Amount : TypeClientData.CouponTypes.Amount;
            couponType = rbPercent.Checked ? TypeClientData.CouponTypes.Percent : couponType;

            clientData.CouponType = couponType;
            clientData.CouponTypeUserChanged = Boolean.Parse(hdnCouponTypeUserChanged.Value);
            clientData.Code = txtCode.Text;
            clientData.Description = txtDescription.Text;
            clientData.CurrencyID = couponType == TypeClientData.CouponTypes.Percent ? _CurrencyApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId : Convert.ToInt32(ddlCurrency.Items[ddlCurrency.SelectedIndex].Value);
            clientData.CurrencyName = couponType == TypeClientData.CouponTypes.Percent ? this.GetLocalizedStringAllCurrencies() : ddlCurrency.Items[ddlCurrency.SelectedIndex].Text;

            //get currency symbol
            foreach (CurrencyData currency in _CurrencyData)
            {
                if (currency.Id == clientData.CurrencyID)
                {
                    clientData.CurrencySymbol = couponType == TypeClientData.CouponTypes.Percent ? "¤" : currency.CurrencySymbol;
                }
            }

            clientData.Enabled = ddlStatus.Items[ddlStatus.SelectedIndex].Value.ToLower() == "true";

            return clientData;
        }

        #endregion

        #region Localized Strings

        private void SetLocalizedControlText()
        {
            //javascript localized strings
            string values = @"Error - Invalid Code Characters!\nCoupon code cannot contain space, single- or double-quote, greater than, less than, ampersand, slash, or plus.";
            string typeChangeAlert = @"Important!\nChanging coupon discount type resets coupon discount to zero.";
            hdnTypeLocalizedStrings.Value = @"{
                        ""invalidCharacterMessage"": """ + values + @""",
                        ""typeChangeAlert"": """ + typeChangeAlert + @"""
                    }";
            
            //code valid/invalid messages
            litCodeValidMessage.Text = GetLocalizedStringCodeIsAvailable();
            litCodeInalidMessage.Text = GetLocalizedStringCodeIsAlreadyAssigned();
            litRequiredFieldMessage.Text = GetLocalizedStringRequiredField();
            btnCodeValidate.Value = "Validate Code";

            //view labels
            litCouponTypeHeader.Text = GetLocalizedStringCouponType();
            litViewCodeLabel.Text = GetLocalizedStringCode();
            litViewDescriptionLabel.Text = GetLocalizedStringDescription();
            litViewCurrencyLabel.Text = GetLocalizedStringCurrency();
            litViewStatusLabel.Text = GetLocalizedStringStatus();

            //radio button labels
            rbAmount.Text = GetLocalizedStringAmount();
            rbPercent.Text = GetLocalizedStringPercent();

            //field lablels
            litTypeLabel.Text = GetLocalizedStringType();
            lblCode.Text = GetLocalizedStringCode();
            lblCurrency.Text = GetLocalizedStringCurrency();
            lblDescription.Text = GetLocalizedStringDescription();
            lblStatus.Text = GetLocalizedStringStatus();

            litAllCurrencies.Text = this.GetLocalizedStringAllCurrencies();
        }

        public string GetEnabledTrueFriendlyName()
        {
            return "Enabled";
        }

        public string GetEnabledFalseFriendlyName()
        {
            return "Disabled";
        }

        public string GetCodeValidFriendlyName()
        {
            return "Valid Code";
        }

        public string GetCodeInvalidFriendlyName()
        {
            return "Invalid Code";
        }

        public string GetLocalizedStringAmount()
        {
            return "Amount";
        }

        public string GetLocalizedStringPercent()
        {
            return "Percentage";
        }

        public string GetLocalizedStringCodeIsAvailable()
        {
            return "Code is available!";
        }

        public string GetLocalizedStringCodeIsAlreadyAssigned()
        {
            return "Code is already assigned!";
        }

        public string GetLocalizedStringRequiredField()
        {
            return "* Required Field";
        }

        public string GetLocalizedStringCouponType()
        {
            return "Coupon Type";
        }

        public string GetLocalizedStringAllCurrencies()
        {
            return "All Currencies";
        }

        public string GetLocalizedStringCode()
        {
            return "Code";
        }

        public string GetLocalizedStringDescription()
        {
            return "Description";
        }

        public string GetLocalizedStringCurrency()
        {
            return "Currency";
        }

        public string GetLocalizedStringStatus()
        {
            return "Status";
        }

        public string GetLocalizedStringType()
        {
            return "Type";
        }

        #endregion

        #region JS, CSS, Images

        protected override void RegisterCSS()
        {
            base.RegisterCSS();
            Css.RegisterCss(this, this.ApplicationPath + "/Commerce/Coupons/SharedComponents/Type/css/type.css", "EktronCommerceCouponsTypeCss");
            Css.RegisterCss(this, this.ApplicationPath + "/Commerce/Coupons/SharedComponents/Type/css/type.ie.css", "EktronCommerceCouponsTypeIeCss", Css.BrowserTarget.LessThanEqualToIE7);
        }

        protected override void RegisterJS()
        {
            base.RegisterJS();
            JS.RegisterJS(this, JS.ManagedScript.EktronJsonJS);
            JS.RegisterJS(this, this.ApplicationPath + "/Commerce/Coupons/SharedComponents/Type/js/type.js", "EktronCommerceCouponsTypeJs");
        }

        public string GetTypeImagesPath()
        {
            return this.ApplicationPath + "/Commerce/Coupons/SharedComponents/Type/css/images";
        }

        #endregion

        #region ICallbackEventHandler

        public string GetCallbackResult()
        {
            return (_CallbackResult);
        }

        public void RaiseCallbackEvent(string eventArgs)
        {
            _PostBackResult = true;
            if (eventArgs != String.Empty)
            {
                //set up coupon code search
                List<CouponData> couponList;
                Criteria<CouponProperty> couponCriteria = new Criteria<CouponProperty>();

                //set search criteria
                couponCriteria.AddFilter(CouponProperty.Code, CriteriaFilterOperator.EqualTo, eventArgs);

                //retreive coupon list data
                couponList = _CouponApi.GetList(couponCriteria);

                //if couponList is greater than zero the couponCode is already in use (return false)
                //if coupoinList is zero couponCode is NOT in use (return true)
                _PostBackResult = couponList.Count > 0 ? false : true;
            }

            _CallbackResult = _PostBackResult.ToString().ToLower();
        }

        #endregion
    }
}