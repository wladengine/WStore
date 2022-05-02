using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WStoreWPFUserInterface.Library.Api;
using WStoreWPFUserInterface.Library.Helpers;
using WStoreWPFUserInterface.Library.Models;

namespace WStoreWPFUserInterface.ViewModels
{
    public class SalesViewModel : Screen
    {
        private IProductEndpoint _productEndpoint;
        private ISaleEndpoint _saleEndpoint;
        private IConfigHelper _configHelper;

        public SalesViewModel(IProductEndpoint productEndpoint, ISaleEndpoint saleEndpoint, IConfigHelper configHelper)
        {
            _productEndpoint = productEndpoint;
            _saleEndpoint = saleEndpoint;
            _configHelper = configHelper;
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

        private ProductModel _selectedProduct;

        public ProductModel SelectedProduct
        {
            get { return _selectedProduct; }
            set 
            { 
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }


        private BindingList<CartItemModel> _cart = new BindingList<CartItemModel>();

        public BindingList<CartItemModel> Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        private int _itemQuantity = 1;

        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set 
            { 
                _itemQuantity = value; 
                NotifyOfPropertyChange(() => ItemQuantity);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        public string SubTotal
        { 
            get
            {
                decimal subTotal = CalculateSubTotal();
                return $"{subTotal:C}";
            }
        }

        private decimal CalculateSubTotal()
        {
            decimal subTotal = Cart
                .Sum(item => item.Product.RetailPrice * item.QuantityInCart);

            return subTotal;
        }

        public string Tax
        {
            get
            {
                decimal taxAmount = CalculateTaxAmount();
                return $"{taxAmount:C}";
            }
        }
        private decimal CalculateTaxAmount()
        {
            decimal taxRate = _configHelper.GetTaxRate();

            decimal taxAmount = Cart
                .Where(item => item.Product.IsTaxable)
                .Sum(item => item.Product.RetailPrice * item.QuantityInCart * taxRate);

            return taxAmount;
        }

        public string Total
        {
            get
            {
                decimal Total = CalculateSubTotal() + CalculateTaxAmount();
                return $"{Total:C}";
            }
        }

        public bool CanAddToCart {
            get
            {
                bool output = false;

                //TODO: make sure that smth is selected
                //TODO: make sure that is an item quantity

                if (ItemQuantity > 0 && SelectedProduct?.QuantityInStock >= ItemQuantity)
                    output = true;

                return output;

            }
        }
        public void AddToCart()
        {
            CartItemModel existingItem = Cart
                .FirstOrDefault(x => x.Product?.Id == SelectedProduct?.Id);

            if (existingItem != null)
            {
                existingItem.QuantityInCart += ItemQuantity;

                // TODO: try to find some better solution to update displaying text
                Cart.Remove(existingItem);
                Cart.Add(existingItem);
            }
            else
            {
                Cart.Add(new CartItemModel()
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantity,
                });
            }

            // Do not forget to decrement QuantityInStock
            SelectedProduct.QuantityInStock -= ItemQuantity;
            ItemQuantity = 1;

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
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
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
        }

        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                //TODO: make sure that smth in the cart
                if (Cart?.Count > 0)
                    output = true;

                return output;

            }
        }
        public async Task CheckOut()
        {
            SaleModel model = new SaleModel();

            foreach (var item in Cart)
            {
                model.SaleDetails.Add(new SaleDetailModel()
                {
                    ProductId = item.Product.Id,
                    Quantity = item.QuantityInCart
                });
            }

            await _saleEndpoint.PostSale(model);
        }
    }
}
