using Microsoft.AspNetCore.Mvc;
using TTTN_ViettelStore.Models;
using BC = BCrypt.Net.BCrypt; // mã hóa password

namespace TTTN_ViettelStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        public MyDbContext db = new MyDbContext();

        public IActionResult Index()
        {
            return Redirect("Login");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LoginPost(IFormCollection fc)
        {
            string _Email = fc["Email"].ToString().Trim();
            string _Password = fc["Password"].ToString().Trim();
            
            // lấy 1 bản ghi tương ứng vs email để truyền vào
            var record = db.Admin.Where(item => item.Email == _Email).FirstOrDefault();
            if (record != null)
            {
                if(BC.Verify(_Password, record.Password))
                {
                    // khởi tạo session UserId
                    HttpContext.Session.SetString("admin_user_id", record.Id.ToString());
                    // khởi tạo session Email
                    HttpContext.Session.SetString("admin_email", _Email);
                    // di chuyển đến url
                    return Redirect("/Admin/Home?notify=invalid");
                }
            }
            return Redirect("/Admin/Account/Login?notify=invalid");
        }

        // logout
        public IActionResult Logout()
        {
            // Xóa các session đa đặt lúc login thành công
            HttpContext.Session.Remove("admin_user_id");
            HttpContext.Session.Remove("admin_email");

            return RedirectToAction("Login", "Account", new { area = "Admin" });
		}
    }
}
