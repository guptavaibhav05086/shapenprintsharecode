using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mLearnBackend.Models
{
    public class OngoingOrder
    {
        public int? OrderId { get; set; }
        public string meetingTime { get; set; }
        public string productSubcategory { get; set; }
        public string MeetingUrl { get; set; }
        public string PrinterInvoiceURL { get; set; }
        public bool eligibleForReschedule { get; set; } = false;
        public string assignedDesignerEmail { get; set; }
        public string assignedPrinterEmail { get; set; }
        public string printerInvoiceFilepath { get; set; }
        public string assignerDesignerMobile { get; set; }
        public string assgnedPrinterMobile { get; set; }
        public string assgnedPrinterName { get; set; }
        public int? gap { get; set; } = 0;
        public int? duration { get; set; } = 0;
        public string CustomerName { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomeDeliveryNumber { get; set; }
        public string CustomerAddress { get; set; }
        public int CustomerId { get; set; }
        public string CustomerEmail { get; set; }
        public string DunzoTaskId { get; set; }
        public string DeliveryStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<OngoingOrderList> ongoingOrders { get; set; }
    }
    public class OngoingOrderList
    {
        public int id { get; set; }
        
        public Nullable<int> orderid { get; set; }
        public string sourceCodeFilePath { get; set; }
        public string finalDesignFilepath { get; set; }
        
        public string content { get; set; }
        public string contentpath { get; set; }
        public Nullable<int> productId { get; set; }
        public string sampleImageLogo { get; set; }
        public string orderType { get; set; }
        public string GSTNumber { get; set; }
        public string BillingName { get; set; }
        public string referenceImageURL { get; set; }
        //public string logoprintreadyImageURL { get; set; }
        //public string industry { get; set; }
        public string DeliveryDays { get; set; }
        public string category { get; set; }
        public string subcategory { get; set; }
        public string orientation { get; set; }
        public string industry { get; set; }
        public string size { get; set; }
        public string paperGSM { get; set; }
        public string quantity { get; set; }
        public string pincode { get; set; }
        public Nullable<System.DateTime> meetingTime { get; set; }
        public string[] sourceLogoFile { get; set; }
        public string[] sourcecodeFinalDesignFile { get; set; }
        public string[] FinalNormalDesignFile { get; set; }
        public string ProductImageURL { get; set; }
        public string meetingDurationMins { get; set; }
        public string Price { get; set; }
        public string GST { get; set; }
        public string Total { get; set; }
        public string BaseDesignPrice { get; set; }
        public string SourceFileFees { get; set; }
        public string ProfessionDesignerFees { get; set; }
        public string DesignGST { get; set; }
        
        public string TotalDesignCost { get; set; }

        public string PrintCost { get; set; }
        public string PrintGST { get; set; }
        public string TotalPrintCost { get; set; }
        public DateTime? UserDesignAcceptance { get; set; }
        public Nullable<bool> selectedProfDesiner { get; set; }
        public Nullable<bool> selectedSourceFile { get; set; }
        public Nullable<bool> isDesignCompleted { get; set; }
        public Nullable<bool> isPrintCompleted { get; set; }
        public Nullable<bool> isDesignerFinishedOrder { get; set; }
        public Nullable<decimal> designerCost { get; set; }
        public Nullable<decimal> printercost { get; set; }
        public Nullable<decimal> ProfessiondesignerFeesAfterCommision { get; set; }

        

    }
}