using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Reflection;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace PayooEDCConnectivity
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId("PayooEDCConnectivity.EDCConnectivity")]
    public class EDCConnectivity
    {

        private string comm = String.Empty;

        //Kết nối server
        //private SqlConnection conn;
        //Chuỗi kết nối sql server | test = "Data Source=ZANTY-IT\\ZANTY;Initial Catalog=PayooEDC;Integrated Security=True"
        //private string connectionString = String.Empty;
        //Tên bảng trong sql server
        //private string tableName = String.Empty;

        private SerialPort comPort = null;

        //Sau khi nhận được resutl từ thiết bị thì sẽ lưu lại vào biến này.
        private string output = "", outputResult;
        private bool isCompleted = false;
        private ResultObjectEventArgs result;

        public delegate void ReceiveHandler(ResultObjectEventArgs result);

        public event ReceiveHandler OnReceiveEDCResult;

        [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
        public interface IResultEvents
        {
            void OnReceiveEDCResult(ResultObjectEventArgs result);
        }

        public EDCConnectivity()
        {
        }

        public void connectCOM(String comm)
        {
            if (comm == null || comm.Trim().Length == 0)
                throw new Exception("Thiếu thông tin COM nhập vào!");
            else
            {
                this.comm = comm;
                //this.connectionString = connectionString;
                //this.tableName = tableName;
                //connectToServer(); //Kết nối với sql server
                initSerialPort(); //Khởi tạo serial port
            }
        }

        //private void connectToServer()
        //{
        //    try
        //    {
        //        conn = new SqlConnection(connectionString);
        //        conn.Open(); //Mở kết nối
        //        conn.Close();
        //    }
        //    catch(Exception ex)
        //    {
        //        throw new Exception("Không thể kết nối với SQL Server", ex);
        //    }
        //}

        /*
        Hàm kiểm tra kết nối với Sql Server
        */
        //public bool isConnectedToServer()
        //{
        //    string cmdText = @"IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES 
        //               WHERE TABLE_NAME='" + tableName + "') SELECT 1 ELSE SELECT 0";
        //    conn.Open();
        //    SqlCommand DateCheck = new SqlCommand(cmdText, conn);
        //    int x = Convert.ToInt32(DateCheck.ExecuteScalar());
        //    conn.Close();
        //    if (x == 1)
        //        return true;
        //    else
        //        return false;
        //}

        /* Hàm khỏi tạo Serial Port
        */
        private void initSerialPort()
        {
            comPort = new SerialPort(comm, 9600, Parity.None, 8, StopBits.One);
            comPort.RtsEnable = false;
            comPort.Handshake = Handshake.None;
            comPort.DataReceived +=
                    new System.IO.Ports.SerialDataReceivedEventHandler(portDataReceived);

            isConnectedToCom(); //Mở kết nối với thiết bị
        }

        /* Hàm kiểm tra kết nối với cổng Comm
        *  Result:
        *       + True: connected
        *       + False: can't connect
        */
        public bool isConnectedToCom()
        {
            bool isConnectedComm = false; //kết quả kiểm tra
            try
            {
                if (comm != String.Empty)
                {
                    comPort.PortName = comm; //khai báo tên cổng kết nối
                    if (comPort != null && !comPort.IsOpen)
                        comPort.Open(); // mở kết nối
                    isConnectedComm = true;
                    //if (comPort != null && comPort.IsOpen)
                    //    comPort.Close();
                    Console.WriteLine("EDCConnectivity: Opened connect");
                }
                else
                {
                    //Đóng kết nối với server nếu không có tên cổng cần kết nối.
                    //if (conn != null && conn.State == ConnectionState.Open)
                    //    conn.Close();
                    Console.WriteLine("EDCConnectivity: comm = null! Cannot connect to comm!");
                    if (comPort != null && comPort.IsOpen)
                        comPort.Close();
                    throw new Exception("Thiếu thông tin COM nhập vào!");
                }
            }
            catch (Exception ex)
            {
                //Đóng kết nối với server nếu xảy ra sự cố kết nối với cổng COM.
                //if (conn != null && conn.State == ConnectionState.Open)
                //    conn.Close();

                if (comPort != null && comPort.IsOpen)
                    comPort.Close();
                throw new Exception("Không thể kết nối với thiết bị", ex);
            }
            return isConnectedComm;
        }

        /**
            Nhận data gửi về từ thiết bị.
        */
        private void portDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var inputData = comPort.ReadExisting();
            if (string.IsNullOrEmpty(inputData))
            {
                return;
            }

            if (string.IsNullOrEmpty(output))
            {
                try
                {
                    if (inputData.Trim().StartsWith("{", StringComparison.Ordinal))
                    {
                        outputResult = "";
                        output += inputData;
                    }
                }
                catch { }

            }
            else
            {
                output += inputData;
                outputResult = "";
            }
            receiveData(output);
        }

        /// <summary>
        /// Receives the data.
        /// Vì inputData được trả về từ máy EDC theo nhiều lần.
        /// Nên mỗi lần trả về thì cộng vào chuỗi output,
        /// và sao đó Parse output đó thành Json Object.
        /// Nếu parse thành công thì đó là chuỗi kết quả hoàn chỉnh, ta tiến hành insert vào DB.
        /// </summary>
        /// <param name="inputData">Input data.</param>
        private void receiveData(String inputData)
        {
            try
            {
                JObject jObject = IsValidJson(output);
                if (jObject != null)
                {
                    try
                    {
                        var response = JsonConvert.DeserializeObject<ResultObjectEventArgs>(output);
                        OnReceiveEDCResult?.Invoke(response);
                    }
                    catch { }

                    isCompleted = true;

                    //Luu ra file
                    outputResult = output;

                    output = "";
                }
            }
            catch { }
        }

        public String getResultObject()
        {
            if (outputResult == null || outputResult.Length == 0)
                return null;
            else
                return outputResult;
        }

        public void createResultFile(String jsonResult)
        {
            //System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + 
            string path = @"C:\Result\result.txt";
            if (!File.Exists(path))
            {
                File.Create(path);
                TextWriter tw = new StreamWriter(path);
                tw.WriteLine(jsonResult);
                tw.Close();
            }
            else
            {
                //Clear content file
                File.WriteAllText(path, jsonResult);
            }
        }

        private static JObject IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    JObject obj = JObject.Parse(strInput);
                    return obj;
                }
                catch (JsonReaderException jex)
                {
                    Console.WriteLine(jex.Message);
                    return null;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public void send(String requestId, long amount)
        {
            if (comPort != null && !comPort.IsOpen)
                comPort.Open();
            isCompleted = false;
            output = "";
            outputResult = "";
            var reqObject = new PaymentRequest
            {
                ReqId = requestId,
                ReqTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"),
                ReqData = new RequestObjectData
                {
                    Price = amount,
                    PayMethod = PayMethod.Card,
                    PayParams = null
                }
            };

            string reqString = JsonConvert.SerializeObject(reqObject);
            comPort.Write(reqString);
        }

        public void send(String requestId, long amount, string lstVouchers, int voucherProvider)
        {
            if (comPort != null && !comPort.IsOpen)
                comPort.Open();
            isCompleted = false;
            output = "";
            outputResult = "";
            var reqObject = new PaymentRequest
            {
                ReqId = requestId,
                ReqTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"),
                ReqData = new RequestObjectData
                {
                    Price = amount,
                    PayMethod = PayMethod.Voucher,
                    PayParams = new VoucherPayParams
                    {
                        LstVoucher = lstVouchers,
                        VchProv = voucherProvider
                    }
                }
            };

            string reqString = JsonConvert.SerializeObject(reqObject);
            comPort.Write(reqString);
        }

        public void checkVoucher(String requestId, string lstVouchers, int voucherProvider)
        {
            if (comPort != null && !comPort.IsOpen)
                comPort.Open();
            isCompleted = false;
            output = "";
            outputResult = "";
            var reqObject = new CheckVoucherRequest
            {
                ReqId = requestId,
                ReqTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"),
                ReqData = new VoucherPayParams
                {
                    LstVoucher= lstVouchers,
                    VchProv = voucherProvider
                }
            };

            string reqString = JsonConvert.SerializeObject(reqObject);
            comPort.Write(reqString);
        }

        public void close()
        {
            outputResult = "";
            if (comPort != null && comPort.IsOpen)
                comPort.Close();
        }
    }

}