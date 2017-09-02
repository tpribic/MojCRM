using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static MojCRM.Models.Organizations;
using MojCRM.Models;
using System.ComponentModel.DataAnnotations;

namespace MojCRM.Helpers
{
    public class EditOrganizationDetails
    {
        public int MerId { get; set; }
        public string MainAddress { get; set; }
        public int MainPostalCode { get; set; }
        public string MainCity { get; set; }
        public string CorrespondenceAddress { get; set; }
        public int CorrespondencePostalCode { get; set; }
        public string CorrespondenceCity { get; set; }
        public string TelephoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string ERP { get; set; }
        public string NumberOfInvoicesSent { get; set; }
        public string NumberOfInvoicesReceived { get; set; }
    }
    public class EditImportantOrganizationInfo
    {
        public int MerId { get; set; }
        public LegalFormEnum? LegalForm { get; set; }
        public OrganizationGroupEnum? OrganizationGroup { get; set; }
        public ServiceProviderEnum? ServiceProvider { get; set; }
        public int? LegalStatus { get; set; }
    }
    public class AddOrganizationAttribute
    {
        public int MerId { get; set; }
        public OrganizationAttribute.AttributeClassEnum AttributeClass { get; set; }
        public OrganizationAttribute.AttributeTypeEnum AttributeType { get; set; }
    }

    //JSON object for dropdown menu when searching the company via autocomplete
    public class OrganizationSearch
    {
        [JsonProperty]
        public int MerId { get; set; }
        public string OrganizationName { get; set; }
    }
}