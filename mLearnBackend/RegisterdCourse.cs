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
    
    public partial class RegisterdCourse
    {
        public int registrationId { get; set; }
        public Nullable<int> userId { get; set; }
        public Nullable<int> courseId { get; set; }
        public Nullable<int> mLearnTopic { get; set; }
        public Nullable<bool> ismLearn { get; set; }
        public Nullable<System.DateTime> registartionDate { get; set; }
        public Nullable<int> transactionId { get; set; }
        public Nullable<int> trainerId { get; set; }
    
        public virtual CourseDetail CourseDetail { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual Transaction Transaction { get; set; }
        public virtual UserProfile UserProfile1 { get; set; }
    }
}
