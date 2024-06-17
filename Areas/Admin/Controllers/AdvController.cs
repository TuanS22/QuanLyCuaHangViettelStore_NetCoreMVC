using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using TTTN_ViettelStore.Models;
using X.PagedList;
using TTTN_ViettelStore.Areas.Admin.Attributes;

namespace TTTN_ViettelStore.Areas.Admin.Controllers
{
    [Area("Admin")]

    // kiểm tra login
    [CheckLogin]
    public class AdvController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Index()
        {
            return RedirectToAction("Read", "Adv", new { area = "Admin"});
        }

        public IActionResult Read(int? page)
        {
            // số trang ghi trên 1 bảng
            int pageSize = 4;
            // số trang
            int pageNumber = page ?? 1;
            List<ItemAdv> list_adv = db.Adv.OrderByDescending(item => item.Id).ToList();
            return View("Read", list_adv.ToPagedList(pageNumber, pageSize));
        }

        public IActionResult Create()
        {
            ViewBag.action = "/Admin/Adv/CreatePost";
            return View("CreateUpdate");
        }

        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            try
            {
                string _Name = fc["Name"].ToString().Trim();
                int _Position = Convert.ToInt32(fc["Position"].ToString().Trim());

                ItemAdv list_adv = new ItemAdv
                {
                    Name = _Name,
                    Position = _Position
                };

                // Xử lý Photo
                var photoFile = fc.Files.GetFile("Photo");
                if (photoFile != null && photoFile.Length > 0)
                {
                    // nối chuỗi thời gian vào biến fileName
                    string fileName = DateTime.Now.ToFileTime() + "_" + photoFile.FileName;

                    // Part.Combine(duongdan1, duongdan2, ...) nối đường dẫn 1 và đường dẫn 2 thành một đường dẫn
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Adv", fileName);

                    // upload file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        Request.Form.Files[0].CopyTo(stream);
                    }
                    // cập nhật lại đường dẫn ảnh
                    list_adv.Photo = fileName;
                }

                db.Adv.Add(list_adv);
                db.SaveChanges();

                // thông báo thêm thành công
                TempData["SuccessMessage"] = "Thêm ảnh quảng cáo thành công!";
            }
            catch(Exception)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm ảnh quảng cáo!";
            }
            
            return RedirectToAction("Read", "Adv", new { area = "Admin" });
        }

        // update
        public IActionResult Update(int id)
        {
            // khai báo biến action để truyền vào tham số
            ViewBag.action = "/Admin/Adv/UpdatePost/" + id;
            // lấy 1 bản ghi
            var list_adv = db.Adv.Where(item => item.Id == id).FirstOrDefault();
            if (list_adv != null)
            {
                return View("CreateUpdate", list_adv);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult UpdatePost(int id, IFormCollection fc)
        {
            try
            {
                //lay ban ghi tuong ung voi id truyen vao
                var record = db.Adv.Where(item => item.Id == id).FirstOrDefault();

                string _Name = fc["Name"].ToString().Trim();
                int _Position = Convert.ToInt32(fc["Position"].ToString().Trim());
                
                //update ban ghi
                record.Name = _Name;
                record.Position = _Position;
                //---
                //lay thong tin o the file co type="file"
                string _FileName = "";
                try
                {
                    //lay ten file
                    _FileName = Request.Form.Files[0].FileName;
                }
                catch {; }
                if (!String.IsNullOrEmpty(_FileName))
                {
                    // xóa ảnh cũ
                    if (record.Photo != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Adv", record.Photo)))
                    {
                        System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Adv", record.Photo));
                    }

                    //upload anh moi
                    //lay thoi gian gan vao ten file -> tranh cac file co ten trung ten voi file se upload
                    var timestap = DateTime.Now.ToFileTime();
                    _FileName = timestap + "_" + _FileName;
                    //lay duong dan cua file
                    string _Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Adv", _FileName);
                    //upload file
                    using (var stream = new FileStream(_Path, FileMode.Create))
                    {
                        Request.Form.Files[0].CopyTo(stream);
                    }
                    //update gia tri vao cot Photo trong csdl
                    record.Photo = _FileName;
                }
                //---
                //cap nhat ban ghi
                db.SaveChanges();

                TempData["SuccessMessage"] = $"Cập nhật ảnh quảng cáo id {id} thành công!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = $"Có lỗi xảy ra khi cập nhật adv id {id}!";
            }

            return RedirectToAction("Read", "Adv", new { area = "Admin" });
        }

        // delete
        public IActionResult Delete(int id)
        {
            try
            {
                //lay ban ghi tuong ung voi id truyen vao
                var record = db.Adv.Where(item => item.Id == id).FirstOrDefault();
                //xoa anh
                if (record.Photo != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Adv", record.Photo)))
                {
                    System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Adv", record.Photo));
                }

                //xoa ban ghi
                db.Adv.Remove(record);
                //cap nhat csdl
                db.SaveChanges();

                TempData["SuccessMessage"] = $"Xóa thành công adv id {id}!";
            }
            catch 
            {
                TempData["ErrorMessage"] = $"Có lỗi xảy ra khi xóa adv id {id}!";
            }

            return RedirectToAction("Read", "Adv", new { area = "Admin" });
        }
    }
}
