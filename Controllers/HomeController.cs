using Microsoft.AspNetCore.Mvc;

namespace TTTN_ViettelStore.Controllers
{
    public class HomeController :Controller
    {
        public IActionResult Index()
        {
            return View();
            // Di chuyển đến Admin
            // return Redirect("/Admin");
        }
    }
}
