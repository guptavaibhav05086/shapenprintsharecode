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
    
    public partial class TrainerCourseMapping
    {
        public int id { get; set; }
        public Nullable<int> trainerId { get; set; }
        public Nullable<int> courseId { get; set; }
    
        public virtual CourseDetail CourseDetail { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}
