using System;
using System.Runtime.InteropServices;

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
        public ResponsePayment ResData { get; set; }

    }
}
