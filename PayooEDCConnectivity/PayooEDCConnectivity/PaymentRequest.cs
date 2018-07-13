using System;
namespace PayooEDCConnectivity
{
    /// <summary>
    /// Payment request.
    /// </summary>
    public class PaymentRequest
    {
        /// <summary>
        /// The request identifier
        /// </summary>
        public string ReqId { get; set; }
        public string ReqTime { get; set; }
        public RequestOperation Operation { get; set; }
        public RequestObjectData ReqData { get; set; }
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
