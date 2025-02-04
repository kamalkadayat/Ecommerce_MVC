using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class BlogController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public BlogController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            List<Blog> objBlogList = _unitOfWork.Blog.GetAll().ToList();
            return View(objBlogList);
        }

        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                // Create
                return View(new Blog());
            }
            else
            {
                // Update
                Blog blogObj = _unitOfWork.Blog.Get(u => u.Id == id);
                return View(blogObj);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Blog blogObj, IFormFile? BlogImageFile)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;

                if (BlogImageFile != null) // Check if a file was uploaded
                {
                    // Generate a unique filename and save it to a folder
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(BlogImageFile.FileName);
                    string uploadsFolder = Path.Combine(wwwRootPath, "images", "blogs");
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Delete the old image if updating
                    if (!string.IsNullOrEmpty(blogObj.BlogImage))
                    {
                        string oldImagePath = Path.Combine(wwwRootPath, blogObj.BlogImage.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        BlogImageFile.CopyTo(fileStream); // Save the file to the server
                    }

                    blogObj.BlogImage = $"/images/blogs/{fileName}"; // Save the image path
                }

                // Add or update the testimonial
                if (blogObj.Id == 0)
                {
                    _unitOfWork.Blog.Add(blogObj);
                    TempData["success"] = "Blog created successfully!";
                }
                else
                {
                    _unitOfWork.Blog.Update(blogObj);
                    TempData["success"] = "Blog updated successfully!";
                }

                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(blogObj);
        }



        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Blog> objBlogList = _unitOfWork.Blog.GetAll().ToList();
            return Json(new { data = objBlogList });
        }

        #endregion

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var blogToBeDeleted = _unitOfWork.Blog.Get(u => u.Id == id);
            if (blogToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            // Delete image from server
            if (!string.IsNullOrEmpty(blogToBeDeleted.BlogImage))
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string oldImagePath = Path.Combine(wwwRootPath, blogToBeDeleted.BlogImage.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            _unitOfWork.Blog.Remove(blogToBeDeleted);
            TempData["success"] = "Blog deleted successfully!";
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }
    }
}
