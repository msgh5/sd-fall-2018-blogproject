using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GuiBlogProject.Models
{
    public class EditCommentViewModel
    {
        [Required]
        public string Body { get; set; }

        [Required]
        public string UpdatedReason { get; set; }
    }
}