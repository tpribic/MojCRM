using System.Web.Mvc;

namespace MojCRM.Areas.Stats
{
    public class StatsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Stats";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Stats_default",
                "Stats/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "MojCRM.Areas.Stats.Controllers" }
            );
        }
    }
}