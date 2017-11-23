﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using MojCRM.Models;

namespace MojCRM.Areas.Campaigns.Models
{
    public class CampaignMember
    {
        [Key]
        public int Id { get; set; }

        public int CampaignId { get; set; }

        [ForeignKey("CampaignId")]
        public virtual Campaign Campaign { get; set; }

        public string MemberName { get; set; }
        public MemberRoleEnum MemberRole { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public enum MemberRoleEnum
        {
            [Description("Nositelj kampanje")]
            HEAD,

            [Description("Nadzornik kampanje")]
            SUPERVISOR,

            [Description("Član")]
            MEMBER
        }

        public string MemberRoleString
        {
            get
            {
                switch (MemberRole)
                {
                    case MemberRoleEnum.HEAD: return "Nositelj kampanje";
                    case MemberRoleEnum.SUPERVISOR: return "Nadzornik kampanje";
                    case MemberRoleEnum.MEMBER: return "Član kampanje";
                }
                return "Rola";
            }
        }

        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        public IQueryable<CampaignMember> GetCamapigns(string agent)
        {
            var model = _db.CampaignMembers.Where(cm => cm.MemberName == agent);
            return model;
        }
    }
}