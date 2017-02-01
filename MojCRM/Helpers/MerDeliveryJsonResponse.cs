using Newtonsoft.Json;
using System;

namespace MojCRM.Helpers
{
    public class MerDeliveryJsonResponse
    {
        [JsonProperty]
        public string BuyerID { get; set; }
        public int Id { get; set; }
        public string InterniBroj { get; set; }
        public string SupplierName { get; set; }
        public string SupplierID { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public string ParentDocumentID { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Message { get; set; }
    }
}