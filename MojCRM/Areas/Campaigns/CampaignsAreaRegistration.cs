using System.Web.Mvc;

namespace MojCRM.Areas.Campaigns
{
    public class CampaignsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Campaigns";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Campaigns_default",
                "Campaigns/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new [] {"MojCRM.Areas.Campaigns.Controllers"}
            );
        }
    }
}