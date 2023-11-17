using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using log4net;
using TMDT.Models;
using static TMDT.Models.Payments.VNPayLibrary;

namespace TMDT.Areas.KhachHang.Controllers
{
    public class GioHangController : Controller
    {
        TMDTThucAnNhanhEntities db = new TMDTThucAnNhanhEntities();
        // GET: KhachHang/GioHang
        public List<MatHangMua> LayGioHang()
        {
            List<MatHangMua> gioHang = Session["GioHang"] as List<MatHangMua>;

            //Nếu giỏ hàng chưa tồn tại thì tạo mới và đưa vào session
            if (gioHang == null) {
                gioHang = new List<MatHangMua>();
                Session["GioHang"] = gioHang;
            }
            return gioHang;
        }

        public ActionResult GioHangTrong()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ThemSanPhamVaoGio(int comboID, int soLuong, string size)
        {
            //Lấy giỏ hàng hiện tại
            List<MatHangMua> gioHang = LayGioHang();

            //Kiểm tra xem có tồn tại mặt hàng trong giỏ hay chưa
            //Nếu có thì tăng số lượng lên 1, ngược lại thì thêm vào giỏ
            MatHangMua sanPham = gioHang.FirstOrDefault(s => s.ComboID == comboID && s.size == size);

            if (sanPham == null) //Sản phẩm chưa có trong giỏ
            {
                sanPham = new MatHangMua(comboID, size);
                gioHang.Add(sanPham);
                sanPham.soLuong = soLuong;
            }
            else {
                sanPham.soLuong = sanPham.soLuong + soLuong; //sản phẩm đã có trong giỏ thì tăng số lượng lên 1
            }

            var jsonData = new {
                redirectToAction = true,
                action = "ChiTietSP",
                controller = "Home",
                routeValues = new { id = comboID }
            };
            //return RedirectToAction("ChiTietSP", "Home", new { id = cateID });
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        private int TinhTongSL()
        {
            int tongSL = 0;
            List<MatHangMua> gioHang = LayGioHang();
            if (gioHang != null)
                tongSL = gioHang.Sum(sp => sp.soLuong);
            return tongSL;
        }

        private double TinhTongTien()
        {
            double tongTien = 0;
            List<MatHangMua> gioHang = LayGioHang();
            if (gioHang != null)
                tongTien = gioHang.Sum(sp => sp.ThanhTien());
            return tongTien;
        }
        [HttpPost]
        public ActionResult CapNhatMatHang(int comboID, string size, int soLuong)
        {
            List<MatHangMua> gioHang = LayGioHang();
            var sanPham = gioHang.FirstOrDefault(s => s.ComboID == comboID && s.size == size);
            if (sanPham != null) {
                sanPham.soLuong = soLuong;
            }
            double thanhTien = sanPham.soLuong * (double)sanPham.price;
            int tongSL = TinhTongSL();
            double tongTien = TinhTongTien();
            return Json(new { success = true, tongSL = tongSL, tongTien = tongTien, thanhTien = thanhTien });
        }

        public ActionResult HienThiGioHang()
        {
            List<MatHangMua> gioHang = LayGioHang();

            //Nếu giỏ hàng trống thì trả về trang ban đầu
            if (gioHang == null || gioHang.Count == 0) {
                return RedirectToAction("GioHangTrong", "GioHang");
            }
            ViewBag.TongSL = TinhTongSL();
            ViewBag.TongTien = TinhTongTien();
            return View(gioHang);
        }

        public ActionResult GioHangThuGon()
        {
            List<MatHangMua> gioHang = LayGioHang();

            ViewBag.TongSL = TinhTongSL();
            ViewBag.TongTien = TinhTongTien();
            return PartialView(gioHang);
        }

        public ActionResult XoaGioHang()
        {
            Session["GioHang"] = null;
            return RedirectToAction("GioHangTrong", "GioHang");
        }


        [HttpPost]
        public JsonResult XoaSanPham(int comboID, string size)
        {
            // Lấy giỏ hàng hiện tại
            List<MatHangMua> gioHang = LayGioHang();

            // Tìm sản phẩm trong giỏ hàng dựa trên comboID và size
            MatHangMua sanPham = gioHang.FirstOrDefault(s => s.ComboID == comboID && s.size.Contains(size));

            if (sanPham != null) {
                // Xóa sản phẩm khỏi giỏ hàng
                gioHang.Remove(sanPham);
                // Tính tổng số lượng
                int tongSL = TinhTongSL();
                if (tongSL == 0) {
                    return Json(new { success = false });
                }

                // Tính tổng tiền
                double tongTien = TinhTongTien();
                return Json(new { success = true, tongSL = tongSL, tongTien = tongTien });
            }

            return Json(new { success = false });
        }



        [HttpPost]
        public JsonResult DatHang()
        {
            if (Session["TaiKhoan"] == null) {
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }

        public ActionResult ThongTinDatHang()
        {
            List<MatHangMua> gioHang = LayGioHang();
            ViewBag.TongSL = TinhTongSL();
            ViewBag.TongTien = TinhTongTien();
            return View(gioHang);
        }

        public ActionResult DongYDatHang(FormCollection form)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            List<MatHangMua> gioHang = LayGioHang();
            Order donHang = new Order();
            
            var user = (User)Session["TaiKhoan"];
            if (user != null) {
                donHang.numberPhone = user.numberPhone;
                donHang.recipientsNumber = form["numberPhone"];
            } else {
                var number = form["numberPhone"];
                var userCheck = db.User.FirstOrDefault(s => s.numberPhone == number);
                if (userCheck == null) {
                    userCheck = new User();
                    userCheck.numberPhone = number;
                    db.User.Add(userCheck);
                    donHang.numberPhone = form["numberPhone"];
                    donHang.recipientsNumber= form["numberPhone"];
                }
            }
            donHang.recipient = form["recipient"];
            donHang.fullAddress = form["fullAddress"];
            donHang.datetime = DateTime.Now;
            donHang.note = form["message"];
            donHang.conditionID = 1;
            donHang.total = (decimal)TinhTongTien();
            var hinhThucThanhToan = form["HinhThucThanhToan"];
            if (hinhThucThanhToan == "1") {
                donHang.TypePayment = 1;
            }
            else donHang.TypePayment = 2;
            if (donHang.TypePayment == 2) {
                var phuongThucThanhToan = form["TypePaymentVN"];

                if (phuongThucThanhToan == "0") {
                    donHang.TypePaymentVN = 0;
                }
                else if (phuongThucThanhToan == "1") {
                    donHang.TypePaymentVN = 1;
                }
                else if (phuongThucThanhToan == "2") {
                    donHang.TypePaymentVN = 2;
                }
                else {
                    donHang.TypePaymentVN = 3;
                }

            }
            db.Order.Add(donHang);
            db.SaveChanges();

            //Thêm chi tiết cho từng sản phẩm
            foreach (var sanpham in gioHang) {
                OrderDetail chiTiet = new OrderDetail();
                chiTiet.orderID = donHang.orderID;
                chiTiet.comboID = sanpham.ComboID;
                chiTiet.quantity = sanpham.soLuong;
                db.OrderDetail.Add(chiTiet);
            }
            db.SaveChanges();
            code = new { Success = true, Code = 1, Url = "" };
            Session["Order"] = donHang;
            //Xóa giỏ hàng
            Session["GioHang"] = null;
            if (donHang.TypePaymentVN != null) {
                var url = UrlPayMent((int)donHang.TypePaymentVN, donHang.orderID);
                code = new { Success = true, Code = 2, Url = url };
            }
            return Json(code);
        } 

        public ActionResult DatHangThanhCong()
        {
            var order = Session["Order"] as Order;
            return View(order);
        }

        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(int typePaymentVN, int maDH)
        {

        }

        //Thanh Toán VNPay
        // GET: KhachHang/ShoppingCart
        public string UrlPayMent(int typePaymentVN, int maDH)
        {
            var urlPayment = "";
            var donHang = db.Order.FirstOrDefault(s => s.orderID == maDH);

            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = donHang.total*100;
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (donHang.total*100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (typePaymentVN == 1) {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (typePaymentVN == 2) {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (typePaymentVN == 3) {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            vnpay.AddRequestData("vnp_CreateDate", donHang.datetime.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng:" + donHang.orderID);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", donHang.orderID.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            Session["Order"] = donHang;
            return urlPayment;
        }

        public ActionResult VNPayReturn()
        {
            log.InfoFormat("Begin VNPAY Return, URL={0}", Request.RawUrl);
            if (Request.QueryString.Count > 0) {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData) {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_")) {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                //vnp_TxnRef: Ma don hang merchant gui VNPAY tai command=pay    
                //vnp_TransactionNo: Ma GD tai he thong VNPAY
                //vnp_ResponseCode:Response code from VNPAY: 00: Thanh cong, Khac 00: Xem tai lieu
                //vnp_SecureHash: HmacSHA512 cua du lieu tra ve

                long orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature) {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00") {
                        //Thanh toan thanh cong
                        ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                        log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
                        var order = Session["Order"] as Order;

                        return View(order);
                    }
                    else {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                        log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
                    }
                    //displayTmnCode.InnerText = "Mã Website (Terminal ID):" + TerminalID;
                    //displayTxnRef.InnerText = "Mã giao dịch thanh toán:" + orderId.ToString();
                    //displayVnpayTranNo.InnerText = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();
                    //displayAmount.InnerText = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
                    //displayBankCode.InnerText = "Ngân hàng thanh toán:" + bankCode;
                }
            }
            return View();
        }
    }
}  