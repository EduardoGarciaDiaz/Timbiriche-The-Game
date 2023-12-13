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
        [FaultContract(typeof(TimbiricheServerException))]
        int AddUser(Player player);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        Player ValidateLoginCredentials(string username, string password);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        Player GetPlayerByIdPlayer(int idPlayer);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        bool ValidateUniqueIdentifierUser(string identifier);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        int UpdateAccount(Account account);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        string GetUsernameByIdPlayer(int idPlayer);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        bool ValidateIsUserAlreadyOnline(string username);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        int GetIdPlayerByUsername(string username);
    }
}