using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MojCRM.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string Hometown { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string MerUserUsername { get; set; }
        public string MerUserPassword { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<MojCRM.Areas.HelpDesk.Models.Delivery> DeliveryTicketModels { get; set; }

        public System.Data.Entity.DbSet<MojCRM.Models.Organizations> Organizations { get; set; }

        public System.Data.Entity.DbSet<MojCRM.Models.Contact> Contacts { get; set; }

        public System.Data.Entity.DbSet<MojCRM.Areas.HelpDesk.Models.DeliveryDetail> DeliveryDetails { get; set; }

        public System.Data.Entity.DbSet<MojCRM.Models.LogError> LogError { get; set; }

        public System.Data.Entity.DbSet<MojCRM.Models.ActivityLog> ActivityLogs { get; set; }

        public System.Data.Entity.DbSet<MojCRM.Models.MerDeliveryDetails> MerDeliveryDetails { get; set; }

        public System.Data.Entity.DbSet<MojCRM.Areas.Campaigns.Models.Campaign> Campaigns { get; set; }

        public System.Data.Entity.DbSet<MojCRM.Areas.Campaigns.Models.CampaignMember> CampaignMembers { get; set; }

        public System.Data.Entity.DbSet<MojCRM.Areas.Sales.Models.Opportunity> Opportunities { get; set; }

        public System.Data.Entity.DbSet<MojCRM.Areas.Sales.Models.Lead> Leads { get; set; }

        public System.Data.Entity.DbSet<MojCRM.Models.OrganizationDetail> OrganizationDetails { get; set; }

        public System.Data.Entity.DbSet<MojCRM.Areas.Sales.Models.OpportunityNote> OpportunityNotes { get; set; }

        public System.Data.Entity.DbSet<MojCRM.Areas.Sales.Models.LeadNote> LeadNotes { get; set; }

        public System.Data.Entity.DbSet<MojCRM.Models.OrganizationAttribute> OrganizationAttributes { get; set; }

        public System.Data.Entity.DbSet<MojCRM.Areas.Cooperation.Models.MerIntegrationSoftware> MerIntegrationSoftware { get; set; }

        public System.Data.Entity.DbSet<MojCRM.Areas.Stats.Models.MerDocumentExchangeHistory> MerDocumentExchangeHistory { get; set; }

        public System.Data.Entity.DbSet<MojCRM.Areas.HelpDesk.Models.AcquireEmail> AcquireEmails { get; set; }
    }
}