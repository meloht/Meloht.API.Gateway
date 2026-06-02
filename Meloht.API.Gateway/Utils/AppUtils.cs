using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.Utils
{
    internal static class AppUtils
    {
        public static int ConvertInt(string s, int defaultValue = 0)
        {
            int num = 0;
            if (int.TryParse(s, out num))
            {
                return num;
            }
            return defaultValue;
        }
        public static long ConvertLongInt(string s, long defaultValue = 0)
        {
            long num = 0;
            if (long.TryParse(s, out num))
            {
                return num;
            }
            return defaultValue;
        }

        public static string CountSize(long size)
        {
            string mStrSize = "";
            long factSize = 0;
            factSize = size;
            if (factSize < 1024.00)
                mStrSize = factSize.ToString("F2") + " Byte";
            else if (factSize >= 1024.00 && factSize < 1048576)
                mStrSize = (factSize / 1024.00).ToString("F2") + " K";
            else if (factSize >= 1048576 && factSize < 1073741824)
                mStrSize = (factSize / 1024.00 / 1024.00).ToString("F2") + " M";
            else if (factSize >= 1073741824)
                mStrSize = (factSize / 1024.00 / 1024.00 / 1024.00).ToString("F2") + " G";
            return mStrSize;
        }
    }
}
