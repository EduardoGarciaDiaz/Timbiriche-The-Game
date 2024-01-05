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
    public class ConfigurationShopManagementTests : IDisposable
    {
        private ILogger _logger = LoggerManager.GetLogger();

        public ConfigurationShopManagementTests()
        {
            try
            {
                CreateTestUser();

                AddPlayerColor(IdTestPlayer);
                AddPlayerStyle(IdTestPlayer);

                AddPlayerColor(IdTestPlayerColorStyles);
                AddPlayerStyle(IdTestPlayerColorStyles);
            }
            catch (SqlException ex)
            {
                _logger.Error("SQLException on ShopManagementTest: " + ex.Message);
            }
            catch (EntityException ex)
            {
                _logger.Error("Entity Exception on ShopManagementTest: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected Exception on ShopManagementTest: " + ex.Message + ": \n" + ex.StackTrace);
            }
        }

        public int IdTestPlayer { get; set; }
        public int IdTestPlayerColorStyles { get; set; }

        private void CreateTestUser()
        {
            using (var context = new TimbiricheDBEntities())
            {
                var newAccountTest = CreateTestAccount(context, "JhonNameTest2000");
                var newPlayerTest = CreateTestPlayer(context, newAccountTest, "JhonUsernameTest2000");
                UpdatePasswordAndSalt(newPlayerTest);

                var newAccountTestPlayerColorStyles = CreateTestAccount(context, "JhonNameTest2002");
                var newPlayerTestPlayerColorStyles = CreateTestPlayer(context, newAccountTestPlayerColorStyles, "JhonUsernameTest2002");
                UpdatePasswordAndSalt(newPlayerTestPlayerColorStyles);

                context.SaveChanges();

                IdTestPlayer = newPlayerTest.idPlayer;
                IdTestPlayerColorStyles = newPlayerTestPlayerColorStyles.idPlayer;
            }
        }

        private Accounts CreateTestAccount(TimbiricheDBEntities context, string name)
        {
            var newAccountTest = context.Accounts.Add(new Accounts()
            {
                name = name,
                lastName = "JhonMercuryLastNameTest",
                surname = "JhonLopezSurnameTest",
                birthdate = DateTime.Now
            });

            return newAccountTest;
        }

        private Players CreateTestPlayer(TimbiricheDBEntities context, Accounts newAccountTest, string username)
        {
            var newPlayerTest = context.Players.Add(new Players()
            {
                username = username,
                email = "jhonemailtest2000@gmail.com",
                password = "My7_ScrT3Pa5s_W0r6",
                coins = 20,
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

        private void AddPlayerColor(int idPlayer)
        {
            using (var context = new TimbiricheDBEntities())
            {
                PlayerColors playerColor = new PlayerColors();
                playerColor.idColor = 1;
                playerColor.idPlayer = idPlayer;

                context.PlayerColors.Add(playerColor);

                context.SaveChanges();
            }
        }

        private void AddPlayerStyle(int idPlayer)
        {
            using (var context = new TimbiricheDBEntities())
            {
                PlayerStyles playerStyle = new PlayerStyles();
                playerStyle.idStyle = 1;
                playerStyle.idPlayer = idPlayer;

                context.PlayerStyles.Add(playerStyle);

                context.SaveChanges();
            }
        }

        public void Dispose()
        {
            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    DeletePlayerColor(context);
                    DeletePlayerStyle(context);
                    DeletePlayersAndAccounts(context);

                    context.SaveChanges();
                }
            }
            catch (SqlException ex)
            {
                _logger.Error("SQLException on ShopManagementTest: " + ex.Message);

            }
            catch (EntityException ex)
            {
                _logger.Error("EntityException on ShopManagementTest: " + ex.Message);
            }
        }

        private void DeletePlayersAndAccounts(TimbiricheDBEntities context)
        {
            Players testPlayer = context.Players.Find(IdTestPlayer);
            Players testPlayerColorStyles = context.Players.Find(IdTestPlayerColorStyles);

            context.Players.Remove(testPlayer);
            context.Players.Remove(testPlayerColorStyles);

            Accounts testAccount = context.Accounts.FirstOrDefault(a => a.name == "JhonNameTest2000");
            Accounts testAccountColorStyles = context.Accounts.FirstOrDefault(a => a.name == "JhonNameTest2002");

            context.Accounts.Remove(testAccount);
            context.Accounts.Remove(testAccountColorStyles);
        }

        private void DeletePlayerColor(TimbiricheDBEntities context)
        {
            var playerColorToDelete = context.PlayerColors.Where(pc => pc.idPlayer == IdTestPlayer || pc.idPlayer == IdTestPlayerColorStyles);

            context.PlayerColors.RemoveRange(playerColorToDelete);
        }

        private void DeletePlayerStyle(TimbiricheDBEntities context)
        {
            var playerStyleToDelete = context.PlayerStyles.Where(ps => ps.idPlayer == IdTestPlayer || ps.idPlayer == IdTestPlayerColorStyles);

            context.PlayerStyles.RemoveRange(playerStyleToDelete);
        }
    }

    public class ShopManagementTest : IClassFixture<ConfigurationShopManagementTests>
    {
        private readonly ConfigurationShopManagementTests _configuration;

        public ShopManagementTest(ConfigurationShopManagementTests configuration)
        {
            _configuration = configuration;
        }

        [Fact]
        public void TestGetColorsSuccess()
        {
            List<Colors> colors = ShopManagement.GetColors();

            Assert.True(colors.Count > 0);
        }

        [Fact]
        public void TestGetPlayerColorsSuccess()
        {
            int playerId = _configuration.IdTestPlayerColorStyles;

            List<PlayerColors> playerColors = ShopManagement.GetPlayerColors(playerId);

            Assert.True(playerColors.Count == 1);
        }

        [Fact]
        public void TestGetPlayerColorsFail()
        {
            int invalidPlayerId = -1;

            List<PlayerColors> playerColors = ShopManagement.GetPlayerColors(invalidPlayerId);

            Assert.True(playerColors.Count == 0);
        }

        [Fact]
        public void TestGetStylesSuccess()
        {
            var styles = ShopManagement.GetStyles();

            Assert.True(styles.Count > 0);
        }

        [Fact]
        public void TestGetPlayerStylesSuccess()
        {
            int validPlayerId = _configuration.IdTestPlayerColorStyles;

            List<PlayerStyles> playerStyles = ShopManagement.GetPlayerStyles(validPlayerId);

            Assert.True(playerStyles.Count == 1);
        }

        [Fact]
        public void TestGetPlayerStylesFail()
        {
            int invalidPlayerId = -1;

            List <PlayerStyles> playerStyles = ShopManagement.GetPlayerStyles(invalidPlayerId);

            Assert.True(playerStyles.Count == 0);
        }

        [Fact]
        public void TestBuyColorSuccess()
        {
            int validColorId = 2;
            int validPlayerId = _configuration.IdTestPlayer;

            bool result = ShopManagement.BuyColor(validColorId, validPlayerId);

            Assert.True(result);
        }

        [Fact]
        public void TestBuyColorFail()
        {
            int invalidColorId = -1;
            int validPlayerId = _configuration.IdTestPlayer;

            bool result = ShopManagement.BuyColor(invalidColorId, validPlayerId);

            Assert.False(result);
        }

        [Fact]
        public void TestBuyStyleSuccess()
        {
            int validStyleId = 2;
            int validPlayerId = _configuration.IdTestPlayer;

            bool result = ShopManagement.BuyStyle(validStyleId, validPlayerId);

            Assert.True(result);
        }

        [Fact]
        public void TestBuyStyleFail()
        {
            int invalidStyleId = -1;
            int validPlayerId = _configuration.IdTestPlayer;

            bool result = ShopManagement.BuyStyle(invalidStyleId, validPlayerId);

            Assert.False(result);
        }

        [Fact]
        public void TestSubstractPlayerCoinsSuccess()
        {
            int coinsToSubtract = 10;
            int validPlayerId = _configuration.IdTestPlayer;

            bool result = ShopManagement.SubstractPlayerCoins(validPlayerId, coinsToSubtract);

            Assert.True(result);
        }

        [Fact]
        public void TestSubstractPlayerCoinsFail()
        {
            int coinsToSubtract = 30;
            int validPlayerId = _configuration.IdTestPlayer;

            var result = ShopManagement.SubstractPlayerCoins(validPlayerId, coinsToSubtract);

            Assert.False(result);
        }
    }
}
