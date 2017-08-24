using MojCRM.Areas.Sales.Models;
using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MojCRM.ViewModels
{
    public class OrganizationDetailsViewModel
    {
        public Organizations Organization { get; set; }
        public IQueryable<Organizations> OrganizationBusinessUnits { get; set; }
        public IQueryable<Opportunity> Opportunities { get; set; }
        public int OpportunitiesCount { get; set; }
        public IQueryable<Lead> Leads { get; set; }
        public int LeadsCount { get; set; }
        public IQueryable<Delivery> TicketsAsReceiver { get; set; }
        public int TicketsAsReceiverCount { get; set; }
        public IQueryable<Delivery> TicketsAsSender { get; set; }
        public int TicketsAsSenderCount { get; set; }
    }
}