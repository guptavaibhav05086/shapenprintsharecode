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
    
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.Meetings = new HashSet<Meeting>();
            this.NotificationDesigners = new HashSet<NotificationDesigner>();
            this.Notificationprnters = new HashSet<Notificationprnter>();
            this.TransactionsPAYOUTs = new HashSet<TransactionsPAYOUT>();
            this.orderDetails = new HashSet<orderDetail>();
        }
    
        public int orderId { get; set; }
        public Nullable<int> customerId { get; set; }
        public Nullable<int> deliveryAddress { get; set; }
        public string finalDesignFileAddress { get; set; }
        public Nullable<int> meetingId { get; set; }
        public Nullable<int> designerId { get; set; }
        public string totalOrderprice { get; set; }
        public string orderType { get; set; }
        public Nullable<int> printerId { get; set; }
        public string orderStatus { get; set; }
        public Nullable<int> pickupAddess { get; set; }
        public string deliveryVendorId { get; set; }
        public Nullable<int> transactionid { get; set; }
        public Nullable<System.DateTime> createdDate { get; set; }
        public Nullable<System.DateTime> updatedDate { get; set; }
        public Nullable<decimal> BasePrint { get; set; }
        public Nullable<decimal> DeliveryFees { get; set; }
        public Nullable<decimal> GST { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> DiscountedTotal { get; set; }
        public Nullable<bool> IsOrderCompleted { get; set; }
        public string userSourceFileAddress { get; set; }
        public string printerInvoiceAddress { get; set; }
        public string deliveryRefNumber { get; set; }
        public string DunzoTaskId { get; set; }
        public string DeliveryStatus { get; set; }
        public Nullable<decimal> DiscountedGST { get; set; }
    
        public virtual AddressPrinter AddressPrinter { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Designer Designer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Meeting> Meetings { get; set; }
        public virtual Meeting Meeting { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NotificationDesigner> NotificationDesigners { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Notificationprnter> Notificationprnters { get; set; }
        public virtual Printer Printer { get; set; }
        public virtual Transaction Transaction { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TransactionsPAYOUT> TransactionsPAYOUTs { get; set; }
        public virtual AddressUser AddressUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<orderDetail> orderDetails { get; set; }
    }
}
