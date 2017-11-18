using MojCRM.Models;
using MojCRM.Areas.Campaigns.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
            Created,

            [Description("Provjereno")]
            Checked,

            [Description("Verificirano")]
            Verified,

            [Description("Revidirano")]
            Reviewed
        }

        public string AcquireEmailStatusString
        {
            get
            {
                switch (AcquireEmailStatus)
                {
                    case AcquireEmailStatusEnum.Created: return "Kreirano";
                    case AcquireEmailStatusEnum.Checked: return "Provjereno";
                    case AcquireEmailStatusEnum.Verified: return "Verificirano";
                    case AcquireEmailStatusEnum.Reviewed: return "Revidirano";
                }
                return "Status provjere";
            }
        }
    }
}