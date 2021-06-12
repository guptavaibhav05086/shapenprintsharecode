using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace mLearnBackend.Models
{
    public partial class OrderDetails
    {
        [JsonProperty("cart")]
        public Cart[] Cart { get; set; }

        [JsonProperty("orderPrice")]
        public OrderPrice OrderPrice { get; set; }

        [JsonProperty("deliveryAddress")]
        public int DeliveryAddress { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }
    }

    public partial class Cart
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("GSTNumber")]
        public string GstNumber { get; set; }

        [JsonProperty("BillingName")]
        public string BillingName { get; set; }

        [JsonProperty("uploadedimages")]
        public Uploadedimages Uploadedimages { get; set; }


      

        [JsonProperty("logoImage")]
        public List<LogoImage> LogoImage { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("industry")]
        public string Industry { get; set; }

        [JsonProperty("category")]
        public Category[] Category { get; set; }
    }

    public partial class Category
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("specs")]
        public Specs Specs { get; set; }

        [JsonProperty("meetingDetails")]
        public MeetingDetails MeetingDetails { get; set; }

        [JsonProperty("price")]
        public Price Price { get; set; }

        [JsonProperty("professionalDesigner")]
        public bool ProfessionalDesigner { get; set; }

        [JsonProperty("sourceFile")]
        public bool SourceFile { get; set; }

        [JsonProperty("purpose")]
        public string Purpose { get; set; }
    }

    public partial class MeetingDetails
    {
        [JsonProperty("day")]
        public Day Day { get; set; }

        [JsonProperty("slot")]
        public Slot Slot { get; set; }

        [JsonProperty("timeGap")]
        public long TimeGap { get; set; }

        [JsonProperty("meetingDuration")]
        public long MeetingDuration { get; set; }
    }

    public partial class Day
    {
        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("month")]
        public int Month { get; set; }

        [JsonProperty("day")]
        public int DayDay { get; set; }
    }

    public partial class Slot
    {
        [JsonProperty("hour")]
        public int Hour { get; set; }

        [JsonProperty("minute")]
        public int Minute { get; set; }

        [JsonProperty("second")]
        public int Second { get; set; }
    }

    public partial class Price
    {
        [JsonProperty("price")]
        public long PricePrice { get; set; }

        [JsonProperty("deliveryFee")]
        public long DeliveryFee { get; set; }

        [JsonProperty("GST")]
        public long Gst { get; set; }

        [JsonProperty("Total")]
        public long Total { get; set; }

        [JsonProperty("baseDesignPrice")]
        public long BaseDesignPrice { get; set; }

        [JsonProperty("designGST")]
        public long DesignGst { get; set; }

        [JsonProperty("printGST")]
        public long PrintGst { get; set; }

        [JsonProperty("totalDesignCost")]
        public long TotalDesignCost { get; set; }

        [JsonProperty("totalPrintCost")]
        public long TotalPrintCost { get; set; }

        [JsonProperty("deliveryDays")]
        
        public string DeliveryDays { get; set; }

        [JsonProperty("professiondesignerFees")]
        public long ProfessiondesignerFees { get; set; }

        [JsonProperty("professiondesignerFeesAfterCommision")]
        public long ProfessiondesignerFeesAfterCommision { get; set; }

        [JsonProperty("sourceFileFees")]
        public long SourceFileFees { get; set; }

        [JsonProperty("discountPerc")]
        public long DiscountPerc { get; set; }

        [JsonProperty("discount")]
        public long Discount { get; set; }

        [JsonProperty("discountedTotal")]
        public long DiscountedTotal { get; set; }

        [JsonProperty("designerCost")]
        public long DesignerCost { get; set; }

        [JsonProperty("printerCost")]
        public long PrinterCost { get; set; }

    }

    public partial class Specs
    {
        [JsonProperty("subCategory")]
        public string SubCategory { get; set; }

        [JsonProperty("orientation")]
        public string Orientation { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("paperGSM")]
        
        public string PaperGsm { get; set; }

        [JsonProperty("quantity")]
        public long Quantity { get; set; }

        [JsonProperty("pinCode")]
        
        public long PinCode { get; set; }
    }

    public partial class LogoImage
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displayLoadingGif")]
        public bool DisplayLoadingGif { get; set; }

        [JsonProperty("displayFileName")]
        public bool DisplayFileName { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("serverFileName")]
        public string serverFileName { get; set; }


        [JsonProperty("fileSize")]
        public long FileSize { get; set; }

        [JsonProperty("fileError")]
        public bool FileError { get; set; }
    }

    public partial class Uploadedimages
    {
        [JsonProperty("product")]
        public string Product { get; set; }

        [JsonProperty("image1")]
        public LogoImage Image1 { get; set; }

        [JsonProperty("image2")]
        public LogoImage Image2 { get; set; }

        [JsonProperty("image3")]
        public LogoImage Image3 { get; set; }

        [JsonProperty("image4")]
        public LogoImage Image4 { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("contentServerFile")]
        public string ContentServerFile { get; set; }

        [JsonProperty("productServerFile")]
        public string ProductServerFile { get; set; }

        [JsonProperty("IsImageUploaded")]
        public bool IsImageUploaded { get; set; }

        [JsonProperty("contentValidation")]
        public bool ContentValidation { get; set; }

        [JsonProperty("IsproductRefUploaded")]
        public bool IsproductRefUploaded { get; set; }

        [JsonProperty("displayErrororbutton")]
        public bool DisplayErrororbutton { get; set; }

        [JsonProperty("displayLoadingProductGif")]
        public bool DisplayLoadingProductGif { get; set; }

        [JsonProperty("displayLoadingContentGif")]
        public bool DisplayLoadingContentGif { get; set; }

        [JsonProperty("IscontentUploaded")]
        public bool IscontentUploaded { get; set; }

        [JsonProperty("displayReadOnlyError")]
        public bool DisplayReadOnlyError { get; set; }
    }

    public partial class OrderPrice
    {
        [JsonProperty("calPrice")]
        public long CalPrice { get; set; }

        [JsonProperty("calDelivery")]
        public long CalDelivery { get; set; }

        [JsonProperty("calGST")]
        public long CalGst { get; set; }

        [JsonProperty("calDiscountedGST")]
        public long CalDiscountedGST { get; set; }

        [JsonProperty("calFinalTotal")]
        public long CalFinalTotal { get; set; }

        [JsonProperty("calDiscount")]
        public long CalDiscount { get; set; }

        [JsonProperty("calDiscountedTotal")]
        public long CalDiscountedTotal { get; set; }
    }
}