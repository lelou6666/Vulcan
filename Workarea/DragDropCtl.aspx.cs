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
using System.IO;
using Ektron.Cms;
using Ektron.Cms.Common;
using System.Windows.Forms;
using Ektron.ASM.AssetConfig;
using System.Collections.Generic;
using Ektron.Cms.API;
using Ektron.Cms.Controls;


public partial class Workarea_DragDropCtl : System.Web.UI.Page
{
    protected ContentAPI contentAPI = new ContentAPI();
    protected Ektron.Cms.Common.EkMessageHelper _messageHelper;
    protected bool mode_set = false;
    protected int mode_id = 0;  //mode=0->mode_id=folder_id, mode=1->mode_id=content_id
    protected int mode = 0; // 0=add, 1=update
    protected long _folderID = -1;
    protected int _contentLanguage = 1033;
    protected long _contentID = -1;
    protected bool _hasRequiredMetadata = false;
    protected string metaUrl = "";
    protected string _fileExtension = "";
    protected bool _bTrue = true;
    protected string _taxonomyIdList = "";
    protected bool _isImageGallery = false;
    protected bool isMetadataOrTaxonomyRequired = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        _messageHelper = contentAPI.EkMsgRef;
        if (contentAPI.RequestInformationRef.IsMembershipUser == 1 || contentAPI.RequestInformationRef.UserId == 0)
        {
            Response.Redirect(contentAPI.ApplicationPath + "reterror.aspx?info=" + Server.UrlEncode(_messageHelper.GetMessage("msg login cms user")), false);
            return;
        }
        RegisterResources();

        ExplorerDragDrop dropuploader = new ExplorerDragDrop();
        if (Request.QueryString["mode"] == null || Request.QueryString["mode"] == "0")
        {
            if (Request.QueryString["addforlang"] == null || Request.QueryString["addforlang"] == "0")
            {
                if (Request.QueryString["id"] != null && Request.QueryString["id"] != "")
                {
                    _folderID = Convert.ToInt64(Request.QueryString["id"]);
                    dropuploader.FolderID = _folderID;
                }
                else
                {
                    if (Request.QueryString["folder_id"] != null && Request.QueryString["folder_id"] != "")
                        _folderID = Convert.ToInt64(Request.QueryString["folder_id"]);
                    else
                        _folderID = Convert.ToInt64(Request.QueryString["mode_id"]);
                    dropuploader.FolderID = _folderID;
                }
            }
            else
            {
                if (Request.QueryString["id"] != null && Request.QueryString["id"] != "")
                    dropuploader.AssetID = Request.QueryString["id"];
                else
                    dropuploader.AssetID = Request.QueryString["mode_id"];

                if (((Request.QueryString["lang_id"] != null) && Microsoft.VisualBasic.Information.IsNumeric(Request.QueryString["lang_id"]) && (0 < Convert.ToInt32(Request.QueryString["lang_id"]))))
                {
                    _contentLanguage = Convert.ToInt32(Request.QueryString["lang_id"]);
                    dropuploader.ContentLanguage = _contentLanguage;
                }
                _folderID = Convert.ToInt64(Request.QueryString["folder_id"]);
                dropuploader.FolderID = _folderID;
            }
        }
        else
        {
            if (Request.QueryString["id"] != null && Request.QueryString["id"] != "")
            {
                dropuploader.AssetID = Request.QueryString["id"];
            }
            else
            {
                dropuploader.AssetID = Request.QueryString["mode_id"];
            }
            _folderID = Convert.ToInt64(Request.QueryString["folder_id"]);
            dropuploader.FolderID = _folderID;
        }
        if (Request.QueryString["lang_id"] != null && Request.QueryString["lang_id"] != "")
        {
            _contentLanguage = Convert.ToInt32(Request.QueryString["lang_id"]);
            dropuploader.ContentLanguage = _contentLanguage;
        }
        if (Request.QueryString["TaxonomyId"] != null && Request.QueryString["TaxonomyId"] != "")
        {
            dropuploader.TaxonomyId = Convert.ToInt64(Request.QueryString["TaxonomyId"]);
            _taxonomyIdList = Request.QueryString["TaxonomyId"].ToString();
        }

        this.isImage.Value = "0";
        if (Request.QueryString["isimage"] != null && Request.QueryString["isimage"] != "")
        {
            dropuploader.IsImage = Convert.ToInt32(Request.QueryString["isimage"]);
            if (dropuploader.IsImage == 1) 
                this.isImage.Value = "1";
        }

        if (Request.QueryString["overrideextension"] != null && Request.QueryString["overrideextension"] != "")
        {
            dropuploader.OverrideExtension = Request.QueryString["overrideextension"];
        }
        if (Request.QueryString["hidecancel"] != null && Request.QueryString["hidecancel"] == "true")
        {
            btnCancel.Visible = false;
        }

        if (Request.QueryString["isimagegallery"] != null && Request.QueryString["isimagegallery"] == "true")
        {
            _isImageGallery = true;
        }

        if (contentAPI.EkContentRef.DoesFolderRequireMetadataOrTaxonomy(_folderID, _contentLanguage))
        {
            isMetadataOrTaxonomyRequired = true;
        }

        //If the browser if firefox, showing the drag drop tab
        if (Request.Browser.Type.IndexOf("Firefox") != -1)
        {
            liDragDrop.Visible = true;
            tabDragDrop.Visible = true;
            tabDragDrop.Controls.Add(dropuploader);
        }

        //If updatemode, doesn't make sense to display multiple dms as one can only select one file.
        if (Request.QueryString["mode"] != null && Request.QueryString["mode"] == "1")
        {
            liMultipleDMS.Visible = false;
	        tabMultipleDMS.Visible = false;
        }

        bool _useSSL = Ektron.Cms.Common.EkFunctions.GetConfigBoolean("ek_UseOffloadingSSL", false);
        string _scheme = string.Empty;
        if (_useSSL)
            _scheme = "https";
        else
            _scheme = Page.Request.Url.Scheme;

        destination.Value = _scheme + Uri.SchemeDelimiter + this.Page.Request.Url.Authority + contentAPI.ApplicationPath + "processMultiupload.aspx?close=true";
        PostURL.Value = _scheme + Uri.SchemeDelimiter + this.Page.Request.Url.Authority + contentAPI.ApplicationPath + "processMultiupload.aspx?close=true";
        NextUsing.Value = _scheme + Uri.SchemeDelimiter + Page.Request.Url.Authority + contentAPI.ApplicationPath + "content.aspx"; 

        content_id.Value = "-1";
        content_folder.Value = _folderID.ToString();
        content_language.Value = _contentLanguage.ToString();
        content_teaser.Value = FillImageGalleryDescription();
        requireMetaTaxonomy.Value = isMetadataOrTaxonomyRequired.ToString();
        taxonomyselectedtree.Value = _taxonomyIdList.ToString();

        string filetypes = "";
        if (this.isImage.Value == "1")
        {
            string[] AllowedFileTypes = null;
            if (DocumentManagerData.Instance.FileTypes.Length > 0)
            {
                AllowedFileTypes = DocumentManagerData.Instance.FileTypes.Split(',');
                if (AllowedFileTypes != null && AllowedFileTypes.Length > 0)
                {
                    foreach (string filetype in AllowedFileTypes)
                    {
                        if (EkFunctions.IsImage(filetype.Trim().Replace("*", "")))
                        {
                            if (filetypes.Length > 0)
                                filetypes += "," + filetype;
                            else
                                filetypes = filetype;
                        }
                    }
                }
            }
        }
        else
            filetypes = DocumentManagerData.Instance.FileTypes;  

        HtmlGenericControl linebreak = new HtmlGenericControl("div");
        linebreak.InnerHtml += "<div id='divFileTypes'> " + _messageHelper.GetMessage("lbl valid file types") + "<p class='dmsSupportedFileTypes'>" + filetypes + "</p></div>";
        linebreak.InnerHtml += "<div id=idMultipleView style='display:inline'>";
        linebreak.InnerHtml += "<script type=\"text/javascript\">" + Environment.NewLine;
        linebreak.InnerHtml += " AC_AX_RunContent('id','idUploadCtl','name','idUploadCtl','classid','CLSID:07B06095-5687-4d13-9E32-12B4259C9813','width','100%','height','350px');" + Environment.NewLine;
        linebreak.InnerHtml += Environment.NewLine + " </script> </div> " + Environment.NewLine;
       
        tabMultipleDMS.Controls.Add(linebreak);

        GenerateStrings();

    }
        
   private void RegisterResources()
   {   // Css
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss, Css.BrowserTarget.LessThanEqualToIE7);
        // JS
        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronUICoreJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronUITabsJS);
        JS.RegisterJS(this, contentAPI.ApplicationPath + "java/ActiveXActivate.js", "EktronActiveXActivateJs");
        JS.RegisterJS(this, contentAPI.ApplicationPath + "java/RunActiveContent.js", "EktronRunActiveContentJs");
        JS.RegisterJS(this, contentAPI.ApplicationPath + "java/determineoffice.js", "EktronDetermineOfficeJs");

    }
    private void GenerateStrings()
    {
        jsTaxonomyId.Text = _taxonomyIdList;
        jsTaxonomyIdReloadFrame.Text = _taxonomyIdList;
        jsFolderID.Text = _folderID.ToString();
        jsLanguageID.Text = _contentLanguage.ToString();
        jsModeSet.Text = mode_set.ToString();
        jsMode.Text = mode.ToString();
        jsModeId.Text = mode_id.ToString();
        jsCheckedOutMsg.Text = _messageHelper.GetMessage("js file checked out cannot overwrite");
        jsFileOverwriteMsg.Text = _messageHelper.GetMessage("js file exists overwrite");
        ltrPleaseWait.Text = _messageHelper.GetMessage("msg pls wait file uploads");
        if (_isImageGallery == true)
            jsCheckFileTaxId.Text = _taxonomyIdList;
        else
            jsCheckFileTaxId.Text = "-1";
    }

    protected void uploadFile_Click(object sender, EventArgs e)
    {
        string fileName = string.Empty;
        HttpPostedFile fileUpld = ekFileUpload.PostedFile;
        string hasValidExtension = "";
        List<string> AllowedFileTypes = new List<string>();
        bool createOverwriteThumbnail = false;
        bool isImageAsset = false;
        ContentData cData;

        long _checkTaxID = -1;
        AllowedFileTypes.AddRange(DocumentManagerData.Instance.FileTypes.ToString().Split(','));
        if (fileUpld.ContentLength > 0)
        {
            Ektron.Cms.UserAPI uAPI = new UserAPI();

            _fileExtension = Path.GetExtension(fileUpld.FileName);

            //If its an image asset then create/overwrite the thumbnail
            isImageAsset = Ektron.Cms.Common.EkFunctions.IsImage(_fileExtension);
            if (isImageAsset && !isMetadataOrTaxonomyRequired)
                createOverwriteThumbnail = true;

            hasValidExtension = AllowedFileTypes.Find(new Predicate<string>(delegate(string t) { return t.ToLower().Replace(" ", "") == ("*" + _fileExtension.ToLower()); }));
            if (hasValidExtension != null && hasValidExtension != "")
            {
                //If Image Gallery, Should check if the file type is an image file type
                if (Request.QueryString["isimage"] != null && Request.QueryString["isimage"] != "" && Convert.ToInt32(Request.QueryString["isimage"]) == 1)
                {
                    if (!isImageAsset)
                    {
                        _bTrue = false;
                        ltrStatus.Text = _messageHelper.GetMessage("msg invalid file upload images only");
                        setInvalid();
                    }

                }

                fileName = Path.GetFileName(fileUpld.FileName);
                if (fileName.IndexOf("&") > -1 || fileName.IndexOf("+") > -1 || fileName.IndexOf("%") > -1)
                {
                    _bTrue = false;
                    ltrStatus.Text = _messageHelper.GetMessage("msg cannot add file with add and plus");
                    setInvalid();
                }

                if (_bTrue)
                {
                    int fileLength = fileUpld.ContentLength;
                    byte[] fileData = new byte[fileLength];
                    string file = Convert.ToString(fileUpld.InputStream.Read(fileData, 0, fileLength));

                    if (fileData.Length > 0)
                    {
                        System.IO.MemoryStream stream = new System.IO.MemoryStream(fileData);
                        contentAPI.RequestInformationRef.UserId = uAPI.UserId;
                        contentAPI.ContentLanguage = _contentLanguage;

                        Ektron.ASM.AssetConfig.AssetData asstData = new Ektron.ASM.AssetConfig.AssetData();
                        Ektron.Cms.API.Content.Content cContent = new Ektron.Cms.API.Content.Content();
                        if (_isImageGallery)
                            _checkTaxID = Convert.ToInt64(_taxonomyIdList);
                        asstData = contentAPI.EkContentRef.GetAssetDataBasedOnFileName(fileName.Replace("'", "_"), _folderID, _checkTaxID);
                        if (asstData != null && asstData.ID != "" && asstData.Name != "")
                        {
                            Ektron.Cms.AssetUpdateData astData = new AssetUpdateData();
                            TaxonomyBaseData[] taxonomyCatArray = null;
                            _contentID = Convert.ToInt64(asstData.ID);
                            cData = cContent.GetContent(_contentID, Ektron.Cms.ContentAPI.ContentResultType.Published);

                            astData.FileName = fileName;
                            astData.FolderId = _folderID;
                            astData.ContentId = cData.Id;
                            astData.Teaser = cData.Teaser;
                            astData.Comment = cData.Comment;
                            astData.Title = cData.Title;
                            astData.GoLive = cData.GoLive;

                            //Assigning the categories
                            taxonomyCatArray = contentAPI.ReadAllAssignedCategory(_contentID);
                            if (taxonomyCatArray != null && taxonomyCatArray.Length > 0)
                            {
                                foreach (TaxonomyBaseData tBaseData in taxonomyCatArray)
                                {
                                    if (astData.TaxonomyTreeIds == "")
                                        astData.TaxonomyTreeIds = tBaseData.TaxonomyId.ToString();
                                    else
                                        astData.TaxonomyTreeIds += "," + tBaseData.TaxonomyId.ToString();
                                }
                            }

                            //Assigning the metadata
                            if (cData.MetaData != null && cData.MetaData.Length > 0)
                            {
                                astData.MetaData = new AssetUpdateMetaData[cData.MetaData.Length - 1];
                                for (int i = 0; i < cData.MetaData.Length - 1; i++)
                                {
                                    astData.MetaData[i] = new AssetUpdateMetaData();
                                    astData.MetaData[i].TypeId = cData.MetaData[i].TypeId;
                                    astData.MetaData[i].ContentId = cData.Id;
                                    astData.MetaData[i].Text = cData.MetaData[i].Text;
                                }
                            }
                            astData.EndDate = cData.EndDate;
                            astData.EndDateAction = (Ektron.Cms.Common.EkEnumeration.CMSEndDateAction)cData.EndDateAction;

                            //Updating the Content
                            bool isUpdated = contentAPI.EditAsset(stream, astData);

                            //Creating the thumbnail, as service takes a while to generate and we see a broken Image in that time.
                            if (!isUpdated && createOverwriteThumbnail)
                            {
                                cData = contentAPI.ShowContentById(cData.Id, contentAPI.CmsPreview, !contentAPI.CmsPreview);
                                if (cData.Status == "A")
                                CreateThumbNailIfOneDoesntExist(GetPath(cData), 125, true);
                            }
                        }
                        else
                        {
                            Ektron.Cms.AssetUpdateData astData = new AssetUpdateData();
                            astData.FileName = fileName;
                            astData.FolderId = _folderID;
                            astData.TaxonomyTreeIds = _taxonomyIdList;
                            astData.Teaser = FillImageGalleryDescription();
                            astData.Title = Path.GetFileNameWithoutExtension(fileName);
                            astData.LanguageId = _contentLanguage;
                            _contentID = contentAPI.AddAsset(stream, astData);

                            //Creating the thumbnail, as service takes a while to generate and we see a broken Image in that time.
                            if (_contentID > 0 && createOverwriteThumbnail)
                            {
                                cData = contentAPI.ShowContentById(_contentID, contentAPI.CmsPreview, !contentAPI.CmsPreview);
                                if (cData.Status == "A")
                                CreateThumbNailIfOneDoesntExist(GetPath(cData), 125, false);
                            }
                        }
                        jsMetaUrl.Text = "";
                        if (isMetadataOrTaxonomyRequired)
                        {
                            string _taxString = string.Empty;
                            if (_taxonomyIdList != "")
                                _taxString = "&taxonomyId=" + _taxonomyIdList;
                            jsMetaUrl.Text = contentAPI.AppPath + "DMSMetadata.aspx?contentId=" + _contentID + "&idString=" + _contentID + "&folderId=" + _folderID + _taxString + "&close=true&EkTB_iframe=true&height=550&width=650&modal=true&refreshCaller=true"; ;
                        }

                        isFileUploadComplete.Value = "true";
                        ClientScript.RegisterStartupScript(this.GetType(), "closeThickBox", "uploadClick();", true);
                    }
                }
            }
            else
            {
                setInvalid();
                ltrStatus.Text = _messageHelper.GetMessage("msg invalid file upload");
            }
        }
        else
        {
            setInvalid();
            ltrStatus.Text = _messageHelper.GetMessage("lbl upload file");
        }
    }

    private string FillImageGalleryDescription()
    {
        string description = string.Empty;
        if (Request.QueryString["isimage"] != null && Request.QueryString["isimage"] != "" && Convert.ToInt32(Request.QueryString["isimage"]) == 1)
        {
            if (Ektron.ASM.EkDavProtocol.Constants.GetCustomCacheManger()[string.Format("{0}_{1}_MapMeta", contentAPI.UserId, contentAPI.UniqueId)] != null)
            {
                string[] mapMeta = (string[])Ektron.ASM.EkDavProtocol.Constants.GetCustomCacheManger()[string.Format("{0}_{1}_MapMeta", contentAPI.UserId, contentAPI.UniqueId)];
                if (mapMeta != null)
                {
                    if (mapMeta.Length > 3)
                    {
                        description = mapMeta[3].ToString();
                    }
                }
            }
        }
        return description;
    }

    private void setInvalid()
    {
        DragDropUI.Style.Add("position", "relative");
        DragDropUI.Style.Add("left", "0px");
        ek_DmsFileUploadWait.Style.Add("position", "relative");
        ek_DmsFileUploadWait.Style.Add("left", "-10000px");
        isFileUploadComplete.Value = "invalid";
    }

    [System.Web.Services.WebMethod]
    [System.Web.Script.Services.ScriptMethod]
    public static string CheckFileExists(string FileName, string FolderID, string ContLanguage, string taxonomyID)
    {
        ContentAPI cApi = new ContentAPI();
        string cStatus = string.Empty;
        cApi.ContentLanguage = Convert.ToInt32(ContLanguage);
        Ektron.ASM.AssetConfig.AssetData assetDat = new Ektron.ASM.AssetConfig.AssetData();

        assetDat = cApi.EkContentRef.GetAssetDataBasedOnFileName(Path.GetFileName(FileName.Replace("'", "_")), Convert.ToInt64(FolderID), Convert.ToInt64(taxonomyID));
        if (assetDat != null && assetDat.ID != "" && assetDat.Name != "")
        {
            ContentStateData myContentStateData = new ContentStateData();
            myContentStateData = cApi.GetContentState(Convert.ToInt64(assetDat.ID));
            cStatus = myContentStateData.Status;
            if (cStatus == "O")
            {
                //Set Property variable that identify the user by whom the content is checked out
                if (myContentStateData.CurrentUserId == cApi.UserId)
                    cStatus = "A";
            }
            return cStatus;
        }
        else
            return "";
    }

    private string GetPath(ContentData conData)
    {
        string filePath = string.Empty;
        if (conData != null && conData.AssetData != null && conData.AssetData.Version.Length > 0)
        {
            filePath = this.Page.Server.MapPath(contentAPI.EkContentRef.GetViewUrl(Convert.ToInt32(conData.Type), conData.AssetData.Id).Replace(this.Page.Request.Url.Scheme + "://" + this.Page.Request.Url.Authority, "").Replace(":443", "").Replace(":80", ""));
        }
        return filePath;
    }

    private string CreateThumbNailIfOneDoesntExist(string sourcepath, int width, bool overwrite)
    {
        double factor;
        System.Drawing.Image thumbnail = null;
        System.Drawing.Image fullimage = null;
        FileStream fs = null;

        string file = Path.GetFileName(sourcepath);
        string ext = Path.GetExtension(sourcepath).ToLower();
        if (ext == ".gif")
            ext = ".png";

        string filethumb = Path.GetDirectoryName(sourcepath) + "\\thumb_" + Path.GetFileNameWithoutExtension(sourcepath) + ext;

        if (overwrite || (!overwrite && !File.Exists(filethumb)))
        {
            try
            {
                if (File.Exists(filethumb))
                    File.Delete(filethumb);

                fs = new FileStream(sourcepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                fullimage = System.Drawing.Image.FromStream(fs);
                factor = fullimage.Width / (double)width;
                thumbnail = fullimage.GetThumbnailImage(width, (int)(fullimage.Height / factor), null, IntPtr.Zero);

                switch (ext)
                {
                    case ".bmp":
                        thumbnail.Save(filethumb, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".gif":
                        thumbnail.Save(filethumb, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    case ".jpg":
                        thumbnail.Save(filethumb, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".jpeg":
                        thumbnail.Save(filethumb, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".pbm":
                        thumbnail.Save(filethumb, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".cmx":
                        thumbnail.Save(filethumb, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".ico":
                        thumbnail.Save(filethumb, System.Drawing.Imaging.ImageFormat.Icon);
                        break;
                    case ".cod":
                        thumbnail.Save(filethumb, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".ief":
                        thumbnail.Save(filethumb, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".jfif":
                        thumbnail.Save(filethumb, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".jpe":
                        thumbnail.Save(filethumb, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".pgm":
                        thumbnail.Save(filethumb, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".pnm":
                        thumbnail.Save(filethumb, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".ppm":
                        thumbnail.Save(filethumb, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".png":
                        thumbnail.Save(filethumb, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    case ".ras":
                        thumbnail.Save(filethumb, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".rgb":
                        thumbnail.Save(filethumb, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".tif":
                        thumbnail.Save(filethumb, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".tiff":
                        thumbnail.Save(filethumb, System.Drawing.Imaging.ImageFormat.Tiff);
                        break;
                    case ".xpm":
                        thumbnail.Save(filethumb, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".xbm":
                        thumbnail.Save(filethumb, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".xwd":
                        thumbnail.Save(filethumb, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    default:
                        thumbnail.Save(filethumb, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                }

            }
            catch
            {
                throw;
            }
            finally
            {
                if (thumbnail != null)
                {
                    thumbnail.Dispose();
                    thumbnail = null;
                }
                if (fullimage != null)
                {
                    fullimage.Dispose();
                    fullimage = null;
                }

                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
            }
        }
        return filethumb;
    }

}
