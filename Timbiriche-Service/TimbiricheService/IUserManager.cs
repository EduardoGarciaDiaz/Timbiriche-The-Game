using System;
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
            int AddUser(Account account, Player player);
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
            public int IdAcccount { get { return _idAccount; } set { _idAccount = value; } }
            [DataMember]
            public string Name { get { return _name; } set { _name = value; } }
            [DataMember]
            public string LastName { get { return _lastName; } set { _lastName = value; } }
            [DataMember]
            public string Surname { get { return _surname; } set { _surname = value; } }
            [DataMember]
            public System.DateTime Birthdate { get { return _birthdate; } set { _birthdate = value; } }

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
            public int IdPlayer { get { return _idPlayer; } set { _idPlayer = value; } }
            [DataMember]
            public string Username { get { return _username; } set { _username = value; } }
            [DataMember]
            public string Email { get { return _email; } set { _email = value; } }
            [DataMember]
            public string Password { get { return _password; } set { _password = value; } }
            [DataMember]
            public int Coins { get { return _coins; } set { _coins = value; } }
            [DataMember]
            public string Status { get { return _status; } set { _status = value; } }
            [DataMember]
            public Account AccountFK { get { return _accountFK; } set { _accountFK = value; } }

        }
    }