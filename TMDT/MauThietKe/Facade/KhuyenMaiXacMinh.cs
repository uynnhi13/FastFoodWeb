using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMDT.Models;

namespace TMDT.MauThietKe.Facade
{
    public class KhuyenMaiXacMinh
    {
        TMDTThucAnNhanhEntities db = new TMDTThucAnNhanhEntities();
        
        public bool XacMinhNgay(DateTime HienTai, int id)
        {
            var khuyenmai = db.KhuyenMai.FirstOrDefault(s => s.ID == id);
            if(khuyenmai!=null && khuyenmai.ThoiGianBatDau<=HienTai && HienTai <= khuyenmai.ThoiGianKetThuc) {
                return true;
            }
            return false;
        }

        public bool XacMinhDieuKien(int ID, double tongTien, int soLuong)
        {
            var khuyenmai = db.KhuyenMai.FirstOrDefault(s => s.ID == ID);
           
            if(khuyenmai.DieuKienApDung_DonHangToiThieu<=(decimal)tongTien && soLuong >= khuyenmai.DieuKienApDung_SoLuongToiThieu) {
                return true;
            }
            return false;
        }
    }
}