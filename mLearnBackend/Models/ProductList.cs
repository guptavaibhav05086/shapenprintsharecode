using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mLearnBackend.Models
{
    public class ProductList
    {
        public List<ProductModel> productList { get; set; }
        public List<UniqueProduct> products { get; set; }
         
        public List<ProductprintPrice> printPrice { get; set; }

        public List<DiscountsModel> discountList { get; set; }
        

        public int maxGap { get; set; }

        public string videoUrl { get; set; }
        public string promotionImageURL { get; set; }
        public string displayPromotion { get; set; }
        //public Dictionary<int,string> products { get; set; }

    }
    public class ImageUpload
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool displayLoadingGif { get; set; }
        public bool displayFileName { get; set; }
        public string fileName { get; set; }
        public int fileSize { get; set; }
        public bool fileError { get; set; }
        public string serverFileName { get; set; }
        public bool displyBtn { get; set; }
    }
    public class ProductprintPrice
    {
        
        public int? prodDetailsId { get; set; }
        public int? qunatity { get; set; }
        public double? pricePerUnit { get; set; }

        public string deliveryDays { get; set; }

        public int? printCommission { get; set; }
    }

    public class ProductModel
    {
        public List<ImageUpload> imageUpload { get; set; }
        public  int poductDetailsId { get; set; }
        public int productId { get; set; }
        public int productsubId { get; set; }
        public string productCategory { get; set; }
        public string productName { get; set; }
        public string productSubcategory { get; set; }
        public string productSize { get; set; }
        public string orientation { get; set; }
        public string paperGSM { get; set; }
        public string quantities { get; set; }
        public string productPrice { get; set; }
        public string productImage { get; set; }
        public string productIcon { get; set; }
        public string price { get; set; }
        public string productDescription { get; set; }
        public Nullable<int> producPreference { get; set; }
        public string productCode { get; set; }
        public Nullable<double> DesignPrice { get; set; }
        public Nullable<double> DesignGST { get; set; }
        public Nullable<double> DesignCommision { get; set; }
        public Nullable<double> PrintPrice { get; set; }
        public Nullable<double> PrintGST { get; set; }
        public Nullable<double> PrintCommision { get; set; }
        public Nullable<int> SlotTimeGap { get; set; }
        public string deliveryTime { get; set; }
        public Nullable<double> deliveryFees { get; set; }
        public List<ProductprintPrice> printPrice { get; set; }
        public Nullable<bool> IsPriceInSqFt { get; set; }
        public Nullable<int> meetingDuration { get; set; }
        public Nullable<double> profDesignerFee { get; set; }
        public Nullable<double> sourceFileFees { get; set; }
        public bool? IsDisabled { get; set; }

    }

    public class UniqueProduct
    {
        public int key { get; set; }
        public string value { get; set; }

        public string category { get; set; }
        public Nullable<int> producPreference { get; set; }
        public string price { get; set; }
        public string productIcon { get; set; }
        public string productImage { get; set; }
        public bool? IsDisabled { get; set; }
        public List<ProductCarouselImages> productImages { get; set; }
    }
    public class ProductCarouselImages
    {
        public int Id { get; set; }
        public string Url { get; set; }
    }
}