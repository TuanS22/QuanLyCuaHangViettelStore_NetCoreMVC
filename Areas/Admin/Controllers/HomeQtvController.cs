using Microsoft.AspNetCore.Mvc;
using System.Net;
using TTTN_ViettelStore.Models;
using X.PagedList;
using BC = BCrypt.Net.BCrypt; // mã hóa

namespace TTTN_ViettelStore.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class HomeQtvController : Controller
	{
		public MyDbContext db = new MyDbContext();
		public IActionResult Index()
		{
			return Redirect("/Admin/HomeQtv/Read");
		}

		public IActionResult Read(int? page)
		{
			// số bản ghi trên 1 trang
			int pageSize = 4;
			// số trang
			int pageNumber = page ?? 1;
			List<ItemAdmin> list_admin = db.Admin.OrderByDescending(item => item.Id).ToList();
			return View("Read", list_admin.ToPagedList(pageNumber, pageSize));
		}

		// create
		public IActionResult Create()
		{
			ViewBag.action = "/Admin/HomeQtv/CreatePost";
			return View("CreateUpdate");
		}

		[HttpPost]
		public IActionResult CreatePost(IFormCollection fc)
		{
			try
			{ 
				string _Name = fc["Name"].ToString().Trim();
				string _Email = fc["Email"].ToString().Trim();
				string _Password = fc["Password"].ToString().Trim();

				// khởi tạo giá trị list_admin
				ItemAdmin list_admin = new ItemAdmin
                {
					Name = _Name,
					Email = _Email,
					Password = _Password
				};

                // xử lý Photo
                var photoFile = fc.Files.GetFile("Photo");
                if (photoFile != null && photoFile.Length > 0)
                {
                    // nối chuỗi thời gian vào biến fileName
                    string fileName = DateTime.Now.ToFileTime() + "_" + photoFile.FileName;

                    // Part.Combine(duongdan1, duongdan2, ...) nối đường dẫn 1 và đường dẫn 2 thành một đường dẫn
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Admin", fileName);

                    // upload file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        Request.Form.Files[0].CopyTo(stream);
                    }
                    // cập nhật lại đường dẫn ảnh
                    list_admin.Photo = fileName;
                }

				db.Admin.Add(list_admin);
				db.SaveChanges();

				TempData["SuccessMessage"] = "Thêm tài khoản admin thành công!";
            }
			catch (Exception )
			{
				TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm tài khoản admin!";
			}

			return RedirectToAction("Read", "HomeQtv", new { area = "Admin" });
		}

		// update
		public IActionResult Update(int id)
		{
			// khai báo biến action để truyền vào tham số
			ViewBag.action = "/Admin/HomeQtv/UpdatePost/" + id;

			// lấy bản ghi tương ứng
			var list_admin = db.Admin.Where(item => item.Id == id).FirstOrDefault();

			if (list_admin != null)
			{
				return View("CreateUpdate", list_admin);
			}

			return NotFound();
		}

		[HttpPost]
		public IActionResult UpdatePost(int id, IFormCollection fc)
		{
			try
			{
                // lấy 1 bản ghi
                var list_admin = db.Admin.Where(item => item.Id == id).FirstOrDefault();

                // sử dụng đối tượng IFormCollection để lấy kiểu dữ liệu POST
                string _Name = fc["Name"].ToString().Trim();
                string _Email = fc["Email"].ToString().Trim();
                string _Password = fc["Password"].ToString();

				ItemAdmin list_record = new ItemAdmin();

				if(list_admin != null)
				{
                    // Xử lý ảnh
                    var photoFile = fc.Files.GetFile("Photo");
                    if (photoFile != null && photoFile.Length > 0)
                    {
                        // xóa ảnh cũ
                        if (list_admin?.Photo != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Admin", list_admin.Photo)))
                        {
                            System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Admin", list_admin.Photo));
                        }

                        // nối chuỗi thời gian vào biến fileName
                        string fileName = DateTime.Now.ToFileTime() + "_" + photoFile.FileName;

                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Admin", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            photoFile.CopyTo(stream);
                        }

                        // cập nhật lại đường dẫn ảnh
                        list_record.Photo = fileName;
                    }

                    // kiểm tra xem email đã tồn tại chưa nếu chưa thì update
                    ItemAdmin record_check = db.Admin.FirstOrDefault(item => item.Id != id && item.Email == _Email);

                    if (record_check == null)
                    {
                        // cập nhật các trường của bản ghi
                        list_admin.Name = _Name;
                        list_admin.Email = _Email;

                        // cập nhật lại đường dẫn ảnh
                        if (!string.IsNullOrEmpty(list_record.Photo))
                        {
                            list_admin.Photo = list_record.Photo;
                        }

                        // nếu password k rỗng thì update 
                        if (!String.IsNullOrEmpty(_Password))
                        {
                            // mã hóa password
                            _Password = BC.HashPassword(_Password);
                            list_admin.Password = _Password;
                        }
                        // cập nhật lại dữ liệu
                        db.Update(list_admin);
                        db.SaveChanges();
                    }

                    // thông báo 
                    TempData["SuccessMessage"] = $"Cập nhật thành công tài khoản id {id} admin!";
                }
            }
			catch (Exception )
			{
                TempData["ErrorMessage"] = $"Có lỗi xảy ra khi xóa id {id} admin!";
            }

            return RedirectToAction("Read", "HomeQtv", new { area = "Admin" });
		}

        // delete
        public IActionResult Delete(int id)
        {
            try
            {
                // lấy bản ghi tương ứng với id truyền vào
                var record = db.Admin.Where(item => item.Id == id).FirstOrDefault();
                // xóa ảnh
                if (record?.Photo != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Admin", record.Photo)))
                {
                    System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Admin", record.Photo));
                }

                // xóa bản ghi
                db.Admin.Remove(record);
                // cập nhật
                db.SaveChanges();

                TempData["SuccessMessage"] = $"Xóa thành công admin id {id}!";

            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa admin!";
            }

            return RedirectToAction("Read", "HomeQtv", new { area = "Admin" });
        }
    }
}
