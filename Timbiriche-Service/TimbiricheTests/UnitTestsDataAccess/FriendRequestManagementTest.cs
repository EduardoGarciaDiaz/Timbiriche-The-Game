using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Utils;
using Xunit;

namespace TimbiricheTests.UnitTestsDataAccess
{
    public class ConfigurationFriendRequestManagementTests : IDisposable
    {
        private ILogger _logger = LoggerManager.GetLogger();

        public ConfigurationFriendRequestManagementTests()
        {
            try
            {
                Accounts firstAccount = new Accounts
                {
                    name = "JhonNameTestFriend",
                    lastName = "JhonLastNameTestFriend",
                    surname = "JhonSurnameFriend",
                    birthdate = DateTime.Now
                };
                Players firstPlayer = new Players
                {
                    username = "JhonUsernameTest05",
                    email = "jhontest05@gmail.com",
                    password = "my_Password",
                    Accounts = firstAccount
                };

                Accounts secondAccount = new Accounts
                {
                    name = "LionelNameTestFriend",
                    lastName = "LionelLastNameTestFriend",
                    surname = "LionelSurnameFriend",
                    birthdate = DateTime.Now
                };
                Players secondPlayer = new Players
                {
                    username = "LionelUsernameTest04",
                    email = "lionel04test@gmail.com",
                    password = "My_password01",
                    Accounts = secondAccount
                };

                Accounts thirdAccount = new Accounts
                {
                    name = "TeslaNameTestFriend",
                    lastName = "TeslaLastNameTestFriend",
                    surname = "TeslaSurnameFriend",
                    birthdate = DateTime.Now
                };
                Players thirdPlayer = new Players
                {
                    username = "teslaUsernameTest04",
                    email = "tesla04test@gmail.com",
                    password = "my_8Password1",
                    Accounts = thirdAccount
                };

                Accounts fourthAccount = new Accounts
                {
                    name = "PiqueNameTestFriend",
                    lastName = "PiqueLastNameTestFriend",
                    surname = "PiqueSurnameFriend",
                    birthdate = DateTime.Now
                };
                Players fourthPlayer = new Players
                {
                    username = "PiqueUsernameTest04",
                    email = "pique04test@gmail.com",
                    password = "my_8Password1",
                    Accounts = fourthAccount
                };

                FirstIdTestPlayer = ConfigureTestUser(firstPlayer);
                SecondIdTestPlayer = ConfigureTestUser(secondPlayer);
                ThirdIdTestPlayer = ConfigureTestUser(thirdPlayer);
                FourtIdTestPlayer = ConfigureTestUser(fourthPlayer);

                AddRequestFriend(FirstIdTestPlayer, SecondIdTestPlayer);
                AddRequestFriend(FirstIdTestPlayer, FourtIdTestPlayer);
                AddFriendship(FirstIdTestPlayer, ThirdIdTestPlayer);
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var error in ex.EntityValidationErrors.SelectMany(validationError => validationError.ValidationErrors))
                {
                    _logger.Error($"Entity Validation Error: Property '{error.PropertyName}', Error: '{error.ErrorMessage}'");
                }
            }
            catch (SqlException ex)
            {
                _logger.Error("SQLException on FriendRequestManagementTest: " + ex.Message);
            }
            catch (EntityException ex)
            {
                _logger.Error("Entity Exception on FriendRequestManagementTest: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected Exception on FriendRequestManagementTest: " + ex.Message + ": \n" + ex.StackTrace);
            }
        }

        public int FirstIdTestPlayer { get; set; }
        public int SecondIdTestPlayer { get; set; }
        public int ThirdIdTestPlayer { get; set; }
        public int FourtIdTestPlayer { get; set; }

        private int ConfigureTestUser(Players player)
        {
            using (var context = new TimbiricheDBEntities())
            {
                context.Accounts.Add(player.Accounts);
                context.Players.Add(player);

                context.SaveChanges();
                int idPlayer = player.idPlayer;

                return idPlayer;
            }
        }

        private void AddRequestFriend(int idPlayerSender, int idPlayerRequested)
        {
            FriendShips friendShip = new FriendShips
            {
                idPlayer = idPlayerSender,
                idPlayerFriend = idPlayerRequested,
                statusFriendship = "Request"
            };

            using (var context = new TimbiricheDBEntities())
            {
                context.FriendShips.Add(friendShip);
                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    _logger.Error("DbEntityValidationException on FriendRequestManagementTest: " + ex.Message);
                }
                catch (SqlException ex)
                {
                    _logger.Error("SQLException on FriendRequestManagementTest: " + ex.Message);
                }
                catch (EntityException ex)
                {
                    _logger.Error("Entity Exception on FriendRequestManagementTest: " + ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.Error("Unexpected Exception on FriendRequestManagementTest: " + ex.Message + ": \n" + ex.StackTrace);
                }

            }
        }

        private void AddFriendship(int idPlayerSender, int idPlayerRequested)
        {
            FriendShips friendShip = new FriendShips
            {
                idPlayer = idPlayerSender,
                idPlayerFriend = idPlayerRequested,
                statusFriendship = "Friend"
            };

            using (var context = new TimbiricheDBEntities())
            {
                context.FriendShips.Add(friendShip);
                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    _logger.Error("DbEntityValidationException on FriendRequestManagementTest: " + ex.Message);
                }
                catch (SqlException ex)
                {
                    _logger.Error("SQLException on FriendRequestManagementTest: " + ex.Message);
                }
                catch (EntityException ex)
                {
                    _logger.Error("Entity Exception on FriendRequestManagementTest: " + ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.Error("Unexpected Exception on FriendRequestManagementTest: " + ex.Message + ": \n" + ex.StackTrace);
                }

            }
        }

        public void Dispose()
        {
            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    DeletePlayersAndAccounts(context);
                    DeleteFriendships(FirstIdTestPlayer, SecondIdTestPlayer);
                    DeleteFriendships(FirstIdTestPlayer, ThirdIdTestPlayer);
                    DeleteFriendships(SecondIdTestPlayer, ThirdIdTestPlayer);
                    DeleteFriendships(FirstIdTestPlayer, FourtIdTestPlayer);

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
            Players firstPlayerToDelete = context.Players
                .FirstOrDefault(p => p.username == "JhonUsernameTest05" && p.email == "jhontest05@gmail.com");

            Accounts firstAccountToDelete = context.Accounts.FirstOrDefault(a => a.name == "JhonNameTestFriend"
            && a.lastName == "JhonLastNameTestFriend" && a.surname == "JhonSurnameFriend");

            Players secondPlayerToDelete = context.Players
               .FirstOrDefault(p => p.username == "LionelUsernameTest04" && p.email == "lionel04test@gmail.com");

            Accounts secondAccountToDelete = context.Accounts.FirstOrDefault(a => a.name == "LionelNameTestFriend"
            && a.lastName == "LionelLastNameTestFriend" && a.surname == "LionelSurnameFriend");

            Players thirdPlayerToDelete = context.Players
               .FirstOrDefault(p => p.username == "teslaUsernameTest04" && p.email == "tesla04test@gmail.com");

            Accounts thirdAccountToDelete = context.Accounts.FirstOrDefault(a => a.name == "TeslaNameTestFriend"
            && a.lastName == "TeslaLastNameTestFriend" && a.surname == "TeslaSurnameFriend");

            Players fourthPlayerToDelete = context.Players
               .FirstOrDefault(p => p.username == "PiqueUsernameTest04" && p.email == "pique04test@gmail.com");

            Accounts fourthAccountToDelete = context.Accounts.FirstOrDefault(a => a.name == "PiqueNameTestFriend" 
            && a.lastName == "PiqueLastNameTestFriend" && a.surname == "PiqueSurnameFriend");


            RemovePlayers(context, firstPlayerToDelete);
            RemoveAccounts(context, firstAccountToDelete);

            RemovePlayers(context, secondPlayerToDelete);
            RemoveAccounts(context, secondAccountToDelete);

            RemovePlayers(context, thirdPlayerToDelete);
            RemoveAccounts(context, thirdAccountToDelete);

            RemovePlayers(context, fourthPlayerToDelete);
            RemoveAccounts(context, fourthAccountToDelete);
        }

        private void RemovePlayers(TimbiricheDBEntities context, Players player)
        {
            if (player != null)
            {
                context.Players.Remove(player);
            }
        }

        private void RemoveAccounts(TimbiricheDBEntities context, Accounts account)
        {
            if (account != null)
            {
                context.Accounts.Remove(account);
            }
        }

        private void DeleteFriendships(int idPlayerFriend, int idCurrentPlayer)
        {
            using (var context = new TimbiricheDBEntities())
            {
                var friendship = context.FriendShips.FirstOrDefault(fs =>
                    (fs.idPlayer == idPlayerFriend && fs.idPlayerFriend == idCurrentPlayer 
                    && (fs.statusFriendship == "friend" || fs.statusFriendship == "request"))
                    || (fs.idPlayer == idCurrentPlayer && fs.idPlayerFriend == idPlayerFriend 
                    && (fs.statusFriendship == "friend" || fs.statusFriendship == "request"))
                );

                if (friendship != null)
                {
                    context.FriendShips.Remove(friendship);
                    context.SaveChanges();
                }
            }
        }
    }

    public class FriendRequestManagementTest : IClassFixture<ConfigurationFriendRequestManagementTests>
    {
        private readonly ConfigurationFriendRequestManagementTests _configuration;

        public FriendRequestManagementTest(ConfigurationFriendRequestManagementTests configuration)
        {
            _configuration = configuration;
        }

        [Fact]
        public void TestVerifyFriendshipSuccess()
        {
            FriendRequestManagement friendRequestManagement = new FriendRequestManagement();

            bool currentFriendResult = friendRequestManagement.VerifyFriendship(_configuration.FirstIdTestPlayer,
                _configuration.SecondIdTestPlayer);

            Assert.True(currentFriendResult);
        }

        [Fact]
        public void TestVerifyFriendshipFail()
        {
            FriendRequestManagement friendRequestManagement = new FriendRequestManagement();
            bool currentResult = friendRequestManagement.VerifyFriendship(_configuration.SecondIdTestPlayer, 0);

            Assert.False(currentResult);
        }

        [Fact]
        public void TestAddRequestFriendshipSuccess()
        {
            FriendRequestManagement friendRequestManagement = new FriendRequestManagement();
            int expectedResult = 1;

            int currentResult = friendRequestManagement.AddRequestFriendship(_configuration.SecondIdTestPlayer,
                _configuration.ThirdIdTestPlayer);

            Assert.Equal(expectedResult, currentResult);
        }

        [Fact]
        public void TestAddRequestFriendshipFail()
        {
            FriendRequestManagement friendRequestManagement = new FriendRequestManagement();
            int expectedResult = -1;

            int currentResult = friendRequestManagement.AddRequestFriendship(0, 0);

            Assert.Equal(expectedResult, currentResult);
        }

        [Fact]
        public void TestIsFriendSuccess()
        {
            FriendRequestManagement friendRequestManagement = new FriendRequestManagement();
            bool currentResult = friendRequestManagement.IsFriend(_configuration.FirstIdTestPlayer, _configuration.ThirdIdTestPlayer);

            Assert.True(currentResult);
        }

        [Fact]
        public void TestIsFriendFail()
        {
            FriendRequestManagement friendRequestManagement = new FriendRequestManagement();
            bool currentResult = friendRequestManagement.IsFriend(_configuration.FirstIdTestPlayer, _configuration.SecondIdTestPlayer);

            Assert.False(currentResult);
        }

        [Fact]
        public void TestGetPlayerIdOfFriendRequestersSuccess()
        {
            FriendRequestManagement friendRequestManagement = new FriendRequestManagement();
            List<int> expectedResult = new List<int> { _configuration.FirstIdTestPlayer };

            List<int> currentResult = friendRequestManagement.GetPlayerIdOfFriendRequesters(_configuration.SecondIdTestPlayer);

            Assert.Equal(expectedResult, currentResult);
        }

        [Fact]
        public void TestGetPlayerIdOfFriendRequestersFail()
        {
            FriendRequestManagement friendRequestManagement = new FriendRequestManagement();
            List<int> expectedResult = new List<int>();

            List<int> currentResult = friendRequestManagement.GetPlayerIdOfFriendRequesters(0);

            Assert.Equal(expectedResult, currentResult);
        }

        [Fact]
        public void TestUpdateFriendRequestToAcceptedSuccess()
        {
            FriendRequestManagement friendRequestManagement = new FriendRequestManagement();
            int expectedResult = 1;

            int currentResult = friendRequestManagement.UpdateFriendRequestToAccepted(_configuration.SecondIdTestPlayer, _configuration.FirstIdTestPlayer);

            Assert.Equal(expectedResult, currentResult);    
        }

        [Fact]
        public void TestUpdateFriendRequestToAcceptedFail()
        {
            FriendRequestManagement friendRequestManagement = new FriendRequestManagement();
            int expectedResult = -1;

            int currentResult = friendRequestManagement.UpdateFriendRequestToAccepted(0, 0);

            Assert.Equal(expectedResult, currentResult);
        }

        [Fact]
        public void TestDeleteFriendRequestSuccess()
        {
            FriendRequestManagement friendRequestManagement = new FriendRequestManagement();
            int expectedResult = 1;

            int currentResult = friendRequestManagement.DeleteFriendRequest(_configuration.FirstIdTestPlayer, _configuration.FourtIdTestPlayer);

            Assert.Equal(expectedResult, currentResult);
        }

        [Fact]
        public void TestDeleteFriendRequestFail()
        {
            FriendRequestManagement friendRequestManagement = new FriendRequestManagement();
            int expectedResult = -1;

            int currentResult = friendRequestManagement.DeleteFriendRequest(0, 0);

            Assert.Equal(expectedResult, currentResult);
        }

        [Fact]
        public void TestDeleteFriendshipSuccess()
        {
            FriendRequestManagement friendRequestManagement = new FriendRequestManagement();
            int expectedResult = 1;

            int currentResult = friendRequestManagement.DeleteFriendship(_configuration.FirstIdTestPlayer,
                _configuration.ThirdIdTestPlayer);

            Assert.Equal(expectedResult, currentResult);
        }

        [Fact]
        public void TestDeleteFriendshipFail()
        {
            FriendRequestManagement friendRequestManagement = new FriendRequestManagement();
            int expectedResult = -1;

            int currentResult = friendRequestManagement.DeleteFriendship(0, 0);

            Assert.Equal(expectedResult, currentResult);
        }

        [Fact]
        public void TestGetFriendsSuccess()
        {
            FriendRequestManagement friendRequestManagement = new FriendRequestManagement();
            List<string> currentResult = friendRequestManagement.GetFriends(_configuration.FirstIdTestPlayer);

            Assert.NotNull(currentResult);
        }

        [Fact]
        public void TestGetFriendsFail()
        {
            FriendRequestManagement friendRequestManagement = new FriendRequestManagement();
            List<string> currentResult = friendRequestManagement.GetFriends(_configuration.FirstIdTestPlayer);

            Assert.True(currentResult.Count == 0);
        }


    }
}
