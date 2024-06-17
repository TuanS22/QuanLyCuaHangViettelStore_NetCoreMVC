using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Data;
using System.Data;
using TTTN_ViettelStore.Models;
using X.PagedList;
using TTTN_ViettelStore.Areas.Admin.Attributes;


namespace TTTN_ViettelStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    
    // kiểm tra
    [CheckLogin]
    
    public class CategoriesController : Controller
    {
        // sử dụng câu lệnh dưới cho Linq
        public MyDbContext db = new MyDbContext();

        // tạo biến lưu chuỗi kết nối
        string strConnectionString;
        // định nghĩa hàm tạo: là hàm sẽ tự động được triệu gọi khi class này hoạt động
        public CategoriesController()
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            this.strConnectionString = config.GetConnectionString("MyConnectionString").ToString();
        }

        public IActionResult Index()
        {
            return RedirectToAction("Read", "Categories", new { area = "Admin" });
        }

        public IActionResult Read(int? page)
        {
            // sử dụng ADO để truyền câu truy vấn sql và lấy kết quả trả về
            // tạo đối tượng DataTable để đổ dữ liệu từ kết quả truy vấn
            DataTable dtCategories = new DataTable();
            using (SqlConnection conn = new SqlConnection(strConnectionString))
            {
                // tạo đối tượng DataAdapter để truyền câu lệnh sql và lấy kết quả trả về
                SqlDataAdapter da = new SqlDataAdapter("select * from Categories where ParentId = 0 order by Id desc", conn);
                // Fill dữ liệu từ da vào DataTable
                da.Fill(dtCategories);
            }

            //--
            //do thư viện X.PagedList sử dụng List để phân trang, vì vậy cần chuyển đổi DataTable có tên là dtCategories sang List
            // tạo đối tượng List để lưu trữ dữ liệu từ dtCategories chuyển sang
            List<ItemCategories> list_categories = new List<ItemCategories>();
            // duyệt các row của dtCategories tạo các list để add vào
            foreach (DataRow item in dtCategories.Rows)
            {
                list_categories.Add(new ItemCategories() { Id = Convert.ToInt32(item["id"]), ParentId = Convert.ToInt32(item["ParentId"]), Name = item["Name"].ToString(), DisplayHomePage = Convert.ToInt32(item["DisplayHomePage"]) });
            }

            if (TempData.ContainsKey("SuccessMessage"))
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            // số bản ghi trên 1 trang
            int pageSize = 4;
            // số trang
            int pageNumber = page ?? 1;
            return View("Read", list_categories.ToPagedList(pageNumber, pageSize));
        }

        // create
        public IActionResult Create()
        {
            // tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/Categories/CreatePost";
            return View("CreateUpdate");
        }

        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            try
            {
                // sử dụng đối tượng IFormCollection để lấy dữu liệu theo kiểu POST
                string _Name = fc["Name"].ToString().Trim();
                // sử dụng đối tượng Request để lấy
                int _ParentId = Convert.ToInt32(Request.Form["ParentId"]);
                int _DisplayHomePage = !String.IsNullOrEmpty(Request.Form["DisplayHomePage"]) ? 1 : 0;
                ItemCategories list_record = new ItemCategories();
                list_record.Name = _Name;
                list_record.ParentId = _ParentId;
                list_record.DisplayHomePage = _DisplayHomePage;

                //xử lý ngoại lệ
                if (string.IsNullOrEmpty(_Name))
                {
                    ModelState.AddModelError("Name", "Tên không được để trống");
                    return View("CreateUpdate");
                }

                // thêm vào csdl
                db.Categories.Add(list_record);
                // cập nhật lại dữ liệu
                db.SaveChanges();

                TempData["SuccessMessage"] = "Thêm danh mục thành công!";

            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Có lỗi khi thực hiện thêm danh mục!";
            }

            return RedirectToAction("Read", "Categories", new { area = "Admin" });
        }

        // update
        public IActionResult Update(int id)
        {
            // tạo biến action để đưa các thuộc tính action của thẻ form vào biến này
            ViewBag.action = "/Admin/Categories/UpdatePost/" + id;
            
            // Do đang dùng ADO nên phải sử dụng kiểu này mới cập nhật được dữ liệu. Dùng linq sẽ không được
            // Dùng DataTable
            DataTable list_record = new DataTable();
            // lấy bản ghi tương ứng với id để truyền vào
            using(SqlConnection conn = new SqlConnection(strConnectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("select * from Categories where Id = " + id, conn);
                // Fill dữ liệu vào DataTable
                da.Fill(list_record);
            }
            // bản ghi của list_record là 1 table chứa 1 row (do truy vấn ở trên chỉ trả về 1 row)

            return View("CreateUpdate", list_record);
        }

        [HttpPost]
        public IActionResult UpdatePost(int id, IFormCollection fc)
        {
            try
            {
                // sử dụng đối tượng IFormCollection để lấy dữ liệu theo kiểu POST
                string _Name = fc["Name"].ToString().Trim();
                // sử dụng đối tượng Request để lấy dữ liệu theo kiểu POST
                int _ParentId = Convert.ToInt32(Request.Form["ParentId"]);
                int _DisplayHomePage = !String.IsNullOrEmpty(Request.Form["DisplayHomePage"]) ? 1 : 0;
                using (SqlConnection conn = new SqlConnection(strConnectionString))
                {
                    // chú ý: phải có dòng dưới này
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("update Categories set Name=@var_name, ParentId=@var_parent_id, DisplayHomePage=@var_display_home_page where Id=@id", conn);
                    // truyền các giá trị vào chuỗi truy vấn
                    cmd.Parameters.AddWithValue("@var_name", _Name);
                    cmd.Parameters.AddWithValue("@var_parent_id", _ParentId);
                    cmd.Parameters.AddWithValue("@var_display_home_page", _DisplayHomePage);
                    cmd.Parameters.AddWithValue("@id", id);

                    TempData["SuccessMessage"] = $"Cập nhật danh mục id {id} thành công!";

                    // thực hiện truy vấn
                    cmd.ExecuteNonQuery();
                }
            }
            catch(Exception)
            {
                TempData["ErrorMessage"] = $"Có lỗi thực hiện cập nhập danh mục id {id}!";
            }

            return RedirectToAction("Read", "Categories", new { area = "Admin" });
        }

        // delete
        public IActionResult Delete(int id)
        {
            try
            {
                // đây là sử dụng ADO
                using (SqlConnection conn = new SqlConnection(strConnectionString))
                {
                    // mở data
                    conn.Open();
                    // lấy bản ghi tương ứng với id
                    SqlCommand cmd = new SqlCommand("delete from Categories where ParentId = @id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = $"Xóa danh mục id {id} thành công!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = $"Có lỗi thực hiện xóa danh mục id {id}!";
            }

            return RedirectToAction("Read", "Categories", new { area = "Admin" });
        }
    }
}
