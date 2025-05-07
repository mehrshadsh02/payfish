using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace payfish.Security
{
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var route = context.RouteData;

            var controller = route.Values["controller"]?.ToString()?.ToLower();
            var action = route.Values["action"]?.ToString()?.ToLower();

            // 🚫 اگر خود کنترلر و اکشن لاگین هستند، اجازه بده رد بشه
            if (controller == "admin" && (action == "login" || action == "logout"))
                return;

            // ✅ بررسی لاگین بودن ادمین
            var isLoggedIn = httpContext.Session.GetString("AdminUsername");
            if (string.IsNullOrEmpty(isLoggedIn))
            {
                context.Result = new RedirectToActionResult("Login", "Admin", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
