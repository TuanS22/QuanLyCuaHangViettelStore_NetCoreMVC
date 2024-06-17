using Microsoft.AspNetCore.Mvc;
// để kế thừa class ActionFilterAttribute thì phải using dòng bên dưới
using Microsoft.AspNetCore.Mvc.Filters;

namespace TTTN_ViettelStore.Areas.Admin.Attributes
{
	public class CheckLogin : ActionFilterAttribute
	{
		public override void OnActionExecuted(ActionExecutedContext context)
		{
			// lấy giá trị của session email
			string email = context.HttpContext.Session.GetString("admin_email");
			if(string.IsNullOrEmpty(email) )
			{
				context.HttpContext.Response.Redirect("/Admin/Account/Login");
			}

			base.OnActionExecuted(context);
		}

	}
}
