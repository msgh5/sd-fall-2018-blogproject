using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GuiBlogProject.Models
{
    public class CreateEditPostViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        [AllowHtml]
        public string Body { get; set; }
        
        public bool Published { get; set; }

        public HttpPostedFileBase FileUpload { get; set; }

        public string MedialUrl { get; set; }
    }
}