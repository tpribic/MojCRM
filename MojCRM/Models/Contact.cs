using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MojCRM.Models
{
    public class Contact
    {
        [Key]
        public int ContactId { get; set; }

        public virtual Organizations Organization { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string Title { get; set; }
        public string TelephoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string Email { get; set; }
        public virtual ApplicationUser User { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string ContactType { get; set; }
    }
}