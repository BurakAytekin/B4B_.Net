using System;

namespace B2b.Web.v4.Models.EntityLayer
{
    [Serializable]
    public class SearchCriteria
    {
        public SearchSource Source { get; set; }
        public string Manufacturer { get; set; }
        public string T9Text { get; set; }
        public string ProductGroup1 { get; set; }
        public string ProductGroup2 { get; set; }
        public string ProductGroup3 { get; set; }
        public string VehicleBrand { get; set; }
        public string VehicleBrandModel { get; set; }
        public string VehicleType { get; set; }
        public bool Campaign { get; set; }
        public bool NewArrival { get; set; }
        public bool NewProduct { get; set; }
        public bool OnQuantity { get; set; }
        public bool OnWay { get; set; }
        public bool Picture { get; set; }
        public bool ComparsionProduct { get; set; }
        public bool BannerStatu { get; set; }


        //Admin arama İşlemi için Eklenen Alanlar
        public string Code { get; set; }
        public string Name { get; set; }
        public string ManufacturerCode { get; set; }
        public string ShelfAdress { get; set; }
        public string RuleCode { get; set; }

      
    }
}
public enum SearchSource
{
    General,
    CatalogSearchFromBrand,
    CatalogSearchFromProductGroup,
    CatalogSearchFromManufacturer,
    OriginalPrice,
}