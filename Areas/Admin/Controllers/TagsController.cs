using Microsoft.AspNetCore.Mvc;
using TTTN_ViettelStore.Models;
using X.PagedList;
using TTTN_ViettelStore.Areas.Admin.Attributes;

namespace TTTN_ViettelStore.Areas.Admin.Controllers
{
    [Area("Admin")]

    // kiểm tra
    [CheckLogin]

    public class TagsController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Index()
        {
            return RedirectToAction("Read", "Tags", new { area = "Admin" });
        }

        public IActionResult Read(int? page)
        {
            // số bản ghi trên 1 trang
            int pageSize = 4;
            int pageNumber = page ?? 1;
            List<ItemTags> list_tags = db.Tags.OrderByDescending(item => item.Id).ToList();
            return View("Read", list_tags.ToPagedList(pageNumber, pageSize));
        }

        // create 
        public IActionResult Create()
        {
            // tạo biến action để truyền vào tham số action thẻ form
            ViewBag.action = "/Admin/Tags/CreatePost";
            return View("CreateUpdate");
        }

        [HttpPost]
        public IActionResult CreatePost()
        {
            try
            {
                string _Name = Request.Form["Name"].ToString();
                // khởi tạo giá trị list_tags
                ItemTags list_tags = new ItemTags
                {
                    Name = _Name,
                };

                db.Tags.Add(list_tags);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Thêm thành công tag!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Có lỗi khi xử lý thêm tag!";
            }

            return RedirectToAction("Read", "Tags", new { area = "Admin" });
        }

        // update
        public IActionResult Update(int id)
        {
            // khai báo biến action để truyền vào tham số action thẻ form
            ViewBag.action = "/Admin/Tags/UpdatePost/" + id;

            // lấy bản ghi tương ứng
            var list_tags = db.Tags.Where(item => item.Id == id).FirstOrDefault();
            if (list_tags != null)
            {
                return View("CreateUpdate", list_tags);
            }    
            return NotFound();
        }

        [HttpPost]
        public IActionResult UpdatePost(int id, IFormCollection fc)
        {
            try
            {
                // lấy bản ghi tương ứng
                var list_tags = db.Tags.Where(item => item.Id == id).FirstOrDefault();

                // sử dụng đối tượng IForm để lấy kiểu dữ liệu Post
                string _Name = fc["Name"].ToString().Trim();

                if (list_tags != null)
                {
                    // cập nhật các trường của bản ghi
                    list_tags.Name = _Name;

                    // cập nhật vào db
                    db.Update(list_tags);
                    db.SaveChanges();
                }

                TempData["SuccessMessage"] = $"Cập nhật dữ liệu tag id {id} thành công!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = $"Có lỗi khi thực hiện cập nhật tag id {id}!";
            }

            return RedirectToAction("Read", "Tags", new { area = "Admin" });
        }

        // delete
        public IActionResult Delete(int id)
        {
            try
            {
                // lấy bản ghi tương ứng
                var list_tags = db.Tags.Where(item => item.Id == id).FirstOrDefault();
                if (list_tags != null)
                {
                    db.Tags.Remove(list_tags);
                    db.SaveChanges();
                }    

                TempData["SuccessMessage"] = $"Xóa thành công tag id {id}!";
            }
            catch(Exception)
            {
                TempData["ErrorMessage"] = $"Có lỗi thực hiện xóa tag id {id}!";
            }

            return RedirectToAction("Read", "Tags", new { area = "Admin" });
        }
    }
}
