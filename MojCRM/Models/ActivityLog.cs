using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MojCRM.Helpers;
using System.ComponentModel;

namespace MojCRM.Models
{
    public class ActivityLog
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string User { get; set; }
        public ActivityTypeEnum ActivityType { get; set; }
        public DepartmentEnum Department { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public enum ActivityTypeEnum
        {
            [Description("Test")]
            TEST,

            [Description("Uspješan poziv")]
            SUCCALL,

            [Description("Uspješan poziv (nekonkretni)")]
            SUCCALSHORT,

            [Description("Neuspješan poziv")]
            UNSUCCAL,

            [Description("Izmjena e-mail obavijesti")]
            MAILCHANGE,

            [Description("Ponovno slanje obavijesti o dostavi")]
            RESEND,

            [Description("Slanje e-mailova prema kontaktima dostave")]
            DELMAIL
        }
        public enum DepartmentEnum
        {
            [Description("Moj-CRM")]
            MojCRM,

            [Description("Dostava")]
            Delivery,
        }
    }
}