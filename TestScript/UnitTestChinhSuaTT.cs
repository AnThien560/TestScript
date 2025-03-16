using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;

namespace TestScript
{
    public class UnitTestChinhSuaTT
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));


            driver.Navigate().GoToUrl("http://localhost:3000/loginOwner");
            driver.Manage().Window.Maximize();


            driver.FindElement(By.Id("email")).SendKeys("anthien@gmail.com");
            driver.FindElement(By.Id("password")).SendKeys("123123123");

            driver.FindElement(By.CssSelector(".btn.btn-primary.bg-white.hover\\:scale-105")).Click();

            wait.Until(d => d.Url.Contains("/dashboard") || d.Url.Contains("/Owner"));
        }

        public static class TestData
        {
            public static string LongName = new string('A', 150); // Chuỗi dài 150 ký tự
            public static string LongEmail = new string('a', 95) + "@gmail.com"; // Email dài hơn 100 ký tự
        }

        public static IEnumerable<TestCaseData> GetTestCases()
        {
            yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif",true)
                .SetName("CSTT_01_ValidName");

            yield return new TestCaseData("", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif",false)
                .SetName("CSTT_02_EmptyName");

            yield return new TestCaseData("1", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif",true)
                .SetName("CSTT_03_NameWithNumber");

            yield return new TestCaseData("An1", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif", true)
                .SetName("CSTT_04_NameWithLettersAndNumbers");

            yield return new TestCaseData(TestData.LongName, "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif",false)
                .SetName("CSTT_06_NameTooLong");

            yield return new TestCaseData("An#$#", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif",false)
                .SetName("CSTT_07_NameWithSpecialChars");

            yield return new TestCaseData("An", "anthien", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif",false)
                .SetName("CSTT_08_InvalidEmailFormat");

            yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif",true)
                .SetName("CSTT_08_InvalidEmailFormat");

            yield return new TestCaseData("An", "", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif", false)
                .SetName("CSTT_10_EmptyEmail");

            yield return new TestCaseData("An", TestData.LongEmail, "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif",false)
                .SetName("CSTT_12_LongEmail");

            yield return new TestCaseData("An", "an123%^$@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif", false)
                .SetName("CSTT_13_EmailWithSpecialChars");

            yield return new TestCaseData("An", "an....123@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif", false)
                .SetName("CSTT_14_EmailWithMultipleDots");

            yield return new TestCaseData("An", "anthien@gmail.com", "2020-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif",false)
                .SetName("CSTT_15_AgeUnder16");

            yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif",true)
                .SetName("CSTT_16_AgeOver16");

            yield return new TestCaseData("An", "anthien@gmail.com", "1003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif", false)
                .SetName("CSTT_17_AgeTooOld");

            yield return new TestCaseData("An", "anthien@gmail.com", "", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif", false)
                .SetName("CSTT_18_EmptyDOB");

            yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "0123456", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif", false)
                .SetName("CSTT_20_PhoneTooShort");

            yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "0123456123123", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif", false)
                .SetName("CSTT_21_PhoneTooLong");

            yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "012345678a", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif", false)
                .SetName("CSTT_22_PhoneWithLetter");

            yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "012345678@", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif", false)
                .SetName("CSTT_23_PhoneWithSpecialChars");

            yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif", false)
                .SetName("CSTT_25_EmptyPhone");

            yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif", false)
                .SetName("CSTT_26_NoPhoneInput");

            yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "12312", @"C:\\Users\\ADMIN\\Downloads\\cat.avif",false)
                .SetName("CSTT_28_CCCDTooShort");

            yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif",false)
                .SetName("CSTT_29_CCCDTooLong");

            yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "12312@#", @"C:\\Users\\ADMIN\\Downloads\\cat.avif", false)
                .SetName("CSTT_30_CCCDWithSpecialChars");

            yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "123 123 123 123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif", false)
                .SetName("CSTT_31_CCCDWithSpaces");

            yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "12312a", @"C:\\Users\\ADMIN\\Downloads\\cat.avif", false)
                .SetName("CSTT_32_CCCDWithLetters");

            yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "", @"C:\\Users\\ADMIN\\Downloads\\cat.avif", false)
                .SetName("CSTT_33_CCCDEmpty");

            yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", null, @"C:\\Users\\ADMIN\\Downloads\\cat.avif",false)
                .SetName("CSTT_34_NoCCCDInput");

            yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\avatar_new.png")
                .SetName("CSTT_35_UpdateAvatar");

            yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", null,true)
                .SetName("CSTT_36_DeleteAvatar");

            yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\file.docx")
                .SetName("CSTT_37_InvalidAvatarFormat");

            yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\avatar1.png;C:\\Users\\ADMIN\\Downloads\\avatar2.png",false)
                .SetName("CSTT_38_MultipleAvatars");

            yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", "")
                .SetName("CSTT_39_NoAvatarSelected");
        }

        [Test, TestCaseSource(nameof(GetTestCases))]
        public void CSTT(string name, string email, string birthday, string phone, string idencard, string avatar, bool expectedResult)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                driver.FindElement(By.XPath("//li[contains(@class, 'ant-menu-item')]/span/p[text()='Tài khoản']")).Click();
                wait.Until(d => d.Url.Contains("/owner/Profile"));

                var editButton = wait.Until(d => d.FindElement(By.XPath("//button[span[text()='Chỉnh sửa thông tin']]")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", editButton);
                Thread.Sleep(1000);
                editButton.Click();

                ClearAndType(By.Id("ownerName"), name);
                ClearAndType(By.Id("email"), email);
                ClearAndType(By.Id("birthday"), birthday);

                // Chọn ngày tháng theo TestCase
                if (!birthday.Equals("")) { 
                string dateXpath = $"//td[@title='{birthday}']";
                driver.FindElement(By.XPath(dateXpath)).Click();
                }

                ClearAndType(By.Id("phoneNum"), phone);
                ClearAndType(By.Id("idenCard"), idencard);
                driver.FindElement(By.Id("avatarLink")).SendKeys(avatar);
                Thread.Sleep(5000);

                driver.FindElement(By.XPath("//button[contains(@class, 'ant-btn-primary')]/span[text()='Chỉnh sửa']")).Click();

                // Kiểm tra thông báo
                bool isSuccess = false;
                try
                {
                    IWebElement successMessage = wait.Until(d =>
                    {
                        var elements = d.FindElements(By.XPath(
                            "//div[contains(@class, 'font-bold') and contains(@class, 'text-[20px]') and contains(@style, 'color: green')]"
                        ));
                        return elements.Count > 0 ? elements[0] : null;
                    });

                    isSuccess = successMessage != null && successMessage.Text.Contains("Cập nhật thành công");
                }
                catch (WebDriverTimeoutException)
                {
                    isSuccess = false; // Không thấy thông báo thành công
                }

                // Kiểm tra kết quả có đúng với mong đợi không
                Assert.That(isSuccess, Is.EqualTo(expectedResult), $"Expected: {expectedResult}, but got: {isSuccess}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xảy ra: " + ex.Message);
                Assert.Fail("Test case gặp lỗi: " + ex.Message);
            }
        }


        void ClearAndType(By selector, string text)
        {
            var element = driver.FindElement(selector);

            element.Click();
            element.SendKeys(Keys.Control + "a");
            element.SendKeys(Keys.Backspace);

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].value = '';", element);

            element.SendKeys(text);
        }


        [TearDown]
        public void TearDown()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
            }
        }
    }
}
