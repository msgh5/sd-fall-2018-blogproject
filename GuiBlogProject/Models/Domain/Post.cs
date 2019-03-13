﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GuiBlogProject.Models.Domain
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool Published { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string MediaUrl { get; set; }

        public Post()
        {
            DateCreated = DateTime.Now;
        }

        public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }
    }
}