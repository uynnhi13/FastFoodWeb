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
    
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.Address = new HashSet<Address>();
            this.Order = new HashSet<Order>();
        }
    
        public string numberPhone { get; set; }
        public string gmail { get; set; }
        public string password { get; set; }
        public string fullName { get; set; }
        public Nullable<System.DateTime> bDay { get; set; }
        public Nullable<bool> gender { get; set; }
        public Nullable<int> addressID { get; set; }
        public Nullable<bool> permission { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<decimal> DiemTichLuy { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Address> Address { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Order { get; set; }
    }
}
