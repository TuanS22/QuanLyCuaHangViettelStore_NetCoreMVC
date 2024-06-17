using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TTTN_ViettelStore.Models;
using X.PagedList;
using TTTN_ViettelStore.Areas.Admin.Attributes;

namespace TTTN_ViettelStore.Areas.Admin.Controllers
{
	[Area("Admin")]

    // kiểm tra login
    //[CheckLogin]

	public class ProductsController : Controller
	{
		public MyDbContext db = new MyDbContext();
		public IActionResult Index()
		{
			return RedirectToAction("Read", "Products", new { area = "Admin" });
		}

		public IActionResult Read(int? page)
		{
			// số bản ghi trên 1 trang
			int pageSize = 4;
			int pageNumbers = page ?? 1;
			List<ItemProducts> list_products = db.Products.OrderByDescending(item => item.Id).ToList();
			return View("Read", list_products.ToPagedList(pageNumbers, pageSize));
		}	

		// create
		public IActionResult Create()
		{
			// tạo biến action để truyền 
			ViewBag.action = "/Admin/Products/CreatePost";
			return View("CreateUpdate");
		}

		[HttpPost]
		public IActionResult CreatePost(IFormCollection fc)
		{
			try
			{
				string _Name = fc["Name"].ToString().Trim();
                string _Description = fc["Description"].ToString().Trim();
                double _Price = Convert.ToDouble(fc["Price"].ToString());
                double _Discount = Convert.ToDouble(fc["Discount"].ToString());
                int _Hot = !String.IsNullOrEmpty(fc["Hot"]) ? 1 : 0;

				// khởi tạo giá trị list_products
				ItemProducts list_products = new ItemProducts
				{
					Name = _Name,
					Description = _Description,
					Price = _Price,
					Discount = _Discount,
					Hot = _Hot
				};

                // Xử lý ảnh
                var photoFile = fc.Files.GetFile("Photo");
                if (photoFile != null && photoFile.Length > 0)
                {
                    // nối chuỗi thời gian vào biến fileName
                    string fileName = DateTime.Now.ToFileTime() + "_" + photoFile.FileName;

                    // Part.Combine(duongdan1, duongdan2, ...) nối đường dẫn 1 và đường dẫn 2 thành một đường dẫn
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Products", fileName);

                    // upload file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        Request.Form.Files[0].CopyTo(stream);
                    }
                    // cập nhật lại đường dẫn ảnh
                    list_products.Photo = fileName;
                }

				// cập nhật sự thay đổi
				db.Products.Add(list_products);
				db.SaveChanges();

				// cập nhật categories
				var categories = fc["Categories"];
				// cập nhật lại csdl
				db.SaveChanges();

				// duyệt các phần tử của biến categories
				foreach (var category_id in categories)
				{
                    var record_category_product = new ItemCategoriesProduct();
                    record_category_product.CategoryId = Convert.ToInt32(category_id);
                    record_category_product.ProductId = list_products.Id;//id vừa mới insert bên trên
                                                                  //them ban ghi
                    db.CategoriesProduct.Add(record_category_product);
                    //cap nhat csdl
                    db.SaveChanges();
                }

				// cập nhật tag
				var tags = fc["Tags"];
				db.SaveChanges();

				// duyệt các phần tử
				foreach (var tag_id in tags)
				{
					var record_tag_product = new ItemTagsProducts();
					record_tag_product.TagId = Convert.ToInt32(tag_id);
					record_tag_product.ProductId = list_products.Id;

					db.TagsProducts.Add(record_tag_product);
					db.SaveChanges();
				}	

				// thông báo
				TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
            }
			catch (Exception)
			{
                // Thông báo lỗi
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm sản phẩm!";
            }

			return RedirectToAction("Read", "Products", new { area = "Admin" });
		}

		// update
		public IActionResult Update(int id)
		{
			// tạo biến action để dưa vào thuộc tính action trong thẻ form
			ViewBag.action = "/Admin/Products/UpdatePost/" + id;
			// lấy 1 bản ghi tương ứng
			ItemProducts products = db.Products.FirstOrDefault(item => item.Id == id);
			return View("CreateUpdate", products);
		}

		[HttpPost]
		public IActionResult UpdatePost(int id, IFormCollection fc)
		{
			try
			{
                string _Name = fc["Name"].ToString().Trim();
                string _Description = fc["Description"].ToString().Trim();
                double _Price = Convert.ToDouble(fc["Price"].ToString());
                double _Discount = Convert.ToDouble(fc["Discount"].ToString());
                int _Hot = !String.IsNullOrEmpty(fc["Hot"]) ? 1 : 0;

                //lấy record tương ứng với id truyền vào để update dữ liệu
                ItemProducts list_products = db.Products.Where(item => item.Id == id).FirstOrDefault();

				if(list_products != null)
				{
                    list_products.Name = _Name;
                    list_products.Price = _Price;
                    list_products.Discount = _Discount;
                    list_products.Hot = _Hot;
                    list_products.Description = _Description;

                    // Xử lý Photo
                    var photoFile = fc.Files.GetFile("Photo");
                    if (photoFile != null && photoFile.Length > 0)
                    {
                        //xoa anh
                        if (list_products.Photo != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Products", list_products.Photo)))
                        {
                            System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Products", list_products.Photo));
                        }

                        // nối chuỗi thời gian vào biến fileName
                        string fileName = DateTime.Now.ToFileTime() + "_" + photoFile.FileName;

                        // Part.Combine(duongdan1, duongdan2, ...) nối đường dẫn 1 và đường dẫn 2 thành một đường dẫn
                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Products", fileName);

                        // upload file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            photoFile.CopyTo(stream);
                        }

                        // cập nhật lại đường dẫn ảnh
                        list_products.Photo = fileName;
                    }

					// cập nhật
					db.Update(list_products);
					db.SaveChanges();
                }

				// cập nhật categories
				var categories = fc["Categories"];

				// xóa các bản ghi tương ứng với ProductId trong bảng CategoriesProduct để update lại bản ghi mới
				var list_categories_products = db.CategoriesProduct.Where(item => item.ProductId == id);
                foreach (var item in list_categories_products)
                {
                    db.Remove(item);
                }

				// cập nhật csdl
				db.SaveChanges();

                //duyệt các phần tử của biến categories
                foreach (var category_id in categories)
                {
                    var record_category_product = new ItemCategoriesProduct();
                    record_category_product.CategoryId = Convert.ToInt32(category_id);
                    record_category_product.ProductId = id;
                    //them ban ghi
                    db.CategoriesProduct.Add(record_category_product);
                    //cap nhat csdl
                    db.SaveChanges();
                }

                // cập nhật tags
                var tags = fc["Tags"];
                //xóa các bản ghi tương ứng với ProductId trong bảng TagsProducts để update lại bản ghi mới
                var list_tags_products = db.TagsProducts.Where(item => item.ProductId == id);
                foreach (var item in list_tags_products)
                {
                    db.Remove(item);
                }
                //cap nhat csdl
                db.SaveChanges();
                //duyệt các phần tử của biến tags
                foreach (var category_id in tags)
                {
                    var record_tag_product = new ItemTagsProducts();
                    record_tag_product.TagId = Convert.ToInt32(category_id);
                    record_tag_product.ProductId = id;
                    //them ban ghi
                    db.TagsProducts.Add(record_tag_product);
                    //cap nhat csdl
                    db.SaveChanges();
                }

                TempData["SuccessMessage"] = $"Cập nhật thành công sản phẩm có id {id}!";
            }
			catch(Exception)
			{
                TempData["ErrorMessage"] = $"Có lỗi khi cập nhật sản phẩm id {id}!";
			}

			return RedirectToAction("Read", "Products", new { area = "Admin" });
		}

        // delete
        public IActionResult Delete(int id)
        {
            try
            {
                // lấy bản ghi tương ứng vs id
                var list_products = db.Products.Where(item => item.Id == id).FirstOrDefault();

                // xóa ảnh 
                if (list_products.Photo != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Products", list_products.Photo)))
                {
                    System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Products", list_products.Photo));
                }

                db.Remove(list_products);
                db.SaveChanges();

                TempData["SuccessMessage"] = $"Xóa sản phẩm id {id} thành công!";
            }
            catch(Exception)
            {
                TempData["ErrorMessage"] = $"Có lỗi khi xóa sản phẩm id {id}!";
            }

            return RedirectToAction("Read", "Products", new { area = "Admin" });
        }
	}
}
