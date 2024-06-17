using Microsoft.AspNetCore.Mvc;
using TTTN_ViettelStore.Models;
using X.PagedList;

namespace TTTN_ViettelStore.Controllers
{
    public class ProductsController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Categories(int? id, int? page)
        {
            int CategoryId = id ?? 0;
            ViewBag.CategoryId = CategoryId;

            // số bản ghi trên 1 trang
            int pageSize = 9;

            // số trang
            int pageNumber = page ?? 1;
            List<ItemProducts> list_record = new List<ItemProducts>();

            if (CategoryId > 0)
            {
                // lấy các sản phẩm tương ứng với CategoryId để truyền từ url
                list_record = (from item_category in db.Categories
                              join item_category_product in db.CategoriesProduct on item_category.Id equals item_category_product.CategoryId
                              join item_product in db.Products on item_category_product.ProductId equals item_product.Id
                              where item_category_product.CategoryId == CategoryId
                              select item_product).ToList();
            }
            else
                list_record = db.Products.OrderByDescending(item => item.Id).ToList();

            // lấy biến order truyền vào url
            string order = "";

            if (!String.IsNullOrEmpty(Request.Query["order"]))
            {
                order = Request.Query["order"].ToString();
            }    

            // truyền biến order ra ngoài view để thực hiện selected dropdown
            ViewBag.Order = order;
            switch (order)
            {
                case "name-asc":
                    list_record = list_record.OrderBy(item => item.Name).ToList();
                    break;
                case "name-desc":
                    list_record = list_record.OrderByDescending(item => item.Name).ToList();
                    break;
                case "price-asc":
                    list_record = list_record.OrderBy(item => item.Price).ToList();
                    break;
                case "price-desc":
                    list_record = list_record.OrderByDescending(item => item.Price).ToList();
                    break;
            }

            return View("ProductsCategories", list_record.ToPagedList(pageNumber, pageSize));
        }

        // detail
        public IActionResult Detail(int id)
        {
            var record = db.Products.Where(item => item.Id == id).FirstOrDefault();
            return View("ProductsDetail", record);
        }

        // đánh giá sao
        public IActionResult Rate(int id)
        {
            int star = Convert.ToInt32(Request.Query["star"]);

            // tạo đối tượng ItemRating để insert vào table Rating
            ItemRating record = new ItemRating();
            record.ProductId = id;
            record.Star = star;

            db.Rating.Add(record);
            db.SaveChanges();

            return Redirect("/Products/Detail/" + id);
        }

        // tìm sản phẩm theo giá
        public IActionResult SearchPrice(int? page)
        {
            // khi lấy biến từ url thì mặc định biến này là string -> nếu là số thì cần convert 
            int from_price = 0, to_price = 0;
            if (!String.IsNullOrEmpty(Request.Query["from_price"]))
                from_price = Convert.ToInt32(Request.Query["from_price"]);
            if (!String.IsNullOrEmpty(Request.Query["to_price"]))
                to_price = Convert.ToInt32(Request.Query["to_price"]);

            // tạo 2 biến để đưa giá trị ra ngoài View
            ViewBag.from_price = from_price;
            ViewBag.to_price = to_price;

            // số bản ghi trên một trang
            int pageSize = 9;
            // số trang 
            int pageNumber = page ?? 1;
            List<ItemProducts> list_record = db.Products.Where(item => item.Price > from_price && item.Price <= to_price).ToList();
            return View("SearchPrice", list_record.ToPagedList(pageNumber, pageSize));
        }

        // trả về kết quả tìm kiếm để sử dụng ajax
        public IActionResult AJaxSearch()
        {
            string key = "";
            if (!String.IsNullOrEmpty(Request.Query["key"]))
                key = Request.Query["key"];
            List<ItemProducts> list_products = db.Products.Where(item => item.Name.Contains(key)).ToList();
            string str = "";
            foreach (var item in list_products)
            {
                str += "<li><a href='/Products/Detail/" + item.Id + "' style=\"text-decoration: none; color: #2382F1; font-size: 16px; display: flex; align-items: center;\"><img src='/Upload/Products/" + item.Photo + "'>" + item.Name + "</a></li>";
            }
            return Content(str);
        }

        // click button để tìm kiếm sản phẩm
        public IActionResult SearchName(int? page)
        {
            // khi lấy biến từ url thì mặc định biến này là string -> nếu là số thì cần convert 
            string key = "";
            if (!String.IsNullOrEmpty(Request.Query["key"]))
                key = Request.Query["key"];

            // tạo biến để đưa giá trị ra ngoài view
            ViewBag.key = key;

            // số bản ghi trên 1 trang
            int pageSize = 9;
            // số trang 
            int pageNumber = page ?? 1;
            List<ItemProducts> list_record = db.Products.Where(item => item.Name.Contains(key)).ToList();
            return View("SearchName", list_record.ToPagedList(pageNumber, pageSize));
        }

        // lọc sản phẩm theo tag_id
        public IActionResult Tag(int? id, int? page)
        {
            int tag_id = id ?? 0;
            ViewBag.tag_id = tag_id;
            // số bản ghi trên một trang 
            int pageSize = 9;
            // số trang
            int pageNumber = page ?? 1;
            // join 3 bảng Tags, TagProducts, Products
            List<ItemProducts> list_products = (from item_tag in db.Tags
                                               join item_tag_product in db.TagsProducts on item_tag.Id equals item_tag_product.TagId
                                               join item_product in db.Products on item_tag_product.ProductId equals item_product.Id
                                               where item_tag.Id == tag_id
                                               select item_product).ToList();
            return View("Tag", list_products.ToPagedList(pageNumber, pageSize));
        }
    }
}
