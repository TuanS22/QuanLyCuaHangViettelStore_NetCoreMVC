using Microsoft.AspNetCore.Mvc;
using TTTN_ViettelStore.Models;
using BC = BCrypt.Net.BCrypt; // thư viện mã hóa
using X.PagedList; // thư viện phân trang
using TTTN_ViettelStore.Areas.Admin.Attributes;

namespace TTTN_ViettelStore.Areas.Admin.Controllers
{
    [Area("Admin")]

    // kiểm tra login
    [CheckLogin]

    public class UsersController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Index()
        {
            return RedirectToAction("Read", "Users", new { area = "Admin" });
        }

        public IActionResult Read(int? page)
        {
            // số bản ghi trên 1 dòng
            int pageSize = 4;
            int pageNumber = page ?? 1;
            List<ItemUsers> list_customers = db.Users.OrderByDescending(item => item.Id).ToList();
            return View("Read", list_customers.ToPagedList(pageNumber, pageSize));
        }

        public IActionResult Create()
        {
            // tạo biến action
            ViewBag.action = "/Admin/Users/CreatePost";
            return View("CreateUpdate");
        }

        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            try
            {
                string _Name = fc["Name"].ToString().Trim();
                string _Email = fc["Email"].ToString().Trim();
                string _Address = fc["Address"].ToString().Trim();
                string _Phone = fc["Phone"].ToString().Trim();
                string _Password = fc["Password"].ToString();

                // mã hóa password
                _Password = BC.HashPassword(_Password);
                // kiểm tra trong csdl đã tồn tại chưa nếu chưa thì create
                ItemUsers record_check = db.Users.FirstOrDefault(item => item.Email == _Email);

                if(record_check == null)
                {
                    // khởi tạo giá trị list_users
                    ItemUsers list_users = new ItemUsers
                    {
                        Name = _Name,
                        Email = _Email,
                        Address = _Address,
                        Phone = _Phone,
                        Password = _Password
                    };

                    // xử lý ảnh
                    var photoFile = fc.Files.GetFile("Photo");
                    if (photoFile != null && photoFile.Length > 0)
                    {
                        // nối chuỗi thời gian vào biến fileName
                        string fileName = DateTime.Now.ToFileTime() + "_" + photoFile.FileName;

                        // Part.Combine(duongdan1, duongdan2, ...) nối đường dẫn 1 và đường dẫn 2 thành một đường dẫn
                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Users", fileName);

                        // upload file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            Request.Form.Files[0].CopyTo(stream);
                        }
                        // cập nhật lại đường dẫn ảnh
                        list_users.Photo = fileName;
                    }

                    db.Users.Add(list_users);
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Thêm thành công user!";
                }    
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm user!";
            }

            return RedirectToAction("Read", "Users", new { area = "Admin" });
        }

        // update
        public IActionResult Update(int id)
        {
            // khai báo biến action để truyền vào tham số action thẻ form
            ViewBag.action = "/Admin/Users/UpdatePost/" + id;

            // lấy 1 bản ghi
            var list_users = db.Users.Where(item => item.Id == id).FirstOrDefault();

            if (list_users != null)
            {
                return View("CreateUpdate", list_users);
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
                var list_users = db.Users.Where(item => item.Id == id).FirstOrDefault();

                // sử dụng đối tượng IFormCollection để lấy kiểu dữ liệu POST
                string _Name = fc["Name"].ToString().Trim();
                string _Email = fc["Email"].ToString().Trim();
                string _Address = fc["Address"].ToString().Trim();
                string _Phone = fc["Phone"].ToString().Trim();
                string _Password = fc["Password"].ToString();

                // khởi tạo giá trị list_slides
                ItemSlides list_record = new ItemSlides();

                if (list_users != null)
                {
                    // Xử lý ảnh
                    var photoFile = fc.Files.GetFile("Photo");
                    if (photoFile != null && photoFile.Length > 0)
                    {
                        // xóa ảnh cũ
                        if (list_users?.Photo != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Users", list_users.Photo)))
                        {
                            System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Users", list_users.Photo));
                        }

                        // nối chuỗi thời gian vào biến fileName
                        string fileName = DateTime.Now.ToFileTime() + "_" + photoFile.FileName;

                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "User", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            photoFile.CopyTo(stream);
                        }

                        // cập nhật lại đường dẫn ảnh
                        list_record.Photo = fileName;
                    }

                    // kiểm tra xem email đã tồn tại chưa nếu chưa thì update
                    ItemUsers record_check = db.Users.FirstOrDefault(item => item.Id != id && item.Email == _Email);
                    if(record_check == null)
                    {
                        // cập nhật các trường của bản ghi
                        list_users.Name = _Name;
                        list_users.Email = _Email;
                        list_users.Address = _Address;
                        list_users.Phone = _Phone;

                        // cập nhật lại đường dẫn ảnh
                        if (!string.IsNullOrEmpty(list_record.Photo))
                        {
                            list_users.Photo = list_record.Photo;
                        }

                        // nếu password k rỗng thì update 
                        if (!String.IsNullOrEmpty(_Password))
                        {
                            // mã hóa password
                            _Password = BC.HashPassword(_Password);
                            list_users.Password = _Password;
                        }
                        // cập nhật lại dữ liệu
                        db.Update(list_users);
                        db.SaveChanges();
                    }

                    // thông báo 
                    TempData["SuccessMessage"] = $"Cập nhật thành công user id {id}!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy user!";
                }
            }
            catch (Exception)
            {
                // thông báo lỗi
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật user!";
            }

            return RedirectToAction("Read", "Users", new { area = "Admin" });
        }

        // delete
        public IActionResult Delete(int id)
        {
            try
            {
                // lấy bản ghi tương ứng với id truyền vào
                var record = db.Users.Where(item => item.Id == id).FirstOrDefault();
                // xóa ảnh
                if (record?.Photo != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Users", record.Photo)))
                {
                    System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Users", record.Photo));
                }

                // xóa bản ghi
                db.Users.Remove(record);
                // cập nhật
                db.SaveChanges();

                TempData["SuccessMessage"] = $"Xóa thành công user id {id}!";

            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa user!";
            }

            return RedirectToAction("Read", "Users", new { area = "Admin" });
        }
    }
}
