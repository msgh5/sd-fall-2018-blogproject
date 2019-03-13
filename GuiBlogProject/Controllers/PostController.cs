using GuiBlogProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using GuiBlogProject.Models.Domain;
using System.IO;
using Microsoft.AspNet.Identity;

namespace GuiBlogProject.Controllers
{   
    public class PostController : Controller
    {
        private List<string> AllowedExtenions = new List<string>
                { ".jpeg", ".jpg", ".gif", ".png" };

        private ApplicationDbContext Context { get; set; }

        public PostController()
        {
            Context = new ApplicationDbContext();
        }
        
        public ActionResult Index()
        {
            var posts = Context.Posts
                .Select(p => new IndexPostViewModel
                {
                   Id = p.Id,
                   Body = p.Body,
                   MediaUrl = p.MediaUrl,
                   Title = p.Title,
                   UserEmail = p.User.Email
                })
                .ToList();

            return View(posts);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(CreateEditPostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var fileExtension = Path.GetExtension(model.FileUpload.FileName).ToLower();

            if (!AllowedExtenions.Contains(fileExtension))
            {
                ModelState.AddModelError("", "File extension is not allowed");
                return View();
            }

            var userId = User.Identity.GetUserId();

            var post = new Post();
            post.UserId = userId;
            post.Body = model.Body;
            post.Title = model.Title;
            post.Published = model.Published;
            post.MediaUrl = UploadFile(model.FileUpload);

            Context.Posts.Add(post);
            Context.SaveChanges();

            return RedirectToAction(nameof(PostController.Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(PostController.Index));
            }

            var post = Context.Posts.FirstOrDefault(p => p.Id == id.Value);

            if (post == null)
            {
                return RedirectToAction(nameof(PostController.Index));
            }
            
            var model = new CreateEditPostViewModel();
            model.Body = post.Body;
            model.MedialUrl = post.MediaUrl;
            model.Title = post.Title;
            model.Published = post.Published;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id, CreateEditPostViewModel model)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(PostController.Index));
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var fileExtension = Path.GetExtension(model.FileUpload.FileName).ToLower();

            if (!AllowedExtenions.Contains(fileExtension))
            {
                ModelState.AddModelError("", "File extension is not allowed");
                return View();
            }

            var post = Context.Posts.FirstOrDefault(p => p.Id == id.Value);

            post.Published = model.Published;
            post.Title = model.Title;
            post.Body = model.Body;
            post.DateUpdated = DateTime.Now;
            
            if (model.FileUpload != null)
            {
                post.MediaUrl = UploadFile(model.FileUpload);
            }
            
            Context.SaveChanges();

            return RedirectToAction(nameof(PostController.Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(PostController.Index));
            }

            var post = Context.Posts.FirstOrDefault(p => p.Id == id.Value);

            if (post == null)
            {
                return RedirectToAction(nameof(PostController.Index));
            }
            
            Context.Posts.Remove(post);
            Context.SaveChanges();

            return RedirectToAction(nameof(PostController.Index));
        }

        [HttpGet]
        public ActionResult ViewPost(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(PostController.Index));
            }

            var post = Context.Posts.FirstOrDefault(p => p.Id == id.Value);

            if (post == null)
            {
                return RedirectToAction(nameof(PostController.Index));
            }

            var model = new ViewPostViewModel();
            model.Body = post.Body;
            model.DateCreated = post.DateCreated;
            model.DateUpdated = post.DateUpdated;
            model.Title = post.Title;
            model.MediaUrl = post.MediaUrl;

            return View(model);
        }

        private string UploadFile(HttpPostedFileBase file)
        {
            if (file != null)
            {
                var uploadFolder = "~/Upload";
                var mappedFolder = Server.MapPath("~/Upload");

                if (!Directory.Exists(mappedFolder))
                {
                    Directory.CreateDirectory(mappedFolder);
                }

                file.SaveAs(mappedFolder + file);

                return uploadFolder + file.FileName;
            }

            return null;
        }
    }
}