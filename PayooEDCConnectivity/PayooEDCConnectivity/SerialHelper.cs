using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace PayooEDCConnectivity
{
    public class SerialHelper : IDisposable
    {
        private SerialPort _SerialPort;
        private bool _isRece = true;
        private object _Result;
        private DateTime _BeginTime;
        private int _Timeout;

        public string ResultString
        {
            get;
            private set;
        }

        public SerialHelper(string ComName, int BaudRate)
        {
            _SerialPort = new SerialPort(ComName, BaudRate, Parity.None, 8, StopBits.One);
            _SerialPort.RtsEnable = false;
            _SerialPort.Handshake = Handshake.None;
            _SerialPort.WriteTimeout = 4000;
            _SerialPort.Open();
        }

        /// <summary>
        /// Send and Recieve Data to EDC
        /// </summary>
        /// <typeparam name="T">ResData Type</typeparam>
        /// <param name="Data">Request Data</param>
        /// <param name="Timeout">Second</param>
        /// <returns></returns>
        public EDCResultData<T> SendAndRecieveData<T>(POSSendData Data = null, int Timeout = 180) where T : class
        {
            if (_isRece)
            {
                _Result = null;
                ResultString = null;
                _BeginTime = DateTime.Now;
                _Timeout = Timeout;
                _SerialPort.DataReceived -= _SerialPort_DataReceived<T>;
                _SerialPort.DataReceived += _SerialPort_DataReceived<T>;
                return _SendAndRecieveData<T>(Data);
            }
            else
            {
                throw new Exception("Serial port is busy");
            }
        }

        private EDCResultData<T> _SendAndRecieveData<T>(object Data = null) where T : class
        {
            try
            {
                _isRece = false;
                if (Data != null)
                {
                    var json = JsonConvert.SerializeObject(Data, Formatting.None);
                    var sendData = String.Format("00000000^{0}~@^{1}~@^{1}~@^{0}~00000000", ChecksumHelper.Hash(json), json);
                    _SerialPort.Write(sendData);
                }

                while (!_isRece && (DateTime.Now - _BeginTime).TotalSeconds < _Timeout)
                {
                    Thread.Sleep(500);
                }

                if (!_isRece)
                {
                    _isRece = true;
                    return null;
                }
                _isRece = true;
                var result = _Result as EDCResultData<T>;
                if (result != null && result.ResCode == -1)
                {
                    return _SendAndRecieveData<T>(Data);
                }
                return result;
            }
            catch
            {
                _isRece = true;
                return null;
            }
        }

        public void Dispose()
        {
            try
            {
                if (_SerialPort != null)
                {
                    _SerialPort.Close();
                    _SerialPort.Dispose();
                }
            }
            catch { }
        }

        private string TrimData(string data)
        {
            var text = data.ToString();
            while (text.First() != '^')
            {
                text = text.Substring(1);
            }
            text = text.Substring(1);
            var i = 0;
            for (i = 0; i < text.Length; i++)
            {
                if (text[i] == '~')
                {
                    break;
                }
            }
            text = text.Substring(0, i);
            return text;
        }

        private bool ParseText<T>(string Data, string Hash, out EDCResultData<T> Result)
        {
            try
            {
                var text = TrimData(Data);
                var hashText = ChecksumHelper.Hash(text);
                if (hashText != Hash)
                {
                    Result = null;
                    return false;
                }

                Result = JsonConvert.DeserializeObject<EDCResultData<T>>(text);
                return true;
            }
            catch
            {
                Result = null;
                return false;
            }
        }

        private void ParseResult<T>(string data)
        {
            try
            {
                var text = data.ToString();
                //trim
                //00000000^hash~@^json~@^json~@^hash~00000000
                var ListData = text.Split('@');

                //hash
                string Hash = TrimData(ListData.First());
                if (Hash.Length != 4)
                {
                    Hash = TrimData(ListData.Last());
                }

                EDCResultData<T> result = null; ;

                foreach (var dataItem in ListData)
                {
                    if (TrimData(dataItem).Length > 4)
                    {
                        var CanParse = ParseText<T>(dataItem, Hash, out result);
                        if (CanParse)
                        {
                            break;
                        }
                    }
                }

                _Result = result;
            }
            catch (Exception ex)
            {

            }
        }

        private void _SerialPort_DataReceived<T>(object sender, SerialDataReceivedEventArgs e)
        {
            //cho nhan du du lieu
            Thread.Sleep(2000);
            ResultString = _SerialPort.ReadExisting().Trim();
            ParseResult<T>(ResultString);
            _isRece = true;
        }
    }
}