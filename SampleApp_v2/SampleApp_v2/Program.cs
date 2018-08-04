using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SampleApp
{
    public class ResultData
    {
        public int? TrTy { set; get; }
        public int? TrAmt { get; set; }
        public int? CaTy { get; set; }
        public string CaNum { get; set; }
        public string TrDa { set; get; }
        public string SeCode { set; get; }
        public string PrId { set; get; }
        public string TrID { set; get; }
        public string DeID { set; get; }
        public string ApCode { set; get; }
        public string SeId { set; get; }
        public string CaValue { set; get; }
        public string OrNo { set; get; }
        public string SysTra { set; get; }
        public string CusCode { set; get; }
        public int? NumPro { set; get; }
    }

    public class ResultCheckVoucher
    {
        public string Success { set; get; }
        public string Error { set; get; }
    }
    class Program
    {
        static SerialHelper Serial = null;
        static void Main(string[] args)
        {
            var list = SerialPort.GetPortNames();
            List<string> listShow = new List<string>();
            listShow.Add("Vui long chon com");
            for (int i = 0; i < list.Length; i++)
            {
                listShow.Add((i + 1) + ". " + list[i]);
            }
            if (list.Length >= 0)
            {
                int index = 0;
                while (index <= 0 || index > list.Length)
                {
                    Console.WriteLine(string.Join("\r\n", listShow));
                    string s = Console.ReadLine();
                    if (int.TryParse(s, out index))
                    {

                    }
                }
                Console.WriteLine("Da chon COM: " + list[index - 1]);
                DoFunction(list[index - 1]);
            }
            else
            {
                Console.WriteLine("Vui long ket noi com");
            }
        }


        private static void DoFunction(string comname)
        {
            
            Serial = new SerialHelper(comname,9600);

            /*      SALE BY CARD       */
            var SaleCardResult = Serial.SendAndRecieveData<ResultData>(new POSSendData()
            {
                Operation = 1,
                ReqId = "abc",
                ReqTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                ReqData = new
                {
                    Price = 10000,
                    PayMethod = 1,
                }
            });



            /*      CHECK VOUCHER       */
            var CheckVoucherResult = Serial.SendAndRecieveData<ResultCheckVoucher>(new POSSendData()
            {
                Operation = 2,
                ReqId = "abc",
                ReqTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                ReqData = new
                {
                    LstVoucher = "PY001|PY002|PY003",
                    VchProv = 1

                }
            });



            /*      SALE BY VOUCHER       */
            var SaleVoucherResult = Serial.SendAndRecieveData<ResultData>(new POSSendData()
            {
                Operation = 1,
                ReqId = "abc",
                ReqTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                ReqData = new
                {
                    Price = 10000,
                    PayMethod = 3,
                    PayParams = new
                    {
                        LstVoucher = "PY001#20000|PY002#500000|PY003#900000",
                        VchProv = 1
                    }
                }
            });
        }
    }
}
