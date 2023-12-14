using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMDT.Models
{
    public class KhuyenMaiDetails
    {
        public enum KhuyenMai
        {
            theoPhanTram,
            theoSoTien,
            mienPhiVanChuyen,
        }

        public static class KhuyenMaiHelper
        {
            public static string GetLoaiKhuyenMaiDisplayName(KhuyenMai khuyenMai)
            {
                switch (khuyenMai) {
                    case KhuyenMai.theoPhanTram:
                        return "Theo Phần Trăm";
                    case KhuyenMai.theoSoTien:
                        return "Theo Số Tiền";
                    case KhuyenMai.mienPhiVanChuyen:
                        return "Miễn Phí Vận Chuyển";
                    default:
                        return "";
                }
            }
        }
    }
}