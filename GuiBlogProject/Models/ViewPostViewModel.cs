using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GuiBlogProject.Models
{
    public class ViewPostViewModel
    {
        public ViewPostViewModel()
        {
            Comments = new List<CommentPostViewModel>();
        }

        public string Title { get; set; }
        public string Body { get; set; }
        public string MediaUrl { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public List<CommentPostViewModel> Comments { get; set; }
    }

    public class CommentPostViewModel
    {
        public string Body { get; set; }
    }
}