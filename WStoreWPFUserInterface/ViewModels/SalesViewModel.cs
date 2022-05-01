using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WStoreWPFUserInterface.Library.Api;
using WStoreWPFUserInterface.Library.Models;

namespace WStoreWPFUserInterface.ViewModels
{
    public class SalesViewModel : Screen
    {
        private IProductEndpoint _productEndpoint;
        public SalesViewModel(IProductEndpoint productEndpoint)
        {
            _productEndpoint = productEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            // fill the products
            var data = await _productEndpoint.GetAllAsync();
            Products = new BindingList<ProductModel>(data);
        }

        private BindingList<ProductModel> _products;

        public BindingList<ProductModel> Products
        {
            get { return _products; }
            set 
            { 
                _products = value; 
                NotifyOfPropertyChange(() => Products);
            }
        }

        private BindingList<string> _cart;

        public BindingList<string> Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        private int _itemQuantity;

        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set 
            { 
                _itemQuantity = value; 
                NotifyOfPropertyChange(() => ItemQuantity);
            }
        }

        public string SubTotal
        { 
            get
            {
                //TODO: make a calculation
                return "";
            }
        }
        public string Tax
        {
            get
            {
                //TODO: make a calculation
                return "";
            }
        }
        public string Total
        {
            get
            {
                //TODO: make a calculation
                return "";
            }
        }

        public bool CanAddToCart {
            get
            {
                bool output = false;

                //TODO: make sure that smth is selected
                //TODO: make sure that is an item quantity

                if (_itemQuantity > 0)
                    output = true;

                return output;

            }
        }
        public void AddToCart()
        {
            
        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;

                //TODO: make sure that smth is selected

                return output;

            }
        }
        public void RemoveFromCart()
        {

        }

        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                //TODO: make sure that smth in the cart

                return output;

            }
        }
        public void CheckOut()
        {

        }
    }
}
