using System;
using System.Linq;

namespace TMDT.Models
{
    public class MatHangMua
    {
        TMDTThucAnNhanhEntities db = new TMDTThucAnNhanhEntities();
        public int cateID { get; set; }
        public String name { get; set; }
        public String image { get; set; }
        public double price { get; set; }
        public int soLuong { get; set; }

        //Tính thành tiền = DonGia + SoLuong
        public double ThanhTien()
        {
            return soLuong * price;
        }

        public MatHangMua(int cateID)
        {
            this.cateID = cateID;

            //Tìm sản phẩm trong CSDL có mã id cần và gán cho mặt hàng được mua
            var sanPham = db.Product.Single(s => s.cateID == this.cateID);
            this.name = sanPham.name;
            this.image = sanPham.image;
            this.price = (Double)sanPham.price;
            this.soLuong = 1; //Số lương mua ban đầu của sp là 1 (cho lần click đầu)
        }
    }
}