using System.Web.Mvc;

namespace Web.Areas.APP
{
    public class APPAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "APP";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "APP_default",
                "APP/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}