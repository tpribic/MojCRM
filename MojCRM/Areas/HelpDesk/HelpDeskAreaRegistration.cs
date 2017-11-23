using System.Web.Mvc;

namespace MojCRM.Areas.HelpDesk
{
    public class HelpDeskAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "HelpDesk";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "HelpDesk_default",
                "HelpDesk/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "MojCRM.Areas.HelpDesk.Controllers" }
            );
        }
    }
}