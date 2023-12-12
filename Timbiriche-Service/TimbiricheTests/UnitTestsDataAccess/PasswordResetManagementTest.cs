using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Utils;
using Xunit;

namespace TimbiricheTests.UnitTestsDataAccess
{
    public class ConfigurationPasswordResetManagementTests : IDisposable
    {
        private ILogger _logger = LoggerManager.GetLogger();

        public ConfigurationPasswordResetManagementTests()
        {
            try
            {
                CreateTestUser();
                AddToken();
            }
            catch (SqlException ex)
            {
                _logger.Error("SQLException on PasswordResetManagementTest: " + ex.Message);
            }
            catch (EntityException ex)
            {
                _logger.Error("Entity Exception on PasswordResetManagementTest: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected Exception on PasswordResetManagementTest: " + ex.Message + ": \n" + ex.StackTrace);
            }
        }

        public int IdTestPlayer { get; set; }

        private void CreateTestUser()
        {
            using (var context = new TimbiricheDBEntities())
            {
                var newAccountTest = CreateTestAccount(context);
                var newPlayerTest = CreateTestPlayer(context, newAccountTest);
                UpdatePasswordAndSalt(newPlayerTest);

                context.SaveChanges();
                IdTestPlayer = newPlayerTest.idPlayer;
            }
        }

        private Accounts CreateTestAccount(TimbiricheDBEntities context)
        {
            var newAccountTest = context.Accounts.Add(new Accounts()
            {
                name = "JhonNameTest100",
                lastName = "JhonMercuryLastNameTest",
                surname = "JhonLopezSurnameTest",
                birthdate = DateTime.Now
            });

            return newAccountTest;
        }

        private Players CreateTestPlayer(TimbiricheDBEntities context, Accounts newAccountTest)
        {
            var newPlayerTest = context.Players.Add(new Players()
            {
                username = "JhonNameTest100",
                email = "jhonemailtest100@gmail.com",
                password = "My7_ScrT3Pa5s_W0r6",
                coins = 20,
                Accounts = newAccountTest
            });

            return newPlayerTest;
        }

        private void AddToken()
        {
            DateTime creationDateTime = DateTime.Now;
            DateTime expirationDateTime = creationDateTime.AddMinutes(5);

            PasswordResetTokens passwordResetToken = new PasswordResetTokens
            {
                idPlayer = IdTestPlayer,
                token = 7891011,
                creationDate = creationDateTime,
                expirationDate = expirationDateTime
            };

            bool result = PasswordResetManagement.AddToken(passwordResetToken);
        }

        private void UpdatePasswordAndSalt(Players newPlayerTest)
        {
            PasswordHashManager passwordHashManager = new PasswordHashManager();
            newPlayerTest.password = passwordHashManager.HashPassword(newPlayerTest.password);
            newPlayerTest.salt = passwordHashManager.Salt;
        }

        public void Dispose()
        {
            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    DeleteTokens(context);
                    DeletePlayersAndAccounts(context);

                    context.SaveChanges();
                }
            }
            catch (SqlException ex)
            {
                _logger.Error("SQLException on PasswordResetManagementTest: " + ex.Message);

            }
            catch (EntityException ex)
            {
                _logger.Error("EntityException on PasswordResetManagementTest: " + ex.Message);
            }
        }

        private void DeletePlayersAndAccounts(TimbiricheDBEntities context)
        {
            var usernamesToDelete = new List<string> { "JhonNameTest100" };
            var emailsToDelete = new List<string> { "jhonemailtest100@gmail.com" };

            var playersToDelete = context.Players
                .Where(p => usernamesToDelete.Contains(p.username) && emailsToDelete.Contains(p.email))
                .ToList();

            Accounts generalAccountToDelete = context.Accounts.FirstOrDefault(a => a.name == "JhonNameTest100"
            && a.lastName == "JhonMercuryLastNameTest" && a.surname == "JhonLopezSurnameTest");

            context.Players.RemoveRange(playersToDelete);

            if (generalAccountToDelete != null)
            {
                context.Accounts.Remove(generalAccountToDelete);
            }
        }

        private void DeleteTokens(TimbiricheDBEntities context)
        {
            var passwordResetToken = context.PasswordResetTokens
                .Where(p => p.idPlayer == IdTestPlayer)
                .ToList();

            if(passwordResetToken != null)
            {
                context.PasswordResetTokens.RemoveRange(passwordResetToken);
            }
        }
    }

    public class PasswordResetManagementTest : IClassFixture<ConfigurationPasswordResetManagementTests>
    {
        private readonly ConfigurationPasswordResetManagementTests _configuration;

        public PasswordResetManagementTest(ConfigurationPasswordResetManagementTests configuration)
        {
            _configuration = configuration;
        }

        [Fact]
        public void TestAddTokenSuccess()
        {
            DateTime creationDateTime = DateTime.Now;
            DateTime expirationDateTime = creationDateTime.AddMinutes(5);

            PasswordResetTokens passwordResetToken = new PasswordResetTokens
            {
                idPlayer = _configuration.IdTestPlayer,
                token = 123456,
                creationDate = creationDateTime,
                expirationDate = expirationDateTime
            };

            bool result = PasswordResetManagement.AddToken(passwordResetToken);

            Assert.True(result);
        }

        [Fact]
        public void TestAddTokenFail()
        {
            PasswordResetTokens passwordResetToken = null;

            bool result = PasswordResetManagement.AddToken(passwordResetToken);

            Assert.False(result);
        }

        [Fact]
        public void TestGetPasswordResetTokenByIdPlayerAndTokenSuccess()
        {
            int validPlayerId = _configuration.IdTestPlayer;
            int validToken = 7891011;

            var result = PasswordResetManagement.GetPasswordResetTokenByIdPlayerAndToken(validPlayerId, validToken);

            Assert.NotNull(result);
        }

        [Fact]
        public void TestGetPasswordResetTokenByIdPlayerAndTokenFail()
        {
            int validPlayerId = -1;
            int validToken = 78910112;

            var result = PasswordResetManagement.GetPasswordResetTokenByIdPlayerAndToken(validPlayerId, validToken);

            Assert.Null(result);
        }

        [Fact]
        public void TestChangePasswordByIdSuccess()
        {
            int validPlayerId = _configuration.IdTestPlayer;
            string newPassword = "1My_ScrPa5s_W0r234";

            var result = PasswordResetManagement.ChangePasswordById(validPlayerId, newPassword);

            Assert.True(result);
        }

        [Fact]
        public void TestChangePasswordByIdFail()
        {
            int validPlayerId = -1;
            string newPassword = "1My_ScrPa5s_W0r234";

            var result = PasswordResetManagement.ChangePasswordById(validPlayerId, newPassword);

            Assert.False(result);
        }
    }
}
