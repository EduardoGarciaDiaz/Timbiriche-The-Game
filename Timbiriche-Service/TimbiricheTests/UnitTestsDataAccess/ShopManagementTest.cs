using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                AddPlayerColor();
                AddPlayerStyle();
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
                name = "JhonNameTest2000",
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
                username = "JhonUsernameTest2000",
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

        private void AddPlayerColor()
        {
            using (var context = new TimbiricheDBEntities())
            {
                PlayerColors playerColor = new PlayerColors();
                playerColor.idColor = 1;
                playerColor.idPlayer = IdTestPlayer;

                context.PlayerColors.Add(playerColor);

                context.SaveChanges();
            }
        }

        private void AddPlayerStyle()
        {
            using (var context = new TimbiricheDBEntities())
            {
                PlayerStyles playerStyle = new PlayerStyles();
                playerStyle.idStyle = 1;
                playerStyle.idPlayer = IdTestPlayer;

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
            var usernamesToDelete = new List<string> { "JhonUsernameTest2000" };
            var emailsToDelete = new List<string> {"jhonemailtest2000@gmail.com" };

            var playersToDelete = context.Players
                .Where(p => usernamesToDelete.Contains(p.username) && emailsToDelete.Contains(p.email))
                .ToList();

            Accounts generalAccountToDelete = context.Accounts.FirstOrDefault(a => a.name == "JhonNameTest2000"
            && a.lastName == "JhonMercuryLastNameTest" && a.surname == "JhonLopezSurnameTest");

            if (generalAccountToDelete != null)
            {
                context.Accounts.Remove(generalAccountToDelete);
            }
        }

        private void DeletePlayerColor(TimbiricheDBEntities context)
        {
            var playerColorToDelete = context.PlayerColors.Where(pc => pc.idPlayer == IdTestPlayer);

            context.PlayerColors.RemoveRange(playerColorToDelete);

            context.SaveChanges();
        }

        private void DeletePlayerStyle(TimbiricheDBEntities context)
        {
            var playerStyleToDelete = context.PlayerStyles.Where(ps => ps.idPlayer == IdTestPlayer);

            context.PlayerStyles.RemoveRange(playerStyleToDelete);

            context.SaveChanges();
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
            int playerId = _configuration.IdTestPlayer;

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
            int validPlayerId = _configuration.IdTestPlayer;

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
