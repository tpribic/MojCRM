using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MojCRM.Models
{
    public class Contact
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Key]
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

        public enum ContactTypeEnum
        {
            [Description("Općeniti")]
            GENERAL,

            [Description("Dostava")]
            DELIVERY
        }

        [Display(Name = "Dodijeli novog agenta")]
        [NotMapped]
        public IList<SelectListItem> Agents
        {
            get
            {
                var agents = (from u in db.Users
                              select new SelectListItem()
                              {
                                  Text = u.UserName,
                                  Value = u.UserName
                              }).ToList();
                return agents;
            }
            set { }
        }
    }
}