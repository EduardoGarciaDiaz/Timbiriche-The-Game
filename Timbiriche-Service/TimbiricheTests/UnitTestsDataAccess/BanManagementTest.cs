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

        private ILogger _logger = LoggerManager.GetLogger();
        public int IdReportedTestPlayer { get; set; }
        public int IdReporterTestPlayer { get; set; }

        public ConfigurationBanManagementTests()
        {
            try
            {
                CreateTestUsers();
                CreateReport();
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
                name = "JhonNameTest",
                lastName = "JhonMercuryLastNameTest",
                surname = "JhonLopezSurnameTest",
                birthdate = DateTime.Now
            });

            var newReporterAccountTest = context.Accounts.Add(new Accounts()
            {
                name = "JuanNameTest",
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
                username = "JhonUsernameTest02",
                email = "jhonemailtest@gmail.com",
                password = "My7_ScrT3Pa5s_W0r6",
                coins = 20,
                Accounts = newAccountsTest.Item1
            });

            var newReporterPlayerTest = context.Players.Add(new Players()
            {
                username = "JuanUsernameTest02",
                email = "juanemailtest@gmail.com",
                password = "My7_ScrT3Pa5s_W0r6",
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

        public void Dispose()
        {
            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    DeletePlayersAndAccounts(context);
                    DeleteReports(context);

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
            Players reportedPlayerToDelete = context.Players.Find(IdReportedTestPlayer);
            Players reporterPlayerToDelete = context.Players.Find(IdReporterTestPlayer);

            context.Players.Remove(reportedPlayerToDelete);
            context.Players.Remove(reporterPlayerToDelete);

            Accounts reportedAccountToDelete = context.Accounts.FirstOrDefault(a => a.name == "JhonNameTest");
            Accounts reporterAccountToDelete = context.Accounts.FirstOrDefault(a => a.name == "JuanNameTest");

            context.Players.Remove(reportedPlayerToDelete);
            context.Players.Remove(reporterPlayerToDelete);
        }

        private void DeleteReports(TimbiricheDBEntities context)
        {
            var reportsToDelete = context.Reports
                .Where(r => r.idPlayerReporter == IdReporterTestPlayer && r.idPlayerReported == IdReportedTestPlayer)
                .ToList();

            if (reportsToDelete != null)
            {
                context.Reports.RemoveRange(reportsToDelete);
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
        public void TestVerifyUniqueReportSuccess()
        {
            int idPlayerReported = _configuration.IdReportedTestPlayer;
            int idPlayerReporter = _configuration.IdReporterTestPlayer;

            var result = BanManagement.VerifyUniqueReport(idPlayerReported, idPlayerReporter);

            Assert.True(result);
        }
    }
}
