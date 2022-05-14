using AutoMapper;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WStoreWPFUserInterface.Library.Api;
using WStoreWPFUserInterface.Library.Helpers;
using WStoreWPFUserInterface.Library.Models;
using WStoreWPFUserInterface.Models;

namespace WStoreWPFUserInterface.ViewModels
{
    public class SalesViewModel : Screen
    {
        private IProductEndpoint _productEndpoint;
        private ISaleEndpoint _saleEndpoint;
        private IConfigHelper _configHelper;
        private IMapper _mapper;

        private StatusInfoViewModel _statusInfo;
        private IWindowManager _manager;

        public SalesViewModel(IProductEndpoint productEndpoint, ISaleEndpoint saleEndpoint, IConfigHelper configHelper, 
            IMapper mapper, StatusInfoViewModel statusInfo, IWindowManager manager)
        {
            _productEndpoint = productEndpoint;
            _saleEndpoint = saleEndpoint;
            _configHelper = configHelper;
            _mapper = mapper;

            _statusInfo = statusInfo;
            _manager = manager;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadProducts();
            }
            catch (Exception ex)
            {
                // smth from the bottom of .NET
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Error";

                if (ex.Message == "Unathorized")
                {
                    var info = IoC.Get<StatusInfoViewModel>();

                    info.UpdateMessage("Unathorized Access", "Not permitted");
                    await _manager.ShowDialogAsync(info, settings: settings);
                }
                else
                {
                    var info = IoC.Get<StatusInfoViewModel>();

                    info.UpdateMessage("Fatal exception", ex.Message);
                    await _manager.ShowDialogAsync(info, settings: settings);
                }

                
                await TryCloseAsync();
            }
        }

        private async Task LoadProducts()
        {
            // fill the products
            var data = await _productEndpoint.GetAllAsync();
            // AutoMapper is smart enough and provides the possibility to convert List<ProductModel> => List<ProductDisplayModel>
            // as well as ProductModel => ProductDisplayModel
            var products = _mapper.Map<List<ProductDisplayModel>>(data);
            Products = new BindingList<ProductDisplayModel>(products);
        }

        private BindingList<ProductDisplayModel> _products;

        public BindingList<ProductDisplayModel> Products
        {
            get { return _products; }
            set 
            { 
                _products = value; 
                NotifyOfPropertyChange(() => Products);
            }
        }

        private ProductDisplayModel _selectedProduct;
        public ProductDisplayModel SelectedProduct
        {
            get { return _selectedProduct; }
            set 
            { 
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        private async Task ResetSalesViewModel()
        {
            // TODO: consider to clearing every structure to reduce memory consumption
            Cart = new BindingList<CartItemDisplayModel>();

            // TODO add clearing the SelectedCartItem if it does not do it itself

            await LoadProducts();
            
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
        }

        private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();

        public BindingList<CartItemDisplayModel> Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        private CartItemDisplayModel _selectedCartItem;
        public CartItemDisplayModel SelectedCartItem
        {
            get { return _selectedCartItem; }
            set
            {
                _selectedCartItem = value;
                NotifyOfPropertyChange(() => SelectedCartItem);
                NotifyOfPropertyChange(() => CanRemoveFromCart);
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
            CartItemDisplayModel existingItem = Cart
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
                Cart.Add(new CartItemDisplayModel()
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
                if (SelectedCartItem != null && SelectedCartItem.QuantityInCart > 0)
                    output = true;

                return output;
            }
        }
        public void RemoveFromCart()
        {
            SelectedCartItem.Product.QuantityInStock += 1;
            if (SelectedCartItem.QuantityInCart > 1)
            {
                SelectedCartItem.QuantityInCart -= 1;
            }
            else
            {
                Cart.Remove(SelectedCartItem);
            }

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
            NotifyOfPropertyChange(() => CanAddToCart);
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

            await ResetSalesViewModel();
        }
    }
}
