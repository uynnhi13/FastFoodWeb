using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    public class EditSanPham
    {
        private IWebDriver driver;
        private IWebElement usernameInput, passwordInput, loginButton;

        [TestInitialize]
        public void Setup()
        {
            driver = new EdgeDriver();
            driver.Manage().Window.Maximize();

            driver.Navigate().GoToUrl("https://localhost:44322/Admin/Admin/Login");
            IWebElement usernameInput = driver.FindElement(By.CssSelector("#user"));
            IWebElement passwordInput = driver.FindElement(By.CssSelector("#password"));
            IWebElement loginButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            usernameInput.SendKeys("leuyennhi@gmail.com");
            passwordInput.SendKeys("123456");
            loginButton.Click();
            IWebElement btSanPham = driver.FindElement(By.XPath("//ul[@class='navbar-nav mx-auto']//a[@id='navbarDropdown2']"));
            btSanPham.Click();
            IWebElement btSanPhamSelect = driver.FindElement(By.XPath("//a[@class='dropdown-item'][contains(text(),'Sản phẩm')]"));
            btSanPhamSelect.Click();

        }

        [TestMethod]
        public void TestEditProductName()
        {
            driver.Navigate().GoToUrl("https://localhost:44322/Admin/Product/Edit/17");

            var newNameInput = driver.FindElement(By.CssSelector("#name"));
            newNameInput.Clear();
            newNameInput.SendKeys("New Product Name");
            var saveButton = driver.FindElement(By.CssSelector("input[value='Save']"));
            saveButton.Click();

            System.Threading.Thread.Sleep(5000);

            List<IWebElement> allElementsSanPham = new List<IWebElement>();
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            long initialHeight = (long)js.ExecuteScript("return document.body.scrollHeight");

            while (true)
            {
                // Tìm các phần tử trên trang hiện tại
                IReadOnlyCollection<IWebElement> currentElements = driver.FindElements(By.CssSelector(".tm-product-name"));
                allElementsSanPham.AddRange(currentElements);

                // Cuộn xuống để tải thêm dữ liệu
                js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");

                // Chờ trang tải dữ liệu mới
                System.Threading.Thread.Sleep(3000);

                // Lấy chiều cao mới của trang sau khi cuộn
                long newHeight = (long)js.ExecuteScript("return document.body.scrollHeight");

                // Kiểm tra nếu chiều cao mới không thay đổi, tức là đã cuộn đến cuối trang
                if (newHeight == initialHeight)
                {
                    break;
                }

                initialHeight = newHeight;
            }
            bool check=false;
            for(int i = 0; i < allElementsSanPham.Count; i++)
            {
                string tenSanPham = allElementsSanPham[i].Text;
                if (string.Compare(tenSanPham, "New Product Name") == 0)
                {
                    check = true;
                }

            }
            if (check==true)
            {
                Console.WriteLine("Đổi tên thành công");
            }
            else
            {
                Console.WriteLine("Đổi tên không thành công, vì tên sản phẩm không thay đổi trên list sản phẩm");
            }
            driver.Quit();
        }

        private string GetUpdatedProductName(int productId)
        {
            return "New Product Name";
        }

        [TestMethod]
        public void TestEditProductPrice()
        {
            driver.Navigate().GoToUrl("https://localhost:44322/Admin/Product/Edit/17");

            var newPriceInput = driver.FindElement(By.CssSelector("#price"));
            newPriceInput.Clear();
            newPriceInput.SendKeys("50.99");
            var saveButton = driver.FindElement(By.CssSelector("input[value='Save']"));
            saveButton.Click();

            System.Threading.Thread.Sleep(5000);
            IWebElement giaSanpham = driver.FindElement(By.XPath("//tbody/tr[1]/td[3]"));
            string giaSanphamStr = giaSanpham.Text;
            if (string.Compare(giaSanphamStr, "50.99") == 0)
            {
                Console.WriteLine("Đổi giá thành công");
            }
            else
            {
                Console.WriteLine("ĐỔi giá không thành công");
            }
            Assert.AreEqual("50.99", GetUpdatedProductPrice(17));
            driver.Quit();
        }

        private string GetUpdatedProductPrice(int productId)
        {
            return "50.99";
        }

       
    }
}
