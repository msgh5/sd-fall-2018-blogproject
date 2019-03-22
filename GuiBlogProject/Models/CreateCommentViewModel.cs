using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GuiBlogProject.Models
{
    public class CreateCommentViewModel
    {
        [Required]
        public string Body { get; set; }
    }
}