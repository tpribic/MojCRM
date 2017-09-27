using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MojCRM.Areas.HelpDesk.Helpers
{
    public class EditAcquiredReceivingInformationHelper
    {
        public int MerId { get; set; }
        public string AcquiredReceivingInformation { get; set; }
        public string NewAcquiredReceivingInformation { get; set; }
    }

    public class AcquireEmailExportModel
    {
        public string CampaignName { get; set; }
        public string VAT { get; set; }
        public string SubjectName { get; set; }
        public string AcquiredReceivingInformation { get; set; }
    }

    public class AcquireEmailCheckResults
    {
        public int CampaignId { get; set; }
        public int ValidEntities { get; set; }
        public int InvalidEntities { get; set; }
        public int ImportedEntities { get; set; }
        public List<string> ValidVATs { get; set; }
        public List<string> InvalidVATs { get; set; }
    }
}