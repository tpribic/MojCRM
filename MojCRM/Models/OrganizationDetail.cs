using MojCRM.Areas.Cooperation.Models;
using MojCRM.Helpers;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MojCRM.Models
{
    public class OrganizationDetail
    {
        [Key, ForeignKey("Organization")]
        public int MerId { get; set; }

        //Main address info
        [Display(Name = "Adresa:")]
        public string MainAddress { get; set; }
        [Display(Name = "Poštanski broj:")]
        public int MainPostalCode { get; set; }
        [Display(Name = "Grad / Mjesto:")]
        public string MainCity { get; set; }
        [Display(Name = "Država:")]
        public CountryIdentificationCodeEnum MainCountry { get; set; }

        //Address info for correspondence
        [Display(Name = "Adresa:")]
        public string CorrespondenceAddress { get; set; }
        [Display(Name = "Poštanski broj:")]
        public int CorrespondencePostalCode { get; set; }
        [Display(Name = "Grad / Mjesto:")]
        public string CorrespondenceCity { get; set; }
        [Display(Name = "Država:")]
        public CountryIdentificationCodeEnum CorrespondenceCountry { get; set; }

        public string TelephoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string ERP { get; set; }
        public string NumberOfInvoicesSent { get; set; }
        public string NumberOfInvoicesReceived { get; set; }
        public OrganizationGroupEnum OrganizationGroup { get; set; }

        public int? MerSendSoftware { get; set; }
        [ForeignKey("MerSendSoftware")]
        public virtual MerIntegrationSoftware SendSoftware { get; set; }

        public int? MerReceiveSoftware { get; set; }
        [ForeignKey("MerReceiveSoftware")]
        public virtual MerIntegrationSoftware ReceiveSoftware { get; set; }

        public virtual Organizations Organization { get; set; }

        public enum CountryIdentificationCodeEnum
        {
            [Description("Nema podatka")]
            Noinfo,

            [Description("Hrvatska")]
            Hr,

            [Description("Slovenija")]
            Si
        }
    }
}