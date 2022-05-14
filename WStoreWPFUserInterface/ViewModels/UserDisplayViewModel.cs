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
using WStoreWPFUserInterface.Library.Models;

namespace WStoreWPFUserInterface.ViewModels
{
    public class UserDisplayViewModel : Screen
    {
        private readonly StatusInfoViewModel _statusInfoViewModel;
        private readonly IWindowManager _manager;
        private readonly IUserEndpoint _userEndpoint;

        private BindingList<UserModel> _users;

        public BindingList<UserModel> Users
        {
            get { return _users; }
            set 
            { 
                _users = value; 
                NotifyOfPropertyChange(() => Users);
            }
        }

        public UserDisplayViewModel(StatusInfoViewModel statusInfoViewModel, IWindowManager manager, IUserEndpoint userEndpoint)
        {
            _statusInfoViewModel = statusInfoViewModel;
            _manager = manager;
            _userEndpoint = userEndpoint;
        }
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadUsers();
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

        private async Task LoadUsers()
        {
            // fill the products
            var data = await _userEndpoint.GetAllAsync();

            Users = new BindingList<UserModel>(data);
        }
    }
}
