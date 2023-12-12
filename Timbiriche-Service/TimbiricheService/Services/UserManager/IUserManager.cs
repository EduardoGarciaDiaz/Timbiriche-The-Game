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
        [FaultContract(typeof(TimbiricheServerException))]
        Player GetPlayerByIdPlayer(int idPlayer);
        
        [OperationContract]
        bool ValidateUniqueIdentifierUser(String identifier);
        
        [OperationContract]
        int UpdateAccount(Account account);
        
        [OperationContract]
        string GetUsernameByIdPlayer(int idPlayer);
        
        [OperationContract]
        bool ValidateIsUserAlreadyOnline(string username);
        
        [OperationContract]
        int GetIdPlayerByUsername(string username);
    }
}