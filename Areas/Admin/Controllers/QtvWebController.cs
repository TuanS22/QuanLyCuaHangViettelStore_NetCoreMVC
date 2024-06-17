using Microsoft.AspNetCore.Mvc;
using TTTN_ViettelStore.Models;
using BC = BCrypt.Net.BCrypt; // mã hóa password

namespace TTTN_ViettelStore.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class QtvWebController : Controller
	{
		public MyDbContext db = new MyDbContext();
		public IActionResult Index()
		{
			return Redirect("LoginQtv");
		}

		public IActionResult LoginQtv()
		{
			return View();
		}

		[HttpPost]
		public IActionResult LoginPost(IFormCollection fc)
		{
			try
			{
				string _Email = fc["Email"].ToString().Trim();
				string _Password = fc["Password"].ToString().Trim();

				// lấy bản ghi tương ứng vs email để truyền vào
				var list_record = db.QtvWebs.Where(item => item.Email == _Email).FirstOrDefault();

				if (list_record != null)
				{
					if (BC.Verify(_Password, list_record.Password))
					{
						// khởi tạo session 
						HttpContext.Session.SetString("qtv_user_id", list_record.Id.ToString());
						// khởi tạo session Email
						HttpContext.Session.SetString("qtv_email", _Email);
						// di chuyển đến Url
						return Redirect("/Admin/HomeQtv?notify=invalid");
					}
				}

			}
			catch (Exception)
			{
				;
			}

			return Redirect("/Admin/QtvWeb/LoginQtv?notify=invalid");
		}

		// logout
		public IActionResult Logout()
		{
			// Xóa các session đa đặt lúc login
			HttpContext.Session.Remove("qtv_user_id");
			HttpContext.Session.Remove("qtv_email");

			return RedirectToAction("LoginQtv", "QtvWeb", new { area = "Admin" });
		}
	}
}
