using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Threading;
using RO.Facade3;
using RO.Common3;
using RO.Common3.Data;
using RO.SystemFramewk;
using System.Linq;
using System.Collections.Generic;

// Used by data\home\demo\Guarded\web.config and BatchRptSetup:
namespace RO.Web
{
    public partial class DnLoadModule : RO.Web.ModuleBase
    {
        private byte sid;

        public DnLoadModule()
        {
            this.Init += new System.EventHandler(Page_Init);
        }

        private string GetMimeTypeFromExtension(string extension)
        {
            //using (DirectoryEntry mimeMap = new DirectoryEntry("IIS://Localhost/MimeMap"))
            //{
            //    PropertyValueCollection propValues = mimeMap.Properties["MimeMap"];
            //    foreach (object value in propValues)
            //    {
            //        IISOle.IISMimeType mimeType = (IISOle.IISMimeType)value;
            //        if (extension == mimeType.Extension)
            //        {
            //            return mimeType.MimeType;
            //        }
            //    }
            //}
            return "application/octet-stream";
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!IsPostBack)
            {
                StringBuilder sb = new StringBuilder();
                if (Request.QueryString["tbl"] != null && Request.QueryString["key"] != null)
                {
                    /* prevent manually constructed request that would lead to information leakage via hashing of 
                     * query string and session secret, only apply for database related retrieval which are all generated by the system
                     */
                    ValidatedQS();
                    string dbConnectionString;
                    if (Request.QueryString["sys"] != null)
                    {
                        sid = byte.Parse(Request.QueryString["sys"].ToString());
                    }
                    else
                    {
                        throw new Exception("Please make sure '&sys=' is present and try again.");
                    }
                    if (new AdminSystem().IsMDesignDb(Request.QueryString["tbl"].ToString()))
                    {
                        dbConnectionString = base.SysConnectStr(sid);
                    }
                    else
                    {
                        dbConnectionString = base.AppConnectStr(sid);
                    }
                    DataTable dt = null;
                    try
                    {
                        if (Request.QueryString["knm"] != null && Request.QueryString["col"] != null)     // ImageButton
                        {
                            dt = (new AdminSystem()).GetDbImg(Request.QueryString["key"].ToString(), Request.QueryString["tbl"].ToString(), Request.QueryString["knm"].ToString(), Request.QueryString["col"].ToString(), dbConnectionString, base.AppPwd(sid));
                            Response.Buffer = true; Response.ClearHeaders(); Response.ClearContent();
                            string fileContent = RO.Common3.Utils.DecodeFileStream((byte[])dt.Rows[0][0]);
                            string fileName = "";
                            string mimeType = "application/octet";
                            string contentBase64 = "";
                            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                            try
                            {
                                RO.Common3.FileUploadObj fileInfo = jss.Deserialize<RO.Common3.FileUploadObj>(fileContent);
                                mimeType = fileInfo.mimeType;
                                fileName = fileInfo.fileName;
                                contentBase64 = fileInfo.base64;
                            }
                            catch
                            {
                                try
                                {
                                    List<RO.Common3._ReactFileUploadObj> fileList = jss.Deserialize<List<RO.Common3._ReactFileUploadObj>>(fileContent);
                                    List<FileUploadObj> x = new List<FileUploadObj>();
                                    foreach (var fileInfo in fileList)
                                    {
                                        mimeType = fileInfo.mimeType;
                                        fileName = fileInfo.fileName;
                                        contentBase64 = fileInfo.base64;
                                        break;
                                    }
                                }
                                catch
                                {
                                    contentBase64 = fileContent;
                                    fileName = "";
                                    mimeType = "image/jpeg";
                                }
                            }

                            string contentDisposition = "attachment";
                            if (!string.IsNullOrEmpty(Request.QueryString["inline"]) && Request.QueryString["inline"] == "Y")
                            {
                                contentDisposition = "inline";
                            }

                            byte[] content = new byte[0];
                            try
                            {
                                content = (byte[])Convert.FromBase64String(contentBase64);
                                Response.ContentType = mimeType;
                                Response.AppendHeader("Content-Disposition", contentDisposition + "; Filename=" + fileName);

                            }
                            catch (Exception ex)
                            {
                                if (ex != null)
                                {
                                    try
                                    {
                                        content = (byte[])dt.Rows[0][0];
                                        Response.ContentType = "image/jpeg";
                                        Response.AppendHeader("Content-Disposition", contentDisposition + "; Filename=");
                                    }
                                    catch { }
                                }
                            }
                            Response.Flush();
                            Response.BinaryWrite(content);
                            Response.End();
                        }
                        else // Document.
                        {
                            dt = (new AdminSystem()).GetDbDoc(Request.QueryString["key"].ToString(), Request.QueryString["tbl"].ToString(), dbConnectionString, base.AppPwd(sid));
                            Response.Buffer = true; Response.ClearHeaders(); Response.ClearContent();
                            Response.ContentType = dt.Rows[0]["MimeType"].ToString();
                            string contentDisposition = "attachment";
                            if (!string.IsNullOrEmpty(Request.QueryString["inline"]) && Request.QueryString["inline"] == "Y")
                            {
                                contentDisposition = "inline";
                            }
                            Response.AppendHeader("Content-Disposition", contentDisposition + "; Filename=" + dt.Rows[0]["DocName"].ToString());
                            //Response.AppendHeader("Content-Disposition", "Attachment; Filename=" + dt.Rows[0]["DocName"].ToString());
                            Response.BinaryWrite((byte[])dt.Rows[0]["DocImage"]);
                            Response.End();
                        }
                    }
                    catch (Exception err) {
                        if (!(err is ThreadAbortException))
                        {
                            ApplicationAssert.CheckCondition(false, "DnLoadModule", "", err.Message);
                        }
                    }
                }
                else if (Request.QueryString["file"] != null)
                {
                    /* file based download needs to be catered manually by webrule for protected contents
                     * via access control in the IIS directory level(no access) and gated by dnloadmodule via server side transfer
                     */
                    try
                    {
                        bool pub = true;
                        if (LImpr != null)
                        {
                            string UsrGroup = (char)191 + base.LImpr.UsrGroups + (char)191;
                            if (UsrGroup.IndexOf((char)191 + "25" + (char)191) < 0 && UsrGroup.IndexOf((char)191 + "5" + (char)191) < 0)
                                pub = true;
                            else
                                pub = false;
                        }
                        string fileName = Request.QueryString["file"].ToString();
                        string key = Request.QueryString["key"].ToString();
                        string DownloadLinkCode = Session.SessionID;
                        byte[] Download_code = System.Text.Encoding.ASCII.GetBytes(DownloadLinkCode);
                        System.Security.Cryptography.HMACMD5 bkup_hmac = new System.Security.Cryptography.HMACMD5(Download_code);
                        byte[] Download_hash = bkup_hmac.ComputeHash(System.Text.Encoding.ASCII.GetBytes(fileName));
                        string Download_hashString = BitConverter.ToString(Download_hash);
                        bool allowDownload = Download_hashString == key;
                        fileName = fileName.ToLower().Replace("/guarded/", "/source/");
                        string url = fileName;
                        string fullfileName = Server.MapPath(fileName);   // we enforce everything file for download is under ../files
                        System.IO.FileInfo file = new System.IO.FileInfo(fullfileName);
                        string oname = file.Name;
                        if (!allowDownload && pub && !(file.Name.StartsWith("Pub") || file.Name.StartsWith("pub")))
                        {
                            if (file.Name.EndsWith(".wmv"))
                            {
                                file = new FileInfo(file.DirectoryName + "/PubMsg.wmv");
                                url = fileName.Replace(oname, "PubMsg.wmv");
                            }
                            else
                            {
                                if (LUser == null || LUser.LoginName == "Anonymous")
                                {
                                    string loginUrl = System.Web.Security.FormsAuthentication.LoginUrl;
                                    if (string.IsNullOrEmpty(loginUrl)) { loginUrl = "MyAccount.aspx"; }
                                    this.Redirect(loginUrl + (loginUrl.IndexOf('?') > 0 ? "&" : "?") + "wrn=1&ReturnUrl=" + Server.UrlEncode(Request.Url.PathAndQuery));
                                }
                                else
                                {
                                    throw new Exception("Access Denied");
                                }
                            }
                        }
                        Response.Buffer = true; Response.ClearHeaders(); Response.ClearContent();
                        Response.ContentType = GetMimeTypeFromExtension(file.Extension);
                        Response.AddHeader("Content-Disposition", "Attachment; Filename=" + file.Name);
                        Response.AddHeader("Content-Length", file.Length.ToString());
                        Server.Transfer(url);
                    }
                    catch (Exception err) { ApplicationAssert.CheckCondition(false, "DnLoadModule", "", err.Message); }
                }
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            InitializeComponent();
        }

        #region Web Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion
    }
}