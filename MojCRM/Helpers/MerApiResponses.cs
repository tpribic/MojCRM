using Newtonsoft.Json;
using System;

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

    public class MerGetNondeliveredDocumentsResponse
    {
        [JsonProperty]
        public int Id { get; set; }
        public string InterniBroj { get; set; }
        public int DokumentTypeId { get; set; }
        public DateTime DatumOtpreme { get; set; }
        public int PosiljateljId { get; set; }
        public string PosiljateljOib { get; set; }
        public string PosiljateljNaziv { get; set; }
        public string EmailPosiljatelja { get; set; }
        public int PrimateljId { get; set; }
        public string PrimateljOib { get; set; }
        public string PrimateljNaziv { get; set; }
        public string EmailPrimatelja { get; set; }
        public int TotalReceived { get; set; } // No. of total documents with DocumentStatus = 40
    }

    public class MerGetSentDocumentsResponse
    {
        [JsonProperty]
        public int Id { get; set; }
        public string InterniBroj { get; set; }
        public int DokumentTypeId { get; set; }
        public int DokumentStatusId { get; set; }
        public DateTime DatumOtpreme { get; set; }
        public DateTime DatumZadnjePoruke { get; set; }
        public DateTime DatumDostave { get; set; }
        public string EmailPrimatelja { get; set; }
        public string EmailPosiljatelja { get; set; }
        public int PosiljateljId { get; set; }
        public string PosiljateljOib { get; set; }
        public string PosiljateljNaziv { get; set; }
    }
}