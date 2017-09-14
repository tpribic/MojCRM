using MojCRM.Models;
using MojCRM.Areas.Campaigns.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MojCRM.Areas.HelpDesk.Models
{
    public class AcquireEmail
    {
        [Key]
        public int Id { get; set; }
        public int? RelatedOrganizationId { get; set; }
        public int? RelatedCampaignId { get; set; }
        public bool IsAssigned { get; set; }
        public string AssignedTo { get; set; }
        public AcquireEmailStatusEnum AcquireEmailStatus { get; set; }
        public string LastContactedBy { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? LastContactDate { get; set; }

        [ForeignKey("RelatedOrganizationId")]
        public virtual Organizations Organization { get; set; }
        [ForeignKey("RelatedCampaignId")]
        public virtual Campaign Campaign { get; set; }

        public enum AcquireEmailStatusEnum
        {
            [Description("Kreirano")]
            CREATED,

            [Description("Provjereno")]
            CHECKED,

            [Description("Verificirano")]
            VERIFIED
        }

        public string AcquireEmailStatusString
        {
            get
            {
                switch (AcquireEmailStatus)
                {
                    case AcquireEmailStatusEnum.CREATED: return "Kreirano";
                    case AcquireEmailStatusEnum.CHECKED: return "Provjereno";
                    case AcquireEmailStatusEnum.VERIFIED: return "Verificirano";
                }
                return "Status provjere";
            }
        }
    }
}