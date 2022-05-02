using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WStoreWPFUserInterface.Library.Helpers
{
    public class ConfigHelper : IConfigHelper
    {
        public decimal GetTaxRate()
        {
            string taxRateText = ConfigurationManager.AppSettings["taxRate"];

            if (string.IsNullOrEmpty(taxRateText))
                throw new ConfigurationErrorsException("Tax rate is not set up");

            if (decimal.TryParse(taxRateText.Replace(',', '.'), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, 
                out decimal output) == false)
                throw new ConfigurationErrorsException("Tax rate is not set up properly");

            return output / 100;
        }
    }
}
