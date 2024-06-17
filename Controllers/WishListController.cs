using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace TTTN_ViettelStore.Controllers
{
	public class WishListController : Controller
	{
        public IActionResult Index()
        {
            string str_wish_list = HttpContext.Session.GetString("wish_list");
            if (!string.IsNullOrEmpty(str_wish_list))
            {
                // Deserialize the JSON array properly
                List<int> list_wish_list = JsonConvert.DeserializeObject<List<int>>(str_wish_list);
                ViewBag.list_wish_list = list_wish_list;
            }

            return View();
        }

        // add
        public IActionResult Add(int id)
        {
            string str_wish_list = HttpContext.Session.GetString("wish_list");
            if(!string.IsNullOrEmpty(str_wish_list))
            {
                List<int> list_wish_list = JsonConvert.DeserializeObject<List<int>>(str_wish_list);
                if(!CheckIdExists(id))
                {
                    list_wish_list.Add(id);
                    string json_wish_list = JsonConvert.SerializeObject(list_wish_list);
                    HttpContext.Session.SetString("wish_list", json_wish_list);
                }    
            }
            else
            {
                List<int> list_wish_list = new List<int>();
                list_wish_list.Add(id);
                string json_wish_list = JsonConvert.SerializeObject(list_wish_list);
                HttpContext.Session.SetString("wish_list", json_wish_list);
            }    

            return RedirectToAction("Index");
        }

		public bool CheckIdExists(int id)
		{
            string str_wish_list = HttpContext.Session.GetString("wish_list");
            if (!String.IsNullOrEmpty(str_wish_list))
            {
                List<int> list_wish_list = JsonConvert.DeserializeObject<List<int>>(str_wish_list);
                foreach (var item in list_wish_list)
                {
                    if (item == id)
                        return true;
                }
            }

            return false;
		}

        // delete
        public IActionResult Remove(int id)
        {
            string str_wish_list = HttpContext.Session.GetString("wish_list");
            if (!string.IsNullOrEmpty(str_wish_list))
            {
                List<int> list_wish_list = JsonConvert.DeserializeObject<List<int>>(str_wish_list);
                list_wish_list.Remove(id);
                string json_wish_list = JsonConvert.SerializeObject(list_wish_list);
                HttpContext.Session.SetString("wish_list", json_wish_list);
            }
            return RedirectToAction("Index");
        }
    }
}
