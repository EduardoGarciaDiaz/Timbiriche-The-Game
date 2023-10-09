using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using TimbiricheViews.Utils;
using Xunit;

namespace TimbiricheTests.Tests
{
    public class UtilitiesTest
    {
        [Fact]
        public void TestIsEmptyFieldNullSuccess()
        {
            bool currentResult = false;
            string testText = null;

            currentResult = Utilities.IsEmptyField(testText);

            Assert.True(currentResult);
        }

        [Fact]
        public void TestIsEmptyFieldEmptySuccess()
        {
            bool currentResult = false;
            string testText = "";

            currentResult = Utilities.IsEmptyField(testText);

            Assert.True(currentResult);
        }

        [Fact]
        public void TestIsEmptyFieldFail()
        {
            bool currentResult = false;
            string testText = "  ";

            currentResult = Utilities.IsEmptyField(testText);

            Assert.False(currentResult);
        }
        
        [Fact]
        public void TestIsValidPersonalInformationSuccess()
        {
            bool currentResult = false;
            string testText = "Messi";

            currentResult = Utilities.IsValidPersonalInformation(testText);
            
            Assert.True(currentResult);
        }

        [Fact]
        public void TestIsValidPersonalInformationEmptyFail()
        {
            bool currentResult = false;
            string testText = " ";

            currentResult = Utilities.IsValidPersonalInformation(testText);

            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidPersonalInformationFail()
        {
            bool currentResult = false;
            string testText = "Gohan*+";

            currentResult = Utilities.IsValidPersonalInformation(testText);

            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidUsernameSuccess()
        {
            bool currentResult = false;
            string testText = "Messi_10";

            currentResult = Utilities.IsValidUsername(testText);

            Assert.True(currentResult);
        }

        [Fact]
        public void TestIsValidUsernameEmptyFail()
        {
            bool currentResult = false;
            string testText = " ";

            currentResult = Utilities.IsValidUsername(testText);

            Assert.False(currentResult);
        }
        [Fact]
        public void TestIsValidUsernameFail()
        {
            bool currentResult = false;
            string testText = "Willy*-/";

            currentResult = Utilities.IsValidUsername(testText);

            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidEmailSuccess()
        {
            bool currentResult = false;
            string testText = "messi@gmail.com";

            currentResult = Utilities.IsValidEmail(testText);
            Assert.True(currentResult);
        }

        [Fact]
        public void TestIsValidEmailEmptyFail()
        {
            bool currentResult = false;
            string testText = " ";

            currentResult = Utilities.IsValidEmail(testText);
            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidEmailFail()
        {
            bool currentResult = false;
            string testText = "messi@gmail";

            currentResult = Utilities.IsValidEmail(testText);
            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidPasswordSuccess()
        {
            bool currentResult = false;
            string testText = "M_10Ess1A/tOnL";

            currentResult = Utilities.IsValidPassword(testText);
            Assert.True(currentResult);
        }

        [Fact]
        public void TestIsValidPasswordEmptyFail()
        {
            bool currentResult = false;
            string testText = " ";

            currentResult = Utilities.IsValidPassword(testText);
            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidPasswordFail()
        {
            bool currentResult = false;
            string testText = "M_10eai12";

            currentResult = Utilities.IsValidPassword(testText);
            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidSymbolSuccess()
        {
            bool currentResult = false;
            string testText = "M_10Ess1A/tOnL";

            currentResult = Utilities.IsValidSymbol(testText);
            Assert.True(currentResult);
        }

        [Fact]
        public void TestIsValidSymbolEmptyFail()
        {
            bool currentResult = false;
            string testText = " ";

            currentResult = Utilities.IsValidSymbol(testText);
            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidSymbolFail()
        {
            bool currentResult = false;
            string testText = "M10Ess1AtOnL22";

            currentResult = Utilities.IsValidSymbol(testText);
            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidCapitalLetterSuccess()
        {
            bool currentResult = false;
            string testText = "M_10Ess1A/tOnL";

            currentResult = Utilities.IsValidCapitalLetter(testText);
            Assert.True(currentResult);
        }

        [Fact]
        public void TestIsValidCapitalLetterEmptyFail()
        {
            bool currentResult = false;
            string testText = " ";

            currentResult = Utilities.IsValidCapitalLetter(testText);
            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidCapitalLetterFail()
        {
            bool currentResult = false;
            string testText = "m_10ess1a/t0nl";

            currentResult = Utilities.IsValidCapitalLetter(testText);
            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidLowerLetterSuccess()
        {
            bool currentResult = false;
            string testText = "M_10Ess1A/tOnL";

            currentResult = Utilities.IsValidLowerLetter(testText);
            Assert.True(currentResult);
        }

        [Fact]
        public void TestIsValidLowerLetterEmptyFail()
        {
            bool currentResult = false;
            string testText = " ";

            currentResult = Utilities.IsValidLowerLetter(testText);
            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidLowerLetterFail()
        {
            bool currentResult = false;
            string testText = "M_108ESS1A/TONL3";

            currentResult = Utilities.IsValidLowerLetter(testText);
            Assert.False(currentResult);
        }
        [Fact]
        public void TestIsValidNumberSuccess()
        {
            bool currentResult = false;
            string testText = "M_10Ess1A/tOnL";

            currentResult = Utilities.IsValidNumber(testText);
            Assert.True(currentResult);
        }

        [Fact]
        public void TestIsValidNumberEmptyFail()
        {
            bool currentResult = false;
            string testText = " ";

            currentResult = Utilities.IsValidNumber(testText);
            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidNumberFail()
        {
            bool currentResult = false;
            string testText = "M_DuEssFA/tRnL";

            currentResult = Utilities.IsValidNumber(testText);
            Assert.False(currentResult);
        }
    }
}
