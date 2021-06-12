using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace mLearnBackend.Models
{
    public class FeedbackModel
    {
        public string feedback { get; set; }
        public int starRating { get; set; }

        [Required]
        public int userId { get; set; }
        [Required]
        public int courseId { get; set; }

        public string userName { get; set; }
    }
}