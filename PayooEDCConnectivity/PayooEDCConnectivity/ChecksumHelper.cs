using System;
namespace PayooEDCConnectivity
{
    public class ChecksumHelper
    {
        static ushort[] crc_tabccitt = new ushort[256];
        static int crc_tabccitt_init = 0;
        static int P_CCITT = 0x1021;

        static void Init_crcccitt_tab()
        {
            try
            {
                int i, j;
                ushort crc, c;
                for (i = 0; i < 256; i++)
                {
                    crc = 0;
                    c = (ushort)(((ushort)i) << 8);
                    for (j = 0; j < 8; j++)
                    {
                        if (((crc ^ c) & 0x8000) > 0)
                        {
                            crc = (ushort)((crc << 1) ^ P_CCITT);
                        }
                        else
                        {
                            crc = (ushort)(crc << 1);
                        }
                        c = (ushort)(c << 1);
                    }
                    crc_tabccitt[i] = crc;
                }
                crc_tabccitt_init = 1;
            }
            catch
            {
                throw;
            }
        }

        static ushort Update_crc_ccitt(ushort crc, char c)
        {
            try
            {
                ushort tmp, short_c;
                short_c = (ushort)(0x00ff & (ushort)c);
                if (crc_tabccitt_init == 0)
                {
                    Init_crcccitt_tab();
                }
                tmp = (ushort)((crc >> 8) ^ short_c);
                crc = (ushort)((crc << 8) ^ crc_tabccitt[tmp]);
                return crc;
            }
            catch
            {
                throw;
            }
        }
        public static string Hash(string String)
        {
            try
            {
                string checksum;
                int lenQR, i;
                ushort crc_ccitt = 0xffff;
                lenQR = String.Length;
                for (i = 0; i < lenQR; i++)
                {
                    crc_ccitt = Update_crc_ccitt(crc_ccitt, String[i]);
                }
                checksum = crc_ccitt.ToString("X4");
                return checksum;
            }
            catch
            {
                throw;
            }
        }
    }
}
