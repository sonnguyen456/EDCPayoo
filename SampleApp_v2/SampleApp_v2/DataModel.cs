using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleApp
{
  
    public class EDCResultData<T>
    {
        public int ResCode { set; get; }
        public string ReId { set; get; }
        public T ResData { set; get; }
    }

    public class POSData
    {
        public long Price { set; get; }
        public int PayMethod { set; get; } = 2;
        public int TransactionType { set; get; }
    }
    public class POSSendData
    {
        public string ReqId { set; get; }
        public string ReqTime { set; get; }
        public int Operation { set; get; } = 1;
        public object ReqData { set; get; }
    }
}
