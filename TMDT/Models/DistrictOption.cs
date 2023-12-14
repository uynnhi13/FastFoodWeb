using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMDT.Models
{
    public class DistrictOption
    {
        public enum districtOption
        {
            quan1,
            quan2,
            quan3,
            quan4,
            quan5,
            quan6,
            quan7,
            quan8,
            quan9,
            quan10,
            quan11,
            quan12,
            govap,
            tanbinh,
        }

        public static class DistrictHelper
        {
            public static string GetDistrictDisplayName(districtOption district)
            {
                switch (district) {
                    case districtOption.quan1:
                        return "Quận 1";
                    case districtOption.quan2:
                        return "Quận 2";
                    case districtOption.quan3:
                        return "Quận 3";
                    case districtOption.quan4:
                        return "Quận 4";
                    case districtOption.quan5:
                        return "Quận 5";
                    case districtOption.quan6:
                        return "Quận 6";
                    case districtOption.quan7:
                        return "Quận 7";
                    case districtOption.quan8:
                        return "Quận 8";
                    case districtOption.quan9:
                        return "Quận 9";
                    case districtOption.quan10:
                        return "Quận 10";
                    case districtOption.quan11:
                        return "Quận 11";
                    case districtOption.quan12:
                        return "Quận 12";
                    case districtOption.govap:
                        return "Gò Vấp";
                    case districtOption.tanbinh:
                        return "Tân Bình";
                    // Thêm các trường hợp khác nếu cần
                    default:
                        return "";
                }
            }
        }
        public static class ShippingFeeCalculator
        {
            private static readonly Dictionary<String, decimal> ShippingFees = new Dictionary<String, decimal>()
            {
                { DistrictHelper.GetDistrictDisplayName(districtOption.quan1), 18000 }, // Phí ship cho Quận 1 là 10,000
                { DistrictHelper.GetDistrictDisplayName(districtOption.quan2), 18000 },
                { DistrictHelper.GetDistrictDisplayName(districtOption.quan3), 21000 },
                { DistrictHelper.GetDistrictDisplayName(districtOption.quan4), 18000 },
                { DistrictHelper.GetDistrictDisplayName(districtOption.quan5), 18000 },
                { DistrictHelper.GetDistrictDisplayName(districtOption.quan6), 24000 },
                { DistrictHelper.GetDistrictDisplayName(districtOption.quan7), 32000 },
                { DistrictHelper.GetDistrictDisplayName(districtOption.quan8), 24000 },
                { DistrictHelper.GetDistrictDisplayName(districtOption.quan9), 18000 },
                { DistrictHelper.GetDistrictDisplayName(districtOption.quan10), 18000 },
                { DistrictHelper.GetDistrictDisplayName(districtOption.quan11), 24000 },
                { DistrictHelper.GetDistrictDisplayName(districtOption.quan12), 18000 },
                { DistrictHelper.GetDistrictDisplayName(districtOption.govap), 32000 },
                { DistrictHelper.GetDistrictDisplayName(districtOption.tanbinh), 35000 }, // Phí ship cho Quận 2 là 20,000
                // Thêm phí ship cho các quận/huyện khác nếu cần
            };

            public static decimal GetShippingFee(string district)
            {
                if (ShippingFees.TryGetValue(district, out decimal shippingFee)) {
                    return shippingFee;
                }
                // Nếu không tìm thấy quận/huyện tương ứng, trả về phí mặc định hoặc 0
                return 0; // hoặc một giá trị mặc định khác nếu cần
            }
        }
    }

}