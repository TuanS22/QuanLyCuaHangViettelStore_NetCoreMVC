using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TTTN_ViettelStore.Models;
using X.PagedList;
using TTTN_ViettelStore.Areas.Admin.Attributes;

namespace TTTN_ViettelStore.Areas.Admin.Controllers
{
    [Area("Admin")]

    // kiểm tra login
    [CheckLogin]
    public class NewsController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Index()
        {
            return Redirect("/Admin/News/Read");
        }

        public IActionResult Read(int ? page)
        {
            // số bản ghi trên 1 trang
            int pageSize = 4;
            // số trang
            int pageNumber = page ?? 1;
            List<ItemNews> list_news = db.News.OrderByDescending(item => item.Id).ToList();
            return View("Read", list_news.ToPagedList(pageNumber, pageSize));
        }

        public IActionResult Create()
        {
            ViewBag.action = "/Admin/News/CreatePost";
            return View("CreateUpdate");
        }

        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            try
            {
                string _Name = fc["Name"].ToString().Trim();
                string _Content = fc["Content"].ToString();
                DateTime _Timestamp = Convert.ToDateTime(fc["Timestamp"].ToString());
                int _Hot = !String.IsNullOrEmpty(fc["Hot"]) ? 1 : 0;

                // khởi tạo giá trị list_news
                ItemNews list_news = new ItemNews
                {
                    Name = _Name,
                    Content = _Content,
                    Timestamp = _Timestamp,
                    Hot = _Hot
                };

                // Xử lý Photo
                var photoFile = fc.Files.GetFile("Photo");
                if (photoFile != null && photoFile.Length > 0)
                {
                    // nối chuỗi thời gian vào biến fileName
                    string fileName = DateTime.Now.ToFileTime() + "_" + photoFile.FileName;

                    // Part.Combine(duongdan1, duongdan2, ...) nối đường dẫn 1 và đường dẫn 2 thành một đường dẫn
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/News", fileName);

                    // upload file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        Request.Form.Files[0].CopyTo(stream);
                    }
                    // cập nhật lại đường dẫn ảnh
                    list_news.Photo = fileName;
                }

                db.News.Add(list_news);
                db.SaveChanges();

                // thông báo thêm thành công
                TempData["SuccessMessage"] = "Thêm tin tức thành công!";
            }
            catch (Exception)
            {
                // Thông báo lỗi
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm tin tức!";
            }

            return RedirectToAction("Read", "News", new { area = "Admin" });
        }

        // update
        public IActionResult Update(int id)
        {
            // khai báo biến action để truyền vào tham số action trong thẻ form
            ViewBag.action = "/Admin/News/UpdatePost/" + id;

            // lấy 1 bản ghi
            var list_news = db.News.Where(item => item.Id == id).FirstOrDefault();

            if (list_news != null)
            {
                return View("CreateUpdate", list_news);
            }
            else
                return NotFound();
        }

        [HttpPost]
        public IActionResult UpdatePost(int id, IFormCollection fc)
        {
            try
            {
                // lấy bản ghi
                var list_news = db.News.Where(item => item.Id == id).FirstOrDefault();

                // sử dụng đối tượng IFormCollection để lấy kiểu dữ liệu POST
                string _Name = fc["Name"].ToString().Trim();
                string _Content = fc["Content"].ToString();
                DateTime _Timestamp = Convert.ToDateTime(fc["Timestamp"].ToString());
                int _Hot = !String.IsNullOrEmpty(fc["Hot"]) ? 1 : 0;

                // khởi tạo giá trị list_record
                ItemNews list_record = new ItemNews();

                // Xử lý Photo
                var photoFile = fc.Files.GetFile("Photo");
                if (photoFile != null && photoFile.Length > 0)
                {
                    //xoa anh
                    if (list_news.Photo != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "News", list_news.Photo)))
                    {
                        System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "News", list_news.Photo));
                    }

                    // nối chuỗi thời gian vào biến fileName
                    string fileName = DateTime.Now.ToFileTime() + "_" + photoFile.FileName;

                    // Part.Combine(duongdan1, duongdan2, ...) nối đường dẫn 1 và đường dẫn 2 thành một đường dẫn
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/News", fileName);

                    // upload file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        photoFile.CopyTo(stream);
                    }

                    // cập nhật lại đường dẫn ảnh
                    list_record.Photo = fileName;
                }

                if (list_news != null)
                {
                    // cập nhật các trường của bản ghi
                    list_news.Name = _Name;
                    list_news.Content = _Content;
                    list_news.Timestamp = _Timestamp;
                    list_news.Hot = _Hot;

                    // cập nhật lại đường dẫn ảnh
                    if (!string.IsNullOrEmpty(list_record.Photo))
                    {
                        list_news.Photo = list_record.Photo;
                    }

                    // cập nhật vào cơ sở dữ liệu
                    db.Update(list_news);
                    db.SaveChanges();

                    // thông báo cập nhật thành công
                    TempData["SuccessMessage"] = $"Cập nhật tin tức id {id} thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Không tìm thấy tin tức id {id} cập nhật!";
                }
            }
            catch (Exception)
            {
                // Thông báo lỗi
                TempData["ErrorMessage"] = $"Có lỗi xảy ra khi cập nhật tin tức id {id}!";
            }

            return RedirectToAction("Read", "News", new { area = "Admin" });
        }


        // delete
        public IActionResult Delete(int id)
        {
            try
            {
                //lay ban ghi tuong ung voi id truyen vao
                var record = db.News.Where(item => item.Id == id).FirstOrDefault();
                //xoa anh
                if (record.Photo != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "News", record.Photo)))
                {
                    System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "News", record.Photo));
                }

                //xoa ban ghi
                db.News.Remove(record);
                //cap nhat csdl
                db.SaveChanges();

                TempData["SuccessMessage"] = $"Xóa thành công tin tức id {id}!";
            }   
            catch(Exception)
            {
                // Thông báo lỗi
                TempData["ErrorMessage"] = $"Có lỗi xảy ra khi xóa tin tức id {id}!";
            }
            
            return RedirectToAction("Read", "News", new { area = "Admin"});
        }
    }
}
