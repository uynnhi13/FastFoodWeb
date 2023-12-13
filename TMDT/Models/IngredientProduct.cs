using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMDT.Models
{
    public class IngredientProduct
    {
        public IngredientProduct() { }
        public IngredientProduct(int id, double quantity)
        {
            this.id = id;
            this.quantity = quantity;
        }

        public void upQuantity(double _quantity)
        {
            this.quantity += _quantity;
        }

        public int id { get; set; }
        public double quantity { get; set; } = 0;
    }
}