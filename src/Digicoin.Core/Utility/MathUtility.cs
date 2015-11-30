using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digicoin.Core.Utility
{
    static class MathUtility
    {
        public static decimal RoundForReport(this decimal value)
        {
            // Values in reports are rounded with precision of 3 digits
            return decimal.Round(value, 3);
        }
    }
}
