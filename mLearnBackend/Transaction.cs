//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace mLearnBackend
{
    using System;
    using System.Collections.Generic;
    
    public partial class Transaction
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Transaction()
        {
            this.RegisterdCourses = new HashSet<RegisterdCourse>();
            this.Orders = new HashSet<Order>();
        }
    
        public int id { get; set; }
        public string transactionid { get; set; }
        public string vendorTransactionId { get; set; }
        public Nullable<int> userId { get; set; }
        public Nullable<System.DateTime> transactiondate { get; set; }
        public Nullable<decimal> transactionAmount { get; set; }
        public string transactionStatus { get; set; }
        public string razorPayPaymentId { get; set; }
        public string razorPayOrderId { get; set; }
        public string razorPayRecieptId { get; set; }
        public Nullable<System.DateTime> TransactionCreationDate { get; set; }
        public Nullable<System.DateTime> TransactionUpdateDate { get; set; }
        public Nullable<int> orderId { get; set; }
        public Nullable<int> CustomerId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RegisterdCourse> RegisterdCourses { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual Customer Customer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
