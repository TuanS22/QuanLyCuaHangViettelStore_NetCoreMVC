using Microsoft.AspNetCore.Mvc;
using TTTN_ViettelStore.Models;
using X.PagedList;
using TTTN_ViettelStore.Areas.Admin.Attributes;

namespace TTTN_ViettelStore.Areas.Admin.Controllers
{
    [Area("Admin")]

    // kiểm tra 
    [CheckLogin]

    public class SlidesController : Controller
    {
        public MyDbContext db = new MyDbContext();

        public IActionResult Index()
        {
            return RedirectToAction("Read", "Slides", new { area = "Admin" });
        }

        public IActionResult Read(int? page)
        {
            // số bản ghi trên 1 trang
            int pageSize = 4;
            int pageNumber = page ?? 1;
            List<ItemSlides> list_slides = db.Slides.OrderByDescending(item => item.Id).ToList();
            return View("Read", list_slides.ToPagedList(pageNumber, pageSize));
        }

        public IActionResult Create()
        {
            ViewBag.action = "/Admin/Slides/CreatePost";
            return View("CreateUpdate");
        }

        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            try
            {
                string _Name = fc["Name"].ToString().Trim();
                string _Title = fc["Title"].ToString().Trim();
                string _SubTitle = fc["SubTitle"].ToString().Trim();
                string _Info = fc["Info"].ToString().Trim();
                string _Link = fc["Link"].ToString().Trim();

                // khởi tạo giá trị list_slides
                ItemSlides list_slides = new ItemSlides
                {
                    Name = _Name,
                    Title = _Title,
                    SubTitle = _SubTitle,
                    Info = _Info,
                    Link = _Link
                };

                // xử lý ảnh
                var photoFile = fc.Files.GetFile("Photo");
                if (photoFile != null && photoFile.Length > 0)
                {
                    // nối chuỗi thời gian vào biến fileName
                    string fileName = DateTime.Now.ToFileTime() + "_" + photoFile.FileName;

                    // Part.Combine(duongdan1, duongdan2, ...) nối đường dẫn 1 và đường dẫn 2 thành một đường dẫn
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Slides", fileName);

                    // upload file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        Request.Form.Files[0].CopyTo(stream);
                    }
                    // cập nhật lại đường dẫn ảnh
                    list_slides.Photo = fileName;
                }

                db.Slides.Add(list_slides);
                db.SaveChanges();

                // thông báo thêm thành công
                TempData["SuccessMessage"] = "Thêm slides thành công!";

            }
            catch (Exception)
            {
                // Thông báo lỗi
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm slides!";
            }

            return RedirectToAction("Read", "Slides", new { area = "Admin" });
        }

        // update
        public IActionResult Update(int id)
        {
            // khai báo biến action để truyền vào tham số action thẻ form
            ViewBag.action = "/Admin/Slides/UpdatePost/" + id;

            // lấy 1 bản ghi
            var list_slides = db.Slides.Where(item => item.Id == id).FirstOrDefault();

            if (list_slides != null)
            {
                return View("CreateUpdate", list_slides);
            }
            else
                return NotFound();
        }

        [HttpPost]
        public IActionResult UpdatePost(int id, IFormCollection fc)
        {
            try
            {
                // lấy 1 bản ghi
                var list_slides = db.Slides.Where(item => item.Id == id).FirstOrDefault();

                // sử dụng đối tượng IFormCollection để lấy kiểu dữ liệu POST
                string _Name = fc["Name"].ToString().Trim();
                string _Title = fc["Title"].ToString().Trim();
                string _SubTitle = fc["SubTitle"].ToString().Trim();
                string _Info = fc["Info"].ToString().Trim();
                string _Link = fc["Link"].ToString().Trim();

                // khởi tạo giá trị list_slides
                ItemSlides list_record = new ItemSlides();

                // Xử lý ảnh
                var photoFile = fc.Files.GetFile("Photo");
                if(photoFile != null && photoFile.Length > 0)
                {
                    // xóa ảnh cũ
                    if (list_slides?.Photo != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Slides", list_slides.Photo)))
                    {
                        System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Slides", list_slides.Photo));
                    }    

                    // nối chuỗi thời gian vào biến fileName
                    string fileName = DateTime.Now.ToFileTime() + "_" + photoFile.FileName;

                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Slides", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        photoFile.CopyTo(stream);
                    }

                    // cập nhật lại đường dẫn ảnh
                    list_record.Photo = fileName;
                }

                if(list_slides != null)
                {
                    // cập nhật các trường của bản ghi
                    list_slides.Name = _Name;
                    list_slides.Title = _Title;
                    list_slides.SubTitle = _SubTitle;
                    list_slides.Info = _Info;
                    list_slides.Link = _Link;

                    // cập nhật lại đường dẫn ảnh
                    if (!string.IsNullOrEmpty(list_record.Photo))
                    {
                        list_slides.Photo = list_record.Photo;
                    }

                    // cập nhật vào db
                    db.Update(list_slides);
                    db.SaveChanges();

                    // thông báo 
                    TempData["SuccessMessage"] = $"Cập nhật thành công slides id {id}!";
                }    
                else
                {
                    TempData["ErrorMessage"] = $"Không tìm thấy slides id {id}!";
                }    
            }
            catch (Exception)
            {
                // thông báo lỗi
                TempData["ErrorMessage"] = $"Có lỗi xảy ra khi cập nhật slides id {id}!";
            }

            return RedirectToAction("Read", "Slides", new { area = "Admin" });
        }

        // delete
        public IActionResult Delete(int id)
        {
            try
            {
                // lấy bản ghi tương ứng với id truyền vào
                var record = db.Slides.Where(item => item.Id == id).FirstOrDefault();
                // xóa ảnh
                if (record?.Photo != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Slides", record.Photo)))
                {
                    System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Slides", record.Photo));
                } 

                // xóa bản ghi
                db.Slides.Remove(record);
                // cập nhật
                db.SaveChanges();

                TempData["SuccessMessage"] = $"Xóa thành công slides id {id}!";
                    
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = $"Có lỗi xảy ra khi xóa slides id {id}";
            }

            return RedirectToAction("Read", "Slides", new { area = "Admin" });
        }
    }
}
