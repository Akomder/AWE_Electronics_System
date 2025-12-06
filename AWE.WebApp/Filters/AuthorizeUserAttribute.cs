#nullable disable
using AWE.WebApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AWE.WebApp.Filters
{
    public class AuthorizeUserAttribute : ActionFilterAttribute
    {
        public string[]? AllowedRoles { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = SessionHelper.GetCurrentUser(context.HttpContext.Session);

            if (user == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (AllowedRoles != null && AllowedRoles.Length > 0)
            {
                if (!AllowedRoles.Contains(user.Role))
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
