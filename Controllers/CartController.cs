using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TTTN_ViettelStore.Models;

namespace TTTN_ViettelStore.Controllers
{
    public class CartController : Controller
    {
        public MyDbContext db = new MyDbContext();

        // hiển thị danh sách các sản phẩm trong giỏ hàng
        public IActionResult Index()
        {
            List<Item> list_cart = new List<Item>();
            string json_cart = HttpContext.Session.GetString("cart");

            if(!String.IsNullOrEmpty(json_cart))
            {
                // convert chuỗi json
                list_cart = JsonConvert.DeserializeObject<List<Item>>(json_cart);
            }   
            
            return View(list_cart);
        }

        // thêm sản phẩm vào giỏ hàng
        public IActionResult Buy(int id)
        {
            // gọi hàm CartAdd (trong class Cart) để thêm sản phẩm
            Cart.CartAdd(HttpContext.Session, id);
            // di chuyển đến action Index
            return RedirectToAction("Index");
        }

        // update số lượng sản phẩm
        public IActionResult Update()
        {
            List<Item> shopping_cart = new List<Item>();
            string json_cart = HttpContext.Session.GetString("cart");
            if (!String.IsNullOrEmpty(json_cart))
            {
                // convert chuỗi json
                shopping_cart = JsonConvert.DeserializeObject<List<Item>>(json_cart);
            }
            // duyệt các phần tử
            foreach (var cart_item in shopping_cart)
            {
                // lấy số lượng sản phẩm tử thẻ input
                int new_quantity = Convert.ToInt32(Request.Form["product_" + cart_item.ProductRecord.Id]);
                // gọi hàm cập nhật lại số lượng sản phẩm
                Cart.CartUpdate(HttpContext.Session, cart_item.ProductRecord.Id, new_quantity);
            }
            // di chuyển đến action Index
            return RedirectToAction("Index");
        }

        // Xóa từng sản phẩm
        public IActionResult Remove(int id)
        {
            Cart.CartRemove(HttpContext.Session, id);
            // di chuyển đến action Index
            return RedirectToAction("Index");
        }

        // Xóa toàn bộ sản phẩm
        public IActionResult Destroy()
        {
            Cart.CartDestroy(HttpContext.Session);
            // di chuyển action đến Index
            return Redirect("/Cart/Index?notify=destroy-success");
        }

        // thanh toán đơn hàng thông qua hàm checkout
        public IActionResult CheckOut()
        {
            // nếu user chưa login thì di chuyển đến login 
            if(String.IsNullOrEmpty(HttpContext.Session.GetString("user_id")))
            {
                return Redirect("/Account/Login");
            }    
            else
            {
                // lấy id của user
                int user_id = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
                Cart.CartCheckOut(HttpContext.Session, user_id);
            }
            // di chuyển đến action Index
            return Redirect("/Cart/Index?notify=checkout-success");
        }
    }
}
