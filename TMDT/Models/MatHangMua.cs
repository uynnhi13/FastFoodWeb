using System;
using System.Linq;

namespace TMDT.Models
{
    public class MatHangMua
    {
        TMDTThucAnNhanhEntities db = new TMDTThucAnNhanhEntities();
        public int ComboID { get; set; }
        public String name { get; set; }
        public String image { get; set; }
        public decimal price { get; set; }
        public String size { get; set; }
        public int soLuong { get; set; }
        public bool typeCombo { get; set; }

        //Tính thành tiền = DonGia + SoLuong
        public double ThanhTien()
        {
            return soLuong * (double)price;
        }

        public MatHangMua(int ComboID, string size)
        {
            this.ComboID = ComboID;

            //Tìm sản phẩm trong CSDL có mã id cần và gán cho mặt hàng được mua
            var sanPham = db.Combo.Single(s => s.comboID == this.ComboID);


            this.name = sanPham.nameCombo;
            this.typeCombo = sanPham.typeCombo;
            if (typeCombo == true) {
                this.image = sanPham.image;
                this.size = "Combo";
                this.price = sanPham.price;
            }

            else {
                var comboDetail = db.ComboDetail.FirstOrDefault(s => s.comboID == sanPham.comboID);
                var product = db.Product.FirstOrDefault(s => s.cateID == comboDetail.cateID);
                this.image = product.image;
                this.size = size;
            }
            this.price = sanPham.price;
            //Số lương mua ban đầu của sp là 1 (cho lần click đầu)
            this.soLuong = 1;
        }
    }
}