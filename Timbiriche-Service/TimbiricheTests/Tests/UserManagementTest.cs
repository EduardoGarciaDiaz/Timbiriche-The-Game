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
        public void TestAddUserSuccessFail()
        {
            var userManagement = new UserManagement();
            int expectedResult = -1;
            Accounts account = null;
            Players player = null;

            int currentResult = userManagement.AddUser(player);

            Assert.Equal(expectedResult, currentResult);
        }
        
        [Fact]
        public void TestValidateLoginCredentialsSuccess()
        {
            var userManagement = new UserManagement();
            var username = "Eduar";
            var password = "Tecn0_C*nstr8";

            bool CurrentResult = userManagement.ValidateLoginCredentials(username, password);
           
            Assert.True(CurrentResult);
        }

        [Fact]
        public void TestValidateLoginCredentialsFail()
        {
            var userManagement = new UserManagement();
            var username = "Eduar";
            var password = "password";

            bool CurrentResult = userManagement.ValidateLoginCredentials(username, password);
            
            Assert.False(CurrentResult);
        }

        [Fact]
        public void TestExistUserIdenitifierByUsernameSuccess()
        {
            var userManagement = new UserManagement();
            var identifier = "Eduar";

            bool CurrentResult = userManagement.ExistUserIdenitifier(identifier);

            Assert.True(CurrentResult) ;
        }

        [Fact]
        public void TestExistUserIdenitifierByEmailSuccess()
        {
            var userManagement = new UserManagement();
            var identifier = "eduardo@gmail.com";

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
