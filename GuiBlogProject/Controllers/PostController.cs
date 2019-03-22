using GuiBlogProject.ExtensionMethods;
using GuiBlogProject.Models;
using GuiBlogProject.Models.Domain;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        public ActionResult Index(string searchQuery)
        {
            ViewBag.SearchQuery = searchQuery;

            var isAdmin = User.IsInRole("Admin");

            var query = Context.Posts
               .Where(p => isAdmin || p.Published)
               .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(p => p.Slug.Contains(searchQuery) ||
                                   p.Body.Contains(searchQuery) ||
                                   p.Title.Contains(searchQuery)).AsQueryable();
            }

            var posts = query.Select(p => new IndexPostViewModel
            {
                Id = p.Id,
                Body = p.Body,
                MediaUrl = p.MediaUrl,
                Title = p.Title,
                UserEmail = p.User.Email
            })
               .ToList();


            //if (User.IsInRole("Admin"))
            //{
            //    posts = Context.Posts
            //   .Select(p => new IndexPostViewModel
            //   {
            //       Id = p.Id,
            //       Body = p.Body,
            //       MediaUrl = p.MediaUrl,
            //       Title = p.Title,
            //       UserEmail = p.User.Email
            //   })
            //   .ToList();
            //}
            //else
            //{
            //    posts = Context.Posts
            //    .Where(p => p.Published)
            //    .Select(p => new IndexPostViewModel
            //    {
            //        Id = p.Id,
            //        Body = p.Body,
            //        MediaUrl = p.MediaUrl,
            //        Title = p.Title,
            //        UserEmail = p.User.Email
            //    })
            //    .ToList();
            //}



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
            post.Slug = model.Title.ToSlug();
            post.MediaUrl = UploadFile(model.FileUpload);

            do
            {
                if (Context.Posts.Any(p => p.Slug == post.Slug))
                {
                    var random = new Random();

                    var randomNumber = random.Next(0, 1001);

                    post.Slug += randomNumber;
                }
            } while (Context.Posts.Any(p => p.Slug == post.Slug));


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
        [Route("blog/{slug}")]
        public ActionResult ViewPost(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return RedirectToAction(nameof(PostController.Index));
            }

            var post = Context.Posts.FirstOrDefault(p => p.Slug == slug);

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
            model.Comments = post
                .Comments
                .Select(p => new CommentPostViewModel
                {
                    Body = p.Body
                }).ToList();

            return View(model);
        }

        private string UploadFile(HttpPostedFileBase file)
        {
            if (file != null)
            {
                var uploadFolder = "~/Upload/";
                var mappedFolder = Server.MapPath(uploadFolder);

                if (!Directory.Exists(mappedFolder))
                {
                    Directory.CreateDirectory(mappedFolder);
                }

                file.SaveAs(mappedFolder + file.FileName);

                return uploadFolder + file.FileName;
            }

            return null;
        }
    }
}