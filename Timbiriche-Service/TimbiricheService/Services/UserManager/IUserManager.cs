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
        [FaultContract(typeof(TimbiricheServerExceptions))]
        int AddUser(Player player);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        Player ValidateLoginCredentials(string username, string password);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        Player GetPlayerByIdPlayer(int idPlayer);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        bool ValidateUniqueIdentifierUser(string identifier);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        int UpdateAccount(Account account);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        string GetUsernameByIdPlayer(int idPlayer);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        bool ValidateIsUserAlreadyOnline(string username);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        int GetIdPlayerByUsername(string username);
    }
}