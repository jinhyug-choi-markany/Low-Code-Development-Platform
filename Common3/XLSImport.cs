﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4016
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by wsdl, Version=2.0.50727.1432.
// 
/*
 * with handcoded functionality, cannot rely on auto regen anymore
 */ 
namespace RO.Common3 {
    using System.Xml.Serialization;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System;
    using System.Diagnostics;
    
    // extra reference, not generated
    using System.Data;
    using System.Data.OleDb;
    using System.Collections.Generic;
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="XLSImportSoap", Namespace="http://tempuri.org/")]
    public partial class XLSImport : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetSheetNamesOperationCompleted;
        
        private System.Threading.SendOrPostCallback ImportFileOperationCompleted;

        // extra functions, not generated
        public List<string> _GetSheetNames(string fileFullName)
        {
            List<string> names = new List<string>();
            OleDbConnection conn = new OleDbConnection();
            System.Collections.ArrayList al = new System.Collections.ArrayList();
            try
            {
                conn.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileFullName + ";Extended Properties=\"Excel 12.0; HDR=NO; IMEX=1;\"";
                //conn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileFullName + ";Extended Properties=\"Excel 8.0; HDR=NO; IMEX=1;\"";
                conn.Open();
                // Get original sheet order:
                DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                DataRow[] drs = dt.Select(dt.Columns[2].ColumnName + " not like '*$Print_Area' AND " + dt.Columns[2].ColumnName + " not like '*$''Print_Area'");
                foreach (DataRow dr in drs) { names.Add(dr["TABLE_NAME"].ToString().Replace("'", string.Empty).Replace("$", string.Empty)); }
            }
            catch (Exception e)
            {
                throw (e);
            }
            finally
            {
                conn.Close(); conn = null;
            }

            return names;
        }
        public string _ImportFile(string fileName, string workSheet, string startRow, string fileFullName)
        {
            OleDbConnection conn = new OleDbConnection();
            OleDbDataAdapter da = new OleDbDataAdapter();
            DataTable dt = new DataTable();
            try
            {
                conn.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileFullName + ";Extended Properties=\"Excel 12.0; HDR=NO; IMEX=1;\"";
                //conn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileFullName + ";Extended Properties=\"Excel 8.0; HDR=NO; IMEX=1;\"";
                conn.Open();
                string myQuery = @"SELECT * From [" + workSheet + "$]";
                OleDbCommand myCmd = new OleDbCommand(myQuery, conn);
                da.SelectCommand = myCmd;
                da.Fill(dt);
            }
            catch (Exception e) { throw (e); }
            finally { conn.Close(); conn = null; }
            dt.TableName = workSheet;
            dt = CleanData(dt);
            return dt.DataTableToXml();
        }
        private DataTable CleanData(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    if (dc.DataType == typeof(string))
                    {
                        string r = "[^\x09\x0A\x0D\x20-\uD7FF\uE000-\uFFFD\u10000-\u10FFFF]";
                        dr[dc.ColumnName] = System.Text.RegularExpressions.Regex.Replace(dr[dc.ColumnName].ToString(), r, "", System.Text.RegularExpressions.RegexOptions.Compiled);
                    }
                }
            }
            return dt;
        }
        private bool embedded = true;
        /// <remarks/>
        public XLSImport(string urlEndpoint=null) {
            if (string.IsNullOrEmpty(urlEndpoint))
            {
                this.Url = "http://localhost/wsxls/xlsimport.asmx";
            }
            else
            {
                this.Url = urlEndpoint;
                embedded = false;
            }
        }
        
        /// <remarks/>
        public event GetSheetNamesCompletedEventHandler GetSheetNamesCompleted;
        
        /// <remarks/>
        public event ImportFileCompletedEventHandler ImportFileCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetSheetNames", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string[] GetSheetNames(string fileFullName) {
            try
            {
                List<string> sheets = _GetSheetNames(fileFullName);
                return sheets.ToArray();
            }
            catch {
                if (embedded) throw;
                object[] results = this.Invoke("GetSheetNames", new object[] {
                        fileFullName});
                return ((string[])(results[0]));
            }
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginGetSheetNames(string fileFullName, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("GetSheetNames", new object[] {
                        fileFullName}, callback, asyncState);
        }
        
        /// <remarks/>
        public string[] EndGetSheetNames(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public void GetSheetNamesAsync(string fileFullName) {
            this.GetSheetNamesAsync(fileFullName, null);
        }
        
        /// <remarks/>
        public void GetSheetNamesAsync(string fileFullName, object userState) {
            if ((this.GetSheetNamesOperationCompleted == null)) {
                this.GetSheetNamesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetSheetNamesOperationCompleted);
            }
            this.InvokeAsync("GetSheetNames", new object[] {
                        fileFullName}, this.GetSheetNamesOperationCompleted, userState);
        }
        
        private void OnGetSheetNamesOperationCompleted(object arg) {
            if ((this.GetSheetNamesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetSheetNamesCompleted(this, new GetSheetNamesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ImportFile", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string ImportFile(string fileName, string workSheet, string startRow, string fileFullName) {
            try
            {
                string xml = _ImportFile(fileFullName, workSheet, startRow, fileFullName);
                return xml;
            }
            catch {
                if (embedded) throw;
                object[] results = this.Invoke("ImportFile", new object[] {
                        fileName,
                        workSheet,
                        startRow,
                        fileFullName});
                return ((string)(results[0]));
            }
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginImportFile(string fileName, string workSheet, string startRow, string fileFullName, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("ImportFile", new object[] {
                        fileName,
                        workSheet,
                        startRow,
                        fileFullName}, callback, asyncState);
        }
        
        /// <remarks/>
        public string EndImportFile(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ImportFileAsync(string fileName, string workSheet, string startRow, string fileFullName) {
            this.ImportFileAsync(fileName, workSheet, startRow, fileFullName, null);
        }
        
        /// <remarks/>
        public void ImportFileAsync(string fileName, string workSheet, string startRow, string fileFullName, object userState) {
            if ((this.ImportFileOperationCompleted == null)) {
                this.ImportFileOperationCompleted = new System.Threading.SendOrPostCallback(this.OnImportFileOperationCompleted);
            }
            this.InvokeAsync("ImportFile", new object[] {
                        fileName,
                        workSheet,
                        startRow,
                        fileFullName}, this.ImportFileOperationCompleted, userState);
        }
        
        private void OnImportFileOperationCompleted(object arg) {
            if ((this.ImportFileCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ImportFileCompleted(this, new ImportFileCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    public delegate void GetSheetNamesCompletedEventHandler(object sender, GetSheetNamesCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetSheetNamesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetSheetNamesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    public delegate void ImportFileCompletedEventHandler(object sender, ImportFileCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ImportFileCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ImportFileCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}
