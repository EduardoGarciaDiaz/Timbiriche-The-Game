using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;
using Xunit;


namespace TimbiricheTests.UnitTestsDataAccess
{
    public class ShopManagementTest
    {
        [Fact]
        public void TestGetColorsSuccess()
        {
            List<Colors> colors = ShopManagement.GetColors();

            Assert.NotNull(colors);
            Assert.True(colors.Count > 0);
        }
    }
}
