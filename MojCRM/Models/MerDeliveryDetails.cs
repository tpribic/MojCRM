using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MojCRM.Models
{
    public class MerDeliveryDetails
    {
        [Key, ForeignKey("Organization")]
        public int MerId { get; set; }
        public virtual Organizations Organization { get; set; }
        public string Comments { get; set; }
        public string Telephone { get; set; }
        public string ImportantComments { get; set; }
    }
}