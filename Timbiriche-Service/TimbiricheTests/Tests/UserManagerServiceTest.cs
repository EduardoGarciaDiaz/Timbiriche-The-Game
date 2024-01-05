using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TimbiricheService;
using Xunit;

namespace TimbiricheTests.Tests
{
    /*
    public class UserManagerServiceTest
    {
        [Fact]
        public void TestAddUserSuccess()
        {
            UserManagerService userManagerService = new UserManagerService();
            int currentResult = -1;
            int expectedResult = 2;
            DateTime.TryParse("10/25/2005", out DateTime birthdate);
            Account newAccount = new Account()
            {
                Name = "Juan Carlos",
                LastName = "Pérez",
                Surname = "Arriaga",
                Birthdate = birthdate
            };

            Player newPlayer = new Player()
            {
                Username = "ElRevo",
                Email = "elrevo@gmail.com",
                Password = "E1_R3voTecN0",
                AccountFK = newAccount
            };

            currentResult = userManagerService.AddUser(newPlayer);
            
            Assert.Equal(expectedResult, currentResult);
        }
        
        [Fact]
        public void TestAddUserFail()
        {
            UserManagerService userManagerService = new UserManagerService();
            int currentResult = -1;
            int expectedResult = -1;
            Account newAccount = new Account()
            {
                Name = "Juan Carlos",
                LastName = "",
                Surname = "Arriaga",
                Birthdate = DateTime.Now
            };

            Player newPlayer = new Player()
            {
                Username = "ElRevo",
                Email = "elrevo@gmail.com",
                Password = "",
                AccountFK = newAccount
            };

            currentResult = userManagerService.AddUser(newPlayer);

            Assert.Equal(expectedResult, currentResult);
        }

        [Fact]
        public void TestValidateLoginCredentialsSuccess()
        {
            UserManagerService userManagerService = new UserManagerService();
            string username = "ElRevo";
            string password = "E1_R3voTecN0";
            Player currentResult = userManagerService.ValidateLoginCredentials(username, password);
            Assert.NotNull(currentResult);
        }

        [Fact]
        public void TestValidateLoginCredentialsFail()
        {
            UserManagerService userManagerService = new UserManagerService();
            string username = "ElRevo";
            string password = "my_password";

            Player currentResult = userManagerService.ValidateLoginCredentials(username, password);
            Assert.Null(currentResult);
        }

        [Fact]
        public void TestValidateUniqueIdentifierUserSuccess()
        {
            UserManagerService userManagerService = new UserManagerService();
            string username = "ElRevo";
            bool currentResult = false;

            currentResult = userManagerService.ValidateUniqueIdentifierUser(username);
            Assert.True(currentResult);
        }

        [Fact]
        public void TestValidateUniqueIdentifierUserFail()
        {
            UserManagerService userManagerService = new UserManagerService();
            string username = "Eduar888888888888888888888";
            bool currentResult = false;

            currentResult = userManagerService.ValidateUniqueIdentifierUser(username);
            Assert.False(currentResult);
        }

        [Fact]
        public void TestValidateUniqueIdentifierUserEmailSuccess()
        {
            UserManagerService userManagerService = new UserManagerService();
            string username = "elrevo@gmail.com";
            bool currentResult = false;

            currentResult = userManagerService.ValidateUniqueIdentifierUser(username);
            Assert.True(currentResult);
        }

        [Fact]
        public void TestValidateUniqueIdentifierUserEmailFail()
        {
            UserManagerService userManagerService = new UserManagerService();
            string username = "eduardooooooo1111@gmail.org";
            bool currentResult = false;

            currentResult = userManagerService.ValidateUniqueIdentifierUser(username);
            Assert.False(currentResult);
        }
    }*/
}
