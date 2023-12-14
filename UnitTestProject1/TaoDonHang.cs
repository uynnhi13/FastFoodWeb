using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UnitTestProject1
{
    [TestClass]
    public class TaoDonHang
    {
        IWebDriver driver;
        IWebElement buttonDangNhap, inputNPhone, inputPass, btLogin, addSP1;
        IWebElement btCart, btDatHang, btThanhToan, btAddSP1;

        [TestInitialize]
        public void Setup()
        {
            driver = new EdgeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://localhost:44322/KhachHang/Home/Index/");
            
        }

        [TestMethod]
        public void TaoDHVoiKhachHangThanThiet()
        {
            IWebElement iconUser = driver.FindElement(By.XPath("//i[@class='pe-7s-users']"));
            iconUser.Click();
            buttonDangNhap = driver.FindElement(By.XPath("//a[contains(text(),'Đăng Nhập')]"));
            buttonDangNhap.Click();
            inputNPhone = driver.FindElement(By.Id("numberPhone"));
            inputNPhone.SendKeys("68686868");
            inputPass = driver.FindElement(By.Id("password"));
            inputPass.SendKeys("123456");
            btLogin = driver.FindElement(By.XPath("//button/span"));
            btLogin.Click();
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("window.scrollTo(0, 1000)");
            addSP1 = driver.FindElement(By.XPath("//body/div/div[@class='product-area pt-100px']/div[@class='container']/div[@class='row']/div[@class='col']/div[@class='tab-content mt-60px']/div[@id='tab-fabric-2']/div[@class='row']/div[1]"));
            addSP1.Click();
            btAddSP1 = driver.FindElement(By.XPath("//button[contains(text(),'Thêm Sản Phẩm')]"));
            btAddSP1.Click();
            System.Threading.Thread.Sleep(5000);
            iconUser = driver.FindElement(By.XPath("//i[@class='pe-7s-users']"));
            iconUser.Click();
            System.Threading.Thread.Sleep(1000);
            btCart = driver.FindElement(By.XPath("//a[contains(text(),'Giỏ Hàng')]"));
            btCart.Click();
            btDatHang = driver.FindElement(By.CssSelector(".cart-clear > button"));
            btDatHang.Click();
            js.ExecuteScript("window.scrollTo(0, 300)");
            btThanhToan = driver.FindElement(By.XPath("//button[@id='submit']"));
            btThanhToan.Click();
            Console.WriteLine("Đã đặt hàng thành công");
            System.Threading.Thread.Sleep(3000);
            IWebElement btXemChiTiet = driver.FindElement(By.XPath("//a[@id='detailsOrder']"));
            btXemChiTiet.Click();
            System.Threading.Thread.Sleep(3000);
            IWebElement idDonHang = driver.FindElement(By.CssSelector("table:nth-child(2) td:nth-child(1)"));
            string idDonHangStr = idDonHang.Text;
            Console.WriteLine("Ma don hang: "+idDonHangStr);
            IWebElement tongTienDonHang = driver.FindElement(By.XPath("//td[@class='TongTien']"));
            string tongTienDonHangStr = tongTienDonHang.Text;
            Console.WriteLine("Tong Tien: "+tongTienDonHangStr);
            IWebElement btDongChiTietDH = driver.FindElement(By.XPath("//button[contains(text(),'Đóng')]"));
            btDongChiTietDH.Click();
            System.Threading.Thread.Sleep(3000);
            iconUser = driver.FindElement(By.XPath("//i[@class='pe-7s-users']"));
            iconUser.Click();
            System.Threading.Thread.Sleep(1000);
            IWebElement taiKhoan = driver.FindElement(By.XPath("//a[contains(text(),'Tài Khoản')]"));
            taiKhoan.Click();
            IWebElement donHanglst = driver.FindElement(By.XPath("//a[contains(text(),'Đơn hàng')]"));
            donHanglst.Click();
            js.ExecuteScript("window.scrollTo(0, 200)");
            System.Threading.Thread.Sleep(3000);
            bool isLastPage = false;

            while (!isLastPage)
            {
                try
                {
                    IWebElement chuyenPageCuoi = driver.FindElement(By.XPath("//a[normalize-space()='»']"));
                    chuyenPageCuoi.Click();
                    js.ExecuteScript("window.scrollTo(0, 200)");
                    System.Threading.Thread.Sleep(3000);
                }
                catch (NoSuchElementException)
                {
                    // Nếu không tìm thấy nút chuyển đến trang cuối cùng, coi đây là trang cuối cùng
                    isLastPage = true;
                }
            }


            IList<IWebElement> danhsachMaDon = driver.FindElements(By.CssSelector(".maDonHang"));
            string maDonMoiDatString = "";
            string giaDonMoiDatString = "";
            if (danhsachMaDon.Count > 0)
            {
                IWebElement maDonMoiDat = danhsachMaDon[danhsachMaDon.Count - 1];
                maDonMoiDatString = maDonMoiDat.Text;
                Console.WriteLine("Mã đơn mới đặt: " + maDonMoiDatString);
            }
            IList<IWebElement> danhSachGia = driver.FindElements(By.CssSelector(".tongTien"));
            if (danhSachGia.Count > 0)
            {
                IWebElement giaDonMoiDat = danhSachGia[danhSachGia.Count - 1];
                giaDonMoiDatString = giaDonMoiDat.Text;
                Console.WriteLine("Giá đơn mới đặt: " + giaDonMoiDatString);
            }
            if(string.Compare(idDonHangStr,maDonMoiDatString)==0 && string.Compare(giaDonMoiDatString, tongTienDonHangStr) == 0)
            {
                Console.WriteLine("Thành công: Mã đơn và giá của đơn hàng mới đặt trong tài khoản khách hàng bằng với mã đơn và giá của đơn hàng ở trang thanh toán vừa thực hiện");
            }
            else
            {
                Console.WriteLine("Thất bại: Mã đơn và giá của đơn hàng mới đặt trong tài khoản khách hàng không bằng với mã đơn và giá của đơn hàng ở trang thanh toán vừa thực hiện");
            }

            
            driver.Navigate().GoToUrl("https://localhost:44322/Admin/Admin/Login");
            IWebElement usernameInput = driver.FindElement(By.CssSelector("#user"));
            IWebElement passwordInput = driver.FindElement(By.CssSelector("#password"));
            IWebElement loginButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            usernameInput.SendKeys("leuyennhi@gmail.com");
            passwordInput.SendKeys("123456");

            loginButton.Click();
            System.Threading.Thread.Sleep(5000);

            IWebElement qlDonHang = driver.FindElement(By.XPath("//a[contains(text(),'Đơn hàng')]"));
            qlDonHang.Click();

            IJavaScriptExecutor jsp = (IJavaScriptExecutor)driver;
            List<IWebElement> allElementsDetails = new List<IWebElement>();
            List<IWebElement> allElementsDonHang = new List<IWebElement>();

            long initialHeight = (long)jsp.ExecuteScript("return document.body.scrollHeight");
            
            while (true)
            {
                // Tìm các phần tử trên trang hiện tại
                IReadOnlyCollection<IWebElement> currentElements = driver.FindElements(By.CssSelector(".btChiTiet"));
                allElementsDetails.AddRange(currentElements);
                IReadOnlyCollection<IWebElement> currentEle1 = driver.FindElements(By.CssSelector(".madonhang"));
                allElementsDonHang.AddRange(currentEle1);

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

            // In ra số lượng phần tử thu thập được
            Console.WriteLine($"Số lượng phần tử details: {allElementsDetails.Count}");
            IWebElement donHangMoiDatAdmin = allElementsDonHang[allElementsDonHang.Count - 1];
            string donHangMoiDatAdminStr = donHangMoiDatAdmin.Text;
            Console.WriteLine("Đơn Hàng mới đặt lấy ra từ admin: " + donHangMoiDatAdminStr);
            Console.WriteLine($"Số lượng phần tử đơn hàng: {allElementsDonHang.Count}");
            IWebElement linkDetails = allElementsDetails[allElementsDetails.Count - 1];
            linkDetails.Click();
            IWebElement giaTrongAdmin = driver.FindElement(By.CssSelector(".tongTien"));
            string giaTrongAdminStr = giaTrongAdmin.Text;
            Console.WriteLine("Giá mới đặt của đơn hàng được lấy trong admin: " + giaDonMoiDatString);

            if(string.Compare(donHangMoiDatAdminStr,maDonMoiDatString)==0 && string.Compare(giaTrongAdminStr, giaDonMoiDatString) == 0)
            {
                Console.WriteLine("Thành công: Mã đơn hàng và giá của đơn hàng mới đặt bằng với mã đơn hàng và giá của đơn hàng mới trong admin");
            }
            else
            {
                Console.WriteLine("Thất bại: Mã đơn hàng và giá của đơn hàng mới đặt không bằng với mã đơn hàng và giá của đơn hàng mới trong admin");
            }

            // Làm việc với danh sách các phần tử đã thu thập được ở đây

            driver.Quit();
            
        }

        [TestMethod]
        public void TaoDHVoiKhachHangVangLai()
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("window.scrollTo(0, 1000)");
            addSP1 = driver.FindElement(By.XPath("//body/div/div[@class='product-area pt-100px']/div[@class='container']/div[@class='row']/div[@class='col']/div[@class='tab-content mt-60px']/div[@id='tab-fabric-2']/div[@class='row']/div[1]"));
            addSP1.Click();
            btAddSP1 = driver.FindElement(By.XPath("//button[contains(text(),'Thêm Sản Phẩm')]"));
            btAddSP1.Click();
            System.Threading.Thread.Sleep(5000);
            IWebElement iconUser = driver.FindElement(By.XPath("//i[@class='pe-7s-users']"));
            iconUser.Click();
            System.Threading.Thread.Sleep(1000);
            btCart = driver.FindElement(By.XPath("//a[@class='dropdown-item'][contains(text(),'Giỏ Hàng')]"));
            btCart.Click();
            btDatHang = driver.FindElement(By.CssSelector(".cart-clear > button"));
            btDatHang.Click();
            
            IWebElement continueDatHang = driver.FindElement(By.XPath("//button[contains(text(),'Tiếp tục đặt hàng')]"));
            continueDatHang.Click();
            IWebElement tenDatHang=driver.FindElement(By.CssSelector("input[name='recipient']"));
            tenDatHang.SendKeys("Uyen Nhi Le");
            IWebElement SDT= driver.FindElement(By.CssSelector("input[name='numberPhone']"));
            SDT.SendKeys("65478392");
            IWebElement DiaChi= driver.FindElement(By.CssSelector("input[name='fullAddress']"));
            DiaChi.SendKeys("Vo truong toan");

            js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("window.scrollTo(0, 300)");
            btThanhToan = driver.FindElement(By.XPath("//button[contains(text(),'Đặt Hàng')]"));
            btThanhToan.Click();
            Console.WriteLine("Đã đặt hàng thành công");
            System.Threading.Thread.Sleep(3000);
            IWebElement btXemChiTiet = driver.FindElement(By.XPath("//a[@id='detailsOrder']"));
            btXemChiTiet.Click();
            System.Threading.Thread.Sleep(3000);
            IWebElement idDonHang = driver.FindElement(By.CssSelector("table:nth-child(2) td:nth-child(1)"));
            string idDonHangStr = idDonHang.Text;
            Console.WriteLine("Ma don hang: " + idDonHangStr);
            IWebElement tongTienDonHang = driver.FindElement(By.XPath("//td[@class='TongTien']"));
            string tongTienDonHangStr = tongTienDonHang.Text;
            Console.WriteLine("Tong Tien: " + tongTienDonHangStr);
            IWebElement btDongChiTietDH = driver.FindElement(By.XPath("//button[contains(text(),'Đóng')]"));
            btDongChiTietDH.Click();
            System.Threading.Thread.Sleep(3000);
            

            driver.Navigate().GoToUrl("https://localhost:44322/Admin/Admin/Login");
            IWebElement usernameInput = driver.FindElement(By.CssSelector("#user"));
            IWebElement passwordInput = driver.FindElement(By.CssSelector("#password"));
            IWebElement loginButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            usernameInput.SendKeys("leuyennhi@gmail.com");
            passwordInput.SendKeys("123456");

            loginButton.Click();
            System.Threading.Thread.Sleep(5000);

            IWebElement qlDonHang = driver.FindElement(By.XPath("//a[contains(text(),'Đơn hàng')]"));
            qlDonHang.Click();

            IJavaScriptExecutor jsp = (IJavaScriptExecutor)driver;
            List<IWebElement> allElementsDetails = new List<IWebElement>();
            List<IWebElement> allElementsDonHang = new List<IWebElement>();

            long initialHeight = (long)jsp.ExecuteScript("return document.body.scrollHeight");

            while (true)
            {
                // Tìm các phần tử trên trang hiện tại
                IReadOnlyCollection<IWebElement> currentElements = driver.FindElements(By.CssSelector(".btChiTiet"));
                allElementsDetails.AddRange(currentElements);
                IReadOnlyCollection<IWebElement> currentEle1 = driver.FindElements(By.CssSelector(".madonhang"));
                allElementsDonHang.AddRange(currentEle1);

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

            // In ra số lượng phần tử thu thập được
            Console.WriteLine($"Số lượng phần tử details: {allElementsDetails.Count}");
            IWebElement donHangMoiDatAdmin = allElementsDonHang[allElementsDonHang.Count - 1];
            string donHangMoiDatAdminStr = donHangMoiDatAdmin.Text;
            Console.WriteLine("Đơn Hàng mới đặt lấy ra từ admin: " + donHangMoiDatAdminStr);
            Console.WriteLine($"Số lượng phần tử đơn hàng: {allElementsDonHang.Count}");
            IWebElement linkDetails = allElementsDetails[allElementsDetails.Count - 1];
            linkDetails.Click();
            IWebElement giaTrongAdmin = driver.FindElement(By.CssSelector(".tongTien"));
            string giaTrongAdminStr = giaTrongAdmin.Text;
            Console.WriteLine("Giá mới đặt của đơn hàng được lấy trong admin: " + giaTrongAdminStr);

            if (string.Compare(donHangMoiDatAdminStr,idDonHangStr) == 0 && string.Compare(giaTrongAdminStr, tongTienDonHangStr) == 0)
            {
                Console.WriteLine("Thành công: Mã đơn hàng và giá của đơn hàng mới đặt bằng với mã đơn hàng và giá của đơn hàng mới trong admin");
            }
            else
            {
                Console.WriteLine("Thất bại: Mã đơn hàng và giá của đơn hàng mới đặt không bằng với mã đơn hàng và giá của đơn hàng mới trong admin");
            }

            driver.Quit();

        }


    }
}
