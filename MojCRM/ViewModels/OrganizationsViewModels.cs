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
        public IQueryable<Delivery> TicketsAsReceiver { get; set; }
        public IQueryable<Delivery> TicketsAsSender { get; set; }

    }
}