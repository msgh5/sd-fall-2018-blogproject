using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GuiBlogProject.Models
{
    public class IndexPostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string MediaUrl { get; set; }
        public string UserEmail { get; set; }
    }
}