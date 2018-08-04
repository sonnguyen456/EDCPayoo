using System;
using System.Runtime.InteropServices;

namespace PayooEDCConnectivity
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ResponseCheckVoucher
    {
        public string Success { set; get; }
        public string Error { set; get; }
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ResponsePayment
    {
        /// <summary>
        /// Transaction Type
        /// </summary>
        public int TrTy { get; set; }

        /// <summary>
        /// The transaction date.
        /// </summary>
        public string TrDa { get; set; }

        /// <summary>
        /// The transaction amount.
        /// </summary>
        public double TrAmt { get; set; }

        /// <summary>
        /// The type of the card.
        /// </summary>
        public int CaTy { get; set; }
        /// <summary>
        /// The card number.
        /// </summary>
        public string CaNum { get; set; }

        /// <summary>
        /// The service code.
        /// </summary>
        public string SeCode { get; set; }

        //public int providerCode; //deleted ver4 12/10/2017

        /// <summary>
        /// The transaction identifier.
        /// </summary>
        public string TrID { get; set; } //Mã giao dịch Payoo

        /// <summary>
        /// The device identifier. Mã máy EDC
        /// </summary>
        public string DeID { get; set; }

        /// <summary>
        /// The approval code. Mã xác nhận chi từ ngân hàng. Khác NULL nếu CardType != 0
        /// </summary>
        public string ApCode { get; set; }

        //add ver4 12/10/2017
        /// <summary>
        /// The provider identifier.
        /// </summary>
        public string PrId { get; set; }

        /// <summary>
        /// The service identifier.
        /// </summary>
        public string SeId { get; set; }

        /// <summary>
        /// The card value.
        /// </summary>
        public string CaValue { get; set; }

        //add ver5 06/11/2017
        /// <summary>
        /// The order no.
        /// </summary>
        public string OrNo { get; set; }

        /// <summary>
        /// The system trace.
        /// </summary>
        public string SysTra { get; set; }

        /// <summary>
        /// The customer code.
        /// </summary>
        public string CusCode { get; set; }

        /// <summary>
        /// The number of product.
        /// </summary>
        public int NumPro { get; set; }
    }

    public class EDCResultData<T>
    {
        public int ResCode { set; get; }
        public string ReId { set; get; }
        public T ResData { set; get; }
    }

    public abstract class POSSendData
    {
        public string ReqId { set; get; }
        public string ReqTime { set; get; }
        public virtual RequestOperation Operation { set; get; }
    }

    /// <summary>
    /// Payment request.
    /// </summary>
    public class PaymentRequest : POSSendData
    {
        public override RequestOperation Operation => RequestOperation.Sale;
        public RequestObjectData ReqData { get; set; }
    }


    /// <summary>
    /// Payment request.
    /// </summary>
    public class CheckVoucherRequest : POSSendData
    {
        public override RequestOperation Operation => RequestOperation.CheckVoucher;
        public VoucherPayParams ReqData { get; set; }
    }

    public enum RequestOperation {
        Sale = 1,
        CheckVoucher
    }

    public class RequestObjectData
    {
        public long Price { get; set; }
        public PayMethod PayMethod { get; set; }
        public VoucherPayParams PayParams { get; set; } 
    }

    public enum PayMethod {
        Card = 1,
        QRCode = 2,
        Voucher = 3
    }

    /// <summary>
    /// Voucher pay parameters.
    /// </summary>
    public class VoucherPayParams
    {
        // VoucherCode1#VoucherPrice1|VoucherCode2# VoucherPrice2...
        public string LstVoucher { get; set; }

        // Mã nhà cung cấp Voucher (Xem Phụ lục 4.2)
        public int VchProv { get; set; }
    }
}
