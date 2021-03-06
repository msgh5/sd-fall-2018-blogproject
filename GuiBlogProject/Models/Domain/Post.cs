﻿using System;
using System.Collections.Generic;

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
        public string Slug { get; set; }

        public Post()
        {
            DateCreated = DateTime.Now;
        }

        public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }

        public virtual List<Comment> Comments { get; set; }
    }
}