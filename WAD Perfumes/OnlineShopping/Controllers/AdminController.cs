using Newtonsoft.Json;
using OnlineShopping.DAL;
using OnlineShopping.Filters;
using OnlineShopping.Models;
using OnlineShopping.Repository;
using OnlineShopping.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopping.Controllers
{
    [AuthorizeUser(Roles = "Admin")]
    public class AdminController : Controller
    {
        #region Other Class references ...
        // Instance on Unit of Work
        private GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();
        UploadContent uc = new UploadContent();
        #endregion

        #region Admin Login ...
        /// <summary>
        /// Login Page
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            if (CheckAlreadyLoggedIn())
                return Redirect(returnUrl != null ? returnUrl : "/admin/categories");
            else
            {
                LoginViewModel loginModel = new LoginViewModel();
                loginModel.UserEmailId = Request.Cookies["RememberMe_UserEmailId"] != null ? Request.Cookies["RememberMe_UserEmailId"].Value : "";
                loginModel.Password = Request.Cookies["RememberMe_Password"] != null ? Request.Cookies["RememberMe_Password"].Value : "";
                ViewBag.ReturnUrl = returnUrl;
                return View(loginModel);
            }
        }

        /// <summary>
        /// Check if user is already logged in
        /// </summary>
        /// <returns></returns>
        private bool CheckAlreadyLoggedIn()
        {
            bool alreadyLoggedIn = false;
            if (Session["MemberId"] != null && Request.Cookies["MemberRole"] != null && Request.Cookies["MemberRole"].Value == "Admin")
            {
                int memberId = Convert.ToInt32(Session["MemberId"]);
                var user = _unitOfWork.GetRepositoryInstance<Tbl_Members>().GetFirstOrDefaultByParameter(i => i.MemberId == memberId && i.IsActive == true && i.IsDelete == false);
                if (user != null)
                {
                    Response.Cookies["MemberName"].Value = user.FirstName;
                    var roles = _unitOfWork.GetRepositoryInstance<Tbl_MemberRole>().GetFirstOrDefaultByParameter(i => i.MemberId == user.MemberId && i.RoleId == 3);
                    if (roles != null)
                        alreadyLoggedIn = true;
                }
            }
            return alreadyLoggedIn;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Index(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                string EncryptedPassword = EncryptDecrypt.Encrypt(model.Password, true);
                var user = _unitOfWork.GetRepositoryInstance<Tbl_Members>().GetFirstOrDefaultByParameter(i => i.EmailId == model.UserEmailId && i.Password == EncryptedPassword && i.IsActive == true && i.IsDelete == false);
                if (user != null)
                {
                    Session["MemberId"] = user.MemberId;
                    Response.Cookies["MemberName"].Value = user.FirstName;
                    var roles = _unitOfWork.GetRepositoryInstance<Tbl_MemberRole>().GetFirstOrDefaultByParameter(i => i.MemberId == user.MemberId && i.RoleId == model.UserType);
                    if (roles != null)
                    {
                        Response.Cookies["MemberRole"].Value = _unitOfWork.GetRepositoryInstance<Tbl_Roles>().GetFirstOrDefaultByParameter(i => i.RoleId == model.UserType).RoleName;
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Password is wrong");
                        return View(model);
                    }
                    if (Request["RememberMe1"] == "on")
                    {
                        Response.Cookies["RememberMe_UserEmailId"].Value = user.EmailId;
                        Response.Cookies["RememberMe_Password"].Value = user.Password;
                    }
                    else
                    {
                        Response.Cookies["RememberMe_UserEmailId"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["RememberMe_Password"].Expires = DateTime.Now.AddDays(-1);
                    }
                    return Redirect(returnUrl != null ? returnUrl : "/admin/categories");
                }
                else
                {
                    ModelState.AddModelError("Password", "Invalid email address or password");
                }
            }
            return View(model);
        }
        #endregion

        #region Admin Change Password ...
        /// <summary>
        /// Change Password
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// Change Password
        /// </summary>
        /// <param name="cpm"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ChangePassword(ChangePasswordViewModel cpm)
        {
            int LoginMemberId = Convert.ToInt32(Session["MemberId"]);
            var ExistingDetails = _unitOfWork.GetRepositoryInstance<Tbl_Members>().GetFirstOrDefaultByParameter(i => i.MemberId == LoginMemberId && i.IsActive == true && i.IsDelete == false);
            if (EncryptDecrypt.Encrypt(cpm.OldPassword, true) == ExistingDetails.Password)
            {
                ExistingDetails.Password = EncryptDecrypt.Encrypt(cpm.NewPassword, true);
                _unitOfWork.SaveChanges();
                ViewBag.PasswordChangeMsg = "Password changed successfully";
            }
            else
                ModelState.AddModelError("OldPassword", "Current password is wrong");
            return View();
        }
        #endregion

        #region Admin Dashboard ...
        /// <summary>
        /// Admin Dashboard
        /// </summary>
        /// <returns></returns>
        public ActionResult Dashboard()
        {
            return View();
        }
        #endregion

        /// <summary>
        /// Categories
        /// </summary>
        /// <returns></returns>
        #region Manage Categories ...
        public ActionResult Categories()
        {
            List<OnlineShopping.DAL.Tbl_Category> AllCategories = _unitOfWork.GetRepositoryInstance<OnlineShopping.DAL.Tbl_Category>().GetAllRecordsIQueryable().Where(i => i.IsDelete == false).ToList();
            return View(AllCategories);
        }

        /// <summary>
        /// Add Category
        /// </summary>
        /// <returns></returns>
        public ActionResult AddCategory()
        {
            return UpdateCategory(0);
        }

        /// <summary>
        /// Update Category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public ActionResult UpdateCategory(int categoryId)
        {
            CategoryDetail cd;
            if (categoryId != 0)
                cd = _unitOfWork.GetRepositoryInstance<Tbl_Category>().GetListByParameter(i => i.CategoryId == categoryId).Select(j => new CategoryDetail { CategoryId = j.CategoryId, CategoryName = j.CategoryName, IsActive = j.IsActive ?? default(bool), IsDelete = j.IsDelete ?? default(bool) }).FirstOrDefault();
           // cd = JsonConvert.DeserializeObject<CategoryDetail>(JsonConvert.SerializeObject(_unitOfWork.GetRepositoryInstance<OnlineShopping.DAL.Tbl_Category>().GetFirstOrDefault(categoryId)));
            else
                cd = new CategoryDetail();
            return View("UpdateCategory", cd);
        }

        /// <summary>
        /// Update Category
        /// </summary>
        /// <param name="cd"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCategory(CategoryDetail cd)
        {
            if (ModelState.IsValid)
            {
                Tbl_Category cat = _unitOfWork.GetRepositoryInstance<Tbl_Category>().GetFirstOrDefault(cd.CategoryId);
                cat = cat != null ? cat : new Tbl_Category();
                cat.CategoryName = cd.CategoryName;
                if (cd.CategoryId != 0)
                {
                    _unitOfWork.GetRepositoryInstance<Tbl_Category>().Update(cat);
                    _unitOfWork.SaveChanges();
                }
                else
                {
                    cat.IsActive = true;
                    cat.IsDelete = false;
                    _unitOfWork.GetRepositoryInstance<Tbl_Category>().Add(cat);
                }

                return RedirectToAction("Categories");
            }
            else
                return View("UpdateCategory", cd);
        }

        /// <summary>
        /// Delete Category
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public int DeleteCategory(int itemId)
        {
            _unitOfWork.GetRepositoryInstance<OnlineShopping.DAL.Tbl_Category>().InactiveAndDeleteMarkByWhereClause(i => i.CategoryId == itemId, (u => { u.IsActive = false; u.IsDelete = true; }));
            return 1;
        }

        /// <summary>
        /// Check Category Exist
        /// </summary>
        /// <param name="CategoryName"></param>
        /// <returns></returns>
        public JsonResult CheckCategoryExist(string CategoryName)
        {
            int CategoryId = 0;
            if (HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["categoryId"] != null)
                CategoryId = Convert.ToInt32(HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["categoryId"]);
            var CategoryExist = _unitOfWork.GetRepositoryInstance<OnlineShopping.DAL.Tbl_Category>().GetAllRecordsIQueryable().Where(i => i.CategoryName == CategoryName && i.CategoryId != CategoryId && i.IsActive == true && i.IsDelete == false).Count();
            return CategoryExist == 0 ? Json(true, JsonRequestBehavior.AllowGet) : Json(false, JsonRequestBehavior.AllowGet);
        }
        #endregion












        
        #region ManageBrands ...
        public ActionResult Brands()
        {
            List<OnlineShopping.DAL.Tbl_Brand> AllBrands = _unitOfWork.GetRepositoryInstance<OnlineShopping.DAL.Tbl_Brand>().GetAllRecordsIQueryable().Where(i => i.IsDelete == false).ToList();
            return View(AllBrands);
        }

        /// <summary>
        /// Add Brand
        /// </summary>
        /// <returns></returns>
        public ActionResult AddBrand()
        {
            return UpdateBrand(0);
        }

        /// <summary>
        /// Update Brand
        /// </summary>
        /// <param name="brandId"></param>
        /// <returns></returns>
        public ActionResult UpdateBrand(int brandId)
        {
            BrandDetail cd;
            if (brandId != 0)
                cd = _unitOfWork.GetRepositoryInstance<Tbl_Brand>().GetListByParameter(i => i.BrandId == brandId).Select(j => new BrandDetail { BrandId = j.BrandId, BrandName = j.BrandName, IsActive = j.IsActive ?? default(bool), IsDelete = j.IsDelete ?? default(bool) }).FirstOrDefault();
            // cd = JsonConvert.DeserializeObject<CategoryDetail>(JsonConvert.SerializeObject(_unitOfWork.GetRepositoryInstance<OnlineShopping.DAL.Tbl_Category>().GetFirstOrDefault(categoryId)));
            else
                cd = new BrandDetail();
            return View("UpdateBrand", cd);
        }

        /// <summary>
        /// Update Brand
        /// </summary>
        /// <param name="cd"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateBrand(BrandDetail cd, HttpPostedFileBase _BrandImage)
        {
            int poz = 0;
            if (ModelState.IsValid)
            {
                Tbl_Brand b = _unitOfWork.GetRepositoryInstance<Tbl_Brand>().GetFirstOrDefault(cd.BrandId);
                b = b != null ? b : new Tbl_Brand();
                b.BrandName = cd.BrandName;
                if (_BrandImage != null)
                {
                    for (int i = _BrandImage.FileName.Length - 4; i >= 0; i--)
                    {
                        if (_BrandImage.FileName[i] == '\\')
                        {
                            poz = i;
                            break;
                        }
                    }
                }
                b.BrandImage = _BrandImage != null ? _BrandImage.FileName.Substring(poz + 1) : b.BrandImage;
                if (cd.BrandId != 0)
                {
                    _unitOfWork.GetRepositoryInstance<Tbl_Brand>().Update(b);
                    _unitOfWork.SaveChanges();
                }
                else
                {
                    b.IsActive = true;
                    b.IsDelete = false;
                    _unitOfWork.GetRepositoryInstance<Tbl_Brand>().Add(b);
                }

                return RedirectToAction("Brands");

                if (_BrandImage != null)
                    uc.UploadImage(_BrandImage, b.BrandId + "_", "/Content/ProductImage/", Server, _unitOfWork, 0, b.BrandId, 0);
                return RedirectToAction("Products");
            }
            else
                return View("UpdateBrand", cd);
        }

        /// <summary>
        /// Delete Brand
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public int DeleteBrand(int itemId)
        {
            _unitOfWork.GetRepositoryInstance<OnlineShopping.DAL.Tbl_Brand>().InactiveAndDeleteMarkByWhereClause(i => i.BrandId == itemId, (u => { u.IsActive = false; u.IsDelete = true; }));
            return 1;
        }

        /// <summary>
        /// Check Brand Exist
        /// </summary>
        /// <param name="BrandName"></param>
        /// <returns></returns>
        public JsonResult CheckBrandExist(string BrandName)
        {
            int BrandId = 0;
            if (HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["brandId"] != null)
                BrandId = Convert.ToInt32(HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["brandId"]);
            var BrandExist = _unitOfWork.GetRepositoryInstance<OnlineShopping.DAL.Tbl_Brand>().GetAllRecordsIQueryable().Where(i => i.BrandName == BrandName && i.BrandId != BrandId && i.IsActive == true && i.IsDelete == false).Count();
            return BrandExist == 0 ? Json(true, JsonRequestBehavior.AllowGet) : Json(false, JsonRequestBehavior.AllowGet);
        }
        #endregion












        #region Manage Products...
        #region Product Listing...
        /// <summary>
        /// Product Listing
        /// </summary>
        /// <returns></returns>
        public ActionResult Products()
        {
            List<OnlineShopping.DAL.Tbl_Product> products = _unitOfWork.GetRepositoryInstance<OnlineShopping.DAL.Tbl_Product>().GetAllRecordsIQueryable().Where(i => i.IsDelete == false).ToList();
            return View(products);
        }
        #endregion

        #region Add/Update Product...
        /// <summary>
        /// Add Product
        /// </summary>
        /// <returns></returns>
        public ActionResult AddProduct()
        {
            return UpdateProduct(0);
        }

        /// <summary>
        /// Update Product
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public ActionResult UpdateProduct(int productId)
        {
            ProductDetail pd = _unitOfWork.GetRepositoryInstance<Tbl_Product>().GetListByParameter(i => i.ProductId == productId).Select(j => new ProductDetail { CategoryId = j.CategoryId, BrandId = j.BrandId, Description = j.Description, IsActive = j.IsActive ?? default(bool), Price = j.Price ?? default(decimal), ProductId = j.ProductId, ProductImage = j.ProductImage, ProductName = j.ProductName, IsFeatured = j.IsFeatured ?? default(bool) }).FirstOrDefault();
            pd = pd != null ? pd : new ProductDetail();
            pd.Categories = new SelectList(_unitOfWork.GetRepositoryInstance<Tbl_Category>().GetAllRecordsIQueryable(), "CategoryId", "CategoryName");
            pd.Brands = new SelectList(_unitOfWork.GetRepositoryInstance<Tbl_Brand>().GetAllRecordsIQueryable(), "BrandId", "BrandName");
            return View("UpdateProduct", pd);
        }

        /// <summary>
        /// Updating Product details to DB
        /// </summary>
        /// <param name="pd"></param>
        /// <param name="_ProductImage"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProduct(ProductDetail pd, HttpPostedFileBase _ProductImage)
        {
            int poz = 0;
            if (ModelState.IsValid)
            {
                Tbl_Product prod = _unitOfWork.GetRepositoryInstance<Tbl_Product>().GetFirstOrDefault(pd.ProductId);
                prod = prod != null ? prod : new Tbl_Product();
                prod.CategoryId = pd.CategoryId;
                prod.BrandId = pd.BrandId;
                prod.Description = pd.Description;
                prod.IsActive = pd.IsActive;
                prod.IsFeatured = pd.IsFeatured;
                prod.Price = pd.Price;
                if (_ProductImage != null)
                {
                    for (int i = _ProductImage.FileName.Length - 4; i >= 0; i--)
                    {
                        if (_ProductImage.FileName[i] == '\\')
                        {
                            poz = i;
                            break;
                        }
                    }
                }
                prod.ProductImage = _ProductImage != null ? _ProductImage.FileName.Substring(poz + 1) : prod.ProductImage;
                prod.ProductName = pd.ProductName;
                prod.ModifiedDate = DateTime.Now;
                if (prod.ProductId == 0)
                {
                    prod.CreatedDate = DateTime.Now;
                    prod.IsDelete = false;
                    _unitOfWork.GetRepositoryInstance<Tbl_Product>().Add(prod);
                }
                else
                {
                    _unitOfWork.GetRepositoryInstance<Tbl_Product>().Update(prod);
                    _unitOfWork.SaveChanges();
                }
                if (_ProductImage != null)
                    uc.UploadImage(_ProductImage, prod.ProductId + "_", "/Content/ProductImage/", Server, _unitOfWork, 0, prod.ProductId, 0);
                return RedirectToAction("Products");
            }
            pd.Categories = new SelectList(_unitOfWork.GetRepositoryInstance<Tbl_Category>().GetAllRecordsIQueryable(), "CategoryId", "CategoryName");
            pd.Brands = new SelectList(_unitOfWork.GetRepositoryInstance<Tbl_Brand>().GetAllRecordsIQueryable(), "BrandId", "BrandName");
            return View("UpdateProduct", pd);
        }

        /// <summary>
        /// Delete Product
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public int DeleteProduct(int itemId)
        {
            _unitOfWork.GetRepositoryInstance<OnlineShopping.DAL.Tbl_Product>().InactiveAndDeleteMarkByWhereClause(i => i.ProductId == itemId, (u => { u.IsActive = false; u.IsDelete = true; }));
            return 1;
        }

        /// <summary>
        /// Check Product Exist
        /// </summary>
        /// <param name="ProductName"></param>
        /// <returns></returns>
        public JsonResult CheckProductExist(string ProductName)
        {
            int productId = 0;
            if (HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["productId"] != null)
                productId = Convert.ToInt32(HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["productId"]);
            var productExist = _unitOfWork.GetRepositoryInstance<OnlineShopping.DAL.Tbl_Product>().GetAllRecordsIQueryable().Where(i => i.ProductName == ProductName && i.ProductId != productId && i.IsActive == true && i.IsDelete == false).Count();
            return productExist == 0 ? Json(true, JsonRequestBehavior.AllowGet) : Json(false, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion

        #region Product Details...
        /// <summary>
        /// Product Detail
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public ActionResult ProductDetail(int productId)
        {
            Tbl_Product pd = _unitOfWork.GetRepositoryInstance<Tbl_Product>().GetFirstOrDefault(productId);
            return View(pd);
        }
        #endregion

        /// <summary>
        /// All orders
        /// </summary>
        /// <returns></returns>
        public ActionResult Orders()
        {
            List<Tbl_Cart> AllOrders = _unitOfWork.GetRepositoryInstance<Tbl_Cart>().GetListByParameter(i => i.CartStatusId == 3).ToList();
            return View(AllOrders);
        }

        /// <summary>
        /// Order details of a product
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public ActionResult OrderDetail(int productId)
        {
            List<Tbl_Cart> ProductOrders = _unitOfWork.GetRepositoryInstance<Tbl_Cart>().GetListByParameter(i => i.CartStatusId == 3 && i.ProductId == productId).ToList();
            return View(ProductOrders);
        }

        #region Disposing UnitOfWork Context ...
        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
        #endregion
    }
}
