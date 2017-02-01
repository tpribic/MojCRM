using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MojCRM.Models
{
    public class Organizations
    {
        [Key]
        public int OrganizationId { get; set; }

        [Display(Name = "OIB)")]
        public string VAT { get; set; }

        [Display(Name = "Naziv tvrtke")]
        public string SubjectName { get; set; }

        [Display(Name = "Moj-eRačun ID")]
        public int MerId { get; set; }
    }

    public class OrganizationsDbContext : ApplicationDbContext
    {
        public DbSet<Organizations> Organization { get; set; } 
    }
}