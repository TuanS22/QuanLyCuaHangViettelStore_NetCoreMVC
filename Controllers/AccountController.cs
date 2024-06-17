using Microsoft.AspNetCore.Mvc;
using TTTN_ViettelStore.Models;
using X.PagedList;
using BC = BCrypt.Net.BCrypt;

namespace TTTN_ViettelStore.Controllers
{
    public class AccountController : Controller
    {

        public MyDbContext db = new MyDbContext();
        public IActionResult Index()
        {
            return View();
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

            // lấy bản ghi tương ứng với email truyền vào
            var record = db.Users.Where(item => item.Email == _Email).FirstOrDefault();

            if (record != null)
            {
                if (BC.Verify(_Password, record.Password))
                {
                    // khởi tạo session UserId
                    HttpContext.Session.SetString("user_id", record.Id.ToString());
                    // khởi tạo session Email
                    HttpContext.Session.SetString("user_email", _Email);
                    // di chuyển đến url
                    return Redirect("/Home");
                }
                else
                   return Redirect("/Account/Login?notify=invalid");
            }
            else
                return Redirect("/Account/Login?notify=invalid");
        }

        // đăng xuất
        public IActionResult Logout()
        {
            // Xóa session
            HttpContext.Session.Remove("user_id");
            HttpContext.Session.Remove("user_email");
            return Redirect("/Home");
        }

        // đăng ký
        [HttpPost]
        public IActionResult RegisterPost(IFormCollection fc)
        {
            string _Name = fc["Name"].ToString().Trim();
            string _Email = fc["Email"].ToString().Trim();
            string _Phone = fc["Phone"].ToString().Trim();
            string _Address = fc["Address"].ToString().Trim();
            string _Password = fc["Password"].ToString().Trim();

            // mã hóa password
            _Password = BC.HashPassword(_Password);

            // kiểm tra trong csdl đã tồn tại chưa, nếu chưa thì add
            ItemUsers record_check = db.Users.FirstOrDefault(item => item.Email == _Email);

            // nếu bằng null thì add
            if (record_check == null)
            {
                ItemUsers record = new ItemUsers();
                record.Name = _Name;
                record.Email = _Email;
                record.Phone = _Phone;
                record.Address = _Address;
                record.Password = _Password;

                // cập nhật lại dữ liệu
                db.Users.Add(record);
                db.SaveChanges();
            }
            else
                return Redirect("/Account/Login?notify=email-exists");
            return Redirect("/Account/Login?notify=success");
        }

        // danh sách đơn hàng   
        public IActionResult Orders(int? page)
        {
            // lấy UserId để hiển thị các đơn hàng của người này
            int user_id = 0;
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("user_id")))
                return Redirect("/Account/Login");
            else
                user_id = Convert.ToInt32(HttpContext.Session.GetString("user_id"));

            int pageSize = page ?? 1;
            // định nghĩa số bản ghi trên một trang
            int pageNumber = 10;

            // lấy tất cả các bản ghi trong table
            List<ItemOrders> list_order = db.Orders.Where(item => item.CustomerId == user_id).OrderByDescending(item => item.Id).ToList();
            
            // truyền gí trị ra view có phân trang
            return View("Orders", list_order.ToPagedList(pageSize, pageNumber));
        }

        // chi tiết sản phẩm
        public IActionResult Detail(int? id)
        {
            int _OrderId = id ?? 0;
            ViewBag.OrderId = _OrderId;
            // lấy danh sách các sản phẩm thuộc đơn hàng
            List<ItemOrderDetail> listRecord = db.OrderDetail.Where(item => item.OrderId == _OrderId).ToList();
            return View("Detail", listRecord);
        }

        // hủy đơn hàng
        public IActionResult Cancel(int? id)
        {
            int _OrderId = id ?? 0;
            ItemOrders record = db.Orders.Where(item => item.Id == _OrderId).FirstOrDefault();
            if (record != null)
            {
                record.Status = 2; // Status = 2 tương ứng vs hủy đơn hàng
                // cập nhật lại kêt quả
                db.SaveChanges();
            }
            return Redirect("/Account/Orders");
        }
    }
}
