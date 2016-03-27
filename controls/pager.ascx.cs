using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vulcan.controls
{
    public partial class pager : System.Web.UI.UserControl
    {
        public int PageSize
        {
            get;
            set;
        }
        public bool Enabled
        {
            get;
            set;
        }

        public string PseudoID
        {
            get;
            set;
        }
        public int CurrentPage
        {
            get;
            set;
        }
        public int RecordSize
        {
            get;
            set;
        }
        protected int NumberOfPages
        {
            get
            {
                if (PageSize > 0)
                {
                    return Convert.ToInt32(Math.Ceiling((double)((double)RecordSize / (double)PageSize)));
                }
                else return 0;
            }
        }
        public bool QueryString
        {
            get;
            set;
        }

        public event EventHandler PageChanged;

        protected void Page_Init(object sender, EventArgs e)
        {
            Enabled = true;

            // Check for the data pager request variable, and if it exists, this is an HTTP GET pager, not a postback pager
            var pagerVar = Request["pager" + this.ID];
            if (pagerVar != null)
            {
                // Split the variable and store values into properties
                var pagerVarParts = pagerVar.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                CurrentPage = Convert.ToInt32(pagerVarParts[0]);
                PageSize = Convert.ToInt32(pagerVarParts[1]);
                RecordSize = Convert.ToInt32(pagerVarParts[2]);
            }
            else
            {
                CurrentPage = 1;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (PageChanged != null && Enabled)
            {
                PageChanged(this, e);
                
            }
            if (string.IsNullOrEmpty(PseudoID)) PseudoID = ID;
            if (QueryString)
                SetupQButtons();
            else
                SetupButtons();

            ShowHideButtons();
        }

        /// <summary>
        /// this is used when standard query string and not rewritten url is used. e.g. search page
        /// </summary>
        private void SetupQButtons()
        {
            var origPath = Request.Url.AbsolutePath;
            var qs = Request.Url.Query;
            //origPath = origPath.Remove(origPath.Length - 5, 5);
            var qry = HttpUtility.ParseQueryString(qs);
            // Try to parse out "pager" variable if it exists
            if (qry["pager" + PseudoID] != null)
            {
                qry["pager" + PseudoID] = "{0}";
            }
            else
            {
                qry.Add("pager" + PseudoID, "{0}");
            }

            for (int i = 0; i < qry.Keys.Count; i++)
            {
                origPath += (i == 0 ? "?" : "&") + qry.Keys[i] + "=" + qry[qry.Keys[i]];
            }
            lbtnFirst.NavigateUrl = string.Format(origPath, "1," + PageSize + "," + RecordSize);
            lbtnPrev.NavigateUrl = string.Format(origPath, (CurrentPage - 1) + "," + PageSize + "," + RecordSize);
            lbtnNext.NavigateUrl = string.Format(origPath, (CurrentPage + 1) + "," + PageSize + "," + RecordSize);
            lbtnLast.NavigateUrl = string.Format(origPath, NumberOfPages + "," + PageSize + "," + RecordSize);


        }
        private void SetupButtons()
        {

            var origPath = Request.Url.AbsolutePath;


            origPath = origPath.Remove(origPath.Length - 5, 5);
            if (origPath.EndsWith("/"))
            {
                origPath = origPath.Remove(origPath.Length - 1);
            }

            // Try to parse out "pager" variable if it exists
            int pagerIdx = origPath.IndexOf("/pager" + PseudoID + "/");
            if (pagerIdx >= 0)
            {
                pagerIdx += 7 + PseudoID.Length;
                int nextSlash = origPath.IndexOf("/", pagerIdx);
                if (nextSlash >= 0)
                {
                    origPath = origPath.Remove(pagerIdx, nextSlash - pagerIdx).Insert(pagerIdx, "{0}");
                }
                else
                {
                    origPath = origPath.Remove(pagerIdx, origPath.Length - pagerIdx).Insert(pagerIdx, "{0}");
                }
            }
            else
            {
                origPath += "/pager" + PseudoID + "/{0}";
            }

            origPath += ".aspx";

            lbtnFirst.NavigateUrl = string.Format(origPath, "1," + PageSize + "," + RecordSize);
            lbtnPrev.NavigateUrl = string.Format(origPath, (CurrentPage - 1) + "," + PageSize + "," + RecordSize);
            lbtnNext.NavigateUrl = string.Format(origPath, (CurrentPage + 1) + "," + PageSize + "," + RecordSize);
            lbtnLast.NavigateUrl = string.Format(origPath, NumberOfPages + "," + PageSize + "," + RecordSize);

        }

        private void ShowHideButtons()
        {
            lbtnFirst.Visible = lbtnPrev.Visible = spanSeparator1.Visible = CurrentPage > 1;
            lbtnLast.Visible = lbtnNext.Visible = spanSeparator2.Visible = CurrentPage < NumberOfPages;
        }

        public void UpdateFromPager(pager dp)
        {
            this.PseudoID = dp.ID;
            this.CurrentPage = dp.CurrentPage;
            this.PageSize = dp.PageSize;
            this.RecordSize = dp.RecordSize;
            ShowHideButtons();
        }
    }
}