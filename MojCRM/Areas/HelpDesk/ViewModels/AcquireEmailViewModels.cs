using System.Linq;
using MojCRM.Areas.HelpDesk.Models;
using MojCRM.Models;

namespace MojCRM.Areas.HelpDesk.ViewModels
{
    public class AcquireEmailViewModel
    {
        public AcquireEmail Entity { get; set; }
        public IQueryable<ActivityLog> Activities { get; set; }
    }
}