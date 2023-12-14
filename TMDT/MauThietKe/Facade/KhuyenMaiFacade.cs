using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMDT.Models;
namespace TMDT.MauThietKe.Facade
{
    public class KhuyenMaiFacade
    {
        TMDTThucAnNhanhEntities db = new TMDTThucAnNhanhEntities();
        private KhuyenMaiXacMinh xacMinh = new KhuyenMaiXacMinh();
        private KhuyenMaiApDung apdung = new KhuyenMaiApDung();
        
        public bool KhuyenMaiSuDungDuoc(double tongTien, int soLuong, int ID)
        {
            var khuyenmai = db.KhuyenMai.FirstOrDefault(s => s.ID == ID);
            if (xacMinh.XacMinhDieuKien(ID, tongTien,soLuong)) {
                DateTime hientai = DateTime.Now;
                return xacMinh.XacMinhNgay(hientai,ID);
            }
            return false;
        }

        public decimal thongTinKhuyenMai(int id, decimal total, decimal phiShip, int Value)
        {
            var khuyenmai = db.KhuyenMai.FirstOrDefault(s => s.ID == id);
            if(khuyenmai.LoaiKhuyenMai== "Theo Phần Trăm") {
                return apdung.ApDungKhuyenMaiTheoPhanTram(total,(decimal)khuyenmai.GiaTriKhuyenMai,(decimal)khuyenmai.GiaTriGiamToiDa);
            } else if(khuyenmai.LoaiKhuyenMai== "Theo Số Tiền") {
                return apdung.ApDungKhuyenMaiTheoSoTien(total,(decimal)khuyenmai.SoTienGiam);
            }else if(khuyenmai.LoaiKhuyenMai== "Miễn Phí Vận Chuyển") {
                return apdung.ApDungKhuyenMaiMienPhiVanChuyen((decimal)khuyenmai.SoTienGiam, phiShip, Value);
            }

            return 0;
        }
    }
}