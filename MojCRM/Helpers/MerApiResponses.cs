using Newtonsoft.Json;

namespace MojCRM.Helpers
{
    public class MerResendChangeEmailResponse
    {
        [JsonProperty]
        public int DocumentId { get; set; }
        public string Email { get; set; }
        public string Error { get; set; }
    }

    public class MerGetSubjektDataResponse
    {
        [JsonProperty]
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string PoslovnaJedinica { get; set; }
        public string Oib { get; set; }
        public string GLN { get; set; }
        public string Adresa { get; set; }
        public string Mjesto { get; set; }
        public string Županija { get; set; }
    }
}