//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TMDT.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ComboDetail
    {
        public int comboID { get; set; }
        public int cateID { get; set; }
        public int quantity { get; set; }
        public Nullable<bool> sizeUP { get; set; }
    
        public virtual Combo Combo { get; set; }
        public virtual Product Product { get; set; }
        public ComboDetail() { }
        public ComboDetail(int comboID, int cateID, int quantity, bool? sizeUP)
        {
            this.comboID = comboID;
            this.cateID = cateID;
            this.quantity = quantity;
            this.sizeUP = sizeUP;
        }
        
    }

}
