using GuiBlogProject.Models;
using GuiBlogProject.Models.Domain;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GuiBlogProject.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext Context { get; set; }

        public CommentsController()
        {
            Context = new ApplicationDbContext();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(int id, CreateCommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var post = Context.Posts.First(p => p.Id == id);

            var comment = new Comment();
            comment.Body = model.Body;
            comment.DateCreated = DateTime.Now;
            comment.UserId = User.Identity.GetUserId();
            comment.PostId = id;

            Context.Comments.Add(comment);
            Context.SaveChanges();

            return RedirectToAction(nameof(PostController.ViewPost), new { slug = post.Slug });
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public ActionResult Edit(int id, EditCommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var comment = Context.Comments.FirstOrDefault(p => p.Id == id);

            comment.Body = model.Body;
            comment.UpdatedReason = model.UpdatedReason;
            comment.DateUpdated = DateTime.Now;

            var slug = comment.Post.Slug;

            Context.SaveChanges();

            return RedirectToAction(nameof(PostController.ViewPost), new { slug = slug });
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var comment = Context.Comments.FirstOrDefault(p => p.Id == id);

            var slug = comment.Post.Slug;

            Context.Comments.Remove(comment);
            Context.SaveChanges();

            return RedirectToAction(nameof(PostController.ViewPost), new { slug = slug });
        }
    }
}