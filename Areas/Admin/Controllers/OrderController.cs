using Microsoft.AspNetCore.Mvc;
using TTTN_ViettelStore.Models;
using X.PagedList;
using TTTN_ViettelStore.Areas.Admin.Attributes;

namespace TTTN_ViettelStore.Areas.Admin.Controllers
{
    [Area("Admin")]

    // kiểm tra login
    [CheckLogin]

    public class OrderController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Index(int? page)
        {
            // số bản ghi trên 1 trang
            int pageSize = 10;
            int pageNumber = page ?? 1;
            List<ItemOrders> list_order = db.Orders.OrderByDescending(item => item.Id).ToList();
            return View("Read", list_order.ToPagedList(pageNumber, pageSize));
        }

        // chi tiết sản phẩm
        public IActionResult Detail(int? id)
        {
            int _OrderId = id ?? 0;
            ViewBag.OrderId = _OrderId;
            // lấy danh sách các sản phẩm thuộc đơn hàng
            List<ItemOrderDetail> list_record = db.OrderDetail.Where(item => item.OrderId == _OrderId).ToList();
            return View("Detail", list_record);
        }

        // giao hàng
        public IActionResult Delivery(int? id)
        {
            int _OrderId = id ?? 0;
            ItemOrders record = db.Orders.Where(item => item.Id == _OrderId).FirstOrDefault();
            if(record != null)
            {
                record.Status = 1;
                db.SaveChanges();
            }

            return Redirect("/Admin/Orders");
        }
    }
}
