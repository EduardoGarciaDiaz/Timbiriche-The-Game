using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;
using Xunit;

namespace TimbiricheTests.Tests
{
    public class UserManagementTest
    {
        
        [Fact]
        public void TestAddUserSuccess()
        {
            var userManagement = new UserManagement();
            int expectedResult = 2;
            Accounts account = new Accounts
            {
                name = "Eduardo",
                lastName = "García",
                surname = "Díaz",
                birthdate = DateTime.Now
            };
            Players player = new Players
            {
                username = "edu",
                email = "ed@gmail.com",
                password = "Pa4sw*rd53C2r-e",
                Accounts = account
            };

            int currentResult = userManagement.AddUser(player);

            Assert.Equal(expectedResult, currentResult);
        }

        [Fact]
        public void TestAddUserFail()
        {
            var userManagement = new UserManagement();
            int expectedResult = -1;
            Accounts account = null;
            Players player = new Players
            {
                username = "Eduar",
                email = "eduardo@gmail.com",
                password = "Pa4sw*rd53C2r-e",
                Accounts = account
            };
            int currentResult = userManagement.AddUser(player);

            Assert.Equal(expectedResult, currentResult);
        }
        
        [Fact]
        public void TestValidateLoginCredentialsSuccess()
        {
            var userManagement = new UserManagement();
            var username = "ElRevo";
            var password = "E1_R3voTecN0";

            Players currentResult = userManagement.ValidateLoginCredentials(username, password);
            Assert.NotNull(currentResult);
        }

        [Fact]
        public void TestValidateLoginCredentialsFail()
        {
            var userManagement = new UserManagement();
            var username = "ElRevo";
            var password = "My_PassWord1N0RR325";
            
            Players currentResult = userManagement.ValidateLoginCredentials(username, password);
            Assert.Null(currentResult);
        }

        [Fact]
        public void TestExistUserIdenitifierByUsernameSuccess()
        {
            var userManagement = new UserManagement();
            var identifier = "Paco01";

            bool CurrentResult = userManagement.ExistUserIdenitifier(identifier);

            Assert.True(CurrentResult) ;
        }

        [Fact]
        public void TestExistUserIdenitifierByEmailSuccess()
        {
            var userManagement = new UserManagement();
            var identifier = "cap@gmail.com";

            bool CurrentResult = userManagement.ExistUserIdenitifier(identifier);

            Assert.True(CurrentResult);
        }

        [Fact]
        public void TestExistUserIdenitifierByUsernameFail()
        {
            var userManagement = new UserManagement();
            var identifier = "Eds165U17";

            bool CurrentResult = userManagement.ExistUserIdenitifier(identifier);

            Assert.False(CurrentResult);
        }

        [Fact]
        public void TestExistUserIdenitifierByEmailFail()
        {
            var userManagement = new UserManagement();
            var identifier = "eduardooduas8rde@gg.com";

            bool CurrentResult = userManagement.ExistUserIdenitifier(identifier);

            Assert.False(CurrentResult);
        }

    }

}
