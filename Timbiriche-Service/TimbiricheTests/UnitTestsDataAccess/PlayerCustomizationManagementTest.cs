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

    public class ConfigurationPlayerCustomizationManagementTests : IDisposable
    {
        private ILogger _logger = LoggerManager.GetLogger();

        public ConfigurationPlayerCustomizationManagementTests()
        {
            try
            {
                CreateTestUser();
                CreateColorsTest();
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
                name = "JhonNameTestCustomization",
                lastName = "JhonMercuryLastNameTestCustomization",
                surname = "JhonLopezSurnameTestCustomization",
                birthdate = DateTime.Now
            });

            return newAccountTest;
        }

        private Players CreateTestPlayer(TimbiricheDBEntities context, Accounts newAccountTest)
        {
            var newPlayerTest = context.Players.Add(new Players()
            {
                username = "JhonUsernameTest066",
                email = "jhonemailtest066@gmail.com",
                password = "My7_ScrT3Pa5s_W0r6",
                coins = 0,
                idStyleSelected = 1,
                idColorSelected = 1,
                Accounts = newAccountTest
            });

            return newPlayerTest;
        }

        private void CreateColorsTest()
        {
            using (var context = new TimbiricheDBEntities())
            {
                PlayerColors playerColor = new PlayerColors
                {
                    idPlayer = IdTestPlayer,
                    idColor = 1
                };
                
                context.PlayerColors.Add(playerColor);
                context.SaveChanges();
            }
        }

        public void Dispose()
        {
            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    DeletePlayersAndAccounts(context);
                    DeletePlayerColors(context);

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
                .FirstOrDefault(p => p.username == "JhonUsernameTest066" && p.email == "jhonemailtest066@gmail.com");

            Accounts accountToDelete = context.Accounts.FirstOrDefault(a => a.name == "JhonNameTestCustomization"
            && a.lastName == "JhonMercuryLastNameTestCustomization" && a.surname == "JhonLopezSurnameTestCustomization");

            if (playerToDelete != null)
            {
                context.Players.Remove(playerToDelete);
            }

            if (accountToDelete != null)
            {
                context.Accounts.Remove(accountToDelete);
            }
        }

        private void DeletePlayerColors(TimbiricheDBEntities context)
        {
            var playerColorToDelete = context.PlayerColors
                .FirstOrDefault(pc => pc.idColor == IdTestPlayer);

            if ( playerColorToDelete != null)
            {
                context.PlayerColors.Remove(playerColorToDelete);
            }
        }
    }

    public class PlayerCustomizationManagementTest : IClassFixture<ConfigurationPlayerCustomizationManagementTests>
    {
        private readonly ConfigurationPlayerCustomizationManagementTests _configuration;

        public PlayerCustomizationManagementTest(ConfigurationPlayerCustomizationManagementTests configuration)
        {
            _configuration = configuration;
        }

        [Fact]
        public void TestGetMyColorsByIdPlayerSuccess()
        {
            PlayerCustomizationManagement playerCustomizationManagement = new PlayerCustomizationManagement();
            List<PlayerColors> playerColors = playerCustomizationManagement.GetMyColorsByIdPlayer(_configuration.IdTestPlayer);

            Assert.NotNull(playerColors);
            Assert.True(playerColors.Count() > 0);
        }

        [Fact]
        public void TestGetMyColorsByIdPlayerFail()
        {
            PlayerCustomizationManagement playerCustomizationManagement = new PlayerCustomizationManagement();
            List<PlayerColors> playerColors = playerCustomizationManagement.GetMyColorsByIdPlayer(0);

            Assert.Empty(playerColors);
        }

        [Fact]
        public void TestGetHexadecimalColorByIdColorSuccess()
        {
            PlayerCustomizationManagement playerCustomizationManagement = new PlayerCustomizationManagement();
            string expectedResult = "#E46161";

            string currentResult = playerCustomizationManagement.GetHexadecimalColorByIdColor(1);

            Assert.NotNull(currentResult);
            Assert.Equal(expectedResult, currentResult);
        }

        [Fact]
        public void TestGetHexadecimalColorByIdColorFail()
        {
            PlayerCustomizationManagement playerCustomizationManagement = new PlayerCustomizationManagement();

            string currentResult = playerCustomizationManagement.GetHexadecimalColorByIdColor(0);

            Assert.Null(currentResult);
        }

        [Fact]
        public void TestUpdateMyColorSelectedSuccess()
        {
            PlayerCustomizationManagement playerCustomizationManagement = new PlayerCustomizationManagement();
            int extectedResult = 1;

            int currentResult = playerCustomizationManagement.UpdateMyColorSelected(_configuration.IdTestPlayer, 2);

            Assert.Equal(extectedResult, currentResult);
        }

        [Fact]
        public void TestUpdateMyColorSelectedFail()
        {
            PlayerCustomizationManagement playerCustomizationManagement = new PlayerCustomizationManagement();
            int extectedResult = -1;

            int currentResult = playerCustomizationManagement.UpdateMyColorSelected(0, 2);

            Assert.Equal(extectedResult, currentResult);
        }

        [Fact]
        public void TestSearchInMyColorsSuccess()
        {
            PlayerCustomizationManagement playerCustomizationManagement = new PlayerCustomizationManagement();

            bool currentResult = playerCustomizationManagement.SearchInMyColors(_configuration.IdTestPlayer, 1);

            Assert.True(currentResult);
        }

        [Fact]
        public void TestSearchInMyColorsFail()
        {
            PlayerCustomizationManagement playerCustomizationManagement = new PlayerCustomizationManagement();

            bool currentResult = playerCustomizationManagement.SearchInMyColors(0, 3);

            Assert.False(currentResult);
        }

        [Fact]
        public void TestGetMyStylesByIdPlayerSuccess()
        {
            PlayerCustomizationManagement playerCustomizationManagement = new PlayerCustomizationManagement();
            List<PlayerStyles> playerStyles = playerCustomizationManagement.GetMyStylesByIdPlayer(_configuration.IdTestPlayer);

            Assert.NotNull(playerStyles);
            //Assert.True(playerColors.Count() > 0);
        }

        [Fact]
        public void TestGetMyStylesByIdPlayerFail()
        {
            PlayerCustomizationManagement playerCustomizationManagement = new PlayerCustomizationManagement();
            List<PlayerStyles> playerStyles = playerCustomizationManagement.GetMyStylesByIdPlayer(0);

            Assert.Empty(playerStyles);
        }

        [Fact]
        public void TestGetStylePathByIdStyleSuccess()
        {
            PlayerCustomizationManagement playerCustomizationManagement = new PlayerCustomizationManagement();
            string expectedResult = "../../Resources/Skins/basicBox.png";

            string currentResult = playerCustomizationManagement.GetStylePathByIdStyle(2);

            Assert.Equal(expectedResult, currentResult);
        }

        [Fact]
        public void TestGetStylePathByIdStylefail()
        {
            PlayerCustomizationManagement playerCustomizationManagement = new PlayerCustomizationManagement();
            string currentResult = playerCustomizationManagement.GetStylePathByIdStyle(-1);

            Assert.Null(currentResult);
        }

        [Fact]
        public void TestUpdateMyStyleSelectedSuccess()
        {
            PlayerCustomizationManagement playerCustomizationManagement = new PlayerCustomizationManagement();
            int expectedResult = 1;

            int currentResult = playerCustomizationManagement.UpdateMyStyleSelected(_configuration.IdTestPlayer, 2);

            Assert.Equal(expectedResult, currentResult);
        }

        [Fact]
        public void TestUpdateMyStyleSelectedFail()
        {
            PlayerCustomizationManagement playerCustomizationManagement = new PlayerCustomizationManagement();
            int expectedResult = -1;

            int currentResult = playerCustomizationManagement.UpdateMyStyleSelected(0, 0);

            Assert.Equal(expectedResult, currentResult);
        }
    }
}
