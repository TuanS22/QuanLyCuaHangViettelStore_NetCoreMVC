using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using TTTN_ViettelStore.Areas.Admin;
using TTTN_ViettelStore.Areas.Admin.Attributes;

namespace TTTN_ViettelStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    // kiểm tra login
    [CheckLogin]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
