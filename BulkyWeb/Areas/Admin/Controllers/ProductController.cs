
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging.Signing;
using System.Collections.Generic;
using System.Linq;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return View(objProductList);
        }
        public IActionResult Upsert(int? id)
        {
            //IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            //{
            //    Text = u.Name,
            //    Value = u.Id.ToString()
            //});

            //ViewBag.CategoryList = CategoryList;
            //ViewData["CategoryList"]= CategoryList;
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if(id == null || id == 0)
            {
                //create
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product = _unitOfWork.Product.Get(u =>u.Id == id);
                return View(productVM);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? File)
        {
            if (ModelState.IsValid)
            {
                var file = File;
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file!= null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, "images", "product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //delete the old image
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));


                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    if (!Directory.Exists(productPath))
                    {
                        Directory.CreateDirectory(productPath);
                    }

                    // Save the new file
                    string fullPath = Path.Combine(productPath, fileName);
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);

                    }
                    productVM.Product.ImageUrl = Path.Combine("images", "product", fileName).Replace("\\", "/");
                }
                if(productVM.Product.Id==0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }

           
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully !!";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
        }
        //public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
        //    Category? categoryFromDb1 = _db.Categories.FirstOrDefault(u => u.Id == id);
        //    Category? categoryFromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();
        //    if (productFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(productFromDb);
        //}
        //[HttpPost]
        //public IActionResult Edit(Product obj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.Update(obj);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Product edited successfully !!";
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        return View();
        //    }
        //}

        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);

        //    if (productFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(productFromDb);
        //}
        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePost(int? id)
        //{
        //    Product? obj = _unitOfWork.Product.Get(u => u.Id == id);
        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }
        //    _unitOfWork.Product.Remove(obj);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Product deleted successfully !!";
        //    return RedirectToAction("Index");
        //}

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }
        #endregion

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if(productToBeDeleted== null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                productToBeDeleted.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));


            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { sucess = true, message = "Delete Successful" });
        }
    }
}
