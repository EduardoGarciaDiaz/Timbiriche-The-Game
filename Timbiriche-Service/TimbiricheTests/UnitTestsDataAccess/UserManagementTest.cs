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
    public class ConfigurationUserManagementTests : IDisposable
    {
        private ILogger _logger = LoggerManager.GetLogger();

        public ConfigurationUserManagementTests()
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
                    DeletePlayerStyles(context);
                    DeletePlayerColors(context);
                    DeleteGlobalScores(context);

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

            Accounts accountUpdatedToDelete = context.Accounts.FirstOrDefault(a => a.name == "JhonNameTestHAVETODELETE");

            context.Players.RemoveRange(playersToDelete);
            if (accountToDelete != null)
            {
                context.Accounts.Remove(accountToDelete);
            }
            if (generalAccountToDelete != null)
            {
                context.Accounts.Remove(generalAccountToDelete);
            }
            if (accountUpdatedToDelete != null)
            {
                context.Accounts.Remove(accountUpdatedToDelete);
            }
        }

        private void DeletePlayerStyles(TimbiricheDBEntities context)
        {
            var userManagement = new UserManagement();
            var idPlayer = userManagement.GetIdPlayerByUsername("JhonUsernameTest02");

            var playerStylesToDelete = context.PlayerStyles
                .Where(ps => ps.idPlayer == idPlayer)
                .ToList();

            context.PlayerStyles.RemoveRange(playerStylesToDelete);
        }

        private void DeletePlayerColors(TimbiricheDBEntities context)
        {
            var userManagement = new UserManagement();
            var idPlayerColors = userManagement.GetIdPlayerByUsername("JhonUsernameTest02");

            var playerColorsToDelete = context.PlayerColors
                .Where(pc => pc.idPlayer == idPlayerColors)
                .ToList();

            context.PlayerColors.RemoveRange(playerColorsToDelete);
        }

        private void DeleteGlobalScores(TimbiricheDBEntities context)
        {
            var userManagement = new UserManagement();
            var idGlobalScores = userManagement.GetIdPlayerByUsername("JhonUsernameTest02");

            var globalScoresToDelete = context.GlobalScores
                .Where(gs => gs.idPlayer == idGlobalScores)
                .ToList();

            context.GlobalScores.RemoveRange(globalScoresToDelete);
        }
    }

    public class UserManagementTest : IClassFixture<ConfigurationUserManagementTests>
    {
        private readonly ConfigurationUserManagementTests _configuration;

        public UserManagementTest(ConfigurationUserManagementTests configuration)
        {
            _configuration = configuration;
        }

        [Fact]
        public void TestAddUserSuccess()
        {
            UserManagement userManagement = new UserManagement();
            int expectedResult = 2;
            Accounts account = new Accounts
            {
                name = "NameTestX0101",
                lastName = "LastNameTestX0101",
                surname = "SurnameTestX0101",
                birthdate = DateTime.Now
            };
            Players player = new Players
            {
                username = "UsernameTest01",
                email = "emailTimbiricheTest01@gmail.com",
                password = "Pa4sw*rd53C2r-e",
                Accounts = account
            };

            int currentResult = userManagement.AddUser(player);

            Assert.Equal(expectedResult, currentResult);
        }
        
        [Fact]
        public void TestAddUserFail()
        {
            UserManagement userManagement = new UserManagement();
            int expectedResult = -1;
            Accounts account = null;
            Players player = new Players
            {
                username = "EduarTest01",
                email = "emailTest01@gmail.com",
                password = "Pa4sw*rd53C2r-e",
                Accounts = account
            };

            int currentResult = userManagement.AddUser(player);

            Assert.Equal(expectedResult, currentResult);
        }
        
        [Fact]
        public void TestValidateLoginCredentialsSuccess()
        {
            UserManagement userManagement = new UserManagement();
            var username = "JhonUsernameTest02";
            var password = "My7_ScrT3Pa5s_W0r6";

            Players currentResult = userManagement.ValidateLoginCredentials(username, password);
            
            Assert.NotNull(currentResult);
        }

        
        [Theory]
        [InlineData ("JhonUsernameTest02", "wrongPassword")]
        [InlineData("wrongUsername", "My7_ScrT3Pa5s_W0r6")]
        [InlineData("", "")]
        [InlineData("bouthWrong", "bouthWrong2")]
        public void TestValidateLoginCredentialsFail(string username, string password)
        {
            UserManagement userManagement = new UserManagement();
            Players currentResult = userManagement.ValidateLoginCredentials(username, password);
            
            Assert.Null(currentResult);
        }
        
        [Theory]
        [InlineData("JhonUsernameTest02")]
        [InlineData("jhonemailtest@gmail.com")]
        public void TestExistUserIdenitifierSuccess(string userIdentifier)
        {
            UserManagement userManagement = new UserManagement();
            bool CurrentResult = userManagement.ExistUserIdenitifier(userIdentifier);

            Assert.True(CurrentResult);
        }

        [Theory]
        [InlineData("Eds165U17")]
        [InlineData("")]
        [InlineData("inexistentemail@gmail.com")]
        public void TestExistUserIdenitifierFail(string userIdentifier)
        {
            UserManagement userManagement = new UserManagement();
            bool CurrentResult = userManagement.ExistUserIdenitifier(userIdentifier);

            Assert.False(CurrentResult);
        }

        [Fact]
        public void TestAddPlayerStylesSuccess()
        {
            UserManagement userManagement = new UserManagement();
            int expectedResult = 1;
            PlayerStyles playerStyle = new PlayerStyles();

            int idPlayer = _configuration.IdTestPlayer;
            playerStyle.idStyle = 1;
            playerStyle.idPlayer = idPlayer;

            int currentResult = userManagement.AddPlayerStyles(playerStyle);

            Assert.Equal(expectedResult, currentResult);
        }

        [Fact]
        public void TestAddPlayerStylesFail()
        {
            UserManagement userManagement = new UserManagement();
            int expectedResult = -1;

            PlayerStyles playerStyle = null;
            int currentResult = userManagement.AddPlayerStyles(playerStyle);

            Assert.Equal(expectedResult, currentResult);
        }

        [Fact]
        public void TestAddPlayerColorsSuccess()
        {
            UserManagement userManagement = new UserManagement();
            int expectedResult = 1;

            PlayerColors playerColor = new PlayerColors();
            int idPlayer = _configuration.IdTestPlayer;
            playerColor.idColor = 1;
            playerColor.idPlayer = idPlayer;

            int currentResult = userManagement.AddPlayerColors(playerColor);

            Assert.Equal(expectedResult, currentResult);
        }

        [Fact]
        public void TestAddPlayerColorsFail()
        {
            PlayerColors playerColor = null;
            int expectedResult = -1;

            UserManagement userManagement = new UserManagement();
            int currentResult = userManagement.AddPlayerColors(playerColor);

            Assert.Equal(expectedResult, currentResult);
        }

        
        [Fact]
        public void TestAddToGlobalScoreboardsSuccess()
        {
            UserManagement userManagement = new UserManagement();
            int expectedResult = 1;

            GlobalScores globalScore = new GlobalScores();
            int idPlayer = _configuration.IdTestPlayer;
            globalScore.idPlayer = idPlayer;
            globalScore.winsNumber = 0;

            int currentResult = userManagement.AddToGlobalScoreboards(globalScore);

            Assert.Equal(expectedResult, currentResult);
        }

        [Fact]
        public void TestAddToGlobalScoreboardsFail()
        {
            UserManagement userManagement = new UserManagement();
            int expectedResult = -1;
            GlobalScores globalScore = null;

            int currentResult = userManagement.AddToGlobalScoreboards(globalScore);

            Assert.Equal(expectedResult, currentResult);
        }

        [Fact]
        public void TestGetPlayerByIdPlayerSuccess()
        {
            UserManagement userManagement = new UserManagement();
            Players expectedResult = new Players{username = "JhonUsernameTest02"};
            int idPlayer = _configuration.IdTestPlayer;

            Players currentResult = userManagement.GetPlayerByIdPlayer(idPlayer);

            Assert.Equal(expectedResult.username, currentResult.username);
        }

        [Fact]
        public void TestGetPlayerByIdPlayerFail()
        {
            UserManagement userManagement = new UserManagement();
            Players currentResult = userManagement.GetPlayerByIdPlayer(0);

            Assert.Null(currentResult);
        }

        [Fact]
        public void TestGetIdPlayerByEmailSuccess()
        {
            UserManagement userManagement = new UserManagement();
            int expectedResult = 0;
            string userEmail = "jhonemailtest@gmail.com";

            int currentResult = userManagement.GetIdPlayerByEmail(userEmail);

            Assert.NotEqual(expectedResult, currentResult);
        }

        [Fact]
        public void TestGetIdPlayerByEmailFail()
        {
            UserManagement userManagement = new UserManagement();
            int expectedResult = 0;
            string userEmail = "inexistentemail01@gma3il.com";

            int currentResult = userManagement.GetIdPlayerByEmail(userEmail);

            Assert.Equal(expectedResult, currentResult);
        }

        [Fact]
        public void TestGetIdPlayerByUsernameSuccess()
        {
            UserManagement userManagement = new UserManagement();
            int expectedResult = 0;
            string usernmae = "JhonUsernameTest02";

            int currentResult = userManagement.GetIdPlayerByUsername(usernmae);

            Assert.NotEqual(expectedResult, currentResult);
        }

        [Fact]
        public void TestGetIdPlayerByUsernameFail()
        {
            UserManagement userManagement = new UserManagement();

            int expectedResult = 0;
            string userEmail = "inexistentUsername01-";

            int currentResult = userManagement.GetIdPlayerByUsername(userEmail);

            Assert.Equal(expectedResult, currentResult);
        }

        [Fact]
        public void TestGetUsernameByIdPlayerSuccess()
        {
            UserManagement userManagement = new UserManagement();
            int idPlayer = _configuration.IdTestPlayer;
            string expectedResult = "JhonUsernameTest02"; 

            string currentResult = userManagement.GetUsernameByIdPlayer(idPlayer);

            Assert.Equal(expectedResult, currentResult);
        }

        [Fact]
        public void TestGetUsernameByIdPlayerFail()
        {
            UserManagement userManagement = new UserManagement();
            string expectedResult = string.Empty;

            string currentResult = userManagement.GetUsernameByIdPlayer(0);

            Assert.Equal(expectedResult, currentResult);
        }

        
        [Fact]
        public void TestUpdateAccountSuccess()
        {
            UserManagement userManagement = new UserManagement();
            int expectedResult = 1;

            int idAccount = userManagement.GetIdPlayerByUsername("JhonUsernameTest02");
            Accounts editedAccount = new Accounts
            {
                idAccount = idAccount,
                name = "JhonNameTest",
                lastName = "JhonMercuryLastNameTest",
                surname = "JhonLopezSurnameTest",
                birthdate = DateTime.Now
            };

            int rowsAffected = userManagement.UpdateAccount(editedAccount);

            Assert.Equal(expectedResult, rowsAffected);
        }

        [Fact]
        public void TestUpdateAccountFail()
        {
            UserManagement userManagement = new UserManagement();
            int expectedResult = -1;

            Accounts editedAccount = new Accounts
            {
                idAccount = -1,
                name = "UpdatedName",
                surname = "UpdatedSurname",
                lastName = "UpdatedLastName",
                birthdate = DateTime.Now
            };

            int rowsAffected = userManagement.UpdateAccount(editedAccount);

            Assert.Equal(expectedResult, rowsAffected);
        }
    }

}
