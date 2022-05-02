using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WStoreWPFUserInterface.Library.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal RetailPrice { get; set; }
        public string ConvertedRetailPrice
        {
            get 
            {
                // currency in current client culture
                return RetailPrice.ToString("C", System.Globalization.CultureInfo.CurrentCulture);
            }
        }
        public int QuantityInStock { get; set; }
        public bool IsTaxable { get; set; }
    }
}
