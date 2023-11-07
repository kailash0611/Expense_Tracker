using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace Authentication.Filters
{
    
    public class RedirectAuthenticatedUserFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                // User is authenticated, redirect to dashboard
                context.Result = new RedirectToActionResult("Index", "Dashboard", null);
            }
           
        }
    }

}
