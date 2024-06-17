using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging.Signing;
using TTTN_ViettelStore.Models;

namespace TTTN_ViettelStore.Controllers
{
    public class NewsController : Controller
    {
        public MyDbContext db = new MyDbContext();

		public IActionResult Detail(int id)
        {
            var record = db.News.Where(item => item.Id == id).FirstOrDefault();

			return View("NewsDetail", record);
        }
    }
}
