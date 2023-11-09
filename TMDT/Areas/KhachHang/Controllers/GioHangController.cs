using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;

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

        [HttpPost]
        public ActionResult ThemSanPhamVaoGio(int comboID, int soLuong, string size)
        {
            //Lấy giỏ hàng hiện tại
            List<MatHangMua> gioHang = LayGioHang();

            //Kiểm tra xem có tồn tại mặt hàng trong giỏ hay chưa
            //Nếu có thì tăng số lượng lên 1, ngược lại thì thêm vào giỏ
            MatHangMua sanPham = gioHang.FirstOrDefault(s => s.ComboID == comboID&&s.size==size);

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
        public ActionResult CapNhatMatHang(int comboID,string size, int soLuong)
        {
            List<MatHangMua> gioHang = LayGioHang();
            var sanPham = gioHang.FirstOrDefault(s => s.ComboID == comboID && s.size == size);
            if (sanPham != null) {
                sanPham.soLuong = soLuong;
            }
            double thanhTien = sanPham.soLuong * (double)sanPham.price;
            int tongSL = TinhTongSL();
            double tongTien = TinhTongTien();
            return Json(new { success = true, tongSL = tongSL, tongTien = tongTien, thanhTien=thanhTien });
        }

        public ActionResult HienThiGioHang()
        {
            List<MatHangMua> gioHang = LayGioHang();

            //Nếu giỏ hàng trống thì trả về trang ban đầu
            if (gioHang == null || gioHang.Count == 0) {
                return RedirectToAction("Index", "Home");
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
            return RedirectToAction("Index", "Home");
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

    }
}