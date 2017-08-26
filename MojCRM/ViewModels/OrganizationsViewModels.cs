using MojCRM.Areas.Sales.Models;
using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MojCRM.ViewModels
{
    public class OrganizationDetailsViewModel
    {
        public Organizations Organization { get; set; }
        public OrganizationDetail OrganizationDetails { get; set; }
        public MerDeliveryDetails MerDeliveryDetails { get; set; }
        public IQueryable<Organizations> OrganizationBusinessUnits { get; set; }
        public IQueryable<Contact> Contacts { get; set; }
        public IQueryable<Opportunity> Opportunities { get; set; }
        public int OpportunitiesCount { get; set; }
        public IQueryable<Lead> Leads { get; set; }
        public int LeadsCount { get; set; }
        public IQueryable<Delivery> TicketsAsReceiver { get; set; }
        public int TicketsAsReceiverCount { get; set; }
        public IQueryable<Delivery> TicketsAsSender { get; set; }
        public int TicketsAsSenderCount { get; set; }
        public IQueryable<OrganizationAttribute> Attributes { get; set; }

        public string OrganizationGroupString
        {
            get
            {
                switch (OrganizationDetails.OrganizationGroup)
                {
                    case Helpers.OrganizationGroupEnum.Nema: return "Ne pripada grupaciji";
                    case Helpers.OrganizationGroupEnum.AdrisGrupa: return "Adris Grupa";
                    case Helpers.OrganizationGroupEnum.Agrokor: return "Koncern Agrokor";
                    case Helpers.OrganizationGroupEnum.AtlanticGrupa: return "Atlantic Grupa";
                    case Helpers.OrganizationGroupEnum.AutoHrvatska: return "Poslovna grupacija Auto Hrvatska";
                    case Helpers.OrganizationGroupEnum.BabićPekare: return "Babić Pekare";
                    case Helpers.OrganizationGroupEnum.COMET: return "COMET";
                    case Helpers.OrganizationGroupEnum.CIOS: return "CIOS";
                    case Helpers.OrganizationGroupEnum.CVH: return "CVH";
                    case Helpers.OrganizationGroupEnum.Holcim: return "Holcim Grupa";
                    case Helpers.OrganizationGroupEnum.MSAN: return "MSAN Grupa";
                    case Helpers.OrganizationGroupEnum.NEXE: return "NEXE Grupa";
                    case Helpers.OrganizationGroupEnum.NTL: return "NTL Grupa";
                    case Helpers.OrganizationGroupEnum.PivacGrupa: return "Pivac Grupa";
                    case Helpers.OrganizationGroupEnum.RijekaHolding: return "Rijeka Holding";
                    case Helpers.OrganizationGroupEnum.STRABAG: return "STRABAG";
                    case Helpers.OrganizationGroupEnum.StyriaGrupa: return "Styria Grupa";
                    case Helpers.OrganizationGroupEnum.SunceKoncern: return "Koncern Sunce";
                    case Helpers.OrganizationGroupEnum.UltraGros: return "Ultra Gros";
                    case Helpers.OrganizationGroupEnum.Žito: return "Žito Grupa";
                    case Helpers.OrganizationGroupEnum.ZagrebačkiHolding: return "Zagrebački Holding";
                }
                return "Ne pripada grupaciji";
            }
        }
        public string LegalFormString
        {
            get
            {
                switch (Organization.LegalForm)
                {
                    case Organizations.LegalFormEnum.NOINFO: return "Nije navedeno";
                    case Organizations.LegalFormEnum.DOO: return "Društvo s ograničenom odgovornošću";
                    case Organizations.LegalFormEnum.JDOO: return "Jednostavno društvo s ograničenom odgovornošću";
                    case Organizations.LegalFormEnum.DD: return "Dioničko društvo";
                    case Organizations.LegalFormEnum.KDJTD: return "Ostala trgovačka društva (Komanditno društvo, Javno trgovačko društvo";
                    case Organizations.LegalFormEnum.OBRT: return "Obrt";
                    case Organizations.LegalFormEnum.OTHER: return "Ostali pravni oblici (Zadruge, OPG, Udruge, Ustanove i sl.";
                }
                return "Nije navedeno";
            }
        }
        public string MainCountryCodeString
        {
            get
            {
                switch (OrganizationDetails.MainCountry)
                {
                    case OrganizationDetail.CountryIdentificationCodeEnum.NOINFO: return "Nema podatka";
                    case OrganizationDetail.CountryIdentificationCodeEnum.HR: return "Hrvatska";
                }
                return "Nema podatka";
            }
        }
        public string CorrespondenceCountryCodeString
        {
            get
            {
                switch (OrganizationDetails.CorrespondenceCountry)
                {
                    case OrganizationDetail.CountryIdentificationCodeEnum.NOINFO: return "Nema podatka";
                    case OrganizationDetail.CountryIdentificationCodeEnum.HR: return "Hrvatska";
                }
                return "Nema podatka";
            }
        }
        [Display(Name = "Županija:")]
        public string MainCountyString
        {
            get
            {
                if (OrganizationDetails.MainPostalCode.ToString().StartsWith("100") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Grad Zagreb";
                if (OrganizationDetails.MainPostalCode.ToString().StartsWith("10") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Zagrebačka županija";
                if (OrganizationDetails.MainPostalCode.ToString().StartsWith("49") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Krapinsko-zagorska županija";
                if (OrganizationDetails.MainPostalCode.ToString().StartsWith("44") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Sisačko-moslavačka županija";
                if (OrganizationDetails.MainPostalCode.ToString().StartsWith("47") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Karlovačka županija";
                if (OrganizationDetails.MainPostalCode.ToString().StartsWith("42") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Varaždinska županija";
                if (OrganizationDetails.MainPostalCode.ToString().StartsWith("48") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Koprivničko-križevačka županija";
                if (OrganizationDetails.MainPostalCode.ToString().StartsWith("43") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Bjelovarsko-bilogorska županija";
                if (OrganizationDetails.MainPostalCode.ToString().StartsWith("51") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Primorsko-goranska županija";
                if (OrganizationDetails.MainPostalCode.ToString().StartsWith("53") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Ličko-senjska županija";
                if (OrganizationDetails.MainPostalCode.ToString().StartsWith("33") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Virovitičko-podravska županija";
                if (OrganizationDetails.MainPostalCode.ToString().StartsWith("34") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Požeško-slavonska županija";
                if (OrganizationDetails.MainPostalCode.ToString().StartsWith("35") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Brodsko-posavska županija";
                if (OrganizationDetails.MainPostalCode.ToString().StartsWith("23") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Zadarska županija";
                if (OrganizationDetails.MainPostalCode.ToString().StartsWith("31") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Osječko-baranjska županija";
                if (OrganizationDetails.MainPostalCode.ToString().StartsWith("22") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Šibensko-kninska županija";
                if (OrganizationDetails.MainPostalCode.ToString().StartsWith("32") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Vukovarsko-srijemska županija";
                if (OrganizationDetails.MainPostalCode.ToString().StartsWith("21") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Splitsko-dalmatinska županija";
                if (OrganizationDetails.MainPostalCode.ToString().StartsWith("52") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Istarska županija";
                if (OrganizationDetails.MainPostalCode.ToString().StartsWith("20") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Dubrovačko-neretvanska županija";
                if (OrganizationDetails.MainPostalCode.ToString().StartsWith("40") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Međimurska županija";
                return "Nije poznato";
            }
        }
        [Display(Name = "Županija:")]
        public string CorrespondenceCountyString
        {
            get
            {
                if (OrganizationDetails.CorrespondencePostalCode.ToString().StartsWith("100") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Grad Zagreb";
                if (OrganizationDetails.CorrespondencePostalCode.ToString().StartsWith("10") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Zagrebačka županija";
                if (OrganizationDetails.CorrespondencePostalCode.ToString().StartsWith("49") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Krapinsko-zagorska županija";
                if (OrganizationDetails.CorrespondencePostalCode.ToString().StartsWith("44") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Sisačko-moslavačka županija";
                if (OrganizationDetails.CorrespondencePostalCode.ToString().StartsWith("47") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Karlovačka županija";
                if (OrganizationDetails.CorrespondencePostalCode.ToString().StartsWith("42") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Varaždinska županija";
                if (OrganizationDetails.CorrespondencePostalCode.ToString().StartsWith("48") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Koprivničko-križevačka županija";
                if (OrganizationDetails.CorrespondencePostalCode.ToString().StartsWith("43") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Bjelovarsko-bilogorska županija";
                if (OrganizationDetails.CorrespondencePostalCode.ToString().StartsWith("51") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Primorsko-goranska županija";
                if (OrganizationDetails.CorrespondencePostalCode.ToString().StartsWith("53") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Ličko-senjska županija";
                if (OrganizationDetails.CorrespondencePostalCode.ToString().StartsWith("33") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Virovitičko-podravska županija";
                if (OrganizationDetails.CorrespondencePostalCode.ToString().StartsWith("34") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Požeško-slavonska županija";
                if (OrganizationDetails.CorrespondencePostalCode.ToString().StartsWith("35") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Brodsko-posavska županija";
                if (OrganizationDetails.CorrespondencePostalCode.ToString().StartsWith("23") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Zadarska županija";
                if (OrganizationDetails.CorrespondencePostalCode.ToString().StartsWith("31") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Osječko-baranjska županija";
                if (OrganizationDetails.CorrespondencePostalCode.ToString().StartsWith("22") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Šibensko-kninska županija";
                if (OrganizationDetails.CorrespondencePostalCode.ToString().StartsWith("32") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Vukovarsko-srijemska županija";
                if (OrganizationDetails.CorrespondencePostalCode.ToString().StartsWith("21") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Splitsko-dalmatinska županija";
                if (OrganizationDetails.CorrespondencePostalCode.ToString().StartsWith("52") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Istarska županija";
                if (OrganizationDetails.CorrespondencePostalCode.ToString().StartsWith("20") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Dubrovačko-neretvanska županija";
                if (OrganizationDetails.CorrespondencePostalCode.ToString().StartsWith("40") && OrganizationDetails.MainCountry == OrganizationDetail.CountryIdentificationCodeEnum.HR)
                    return "Međimurska županija";
                return "Nije poznato";
            }
        }
    }
}