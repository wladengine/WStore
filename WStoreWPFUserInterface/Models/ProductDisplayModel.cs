using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WStoreWPFUserInterface.Models
{
    public class ProductDisplayModel : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        private decimal _retailPrice;

        public decimal RetailPrice
        {
            get { return _retailPrice; }
            set 
            { 
                _retailPrice = value;
                CallPropertyChanged(nameof(RetailPrice));
                CallPropertyChanged(nameof(ConvertedRetailPrice));
            }
        }

        public string ConvertedRetailPrice
        {
            get
            {
                // currency in current client culture
                return RetailPrice.ToString("C", System.Globalization.CultureInfo.CurrentCulture);
            }
        }
        private int _quantityInStock;
        public int QuantityInStock
        {
            get { return _quantityInStock; }
            set 
            { 
                _quantityInStock = value;
                CallPropertyChanged(nameof(QuantityInStock));
            }
        }

        public bool IsTaxable { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void CallPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
