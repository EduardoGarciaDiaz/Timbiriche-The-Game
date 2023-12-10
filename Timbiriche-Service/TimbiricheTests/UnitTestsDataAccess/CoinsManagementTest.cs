using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;
using TimbiricheService.Utils;
using Xunit;

namespace TimbiricheTests.UnitTestsDataAccess
{
    public class ConfigurationCoinsManagementTests : IDisposable
    {
        private ILogger _logger = LoggerManager.GetLogger();

        public ConfigurationCoinsManagementTests()
        {
            try
            {
                CreateTestUser();
            }
            catch (SqlException ex)
            {
                _logger.Error("SQLException on CoinsManagementTest: " + ex.Message);
            }
            catch (EntityException ex)
            {
                _logger.Error("Entity Exception on CoinsManagementTest: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected Exception on CoinsManagementTest: " + ex.Message + ": \n" + ex.StackTrace);
            }
        }

        private void CreateTestUser()
        {
            using (var context = new TimbiricheDBEntities())
            {
                var newAccountTest = CreateTestAccount(context);
                var newPlayerTest = CreateTestPlayer(context, newAccountTest);

                context.SaveChanges();
            }
        }

        private Accounts CreateTestAccount(TimbiricheDBEntities context)
        {
            var newAccountTest = context.Accounts.Add(new Accounts()
            {
                name = "JhonNameTestCoins",
                lastName = "JhonMercuryLastNameTestCoins",
                surname = "JhonLopezSurnameTestCoins",
                birthdate = DateTime.Now
            });

            return newAccountTest;
        }

        private Players CreateTestPlayer(TimbiricheDBEntities context, Accounts newAccountTest)
        {
            var newPlayerTest = context.Players.Add(new Players()
            {
                username = "JhonUsernameTest03",
                email = "jhonemailtest03@gmail.com",
                password = "My7_ScrT3Pa5s_W0r6",
                coins = 0,
                Accounts = newAccountTest
            });

            return newPlayerTest;
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
                _logger.Error("SQLException on CoinsManagementTest: " + ex.Message);

            }
            catch (EntityException ex)
            {
                _logger.Error("EntityException on CoinsManagementTest: " + ex.Message);
            }
        }

        private void DeletePlayersAndAccounts(TimbiricheDBEntities context)
        {
            var playerToDelete = context.Players
                .FirstOrDefault(p => p.username == "JhonUsernameTest03" && p.email == "jhonemailtest03@gmail.com");

            Accounts accountToDelete = context.Accounts.FirstOrDefault(a => a.name == "JhonNameTestCoins" 
            && a.lastName == "JhonMercuryLastNameTestCoins" && a.surname == "JhonLopezSurnameTestCoins");

            if (playerToDelete != null)
            {
                context.Players.Remove(playerToDelete);
            }

            if (accountToDelete != null)
            {
                context.Accounts.Remove(accountToDelete);
            }
        }
    }

    public class CoinsManagementTest : IClassFixture<ConfigurationCoinsManagementTests>
    {
        private readonly ConfigurationCoinsManagementTests _configuration;

        public CoinsManagementTest(ConfigurationCoinsManagementTests configuration)
        {
            _configuration = configuration;
        }

        [Fact]
        public void TestUpdateCoinsSuccess()
        {
            CoinsManagement coinsManagement = new CoinsManagement();
            int expectedResult = 1;

            int currentResult = coinsManagement.UpdateCoins("JhonUsernameTest03", 50);
            
            Assert.Equal(expectedResult, currentResult);
        }

        [Fact]
        public void TestUpdateCoinsFail()
        {
            CoinsManagement coinsManagement = new CoinsManagement();
            int expectedResult = -1;

            int currentResult = coinsManagement.UpdateCoins("InexistentUsername", 50);

            Assert.Equal(expectedResult, currentResult);
        }
    }
}
