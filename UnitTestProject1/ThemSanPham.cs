using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System;

namespace UnitTestProject1
{
    [TestClass]
    public class ThemSanPham
    {
        private IWebDriver driver;
        private IWebElement usernameInput, passwordInput, loginButton;

        [TestInitialize]
        public void setup()
        {
            driver = new EdgeDriver();
            driver.Manage().Window.Maximize();

            driver.Navigate().GoToUrl("https://localhost:44322/Admin/Admin/Login");
            usernameInput = driver.FindElement(By.CssSelector("#user"));
            passwordInput = driver.FindElement(By.CssSelector("#password"));
            loginButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            usernameInput.SendKeys("leuyennhi@gmail.com");
            passwordInput.SendKeys("123456");

            loginButton.Click();
            System.Threading.Thread.Sleep(5000);
        }

        [TestMethod]
        public void TaoThucDon()
        {
            System.Threading.Thread.Sleep(3000);
            IWebElement btnsp = driver.FindElement(By.Id("navbarDropdown2"));
            btnsp.Click();
            IWebElement btnnl = driver.FindElement(By.XPath("//a[contains(text(),'Nguyên liệu')]"));
            btnnl.Click();
            IWebElement btnThucdon = driver.FindElement(By.XPath("//a[contains(text(),'Tạo thực đơn')]"));
            btnThucdon.Click();

            IWebElement quantityInput1 = driver.FindElement(By.XPath("//tbody/tr[1]/td[3]/div[1]/input[1]"));
            quantityInput1.SendKeys("2");
            IWebElement quantityInput2 = driver.FindElement(By.XPath("//tbody/tr[2]/td[3]/div[1]/input[1]"));
            quantityInput2.SendKeys("1");
            IWebElement btnTao = driver.FindElement(By.XPath("//a[contains(text(),'Tạo thực đơn')]"));
            btnTao.Click();

            IWebElement tenSP = driver.FindElement(By.XPath("//input[@name='name']"));
            tenSP.SendKeys("Ăn trưa cùng nhau");
            IWebElement thanhTien = driver.FindElement(By.XPath("//input[@name='price']"));
            thanhTien.SendKeys("105000");
            IWebElement upSize = driver.FindElement(By.XPath("//input[@name='priceUp']"));
            upSize.SendKeys("30000");

            IWebElement loaiSP = driver.FindElement(By.XPath("//select[@name='typeID']"));
            loaiSP.Click();
            {
                var dropdown = driver.FindElement(By.Name("typeID"));
                dropdown.FindElement(By.XPath("//option[. = 'Mì Ý']")).Click();
            }
            IWebElement btn_off = driver.FindElement(By.CssSelector(".col-md-12"));
            btn_off.Click();

            IWebElement imgInput = driver.FindElement(By.XPath("//input[@id='imageInput']"));
            /*imgInput.Click();*/
            System.Threading.Thread.Sleep(5000); // 10000 milliseconds = 10 giây
            imgInput.SendKeys(@"D:\HUFLIT\Thương mại điện tử\picture\z4889396963904_67d8df257d39bef71db0f36e5708e86c.jpg");
            

            IWebElement btnCreate = driver.FindElement(By.XPath("//input[@value='Create']"));
            btnCreate.Click();

        }
        [TestMethod]
        public void TaoCombo()
        {
            System.Threading.Thread.Sleep(3000);
            IWebElement btnsp = driver.FindElement(By.Id("navbarDropdown2"));
            btnsp.Click();
            IWebElement btnProduct = driver.FindElement(By.XPath("//a[@class='dropdown-item'][contains(text(),'Sản phẩm')]"));
            btnProduct.Click();
            IWebElement btnThemcb = driver.FindElement(By.XPath(" //a[normalize-space()='Thêm combo']"));
            btnThemcb.Click();

            IWebElement quantityInput1 = driver.FindElement(By.XPath("//tbody/tr[7]/td[3]/div[1]/input[1]"));
            quantityInput1.SendKeys("1");
            IWebElement quantityInput2 = driver.FindElement(By.XPath("//body[1]/div[2]/div[1]/div[2]/div[1]/div[1]/div[2]/form[1]/table[1]/tbody[1]/tr[9]/td[3]/div[1]/input[1]"));
            quantityInput2.SendKeys("1");
            IWebElement btnChitiet = driver.FindElement(By.XPath("//button[contains(text(),'Xem chi tiết')]"));
            btnChitiet.Click();
            IWebElement btnTao = driver.FindElement(By.XPath("//button[@id='btnCreateCombo']"));
            btnTao.Click();

            IWebElement tenCB = driver.FindElement(By.XPath("//input[@name='nameCombo']"));
            tenCB.SendKeys("Combo vui vẻ");
            IWebElement sale = driver.FindElement(By.XPath(" //input[@name='sale']"));
            sale.SendKeys("99000");
            IWebElement imgInput = driver.FindElement(By.XPath("//input[@id='imageInput']"));
            /*imgInput.Click();*/
            System.Threading.Thread.Sleep(5000); // 10000 milliseconds = 10 giây
            imgInput.SendKeys(@"D:\HUFLIT\Thương mại điện tử\picture\z4889396963904_67d8df257d39bef71db0f36e5708e86c.jpg");

            IWebElement btnCreate = driver.FindElement(By.XPath("//input[@value='Create']"));
            btnCreate.Click();
        }

        [TestMethod]
        public void TaoThucDonWithoutQuantity()
        {
            //Vào trang nguyên liệu tạo thực đơn 
            System.Threading.Thread.Sleep(3000);
            IWebElement btnsp = driver.FindElement(By.Id("navbarDropdown2"));
            btnsp.Click();
            IWebElement btnnl = driver.FindElement(By.XPath("//a[contains(text(),'Nguyên liệu')]"));
            btnnl.Click();
            IWebElement btnThucdon = driver.FindElement(By.XPath("//a[contains(text(),'Tạo thực đơn')]"));
            btnThucdon.Click();

            IWebElement quantityInput1 = driver.FindElement(By.XPath("//tbody/tr[1]/td[3]/div[1]/input[1]"));
            IWebElement quantityInput2 = driver.FindElement(By.XPath("//tbody/tr[2]/td[3]/div[1]/input[1]"));

            // Kiểm tra nếu quantity1 hoặc quantity2 không được nhập thì hiển thị thông báo chưa nhập 
            if (string.IsNullOrEmpty(quantityInput1.GetAttribute("value")) || string.IsNullOrEmpty(quantityInput2.GetAttribute("value")))
            {
                Console.WriteLine("Chưa nhập số lượng ");
            }
            else
            {

                quantityInput1.SendKeys("1");
                quantityInput2.SendKeys("1");
            }
        }

        [TestMethod]
        public void TaoComboWithoutQuantity()
        {
            System.Threading.Thread.Sleep(3000);
            IWebElement btnsp = driver.FindElement(By.Id("navbarDropdown2"));
            btnsp.Click();
            IWebElement btnProduct = driver.FindElement(By.XPath("//a[@class='dropdown-item'][contains(text(),'Sản phẩm')]"));
            btnProduct.Click();
            IWebElement btnThemcb = driver.FindElement(By.XPath(" //a[normalize-space()='Thêm combo']"));
            btnThemcb.Click();

            IWebElement quantityInput1 = driver.FindElement(By.XPath("//tbody/tr[1]/td[3]/div[1]/input[1]"));
            IWebElement quantityInput2 = driver.FindElement(By.XPath("//tbody/tr[2]/td[3]/div[1]/input[1]"));

            // Kiểm tra nếu quantity1 hoặc quantity2 không được nhập thì hiển thị thông báo chưa nhập 
            if (string.IsNullOrEmpty(quantityInput1.GetAttribute("value")) || string.IsNullOrEmpty(quantityInput2.GetAttribute("value")))
            {
                Console.WriteLine("Chưa nhập số lượng ");
            }
            else
            {
                // Tiếp tục với các bước khác nếu cần thiết
                quantityInput1.SendKeys("1");
                quantityInput2.SendKeys("1");
            }
        }

        [TestMethod]
        public void TaoThucDonWithoutInfor()
        {

            System.Threading.Thread.Sleep(3000);
            IWebElement btnsp = driver.FindElement(By.Id("navbarDropdown2"));
            btnsp.Click();
            IWebElement btnnl = driver.FindElement(By.XPath("//a[contains(text(),'Nguyên liệu')]"));
            btnnl.Click();
            IWebElement btnThucdon = driver.FindElement(By.XPath("//a[contains(text(),'Tạo thực đơn')]"));
            btnThucdon.Click();

            IWebElement quantityInput1 = driver.FindElement(By.XPath("//tbody/tr[1]/td[3]/div[1]/input[1]"));
            quantityInput1.SendKeys("2");
            IWebElement quantityInput2 = driver.FindElement(By.XPath("//tbody/tr[2]/td[3]/div[1]/input[1]"));
            quantityInput2.SendKeys("1");
            IWebElement btnTao = driver.FindElement(By.XPath("//a[contains(text(),'Tạo thực đơn')]"));
            btnTao.Click();

            IWebElement tenSP = driver.FindElement(By.XPath("//input[@name='name']"));
            IWebElement thanhTien = driver.FindElement(By.XPath("//input[@name='price']"));
            IWebElement upSize = driver.FindElement(By.XPath("//input[@name='priceUp']"));
            IWebElement loaiSP = driver.FindElement(By.XPath("//select[@name='typeID']"));
            IWebElement imgInput = driver.FindElement(By.XPath("//input[@id='imageInput']"));
            IWebElement btn_off = driver.FindElement(By.CssSelector(".col-md-12"));

            IWebElement btnCreate = driver.FindElement(By.XPath("//input[@value='Create']"));
            // Kiểm tra nếu một trong các input không được nhập thì hiển thị thông báo chưa nhập thông tin 
            if (string.IsNullOrEmpty(tenSP.GetAttribute("value")) ||
                string.IsNullOrEmpty(thanhTien.GetAttribute("value")) ||
                string.IsNullOrEmpty(upSize.GetAttribute("value")) ||
                string.IsNullOrEmpty(loaiSP.GetAttribute("value")))
            {
                Console.WriteLine("Chưa nhập thông tin sản phẩm");

            }
            else
            {
                tenSP.SendKeys("Ăn trưa cùng nhau");
                thanhTien.SendKeys("105000");
                upSize.SendKeys("30000");

                loaiSP.Click();
                {
                    var dropdown = driver.FindElement(By.Name("typeID"));
                    dropdown.FindElement(By.XPath("//option[. = 'mì']")).Click();
                }


                btn_off.Click();


                System.Threading.Thread.Sleep(5000);
                imgInput.SendKeys(@"D:\HUFLIT\Thương mại điện tử\picture\z4889396963904_67d8df257d39bef71db0f36e5708e86c.jpg");

                btnCreate.Click();
            }

        }

        [TestMethod]
        public void TaoComboWithoutInfor()
        {
            System.Threading.Thread.Sleep(3000);
            IWebElement btnsp = driver.FindElement(By.Id("navbarDropdown2"));
            btnsp.Click();
            IWebElement btnProduct = driver.FindElement(By.XPath("//a[@class='dropdown-item'][contains(text(),'Sản phẩm')]"));
            btnProduct.Click();
            IWebElement btnThemcb = driver.FindElement(By.XPath(" //a[normalize-space()='Thêm combo']"));
            btnThemcb.Click();

            IWebElement quantityInput1 = driver.FindElement(By.XPath("//tbody/tr[1]/td[3]/div[1]/input[1]"));
            IWebElement quantityInput2 = driver.FindElement(By.XPath("//tbody/tr[2]/td[3]/div[1]/input[1]"));
            quantityInput1.SendKeys("1");
            quantityInput2.SendKeys("1");
            IWebElement btnChitiet = driver.FindElement(By.XPath("//button[contains(text(),'Mở Modal')]"));
            btnChitiet.Click();
            IWebElement btnTao = driver.FindElement(By.XPath("//button[@id='btnCreateCombo']"));
            btnTao.Click();

            IWebElement tenCB = driver.FindElement(By.XPath("//input[@name='nameCombo']"));
            IWebElement sale = driver.FindElement(By.XPath(" //input[@name='sale']"));
            IWebElement imgInput = driver.FindElement(By.XPath("//input[@id='imageInput']"));
            IWebElement btnCreate = driver.FindElement(By.XPath("//input[@value='Create']"));
            // Kiểm tra nếu một trong các input không được nhập thì hiển thị thông báo chưa nhập thông tin 
            if (string.IsNullOrEmpty(tenCB.GetAttribute("value")) ||
                string.IsNullOrEmpty(sale.GetAttribute("value")) ||
                string.IsNullOrEmpty(imgInput.GetAttribute("value")))
            {
                Console.WriteLine("Chưa nhập thông tin sản phẩm");
            }
            else
            {
                // Tiếp tục với các bước khác nếu cần thiết
                tenCB.SendKeys("Combo vui vẻ");
                sale.SendKeys("99000");
                System.Threading.Thread.Sleep(5000); // Đợi 5 giây
                imgInput.SendKeys(@"D:\Bàn làm việc\Moon\Moon thủy thủ.png");


                btnCreate.Click();
            }
        }
    }


}
