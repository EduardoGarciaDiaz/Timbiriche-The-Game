using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Utils;
using TimbiricheService.Utils;
using Xunit;


namespace TimbiricheTests.UnitTestsDataAccess
{
    public class ConfigurationShopManagementTests : IDisposable
    {
        private ILogger _logger = LoggerManager.GetLogger();

        public ConfigurationShopManagementTests()
        {
            try
            {
                CreateTestUser();
            }
            catch (SqlException ex)
            {
                _logger.Error("SQLException on UserManagementTest: " + ex.Message);
            }
            catch (EntityException ex)
            {
                _logger.Error("Entity Exception on UserManagementTest: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected Exception on UserManagementTest: " + ex.Message + ": \n" + ex.StackTrace);
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
                name = "JhonNameTest",
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
                username = "JhonUsernameTest02",
                email = "jhonemailtest@gmail.com",
                password = "My7_ScrT3Pa5s_W0r6",
                Accounts = newAccountTest
            });

            return newPlayerTest;
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
                    DeletePlayersAndAccounts(context);

                    context.SaveChanges();
                }
            }
            catch (SqlException ex)
            {
                _logger.Error("SQLException on UserManagementTest: " + ex.Message);

            }
            catch (EntityException ex)
            {
                _logger.Error("EntityException on UserManagementTest: " + ex.Message);
            }
        }

        private void DeletePlayersAndAccounts(TimbiricheDBEntities context)
        {
            var usernamesToDelete = new List<string> { "UsernameTest01", "JhonUsernameTest02" };
            var emailsToDelete = new List<string> { "emailTimbiricheTest01@gmail.com", "jhonemailtest@gmail.com" };

            var playersToDelete = context.Players
                .Where(p => usernamesToDelete.Contains(p.username) && emailsToDelete.Contains(p.email))
                .ToList();

            Accounts accountToDelete = context.Accounts.FirstOrDefault(a => a.name == "NameTestX0101"
            && a.lastName == "LastNameTestX0101" && a.surname == "SurnameTestX0101");


            Accounts generalAccountToDelete = context.Accounts.FirstOrDefault(a => a.name == "JhonNameTest"
            && a.lastName == "JhonMercuryLastNameTest" && a.surname == "JhonLopezSurnameTest");

            context.Players.RemoveRange(playersToDelete);
            if (accountToDelete != null)
            {
                context.Accounts.Remove(accountToDelete);
            }
            if (generalAccountToDelete != null)
            {
                context.Accounts.Remove(generalAccountToDelete);
            }
        }
    }

    public class ShopManagementTest
    {
        [Fact]
        public void TestGetColorsSuccess()
        {
            List<Colors> colors = ShopManagement.GetColors();

            Assert.NotNull(colors);
            Assert.True(colors.Count > 0);
        }

        [Fact]
        public void TestGetPlayerColorsSuccess()
        {
            // Arrange
            int validPlayerId = 1;

            // Act
            var playerColors = ShopManagement.GetPlayerColors(validPlayerId);

            // Assert
            Assert.NotNull(playerColors);
            Assert.True(playerColors.Count >= 0); // Modify this based on your business logic
        }
    }
}
