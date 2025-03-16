using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System.Threading;
using System.Linq;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace TestScript
{
    public class UnitTestThemPhong
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

        public static IEnumerable<TestCaseData> GetTestCasesTP()
        {
            yield return new TestCaseData("", "", "", "", "", "", false)
                .SetName("TP_01_EmptyFields");


        }

        [Test, TestCaseSource(nameof(GetTestCasesTP))]
        public void TP(string rname, string rcap, string rnum,string rnumbed, string rprice, string ravatar, bool expectedResult)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                driver.FindElement(By.XPath("//li[contains(@class, 'ant-menu-item')]/span/p[text()='Phòng']")).Click();
                wait.Until(d => d.Url.Contains("/owner/Room"));

                var editButton = wait.Until(d => d.FindElement(By.XPath("//button[span[text()='Thêm phòng']]")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", editButton);
                Thread.Sleep(1000);
                editButton.Click();

                ClearAndType(By.Id("createRoom_roomName"), rname);
                ClearAndType(By.Id("createRoom_capacity"), rcap);
                ClearAndType(By.Id("createRoom_numberOfRooms"), rnum);
                ClearAndType(By.Id("createRoom_numberOfBeds"), rnumbed);
                ClearAndType(By.Id("createRoom_money"), rprice);

                var roomTypeSelect = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//input[@id='createRoom_typeOfRoom']/ancestor::div[contains(@class, 'ant-select-selector')]")));
                roomTypeSelect.Click();

                var firstRoomType = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//div[contains(@class, 'ant-select-item') and contains(@class, 'ant-select-item-option')][1]")));
                firstRoomType.Click();

                var hotelDropdown = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//input[@id='createRoom_hotelID']/ancestor::div[contains(@class, 'ant-select-selector')]")));
                hotelDropdown.Click();

                // Chờ danh sách khách sạn xuất hiện
                Thread.Sleep(2000); // Nếu bị lỗi mất focus, có thể cần thời gian chờ

                // Chọn khách sạn đầu tiên (có title="Bá")
                var firstHotelOption = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//div[contains(@class, 'ant-select-item-option') and @title='Bá']")));
                firstHotelOption.Click();


                driver.FindElement(By.Id("avatarLink")).SendKeys(ravatar);
                Thread.Sleep(5000);

                driver.FindElement(By.XPath("//button[contains(@class, 'ant-btn-primary')]/span[text()='Thêm phòng']")).Click();

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

                    isSuccess = successMessage != null && successMessage.Text.Contains("Tạo phòng thành công");
                }
                catch (WebDriverTimeoutException)
                {
                    isSuccess = false; 
                }

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