using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMDT.MauThietKe.Facade
{
    public class KhuyenMaiApDung
    {
        public decimal ApDungKhuyenMaiTheoPhanTram(decimal giaTriDonHang, decimal phanTramKhuyenMai, decimal giaTriGiamToiDa)
        {
            decimal giamGia = giaTriDonHang * (phanTramKhuyenMai / 100);

            // Kiểm tra xem giảm giá có vượt quá giá trị giảm tối đa không
            if (giamGia > giaTriGiamToiDa) {
                giamGia = giaTriGiamToiDa;
            }

            return giamGia;
        }

        public decimal ApDungKhuyenMaiTheoSoTien(decimal giaTriDonHang, decimal soTienGiam)
        {
            // Kiểm tra xem giảm giá có vượt quá giá trị đơn hàng không
            decimal giamGia = soTienGiam > giaTriDonHang ? giaTriDonHang : soTienGiam;

            return giamGia;
        }

        public decimal ApDungKhuyenMaiMienPhiVanChuyen(decimal phiVanChuyenToiDa, decimal phiShip, int Value)
        {
            decimal giamGia = 0;
            if (Value != 1) {
                giamGia = phiVanChuyenToiDa > phiShip ? 0 : (phiShip - phiVanChuyenToiDa);

                return giamGia;
            }
            return giamGia;
        }

        
    }
}