using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MojCRM.Helpers
{
    // short: MOO
    public class MessagesOutboundOpenResponse
    {
        [JsonProperty]
        public int TotalCount { get; set; }
        public MOOOpens[] Opens { get; set; }
    }

    public class MOOOpens
    {
        [JsonProperty]
        public bool FirstOpen { get; set; }
        public MOOClient Client { get; set; }
        public MOOOS OS { get; set; }
        public string Platform { get; set; }
        public string UserAgent { get; set; }
        public string ReadSeconds { get; set; }
        public MOOGeo Geo { get; set; }
        public string MessageID { get; set; }
        public DateTime ReceivedAt { get; set; }
        public string Tag { get; set; }
        public string Recipient { get; set; }
    }

    public class MOOClient
    {
        [JsonProperty]
        public string Name { get; set; }
        public string Company { get; set; }
        public string Family { get; set; }
    }

    public class MOOOS
    {
        [JsonProperty]
        public string Name { get; set; }
        public string Company { get; set; }
        public string Family { get; set; }
    }

    public class MOOGeo
    {
        [JsonProperty]
        public string CountryISOCode { get; set; }
        public string Country { get; set; }
        public string RegionISOCode { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Coords { get; set; }
        public string IP { get; set; }
    }
}