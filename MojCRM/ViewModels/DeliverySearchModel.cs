using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MojCRM.ViewModels
{
    public class DeliverySearchModel : Delivery
    {
        public virtual string ReceiverVAT { get; private set; }
        public virtual string ReceiverName { get; set; }


        public virtual Organizations Organization { get; set; }
    }

    public class DeliverySearchDbContext : ApplicationDbContext
    {
        public DbSet<DeliverySearchModel> DeliverySearch { get; set; }
    }
}