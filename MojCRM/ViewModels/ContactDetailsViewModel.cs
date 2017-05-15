using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using static MojCRM.Models.Contact;

namespace MojCRM.ViewModels
{
    public class ContactDetailsViewModel
    {
        public int ContactId { get; set; }

        public int OrganizationId { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organizations Organization { get; set; }

        [Display(Name = "Ime kontakta")]
        public string ContactFirstName { get; set; }

        [Display(Name = "Prezime kontakta")]
        public string ContactLastName { get; set; }

        [Display(Name = "Titula / funkcija")]
        public string Title { get; set; }

        [Display(Name = "Broj telefona")]
        public string TelephoneNumber { get; set; }

        [Display(Name = "Broj mobitela")]
        public string MobilePhoneNumber { get; set; }

        [Display(Name = "E-mail adresa")]
        public string Email { get; set; }

        [Display(Name = "Agent")]
        public string User { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        [Display(Name = "Tip kontakta")]
        public ContactTypeEnum ContactType { get; set; }
        public IEnumerable<DeliveryDetail> DeliveryDetails { get; set; }
    }
}