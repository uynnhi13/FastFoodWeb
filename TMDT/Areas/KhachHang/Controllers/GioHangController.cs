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
        
        // GET: KhachHang/GioHang
        public List<MatHangMua> LayGioHang()
        {
            List<MatHangMua> gioHang = Session["GioHang"] as List<MatHangMua>;

            //Nếu giỏ hàng chưa tồn tại thì tạo mới và đưa vào session
            if (gioHang == null)
            {
                gioHang = new List<MatHangMua>();
                Session["GioHang"] = gioHang;
            }
            return gioHang;
        }

        public ActionResult ThemSanPhamVaoGio(int cateID)
        {
            //Lấy giỏ hàng hiện tại
            List<MatHangMua> gioHang = LayGioHang();

            //Kiểm tra xem có tồn tại mặt hàng trong giỏ hay chưa
            //Nếu có thì tăng số lượng lên 1, ngược lại thì thêm vào giỏ
            MatHangMua sanPham = gioHang.FirstOrDefault(s => s.cateID == cateID);
            if(sanPham==null) //Sản phẩm chưa có trong giỏ
            {
                sanPham = new MatHangMua(cateID);
                gioHang.Add(sanPham);
                ViewBag.ThongBaoThemSP = "Thêm Sản Phẩm Thành Công";
            }    
            else
            {
                sanPham.soLuong++; //sản phẩm đã có trong giỏ thì tăng số lượng lên 1
            }
            return RedirectToAction("ChiTietSP", "Home", new { id = cateID });
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

        public ActionResult HienThiGioHang()
        {
            List<MatHangMua> gioHang = LayGioHang();

            //Nếu giỏ hàng trống thì trả về trang ban đầu
            if(gioHang==null || gioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.TongSL = TinhTongSL();
            ViewBag.TongTien = TinhTongTien();
            return View(gioHang);
        }

        public ActionResult XoaGioHang()
        {
            Session["GioHang"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}