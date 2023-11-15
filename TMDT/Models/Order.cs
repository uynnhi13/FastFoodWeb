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
    
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.OrderDetail = new HashSet<OrderDetail>();
        }
    
        public int orderID { get; set; }
        public string numberPhone { get; set; }
        public System.DateTime datetime { get; set; }
        public string note { get; set; }
        public int conditionID { get; set; }
        public decimal total { get; set; }
        public int employeeID { get; set; }
        public string fullAddress { get; set; }
        public Nullable<int> star { get; set; }
        public string comment { get; set; }
    
        public virtual Condition Condition { get; set; }
        public virtual Employees Employees { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
    }
}
