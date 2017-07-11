using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MojCRM.Areas.Sales.Helpers
{
    public class OpportunityNoteHelper
    {
        public int? RelatedOpportunityId { get; set; }
        public string User { get; set; }
        public string Contact { get; set; }
        public string Note { get; set; }
        public int? NoteId { get; set; }
        public string Email { get; set; }
        public int Identifier { get; set; }
    }

    public class OpportunityContactHelper
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Agent { get; set; }
        public int? RelatedOpportunityId { get; set; }
        public int? ContactId { get; set; }
    }

    public class OpportunityAssignHelper
    {
        public string AssignedTo { get; set; }
        public int? RelatedOpportunityId { get; set; }
        public bool Unassign { get; set; }
    }
}