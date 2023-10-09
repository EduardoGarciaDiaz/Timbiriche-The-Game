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
                name = "Juan",
                lastName = "Durán",
                surname = "Ortiz",
                birthdate = birthdate
            };

            Player newPlayer = new Player()
            {
                username = "Juanillo_1",
                email = "juani@gmail.com",
                password = "Juan_D0*0cT5O",
                accountFK = newAccount
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
                name = "Juan",
                lastName = "",
                surname = "Ortiz",
                birthdate = DateTime.Now
            };

            Player newPlayer = new Player()
            {
                username = "Juanillo_1",
                email = "juani@gmail.com",
                password = "",
                accountFK = newAccount
            };

            currentResult = userManagerService.AddUser(newPlayer);

            Assert.Equal(expectedResult, currentResult);
        }
        
        [Fact]
        public void TestValidateLoginCredentialsSuccess()
        {
            UserManagerService userManagerService = new UserManagerService();
            string username = "Eduar";
            string password = "Tecn0_C*nstr8";
            bool currentResult = false;

            currentResult = userManagerService.ValidateLoginCredentials(username, password);
            Assert.True(currentResult);
        }

        [Fact]
        public void TestValidateLoginCredentialsFail()
        {
            UserManagerService userManagerService = new UserManagerService();
            string username = "Eduar";
            string password = "MyPassword";
            bool currentResult = false;

            currentResult = userManagerService.ValidateLoginCredentials(username, password);
            Assert.False(currentResult);
        }

        [Fact]
        public void TestValidateUniqueIdentifierUserSuccess()
        {
            UserManagerService userManagerService = new UserManagerService();
            string username = "Eduar";
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

        public void TestValidateUniqueIdentifierUserEmailSuccess()
        {
            UserManagerService userManagerService = new UserManagerService();
            string username = "eduardo@gmail.com";
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

    }
}
