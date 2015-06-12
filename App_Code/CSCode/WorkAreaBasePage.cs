namespace Ektron.Cms.Workarea.Framework
{
	/// <summary>
	/// Alias for WorkareaBasePage
	/// </summary>
	public abstract class WorkareaDialogPage : WorkAreaBasePage
	{
	}

	/// <summary>
	/// Base class for all workarea pages - *.aspx pages
	/// Security should be enforced here
	/// </summary>
	public abstract class WorkAreaBasePage : System.Web.UI.Page 
	{
		private Ektron.Cms.Common.EkMessageHelper m_refMsg = null;
		private Ektron.Cms.CommonApi m_api = null;

		public WorkAreaBasePage()
		{
		}

		protected Ektron.Cms.CommonApi GetCommonApi()
		{
			if (null == m_api)
			{
				m_api = new Ektron.Cms.CommonApi();
			}
			return m_api;
		}

		protected string GetMessage(string key)
		{
			if (null == m_refMsg)
			{
				m_refMsg = GetCommonApi().EkMsgRef;
			}
			return m_refMsg.GetMessage(key);
		}

		protected void AssertInternalReferrer()
		{
			if (Request.IsLocal || Request.Url.IsLoopback) return;
			if (null == Request.UrlReferrer || Request.Url.Authority != Request.UrlReferrer.Authority)
			{
				throw new System.Exception("This page may be used only from within this site.");
			}
		}

		protected void RegisterWorkareaCssLink()
		{
			Ektron.Cms.API.Css.RegisterCss(this, SkinControlsPath + "Editor/WorkArea.css", "WorkAreaCSS");
		}

		protected void RegisterDialogCssLink()
		{
			Ektron.Cms.API.Css.RegisterCss(this, SkinControlsPath + "Editor/Dialogs.css", "EditorDialogsCSS");
		}

		#region Properties

		/// <summary>
		/// Gets the path to the Script directory
		/// </summary>
		public string ScriptPath
		{
			get
			{
				return ResolveUrl(GetCommonApi().AppPath + "java/");
			}
		}

		/// <summary>
		/// Gets the path to the Controls directory for the current page's theme
		/// </summary>
		public string SkinControlsPath
		{
			get
			{
				return ResolveUrl(GetCommonApi().AppPath + "csslib/");
			}
		}

		#endregion
	}
}