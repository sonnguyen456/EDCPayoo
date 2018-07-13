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
        public int responceCode;
        public string requestId;
        public int transactionType;
        public string transactionDate;
        public double transactionAmount;
        public int cardType;
        public string cardNumber;
        public string serviceCode;
        //public int providerCode; //deleted ver4 12/10/2017
        public string transactionID; //Mã giao dịch Payoo
        public string deviceID; //Mã máy EDC
        public string approvalCode; //Mã chuẩn chi từ ngân hàng. Khác NULL nếu CardType != 0

        //add ver4 12/10/2017
        public string providerId;
        public string serviceId;
        public string cardValue;

        //add ver5 06/11/2017
        public string orderNo;
        public string systemTrace;
        public string customerCode;
        public int numOfProduct;

        public ResultObjectEventArgs() { }

        public ResultObjectEventArgs(int responceCode, string requestId, int transactionType,
            string transactionDate, double transactionAmount, int cardType, string cardNumber,
            string serviceCode, string transactionID, string deviceID, string approvalCode,
            string ProviderId, string ServiceId, string CardValue,
            string orderNo, string systemTrace, string customerCode, int numOfProduct)
        {
            this.responceCode = responceCode;
            this.requestId = requestId;
            this.transactionType = transactionType;
            this.transactionDate = transactionDate;
            this.transactionAmount = transactionAmount;
            this.cardType = cardType;
            this.cardNumber = cardNumber;
            this.serviceCode = serviceCode;
            //this.providerCode = providerCode;
            this.transactionID = transactionID;
            this.deviceID = deviceID;
            this.approvalCode = approvalCode;

            //add ver4 12/10/2017
            this.providerId = ProviderId;
            this.serviceId = ServiceId;
            this.cardValue = CardValue;

            //add ver4 12/10/2017
            this.orderNo = orderNo;
            this.systemTrace = systemTrace;
            this.customerCode = customerCode;
            this.numOfProduct = numOfProduct;
        }

    }
}
