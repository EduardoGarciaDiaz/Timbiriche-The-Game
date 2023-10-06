﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{

    [ServiceContract]
    public interface IUserManager
    {
        [OperationContract]
        int addUser(Account account, Player player);

        [OperationContract]
        bool validateCredentials(String username, String password);
    }

    [DataContract]
    public class Account
    {
        private int _idAccount;
        private string _name;
        private string _lastName;
        private string _surname;
        private DateTime _birthdate;

        [DataMember]
        public int idAcccount { get { return _idAccount; } set { _idAccount = value; } }
        [DataMember]
        public string name { get { return _name; } set { _name = value; } }
        [DataMember]
        public string lastName { get { return _lastName; } set { _lastName = value; } }
        [DataMember]
        public string surname { get { return _surname; } set { _surname = value; } }
        [DataMember]
        public System.DateTime birthdate { get { return _birthdate; } set { _birthdate = value; } }

    }

    [DataContract]

    public class Player
    {
        private int _idPlayer;
        private string _username;
        private string _email;
        private string _password;
        private int _coins;
        private string _status;
        private Account _accountFK;

        [DataMember]
        public int idPlayer { get { return _idPlayer; } set { _idPlayer = value; } }
        [DataMember]
        public string username { get { return _username; } set { _username = value; } }
        [DataMember]
        public string email { get { return _email; } set { _email = value; } }
        [DataMember]
        public string password { get { return _password; } set { _password = value; } }
        [DataMember]
        public int coins { get { return _coins; } set { _coins = value; } }
        [DataMember]
        public string status { get { return _status; } set { _status = value; } }
        [DataMember]
        public Account accountFK { get { return _accountFK; } set { _accountFK = value; } }

    }
}