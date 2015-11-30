using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digicoin.Core.Utility
{
    static class AmountValidationUtility
    {
        public static void ValidateGreaterThan0(decimal amount, string entityName)
        {
            if (amount <= 0)
                throw new ArgumentException(string.Format("{0} amount must be greater than 0.", entityName));
        }

        public static void ValidateIsMultipleOf10(decimal amount, string entityName)
        {
            if (amount % 10 != 0)
                throw new ArgumentException(string.Format("{0} amount must be a multiple of 10.", entityName));
        }
    }
}
