﻿using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using OfficeOpenXml;
using System.Data;
using System.IO;
using ExcelDataReader;
using System.Linq;

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

        public class ExcelDataProvider
        {
            private static DataTable _excelDataTable;

            private static DataTable ReadExcel(string filePath)
            {
                if (_excelDataTable != null)
                {
                    return _excelDataTable;
                }
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        DataSet dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true 
                            }
                        });

                        _excelDataTable = dataSet.Tables[0];
                        return _excelDataTable;
                    }
                }
            }

            public static IEnumerable<TestCaseData> GetTestCasesFromExcel(string filePath)
            {
                var testCases = new List<TestCaseData>();

                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"File '{filePath}' không tồn tại!");

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = Math.Min(worksheet.Dimension.Rows, 35); 

                    for (int row = 6; row <= rowCount; row++) 
                    {
                        int rowIndex = row - 6; 
                        string cellData = worksheet.Cells[row, 5].Text; 

                        string name = ExtractValue(cellData, "Tên:");
                        string email = ExtractValue(cellData, "Email:");
                        string birthday = ExtractValue(cellData, "Ngày sinh:");
                        string phone = ExtractValue(cellData, "SĐT:");
                        string idencard = ExtractValue(cellData, "CCCD:");
                        string avatar = ExtractValue(cellData, "Avatar Link:");
                        string actual = worksheet.Cells[row, 6].Text; 
                        string expect = worksheet.Cells[row, 7].Text; 
                        bool status = actual.Equals(expect); 

                        testCases.Add(new TestCaseData(rowIndex, name, email, birthday, phone, idencard, avatar, status));
                        worksheet.Cells[row, 8].Value = status; 

                    }
                }

                return testCases;
            }

            private static string ExtractValue(string data, string key)
            {
                int startIndex = data.IndexOf(key);
                if (startIndex == -1) return string.Empty;
                startIndex += key.Length;
                int endIndex = data.IndexOf('\n', startIndex);
                if (endIndex == -1) endIndex = data.Length;
                return data.Substring(startIndex, endIndex - startIndex).Trim();
            }

            private static int rowStart = 3; 
            private static int colIndexActual = 8; 

            public static void WriteResultToExcel(string filePath, string sheetName, int rowIndex, bool actuals, string result)
            {
                try
                {
                    Console.WriteLine($" Đang ghi kết quả vào file: {filePath}");
                    Console.WriteLine($" Sheet: {sheetName}, Row: {rowIndex}, Actual: {actuals}, Result: {result}");

                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine($" File không tồn tại: {filePath}");
                        return;
                    }

                    using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[sheetName];

                        if (worksheet == null)
                        {
                            Console.WriteLine($" Không tìm thấy sheet '{sheetName}'");
                            return;
                        }

                        int rowToWrite = rowStart + rowIndex;
                        Console.WriteLine($" Ghi giá trị {actuals} vào hàng {rowToWrite}, cột {colIndexActual}");
                        Console.WriteLine($" Ghi giá trị '{result}' vào hàng {rowToWrite}, cột {colIndexActual + 1}");

                        worksheet.Cells[rowToWrite, colIndexActual].Value = actuals;
                        worksheet.Cells[rowToWrite, colIndexActual].Value = result;

                        package.Save();
                        Console.WriteLine(" File đã được cập nhật!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" Lỗi khi ghi Excel: {ex.Message}");
                }
            }
        }

        public static class TestData
        {
            public static string LongName = new string('A', 150); 
            public static string LongEmail = new string('a', 95) + "@gmail.com"; 
        }

        public static IEnumerable<TestCaseData> GetTestCasesValid()
         {

             yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_01_ValidName");

             yield return new TestCaseData("1", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_03_NameWithNumber");

             yield return new TestCaseData("An1", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_04_NameWithLettersAndNumbers");

             yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_16_AgeOver16");

             yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\avatar_new.png")
                 .SetName("CSTT_35_UpdateAvatar");

             yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", null)
                 .SetName("CSTT_36_DeleteAvatar");

             yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", "")
                 .SetName("CSTT_39_NoAvatarSelected");
         } 
        
         public static IEnumerable<TestCaseData> GetTestCasesInvalid()
         {

             yield return new TestCaseData("", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_02_EmptyName");

             yield return new TestCaseData(TestData.LongName, "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_06_NameTooLong");

             yield return new TestCaseData("An#$#", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_07_NameWithSpecialChars");

             yield return new TestCaseData("An", "anthien", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_08_InvalidEmailFormat");

             yield return new TestCaseData("An", "", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_10_EmptyEmail");

             yield return new TestCaseData("An", TestData.LongEmail, "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_12_LongEmail");

             yield return new TestCaseData("An", "an123%^$@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_13_EmailWithSpecialChars");

             yield return new TestCaseData("An", "an....123@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_14_EmailWithMultipleDots");

             yield return new TestCaseData("An", "anthien@gmail.com", "2020-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_15_AgeUnder16");

             yield return new TestCaseData("An", "anthien@gmail.com", "1003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_17_AgeTooOld");

             yield return new TestCaseData("An", "anthien@gmail.com", "", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_18_EmptyDOB");

             yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "0123456", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_20_PhoneTooShort");

             yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "0123456123123", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_21_PhoneTooLong");

             yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "012345678a", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_22_PhoneWithLetter");

             yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "012345678@", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_23_PhoneWithSpecialChars");

             yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_25_EmptyPhone");

             yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "12312", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_28_CCCDTooShort");

             yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_29_CCCDTooLong");

             yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "12312@#", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_30_CCCDWithSpecialChars");

             yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "123 123 123 123", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_31_CCCDWithSpaces");

             yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "12312a", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_32_CCCDWithLetters");

             yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "", @"C:\\Users\\ADMIN\\Downloads\\cat.avif")
                 .SetName("CSTT_33_CCCDEmpty");

             yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\file.docx")
                 .SetName("CSTT_37_InvalidAvatarFormat");

             yield return new TestCaseData("An", "anthien@gmail.com", "2003-12-25", "1234567890", "123123123123", @"C:\\Users\\ADMIN\\Downloads\\avatar1.png;C:\\Users\\ADMIN\\Downloads\\avatar2.png")
                 .SetName("CSTT_38_MultipleAvatars");
         }


        public static IEnumerable<TestCaseData> GetTestCasesExcel()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "testcaseEdit.xlsx");
            return ExcelDataProvider.GetTestCasesFromExcel(filePath);
        }

        [Test, TestCaseSource(nameof(GetTestCasesExcel))]
        public void CSTT_Excel(int rowIndex, string name, string email, string birthday, string phone, string idencard, string avatar, bool expectedResult)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                driver.FindElement(By.XPath("//li[contains(@class, 'ant-menu-item')]/span/p[text()='Tài khoản']")).Click();
                wait.Until(d => d.Url.Contains("/owner/Profile"));

                var editButton = wait.Until(d => d.FindElement(By.XPath("//button[span[text()='Chỉnh sửa thông tin']]")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", editButton);
                editButton.Click();

                ClearAndType(By.Id("ownerName"), name);
                ClearAndType(By.Id("email"), email);
                ClearAndType(By.Id("birthday"), birthday);

                if (!birthday.Equals("")) { 
                string dateXpath = $"//td[@title='{birthday}']";
                driver.FindElement(By.XPath(dateXpath)).Click();
                }

                ClearAndType(By.Id("phoneNum"), phone);
                ClearAndType(By.Id("idenCard"), idencard);
                driver.FindElement(By.Id("avatarLink")).SendKeys(avatar);

                driver.FindElement(By.XPath("//button[contains(@class, 'ant-btn-primary')]/span[text()='Chỉnh sửa']")).Click();

                bool isSuccess = false;
                try
                {
                    IWebElement successMessage = wait.Until(d =>
                    {
                        var elements = d.FindElements(By.XPath(
                            "//div[contains(@class, 'font-bold') and contains(@class, 'text-[20px]') and contains(text(), 'Cập nhật thành công') and contains(@style, 'color: green')]"
                        ));
                        return elements.Count > 0 ? elements[0] : null;
                    });

                    isSuccess = successMessage != null;
                    Console.WriteLine();
                }
                catch (WebDriverTimeoutException)
                {
                    isSuccess = false; 
                }

                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "testcaseEdit.xlsx");
                ExcelDataProvider.WriteResultToExcel(filePath, "Bug", rowIndex, isSuccess, isSuccess == expectedResult ? "Passed" : "Failed");

                Assert.That(isSuccess, Is.EqualTo(expectedResult), $"Expected: {expectedResult}, but got: {isSuccess}");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xảy ra: " + ex.Message);
                Assert.Fail("Test case gặp lỗi: " + ex.Message);
            }
        }

        [Test, TestCaseSource(nameof(GetTestCasesValid))]
        public void CSTT_CapNhatThanhCong(string name, string email, string birthday, string phone, string idencard, string avatar)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                driver.FindElement(By.XPath("//li[contains(@class, 'ant-menu-item')]/span/p[text()='Tài khoản']")).Click();
                wait.Until(d => d.Url.Contains("/owner/Profile"));

                var editButton = wait.Until(d => d.FindElement(By.XPath("//button[span[text()='Chỉnh sửa thông tin']]")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", editButton);
                editButton.Click();

                ClearAndType(By.Id("ownerName"), name);

                ClearAndType(By.Id("email"), email);

                ClearAndType(By.Id("birthday"), birthday);


                if (!string.IsNullOrEmpty(birthday))
                {
                    string dateXpath = $"//td[@title='{birthday}']";
                    driver.FindElement(By.XPath(dateXpath)).Click();
                }


                ClearAndType(By.Id("phoneNum"), phone);

                ClearAndType(By.Id("idenCard"), idencard);

                driver.FindElement(By.Id("avatarLink")).SendKeys(avatar);

                driver.FindElement(By.XPath("//button[contains(@class, 'ant-btn-primary')]/span[text()='Chỉnh sửa']")).Click();
                Thread.Sleep(1000);

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
                    isSuccess = false; 
                }

                Assert.That(isSuccess, Is.True, "Cập nhật thông tin không thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xảy ra: " + ex.Message);
                Assert.Fail("Test case gặp lỗi: " + ex.Message);
            }
        }

        [Test, TestCaseSource(nameof(GetTestCasesInvalid))]
        public void CSTT_CapNhatThatBai(string name, string email, string birthday, string phone, string idencard, string avatar)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                driver.FindElement(By.XPath("//li[contains(@class, 'ant-menu-item')]/span/p[text()='Tài khoản']")).Click();
                wait.Until(d => d.Url.Contains("/owner/Profile"));

                var editButton = wait.Until(d => d.FindElement(By.XPath("//button[span[text()='Chỉnh sửa thông tin']]")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", editButton);
                editButton.Click();

                ClearAndType(By.Id("ownerName"), name);
                ClearAndType(By.Id("email"), email);
                ClearAndType(By.Id("birthday"), birthday);

                if (!string.IsNullOrEmpty(birthday))
                {
                    string dateXpath = $"//td[@title='{birthday}']";
                    driver.FindElement(By.XPath(dateXpath)).Click();
                }

                ClearAndType(By.Id("phoneNum"), phone);
                ClearAndType(By.Id("idenCard"), idencard);
                driver.FindElement(By.Id("avatarLink")).SendKeys(avatar);

                driver.FindElement(By.XPath("//button[contains(@class, 'ant-btn-primary')]/span[text()='Chỉnh sửa']")).Click();

                bool hasError = false;
                List<string> errorMessages = new List<string>();

                try
                {
                    wait.Until(d =>
                    {
                        var errors = d.FindElements(By.XPath("//div[contains(@class, 'ant-form-item-explain-error')]"));
                        if (errors.Count > 0)
                        {
                            hasError = true;
                            errorMessages = errors.Select(e => e.Text).Where(t => !string.IsNullOrEmpty(t)).ToList();
                            return true;
                        }
                        return false;
                    });
                }
                catch (WebDriverTimeoutException)
                {
                    hasError = false;
                }

                Console.WriteLine($"Số lượng lỗi tìm thấy: {errorMessages.Count}");
                foreach (var msg in errorMessages)
                {
                    Console.WriteLine($"Lỗi: {msg}");
                }

                Assert.That(hasError, Is.True, "Cập nhật không hợp lệ nhưng hệ thống vẫn cho phép cập nhật!");
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
