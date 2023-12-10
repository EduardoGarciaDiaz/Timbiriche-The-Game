using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;
using TimbiricheService;
using TimbiricheService.Utils;
using Xunit;

namespace TimbiricheTests.UnitTestsDataAccess
{
    public class ConfigurationGlobalScoreTests : IDisposable
    {
        private ILogger _logger = LoggerManager.GetLogger();

        public ConfigurationGlobalScoreTests()
        {
            try
            {
                CreateTestUser();
                AddToGlobalScore();
            }
            catch (SqlException ex)
            {
                _logger.Error("SQLException on GlobalScoreManagementTest: " + ex.Message);
            }
            catch (EntityException ex)
            {
                _logger.Error("Entity Exception on GlobalScoreManagementTest: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected Exception on GlobalScoreManagementTest: " + ex.Message + ": \n" + ex.StackTrace);
            }
        }

        public int IdTestPlayer { get; set; }

        private void CreateTestUser()
        {
            using (var context = new TimbiricheDBEntities())
            {
                var newAccountTest = CreateTestAccount(context);
                var newPlayerTest = CreateTestPlayer(context, newAccountTest);

                context.SaveChanges();

                IdTestPlayer = newPlayerTest.idPlayer;
            }
        }

        private Accounts CreateTestAccount(TimbiricheDBEntities context)
        {
            var newAccountTest = context.Accounts.Add(new Accounts()
            {
                name = "JhonNameTestGlobalScore",
                lastName = "JhonMercuryLastNameTestGlobalScore",
                surname = "JhonLopezSurnameTestGlobalScore",
                birthdate = DateTime.Now
            });

            return newAccountTest;
        }

        private void AddToGlobalScore()
        {
            GlobalScores globalScore = new GlobalScores
            {
                idPlayer = IdTestPlayer,
                winsNumber = 0
            };

            using (var context = new TimbiricheDBEntities())
            {
                context.GlobalScores.Add(globalScore);

                context.SaveChanges();
            }
        }

        private Players CreateTestPlayer(TimbiricheDBEntities context, Accounts newAccountTest)
        {
            var newPlayerTest = context.Players.Add(new Players()
            {
                username = "JhonUsernameTest04",
                email = "jhonemailtest04@gmail.com",
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
                    DeleteGlobalScores(context);

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
                .FirstOrDefault(p => p.username == "JhonUsernameTest04" && p.email == "jhonemailtest04@gmail.com");

            Accounts accountToDelete = context.Accounts.FirstOrDefault(a => a.name == "JhonNameTestGlobalScore" 
            && a.lastName == "JhonMercuryLastNameTestGlobalScore" && a.surname == "JhonLopezSurnameTestGlobalScore");

            if (playerToDelete != null)
            {
                context.Players.Remove(playerToDelete);
            }

            if (accountToDelete != null)
            {
                context.Accounts.Remove(accountToDelete);
            }
        }

        private void DeleteGlobalScores(TimbiricheDBEntities context)
        {
            var globalScoreToDelete = context.GlobalScores
                .FirstOrDefault(gs => gs.idPlayer == IdTestPlayer);

            if (globalScoreToDelete != null)
            {
                context.GlobalScores.Remove(globalScoreToDelete);
            }
        }
    }

    public class GlobalScoreManagementTest : IClassFixture<ConfigurationGlobalScoreTests>
    {
        private readonly ConfigurationGlobalScoreTests _configuration;

        public GlobalScoreManagementTest(ConfigurationGlobalScoreTests configuration)
        {
            _configuration = configuration;
        }

        [Fact]
        public void TestGetGlobalScoresSuccess()
        {
            GlobalScoresManagement globalScoresManagement = new GlobalScoresManagement();
            List<GlobalScores> globalScores = globalScoresManagement.GetGlobalScores();

            Assert.NotNull(globalScores);
        }

        [Fact]
        public void TestUpdateWinsPlayerSuccess()
        {
            GlobalScoresManagement globalScoresManagement = new GlobalScoresManagement();
            int expectedResult = 1;

            int idPlayer = _configuration.IdTestPlayer;
            int currentResult = globalScoresManagement.UpdateWinsPlayer(idPlayer);

            Assert.Equal(expectedResult, currentResult);
        }

        [Fact]
        public void TestUpdateWinsPlayerFail()
        {
            GlobalScoresManagement globalScoresManagement = new GlobalScoresManagement();
            int expectedResult = -1;

            int currentResult = globalScoresManagement.UpdateWinsPlayer(0);

            Assert.Equal(expectedResult, currentResult);
        }
    }
}
