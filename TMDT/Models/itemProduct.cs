using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;

namespace TMDT.Models
{
    public class itemProduct
    {
        public int producID {  get; set; }
        public string productName { get; set; }
        public int quantity { get; set; }
        public bool upSize { get; set; }

        public itemProduct() { }

        public itemProduct(int producID,string name, int quantity, bool upSize)
        {
            this.producID = producID;
            this.productName = name;
            this.quantity = quantity;
            this.upSize = upSize;
        }
    }
}