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
    
    public partial class Notificationprnter
    {
        public int NotificationId { get; set; }
        public Nullable<int> OrderId { get; set; }
        public Nullable<int> PrintersId { get; set; }
        public Nullable<bool> isValid { get; set; }
        public Nullable<System.DateTime> createdDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
    
        public virtual Designer Designer { get; set; }
        public virtual Order Order { get; set; }
    }
}