using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MojCRM.Helpers
{
    public class EditOrganizationDetails
    {
        public int MerId { get; set; }
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
}