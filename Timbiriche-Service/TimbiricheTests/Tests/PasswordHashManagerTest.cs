using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess.Utils;
using Xunit;

namespace TimbiricheTests.Tests
{
    public class PasswordHashManagerTest
    {

        [Fact]
        public void TestVerifyPasswordSuccess()
        {
            string password = "Hol1?asyu81l";
            PasswordHashManager passwordHashManager = new PasswordHashManager();
            string hashedPassword = passwordHashManager.HashPassword(password);

            bool result = passwordHashManager.VerifyPassword(password, hashedPassword);

            Assert.True(result);
        }

        [Fact]
        public void TestVerifyPasswordFail()
        {
            string correctPasword = "Hol1?asyu81l";
            string incorrectPassword = "imyoung78?12";
            PasswordHashManager passwordHashManager = new PasswordHashManager();
            string hashedPassword = passwordHashManager.HashPassword(correctPasword);

            bool result = passwordHashManager.VerifyPassword(incorrectPassword, hashedPassword);

            Assert.False(result);
        }

    }
}
