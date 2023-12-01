using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimbiricheService.Exceptions;

namespace TimbiricheService
{
    [ServiceContract]
    public interface IUserManager
    {
        [OperationContract]
        int AddUser(Player player);
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        Player ValidateLoginCredentials(String username, String password);
        [OperationContract]
        Player GetPlayerByIdPlayer(int idPlayer);
        [OperationContract]
        bool ValidateUniqueIdentifierUser(String identifier);
        [OperationContract]
        int UpdateAccount(Account account);
        [OperationContract]
        string GetUsernameByIdPlayer(int idPlayer);
        [OperationContract]
        bool ValidateIsUserAlreadyOnline(string username);
    }

    [ServiceContract(CallbackContract = typeof(IUserManagerCallback))]
    public interface IOnlineUsersManager
    {
        [OperationContract(IsOneWay = true)]
        void RegisterUserToOnlineUsers(int idPlayer, string username);
        [OperationContract(IsOneWay = true)]
        void UnregisterUserToOnlineUsers(string username);
    }

    [ServiceContract]
    public interface IUserManagerCallback
    {
        [OperationContract]
        void NotifyUserLoggedIn(string username);
        [OperationContract]
        void NotifyUserLoggedOut(string username);
        [OperationContract]
        void NotifyOnlineFriends(List<string> onlineUsernames);
    }

    [DataContract]
    public class Account
    {
        [DataMember]
        public int IdAccount { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Surname { get; set; }

        [DataMember]
        public DateTime Birthdate { get; set; }
    }

    [DataContract]
    public class Player
    {
        [DataMember]
        public int IdPlayer { get; set; }

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public int Coins { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string Salt { get; set; }

        [DataMember]
        public int IdColorSelected { get; set; }

        [DataMember]
        public int IdStyleSelected { get; set; }

        [DataMember]
        public Account AccountFK { get; set; }
    }

}