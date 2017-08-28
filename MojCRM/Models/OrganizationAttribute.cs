using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MojCRM.Models
{
    public class OrganizationAttribute
    {
        [Key]
        public int Id { get; set; }

        public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public Organizations Organization { get; set; }
        public AttributeClassEnum AttributeClass { get; set; }
        public AttributeTypeEnum AttributeType { get; set; }
        public bool IsActive { get; set; }
        public string AssignedBy { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public enum AttributeClassEnum
        {
            [Description("Opći")]
            GENERAL,

            [Description("Moj-eRačun")]
            MER,

            [Description("FINA")]
            FINA,

            [Description("eFaktura")]
            EFAKTURA
        }

        public enum AttributeTypeEnum
        {
            [Description("Opći")]
            GENERAL,

            [Description("Pošiljatelj")]
            SENDER,

            [Description("Primatelj")]
            RECEIVER,

            [Description("Ugovorni korisnik")]
            CONTRACT,

            [Description("PrePaid korisnik")]
            ADVANCE,

            [Description("Moj-DMS korisnik")]
            DMS,

            [Description("Moj-eArhiv korisnik")]
            EARCHIVE,

            [Description("Partner - integrator")]
            INTEGRATOR,

            [Description("Partner - knjigovodstveni servis")]
            ACCOUNTINGPARTNER
        }

        public string AttributeClassString
        {
            get
            {
                switch (AttributeClass)
                {
                    case AttributeClassEnum.GENERAL: return "Opći";
                    case AttributeClassEnum.MER: return "Moj-eRačun";
                    case AttributeClassEnum.FINA: return "FINA";
                    case AttributeClassEnum.EFAKTURA: return "eFaktura";
                }
                return "Nepoznato";
            }
        }
        public string AttributeClassIcon
        {
            get
            {
                switch (AttributeClass)
                {
                    case AttributeClassEnum.GENERAL: return "Opći";
                    case AttributeClassEnum.MER: return @"~/Content/e_racun_logo_source.png";
                    case AttributeClassEnum.FINA: return @"~/Content/630px-FINA_Logo.svg.png";
                    case AttributeClassEnum.EFAKTURA: return @"~/Content/eFaktura2.png";
                }
                return "Nepoznato";
            }
        }
        public string AttributeTypeString
        {
            get
            {
                switch (AttributeType)
                {
                    case AttributeTypeEnum.GENERAL: return "Opći";
                    case AttributeTypeEnum.SENDER: return "Pošiljatelj";
                    case AttributeTypeEnum.RECEIVER: return "Primatelj";
                    case AttributeTypeEnum.CONTRACT: return "Ugovorni korisnik";
                    case AttributeTypeEnum.ADVANCE: return "PrePaid korisnik";
                    case AttributeTypeEnum.DMS: return "Moj-DMS korisnik";
                    case AttributeTypeEnum.EARCHIVE: return "Moj-eArhiv korisnik";
                    case AttributeTypeEnum.INTEGRATOR: return "Partner - integrator";
                    case AttributeTypeEnum.ACCOUNTINGPARTNER: return "Partner - knjigovodstveni servis";
                }
                return "Nepoznato";
            }
        }
        public string AttributeTypeIcon
        {
            get
            {
                switch (AttributeType)
                {
                    case AttributeTypeEnum.GENERAL: return "Opći";
                    case AttributeTypeEnum.SENDER: return @"fa fa-lg fa-upload";
                    case AttributeTypeEnum.RECEIVER: return @"fa fa-lg fa-download";
                    case AttributeTypeEnum.CONTRACT: return @"fa fa-lg fa-file-text";
                    case AttributeTypeEnum.ADVANCE: return @"fa fa-lg fa-money";
                    case AttributeTypeEnum.DMS: return @"fa fa-lg fa-files-o";
                    case AttributeTypeEnum.EARCHIVE: return @"fa fa-lg fa-archive";
                    case AttributeTypeEnum.INTEGRATOR: return @"fa fa-lg fa-share-alt";
                    case AttributeTypeEnum.ACCOUNTINGPARTNER: return @"fa fa-lg fa-share-alt-square";
                }
                return "Nepoznato";
            }
        }
    }
}