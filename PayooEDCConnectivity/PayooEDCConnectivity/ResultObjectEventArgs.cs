using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PayooEDCConnectivity
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ResultObjectEventArgs : EventArgs
    {
        /// <summary>
        /// Response Code
        /// </summary>
        /// <value>The res code.</value>
        public int ResCode { get; set; }

        /// <summary>
        /// Request Id
        /// </summary>
        public string ReId { get; set; }

        /// <summary>
        /// Response Data
        /// </summary>
        /// <value>The res data.</value>
        public ResponseData ResData { get; set; }

    }

    public class ResponseData 
    {
        /// <summary>
        /// Transaction Type
        /// </summary>
        public int TrTy { get; set; }

        /// <summary>
        /// The transaction date.
        /// </summary>
        public string TrDa  { get; set; }

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
}
