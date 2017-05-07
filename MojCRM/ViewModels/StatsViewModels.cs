using MojCRM.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MojCRM.ViewModels
{
    public class DeliveryStatsViewModel
    {

    }

    public class CallCenterDailyStatsViewModel
    {
        public IList<ActivityLog> Activities { get; set; }

        //public ApplicationDbContext db = new ApplicationDbContext();

        //public IList<ActivityLog> Activities
        //{
        //    get
        //    {
        //        var Results = string.Format(@"
        //         SELECT [User], [ActivityType], SUM([ActivityType])
        //         FROM dbo.ActivityLogs al
        //         WHERE InsertDate >= '2017-04-28'
        //         --WHERE InsertDate >= CONCAT (DATEPART(year,GETDATE()), '-', DATEPART(month,GETDATE()), '-', DATEPART(day,GETDATE()), ' 00:00:00.000')
        //         GROUP BY [User], [ActivityType]");

        //        return db.Database.SqlQuery<ActivityLog>(Results, new object[] { }).ToList();
        //    }
        //}
    }

    public class PersonalDailyActivitiesViewModel
    {
        public IList<ActivityLog> PersonalActivities { get; set; }
        public IEnumerable<ApplicationUser> Agents { get; set; }
        public IList<SelectListItem> AgentList
        {
            get
            {
                var ListAgents = (from u in Agents
                                  select new SelectListItem()
                                  {
                                      Text = u.UserName,
                                      Value = u.UserName
                                  }).ToList();
                return ListAgents;
            }
            set { }
        }
    }
}