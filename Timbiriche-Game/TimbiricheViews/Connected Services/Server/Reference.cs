﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TimbiricheViews.Server {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Player", Namespace="http://schemas.datacontract.org/2004/07/TimbiricheService")]
    [System.SerializableAttribute()]
    public partial class Player : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private TimbiricheViews.Server.Account accountFKField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int coinsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string emailField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int idPlayerField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string passwordField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string saltField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string statusField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string usernameField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public TimbiricheViews.Server.Account accountFK {
            get {
                return this.accountFKField;
            }
            set {
                if ((object.ReferenceEquals(this.accountFKField, value) != true)) {
                    this.accountFKField = value;
                    this.RaisePropertyChanged("accountFK");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int coins {
            get {
                return this.coinsField;
            }
            set {
                if ((this.coinsField.Equals(value) != true)) {
                    this.coinsField = value;
                    this.RaisePropertyChanged("coins");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string email {
            get {
                return this.emailField;
            }
            set {
                if ((object.ReferenceEquals(this.emailField, value) != true)) {
                    this.emailField = value;
                    this.RaisePropertyChanged("email");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int idPlayer {
            get {
                return this.idPlayerField;
            }
            set {
                if ((this.idPlayerField.Equals(value) != true)) {
                    this.idPlayerField = value;
                    this.RaisePropertyChanged("idPlayer");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string password {
            get {
                return this.passwordField;
            }
            set {
                if ((object.ReferenceEquals(this.passwordField, value) != true)) {
                    this.passwordField = value;
                    this.RaisePropertyChanged("password");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string salt {
            get {
                return this.saltField;
            }
            set {
                if ((object.ReferenceEquals(this.saltField, value) != true)) {
                    this.saltField = value;
                    this.RaisePropertyChanged("salt");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string status {
            get {
                return this.statusField;
            }
            set {
                if ((object.ReferenceEquals(this.statusField, value) != true)) {
                    this.statusField = value;
                    this.RaisePropertyChanged("status");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string username {
            get {
                return this.usernameField;
            }
            set {
                if ((object.ReferenceEquals(this.usernameField, value) != true)) {
                    this.usernameField = value;
                    this.RaisePropertyChanged("username");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Account", Namespace="http://schemas.datacontract.org/2004/07/TimbiricheService")]
    [System.SerializableAttribute()]
    public partial class Account : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime birthdateField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int idAcccountField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string lastNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string nameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string surnameField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime birthdate {
            get {
                return this.birthdateField;
            }
            set {
                if ((this.birthdateField.Equals(value) != true)) {
                    this.birthdateField = value;
                    this.RaisePropertyChanged("birthdate");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int idAcccount {
            get {
                return this.idAcccountField;
            }
            set {
                if ((this.idAcccountField.Equals(value) != true)) {
                    this.idAcccountField = value;
                    this.RaisePropertyChanged("idAcccount");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string lastName {
            get {
                return this.lastNameField;
            }
            set {
                if ((object.ReferenceEquals(this.lastNameField, value) != true)) {
                    this.lastNameField = value;
                    this.RaisePropertyChanged("lastName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string name {
            get {
                return this.nameField;
            }
            set {
                if ((object.ReferenceEquals(this.nameField, value) != true)) {
                    this.nameField = value;
                    this.RaisePropertyChanged("name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string surname {
            get {
                return this.surnameField;
            }
            set {
                if ((object.ReferenceEquals(this.surnameField, value) != true)) {
                    this.surnameField = value;
                    this.RaisePropertyChanged("surname");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="Server.IUserManager")]
    public interface IUserManager {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUserManager/AddUser", ReplyAction="http://tempuri.org/IUserManager/AddUserResponse")]
        int AddUser(TimbiricheViews.Server.Player player);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUserManager/AddUser", ReplyAction="http://tempuri.org/IUserManager/AddUserResponse")]
        System.Threading.Tasks.Task<int> AddUserAsync(TimbiricheViews.Server.Player player);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUserManager/ValidateLoginCredentials", ReplyAction="http://tempuri.org/IUserManager/ValidateLoginCredentialsResponse")]
        bool ValidateLoginCredentials(string username, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUserManager/ValidateLoginCredentials", ReplyAction="http://tempuri.org/IUserManager/ValidateLoginCredentialsResponse")]
        System.Threading.Tasks.Task<bool> ValidateLoginCredentialsAsync(string username, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUserManager/ValidateUniqueIdentifierUser", ReplyAction="http://tempuri.org/IUserManager/ValidateUniqueIdentifierUserResponse")]
        bool ValidateUniqueIdentifierUser(string identifier);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUserManager/ValidateUniqueIdentifierUser", ReplyAction="http://tempuri.org/IUserManager/ValidateUniqueIdentifierUserResponse")]
        System.Threading.Tasks.Task<bool> ValidateUniqueIdentifierUserAsync(string identifier);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IUserManagerChannel : TimbiricheViews.Server.IUserManager, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class UserManagerClient : System.ServiceModel.ClientBase<TimbiricheViews.Server.IUserManager>, TimbiricheViews.Server.IUserManager {
        
        public UserManagerClient() {
        }
        
        public UserManagerClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public UserManagerClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public UserManagerClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public UserManagerClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public int AddUser(TimbiricheViews.Server.Player player) {
            return base.Channel.AddUser(player);
        }
        
        public System.Threading.Tasks.Task<int> AddUserAsync(TimbiricheViews.Server.Player player) {
            return base.Channel.AddUserAsync(player);
        }
        
        public bool ValidateLoginCredentials(string username, string password) {
            return base.Channel.ValidateLoginCredentials(username, password);
        }
        
        public System.Threading.Tasks.Task<bool> ValidateLoginCredentialsAsync(string username, string password) {
            return base.Channel.ValidateLoginCredentialsAsync(username, password);
        }
        
        public bool ValidateUniqueIdentifierUser(string identifier) {
            return base.Channel.ValidateUniqueIdentifierUser(identifier);
        }
        
        public System.Threading.Tasks.Task<bool> ValidateUniqueIdentifierUserAsync(string identifier) {
            return base.Channel.ValidateUniqueIdentifierUserAsync(identifier);
        }
    }
}
