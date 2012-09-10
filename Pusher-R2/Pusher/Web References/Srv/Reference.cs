﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:2.0.50727.3082
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// 此源代码是由 Microsoft.VSDesigner 2.0.50727.3082 版自动生成。
// 
#pragma warning disable 1591

namespace Ankh.Pusher.Srv {
    using System.Diagnostics;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="PusherSrvSoap", Namespace="http://schema.goldshowing.com/")]
    public partial class PusherSrv : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback AuthenticateOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetChannelsOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetOnlineUserCountOperationCompleted;
        
        private System.Threading.SendOrPostCallback UpdateChannelInfoOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public PusherSrv() {
            this.Url = global::Ankh.Pusher.Properties.Settings.Default.Pusher_Srv_PusherSrv;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event AuthenticateCompletedEventHandler AuthenticateCompleted;
        
        /// <remarks/>
        public event GetChannelsCompletedEventHandler GetChannelsCompleted;
        
        /// <remarks/>
        public event GetOnlineUserCountCompletedEventHandler GetOnlineUserCountCompleted;
        
        /// <remarks/>
        public event UpdateChannelInfoCompletedEventHandler UpdateChannelInfoCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://schema.goldshowing.com/Authenticate", RequestNamespace="http://schema.goldshowing.com/", ResponseNamespace="http://schema.goldshowing.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public AuthResult Authenticate(string AuthStr) {
            object[] results = this.Invoke("Authenticate", new object[] {
                        AuthStr});
            return ((AuthResult)(results[0]));
        }
        
        /// <remarks/>
        public void AuthenticateAsync(string AuthStr) {
            this.AuthenticateAsync(AuthStr, null);
        }
        
        /// <remarks/>
        public void AuthenticateAsync(string AuthStr, object userState) {
            if ((this.AuthenticateOperationCompleted == null)) {
                this.AuthenticateOperationCompleted = new System.Threading.SendOrPostCallback(this.OnAuthenticateOperationCompleted);
            }
            this.InvokeAsync("Authenticate", new object[] {
                        AuthStr}, this.AuthenticateOperationCompleted, userState);
        }
        
        private void OnAuthenticateOperationCompleted(object arg) {
            if ((this.AuthenticateCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.AuthenticateCompleted(this, new AuthenticateCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://schema.goldshowing.com/GetChannels", RequestNamespace="http://schema.goldshowing.com/", ResponseNamespace="http://schema.goldshowing.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public AuthResult GetChannels(string str) {
            object[] results = this.Invoke("GetChannels", new object[] {
                        str});
            return ((AuthResult)(results[0]));
        }
        
        /// <remarks/>
        public void GetChannelsAsync(string str) {
            this.GetChannelsAsync(str, null);
        }
        
        /// <remarks/>
        public void GetChannelsAsync(string str, object userState) {
            if ((this.GetChannelsOperationCompleted == null)) {
                this.GetChannelsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetChannelsOperationCompleted);
            }
            this.InvokeAsync("GetChannels", new object[] {
                        str}, this.GetChannelsOperationCompleted, userState);
        }
        
        private void OnGetChannelsOperationCompleted(object arg) {
            if ((this.GetChannelsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetChannelsCompleted(this, new GetChannelsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://schema.goldshowing.com/GetOnlineUserCount", RequestNamespace="http://schema.goldshowing.com/", ResponseNamespace="http://schema.goldshowing.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public int GetOnlineUserCount(string ChId) {
            object[] results = this.Invoke("GetOnlineUserCount", new object[] {
                        ChId});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void GetOnlineUserCountAsync(string ChId) {
            this.GetOnlineUserCountAsync(ChId, null);
        }
        
        /// <remarks/>
        public void GetOnlineUserCountAsync(string ChId, object userState) {
            if ((this.GetOnlineUserCountOperationCompleted == null)) {
                this.GetOnlineUserCountOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetOnlineUserCountOperationCompleted);
            }
            this.InvokeAsync("GetOnlineUserCount", new object[] {
                        ChId}, this.GetOnlineUserCountOperationCompleted, userState);
        }
        
        private void OnGetOnlineUserCountOperationCompleted(object arg) {
            if ((this.GetOnlineUserCountCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetOnlineUserCountCompleted(this, new GetOnlineUserCountCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://schema.goldshowing.com/UpdateChannelInfo", RequestNamespace="http://schema.goldshowing.com/", ResponseNamespace="http://schema.goldshowing.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void UpdateChannelInfo(string ChId, string ChName, string ChDesc) {
            this.Invoke("UpdateChannelInfo", new object[] {
                        ChId,
                        ChName,
                        ChDesc});
        }
        
        /// <remarks/>
        public void UpdateChannelInfoAsync(string ChId, string ChName, string ChDesc) {
            this.UpdateChannelInfoAsync(ChId, ChName, ChDesc, null);
        }
        
        /// <remarks/>
        public void UpdateChannelInfoAsync(string ChId, string ChName, string ChDesc, object userState) {
            if ((this.UpdateChannelInfoOperationCompleted == null)) {
                this.UpdateChannelInfoOperationCompleted = new System.Threading.SendOrPostCallback(this.OnUpdateChannelInfoOperationCompleted);
            }
            this.InvokeAsync("UpdateChannelInfo", new object[] {
                        ChId,
                        ChName,
                        ChDesc}, this.UpdateChannelInfoOperationCompleted, userState);
        }
        
        private void OnUpdateChannelInfoOperationCompleted(object arg) {
            if ((this.UpdateChannelInfoCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.UpdateChannelInfoCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schema.goldshowing.com/")]
    public partial class AuthResult {
        
        private bool isAuthenticatedField;
        
        private Channel[] channelsField;
        
        private bool isAdminField;
        
        private string versionField;
        
        private string downloadUrlField;
        
        /// <remarks/>
        public bool IsAuthenticated {
            get {
                return this.isAuthenticatedField;
            }
            set {
                this.isAuthenticatedField = value;
            }
        }
        
        /// <remarks/>
        public Channel[] Channels {
            get {
                return this.channelsField;
            }
            set {
                this.channelsField = value;
            }
        }
        
        /// <remarks/>
        public bool IsAdmin {
            get {
                return this.isAdminField;
            }
            set {
                this.isAdminField = value;
            }
        }
        
        /// <remarks/>
        public string Version {
            get {
                return this.versionField;
            }
            set {
                this.versionField = value;
            }
        }
        
        /// <remarks/>
        public string DownloadUrl {
            get {
                return this.downloadUrlField;
            }
            set {
                this.downloadUrlField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schema.goldshowing.com/")]
    public partial class Channel {
        
        private string chIdField;
        
        private System.DateTime expireDateField;
        
        private string chNameField;
        
        private string srvPortField;
        
        private string srvIPField;
        
        private string descriptionField;
        
        /// <remarks/>
        public string ChId {
            get {
                return this.chIdField;
            }
            set {
                this.chIdField = value;
            }
        }
        
        /// <remarks/>
        public System.DateTime ExpireDate {
            get {
                return this.expireDateField;
            }
            set {
                this.expireDateField = value;
            }
        }
        
        /// <remarks/>
        public string ChName {
            get {
                return this.chNameField;
            }
            set {
                this.chNameField = value;
            }
        }
        
        /// <remarks/>
        public string SrvPort {
            get {
                return this.srvPortField;
            }
            set {
                this.srvPortField = value;
            }
        }
        
        /// <remarks/>
        public string SrvIP {
            get {
                return this.srvIPField;
            }
            set {
                this.srvIPField = value;
            }
        }
        
        /// <remarks/>
        public string Description {
            get {
                return this.descriptionField;
            }
            set {
                this.descriptionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    public delegate void AuthenticateCompletedEventHandler(object sender, AuthenticateCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class AuthenticateCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal AuthenticateCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public AuthResult Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((AuthResult)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    public delegate void GetChannelsCompletedEventHandler(object sender, GetChannelsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetChannelsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetChannelsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public AuthResult Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((AuthResult)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    public delegate void GetOnlineUserCountCompletedEventHandler(object sender, GetOnlineUserCountCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetOnlineUserCountCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetOnlineUserCountCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    public delegate void UpdateChannelInfoCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
}

#pragma warning restore 1591