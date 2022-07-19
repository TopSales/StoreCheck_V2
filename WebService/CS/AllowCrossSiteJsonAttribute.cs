using Microsoft.AspNetCore.Mvc.Filters;

/// <summary>
/// https://stackoverflow.com/questions/6290053/setting-access-control-allow-origin-in-asp-net-mvc-simplest-possible-method
/// </summary>

public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
{
   //public override void OnActionExecuting(ActionExecutingContext filterContext)
   //{
   //   filterContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
   //   base.OnActionExecuting(filterContext);
   //}

   public override void OnActionExecuting(ActionExecutingContext filterContext)
   {
      /// https://stackoverflow.com/questions/6290053/setting-access-control-allow-origin-in-asp-net-mvc-simplest-possible-method
      /// https://stackoverflow.com/questions/31942037/how-to-enable-cors-in-asp-net-core/31942128#31942128
      filterContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
      filterContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Methods", "DELETE, POST, GET, OPTIONS");
      filterContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Access-Control-Allow-Headers, Authorization, X-Requested-With");
      filterContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Credentials", new[] { "true" });

      if (filterContext.HttpContext.Request.Method == "OPTIONS")
      {
         filterContext.HttpContext.Response.StatusCode = 200;
         // await filterContext.HttpContext.Response.WriteAsync("OK");
      };

      base.OnActionExecuting(filterContext);
   }
}
