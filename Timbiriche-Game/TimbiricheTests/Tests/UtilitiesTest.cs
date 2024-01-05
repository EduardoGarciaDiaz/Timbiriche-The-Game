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
        public void TestIsValidPersonalInformationSuccess()
        {
            bool currentResult = false;
            string testText = "Messi";

            currentResult = ValidationUtilities.IsValidPersonalInformation(testText);
            
            Assert.True(currentResult);
        }

        [Fact]
        public void TestIsValidPersonalInformationEmptyFail()
        {
            bool currentResult = false;
            string testText = " ";

            currentResult = ValidationUtilities.IsValidPersonalInformation(testText);

            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidPersonalInformationFail()
        {
            bool currentResult = false;
            string testText = "Gohan*+";

            currentResult = ValidationUtilities.IsValidPersonalInformation(testText);

            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidUsernameSuccess()
        {
            bool currentResult = false;
            string testText = "Messi_10";

            currentResult = ValidationUtilities.IsValidUsername(testText);

            Assert.True(currentResult);
        }

        [Fact]
        public void TestIsValidUsernameEmptyFail()
        {
            bool currentResult = false;
            string testText = " ";

            currentResult = ValidationUtilities.IsValidUsername(testText);

            Assert.False(currentResult);
        }
        [Fact]
        public void TestIsValidUsernameFail()
        {
            bool currentResult = false;
            string testText = "Willy*-/";

            currentResult = ValidationUtilities.IsValidUsername(testText);

            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidEmailSuccess()
        {
            bool currentResult = false;
            string testText = "messi@gmail.com";

            currentResult = ValidationUtilities.IsValidEmail(testText);
            Assert.True(currentResult);
        }

        [Fact]
        public void TestIsValidEmailEmptyFail()
        {
            bool currentResult = false;
            string testText = " ";

            currentResult = ValidationUtilities.IsValidEmail(testText);
            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidEmailFail()
        {
            bool currentResult = false;
            string testText = "messi@gmail";

            currentResult = ValidationUtilities.IsValidEmail(testText);
            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidPasswordSuccess()
        {
            bool currentResult = false;
            string testText = "M_10Ess1A/tOnL";

            currentResult = ValidationUtilities.IsValidPassword(testText);
            Assert.True(currentResult);
        }

        [Fact]
        public void TestIsValidPasswordEmptyFail()
        {
            bool currentResult = false;
            string testText = " ";

            currentResult = ValidationUtilities.IsValidPassword(testText);
            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidPasswordFail()
        {
            bool currentResult = false;
            string testText = "M_10eai12";

            currentResult = ValidationUtilities.IsValidPassword(testText);
            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidSymbolSuccess()
        {
            bool currentResult = false;
            string testText = "M_10Ess1A/tOnL";

            currentResult = ValidationUtilities.IsValidSymbol(testText);
            Assert.True(currentResult);
        }

        [Fact]
        public void TestIsValidSymbolEmptyFail()
        {
            bool currentResult = false;
            string testText = " ";

            currentResult = ValidationUtilities.IsValidSymbol(testText);
            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidSymbolFail()
        {
            bool currentResult = false;
            string testText = "M10Ess1AtOnL22";

            currentResult = ValidationUtilities.IsValidSymbol(testText);
            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidCapitalLetterSuccess()
        {
            bool currentResult = false;
            string testText = "M_10Ess1A/tOnL";

            currentResult = ValidationUtilities.IsValidCapitalLetter(testText);
            Assert.True(currentResult);
        }

        [Fact]
        public void TestIsValidCapitalLetterEmptyFail()
        {
            bool currentResult = false;
            string testText = " ";

            currentResult = ValidationUtilities.IsValidCapitalLetter(testText);
            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidCapitalLetterFail()
        {
            bool currentResult = false;
            string testText = "m_10ess1a/t0nl";

            currentResult = ValidationUtilities.IsValidCapitalLetter(testText);
            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidLowerLetterSuccess()
        {
            bool currentResult = false;
            string testText = "M_10Ess1A/tOnL";

            currentResult = ValidationUtilities.IsValidLowerLetter(testText);
            Assert.True(currentResult);
        }

        [Fact]
        public void TestIsValidLowerLetterEmptyFail()
        {
            bool currentResult = false;
            string testText = " ";

            currentResult = ValidationUtilities.IsValidLowerLetter(testText);
            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidLowerLetterFail()
        {
            bool currentResult = false;
            string testText = "M_108ESS1A/TONL3";

            currentResult = ValidationUtilities.IsValidLowerLetter(testText);
            Assert.False(currentResult);
        }
        [Fact]
        public void TestIsValidNumberSuccess()
        {
            bool currentResult = false;
            string testText = "M_10Ess1A/tOnL";

            currentResult = ValidationUtilities.IsValidNumber(testText);
            Assert.True(currentResult);
        }

        [Fact]
        public void TestIsValidNumberEmptyFail()
        {
            bool currentResult = false;
            string testText = " ";

            currentResult = ValidationUtilities.IsValidNumber(testText);
            Assert.False(currentResult);
        }

        [Fact]
        public void TestIsValidNumberFail()
        {
            bool currentResult = false;
            string testText = "M_DuEssFA/tRnL";

            currentResult = ValidationUtilities.IsValidNumber(testText);
            Assert.False(currentResult);
        }
    }
}