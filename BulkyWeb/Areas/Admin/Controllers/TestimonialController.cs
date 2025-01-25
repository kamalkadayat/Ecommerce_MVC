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
    public class TestimonialController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public TestimonialController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            List<Testimonial> objTestimonialList = _unitOfWork.Testimonial.GetAll().ToList();
            return View(objTestimonialList);
        }

        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                // Create
                return View(new Testimonial());
            }
            else
            {
                // Update
                Testimonial testimonialObj = _unitOfWork.Testimonial.Get(u => u.Id == id);
                return View(testimonialObj);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Testimonial testimonialObj, IFormFile? ClientImageFile)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;

                if (ClientImageFile != null) // Check if a file was uploaded
                {
                    // Generate a unique filename and save it to a folder
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ClientImageFile.FileName);
                    string uploadsFolder = Path.Combine(wwwRootPath, "images", "testimonials");
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Delete the old image if updating
                    if (!string.IsNullOrEmpty(testimonialObj.ClientImage))
                    {
                        string oldImagePath = Path.Combine(wwwRootPath, testimonialObj.ClientImage.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        ClientImageFile.CopyTo(fileStream); // Save the file to the server
                    }

                    testimonialObj.ClientImage = $"/images/testimonials/{fileName}"; // Save the image path
                }

                // Add or update the testimonial
                if (testimonialObj.Id == 0)
                {
                    _unitOfWork.Testimonial.Add(testimonialObj);
                    TempData["success"] = "Testimonial created successfully!";
                }
                else
                {
                    _unitOfWork.Testimonial.Update(testimonialObj);
                    TempData["success"] = "Testimonial updated successfully!";
                }

                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(testimonialObj);
        }



        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Testimonial> objTestimonialList = _unitOfWork.Testimonial.GetAll().ToList();
            return Json(new { data = objTestimonialList });
        }

        #endregion

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var testimonialToBeDeleted = _unitOfWork.Testimonial.Get(u => u.Id == id);
            if (testimonialToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            // Delete image from server
            if (!string.IsNullOrEmpty(testimonialToBeDeleted.ClientImage))
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string oldImagePath = Path.Combine(wwwRootPath, testimonialToBeDeleted.ClientImage.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            _unitOfWork.Testimonial.Remove(testimonialToBeDeleted);
            TempData["success"] = "Testimonial deleted successfully!";
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }
    }
}
