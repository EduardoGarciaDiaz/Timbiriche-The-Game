using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Utils;
using TimbiricheService.Utils;
using Xunit;

namespace TimbiricheTests.UnitTestsDataAccess
{

    public class ConfigurationBanManagementTests : IDisposable
    {

        private ILogger _logger = TimbiricheDataAccess.Utils.LoggerManager.GetLogger();
        public int IdReportedTestPlayer { get; set; }
        public int IdReporterTestPlayer { get; set; }
        public DateTime BanEndDate { get; set; }


        public ConfigurationBanManagementTests()
        {
            try
            {
                CreateTestUsers();
                CreateReport();
                CreateBan();
            }
            catch (SqlException ex)
            {
                _logger.Error("SQLException on BanManagementTest: " + ex.Message);
            }
            catch (EntityException ex)
            {
                _logger.Error("Entity Exception on BanManagementTest: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected Exception on BanManagementTest: " + ex.Message + ": \n" + ex.StackTrace);
            }
        }

        private void CreateTestUsers()
        {
            using (var context = new TimbiricheDBEntities())
            {

                var newAccountsTest = CreateTestAccounts(context);
                var newPlayersTest = CreateTestPlayers(context, newAccountsTest);

                context.SaveChanges();
                IdReportedTestPlayer = newPlayersTest.Item1.idPlayer;
                IdReporterTestPlayer = newPlayersTest.Item2.idPlayer;
            }
        }

        private (Accounts, Accounts) CreateTestAccounts(TimbiricheDBEntities context)
        {

            var newReportedAccountTest = context.Accounts.Add(new Accounts()
            {
                name = "JhonNameTest1000",
                lastName = "JhonMercuryLastNameTest",
                surname = "JhonLopezSurnameTest",
                birthdate = DateTime.Now
            });

            var newReporterAccountTest = context.Accounts.Add(new Accounts()
            {
                name = "JuanNameTest1001",
                lastName = "JuanMercuryLastNameTest",
                surname = "JuanLopezSurnameTest",
                birthdate = DateTime.Now
            });

            return (newReportedAccountTest, newReporterAccountTest);
        }

        private (Players, Players) CreateTestPlayers(TimbiricheDBEntities context, (Accounts, Accounts) newAccountsTest)
        {
            var newReportedPlayerTest = context.Players.Add(new Players()
            {
                username = "JhonUsernameTest1000",
                email = "jhonemailtest1000@gmail.com",
                password = "My7_ScrT3Pa5s_W0r6",
                status = "Not-Banned",
                coins = 20,
                Accounts = newAccountsTest.Item1
            });

            var newReporterPlayerTest = context.Players.Add(new Players()
            {
                username = "JuanUsernameTest1001",
                email = "juanemailtest1001@gmail.com",
                password = "My7_ScrT3Pa5s_W0r6",
                status = "Not-Banned",
                coins = 20,
                Accounts = newAccountsTest.Item2
            });

            return (newReportedPlayerTest, newReporterPlayerTest);
        }

        private void CreateReport()
        {
            int idPlayerReported = IdReportedTestPlayer;
            int idPlayerReporter = IdReporterTestPlayer;
            DateTime reportDate = DateTime.Now;

            BanManagement.CreateReport(idPlayerReported, idPlayerReporter, reportDate);
        }

        private void CreateBan()
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = startDate.AddDays(7);
            BanEndDate = endDate;

            BanManagement.CreateBan(IdReporterTestPlayer, startDate, endDate);
        }

        public void Dispose()
        {
            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    DeleteBans(context);
                    DeleteReports(context);
                    DeletePlayersAndAccounts(context);

                    context.SaveChanges();
                }
            }
            catch (SqlException ex)
            {
                _logger.Error("SQLException on BanManagementTest: " + ex.Message);

            }
            catch (EntityException ex)
            {
                _logger.Error("EntityException on BanManagementTest: " + ex.Message);
            }
        }

        private void DeletePlayersAndAccounts(TimbiricheDBEntities context)
        {
            Players reportedPlayerToDelete = context.Players.Find(IdReportedTestPlayer);
            Players reporterPlayerToDelete = context.Players.Find(IdReporterTestPlayer);

            context.Players.Remove(reportedPlayerToDelete);
            context.Players.Remove(reporterPlayerToDelete);

            Accounts reportedAccountToDelete = context.Accounts.FirstOrDefault(a => a.name == "JhonNameTest1000");
            Accounts reporterAccountToDelete = context.Accounts.FirstOrDefault(a => a.name == "JuanNameTest1001");

            context.Players.Remove(reportedPlayerToDelete);
            context.Players.Remove(reporterPlayerToDelete);
        }

        private void DeleteReports(TimbiricheDBEntities context)
        {
            var reportsToDelete = context.Reports
                .Where(r => r.idPlayerReporter == IdReporterTestPlayer || r.idPlayerReported == IdReportedTestPlayer)
                .ToList();

            if (reportsToDelete != null)
            {
                context.Reports.RemoveRange(reportsToDelete);
            }
        }

        private void DeleteBans(TimbiricheDBEntities context)
        {
            var bansToDelete = context.Bans
                .Where(b => b.idBannedPlayer == IdReporterTestPlayer || b.idBannedPlayer == IdReportedTestPlayer)
                .ToList();
            
            if(bansToDelete != null)
            {
                context.Bans.RemoveRange(bansToDelete);
            }
        }
    }
    public class BanManagementTest : IClassFixture<ConfigurationBanManagementTests>
    {
        private readonly ConfigurationBanManagementTests _configuration;

        public BanManagementTest(ConfigurationBanManagementTests configuration)
        {
            _configuration = configuration;
        }

        [Fact]
        public void TestCreateReportSuccess()
        {
            int idPlayerReported = _configuration.IdReportedTestPlayer;
            int idPlayerReporter = _configuration.IdReporterTestPlayer;
            DateTime reportDate = DateTime.Now;

            bool result = BanManagement.CreateReport(idPlayerReported, idPlayerReporter, reportDate);

            Assert.True(result);
        }

        [Fact]
        public void TestCreateReportFail()
        {
            int idPlayerReported = -1;
            int idPlayerReporter = _configuration.IdReporterTestPlayer;
            DateTime reportDate = DateTime.Now;

            bool result = BanManagement.CreateReport(idPlayerReported, idPlayerReporter, reportDate);

            Assert.False(result);
        }

        [Fact]
        public void TestVerifyUniqueReportSuccess()
        {
            int idPlayerReported = _configuration.IdReportedTestPlayer;
            int idPlayerReporter = _configuration.IdReporterTestPlayer;

            var result = BanManagement.VerifyUniqueReport(idPlayerReported, idPlayerReporter);

            Assert.True(result);
        }

        [Fact]
        public void TestVerifyUniqueReportFail()
        {
            int idPlayerReported = -1;
            int idPlayerReporter = _configuration.IdReporterTestPlayer;

            var result = BanManagement.VerifyUniqueReport(idPlayerReported, idPlayerReporter);

            Assert.False(result);
        }

        [Fact]
        public void TestGetNumberOfReportsByIdPlayerReportedSuccess()
        {
            int idPlayerReported = _configuration.IdReporterTestPlayer;

            int result = BanManagement.GetNumberOfReportsByIdPlayerReported(idPlayerReported);

            Assert.True(result == 1);
        }

        [Fact]
        public void TestGetNumberOfReportsByIdPlayerReportedFail()
        {
            int idPlayerReported = -1;

            int result = BanManagement.GetNumberOfReportsByIdPlayerReported(idPlayerReported);

            Assert.False(result == 1);
        }

        [Fact]
        public void TestGetNumberOfBansByIdPlayerSuccess()
        {
            int idPlayer = _configuration.IdReportedTestPlayer;

            var result = BanManagement.GetNumberOfBansByIdPlayer(idPlayer);

            Assert.True(result == 1);
        }

        [Fact]
        public void TestGetNumberOfBansByIdPlayerFail()
        {
            int idPlayer = -1;

            var result = BanManagement.GetNumberOfBansByIdPlayer(idPlayer);

            Assert.False(result == 1);
        }

        [Fact]
        public void TestCreateBanSuccess()
        {
            int idReportedPlayer = _configuration.IdReportedTestPlayer;
            DateTime startDate = DateTime.Now;
            DateTime endDate = startDate.AddDays(7);

            bool result = BanManagement.CreateBan(idReportedPlayer, startDate, endDate);

            Assert.True(result);
        }

        [Fact]
        public void TestCreateBanFail()
        {
            int idReportedPlayer = -1;
            DateTime startDate = DateTime.Now;
            DateTime endDate = startDate.AddDays(7);

            bool result = BanManagement.CreateBan(idReportedPlayer, startDate, endDate);

            Assert.False(result);
        }

        [Fact]
        public void TestClearReportsByIdPlayerSuccess()
        {
            int idPlayer = _configuration.IdReportedTestPlayer;

            bool result = BanManagement.ClearReportsByIdPlayer(idPlayer);

            Assert.True(result);
        }

        [Fact]
        public void TestClearReportsByIdPlayerFail()
        {
            int idPlayer = -1;

            bool result = BanManagement.ClearReportsByIdPlayer(idPlayer);

            Assert.False(result);
        }

        [Fact]
        public void TestUpdatePlayerStatusSuccess()
        {
            int idPlayer = _configuration.IdReportedTestPlayer;
            string status = "Banned";

            bool result = BanManagement.UpdatePlayerStatus(idPlayer, status);

            Assert.True(result);
        }

        [Fact]
        public void TestUpdatePlayerStatusFail()
        {
            int idPlayer = -1;
            string status = "Banned";

            bool result = BanManagement.UpdatePlayerStatus(idPlayer, status);

            Assert.False(result);
        }

        [Fact]
        public void TestGetBanEndDateByIdPlayerSuccess()
        {
            int idPlayer = _configuration.IdReportedTestPlayer;
            DateTime validEndDate = _configuration.BanEndDate;

            DateTime result = BanManagement.GetBanEndDateByIdPlayer(idPlayer);

            result = new DateTime(result.Ticks - (result.Ticks % TimeSpan.TicksPerSecond), result.Kind);
            validEndDate = new DateTime(validEndDate.Ticks - (validEndDate.Ticks % TimeSpan.TicksPerSecond), validEndDate.Kind);

            Assert.Equal(validEndDate, result);
        }

        [Fact]
        public void TestGetBanEndDateByIdPlayerFail()
        {
            int idPlayer = -1;
            DateTime validEndDate = _configuration.BanEndDate;

            DateTime result = BanManagement.GetBanEndDateByIdPlayer(idPlayer);

            Assert.True(result == DateTime.MinValue);
        }

        [Fact]
        public void TestGetPlayerStatusByIdPlayerSuccess()
        {
            int idPlayer = _configuration.IdReporterTestPlayer;
            string validPlayerStatus = "Not-Banned";

            string result = BanManagement.GetPlayerStatusByIdPlayer(idPlayer);

            Assert.Equal(validPlayerStatus, result);
        }

        [Fact]
        public void TestGetPlayerStatusByIdPlayerFail()
        {
            int idPlayer = -1;

            string result = BanManagement.GetPlayerStatusByIdPlayer(idPlayer);

            Assert.Null(result);
        }
    }
}
