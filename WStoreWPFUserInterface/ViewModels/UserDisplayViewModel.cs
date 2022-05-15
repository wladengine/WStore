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

        private UserModel _selectedUser;

        public UserModel SelectedUser
        {
            get { return _selectedUser; }
            set 
            { 
                _selectedUser = value;
                SelectedUserName = value.Email;

                UserRoles.Clear();
                UserRoles = new BindingList<string>(value.Roles.Values.ToList());

                // TODO: make this async-ready
                LoadRolesAsync();

                NotifyOfPropertyChange(() => SelectedUser);
                
            }
        }

        private BindingList<string> _availableRoles = new BindingList<string>();

        public BindingList<string> AvailableRoles
        {
            get { return _availableRoles; }
            set 
            { 
                _availableRoles = value;
                NotifyOfPropertyChange(() => AvailableRoles);
            }
        }


        private string _selectedUserName;

        public string SelectedUserName
        {
            get { return _selectedUserName; }
            set 
            {
                _selectedUserName = value;
                NotifyOfPropertyChange(() => SelectedUserName);
            }
        }

        private BindingList<string> _UserRoles = new BindingList<string>();

        public BindingList<string> UserRoles
        {
            get { return _UserRoles; }
            set 
            {
                _UserRoles = value; 
                NotifyOfPropertyChange(() => UserRoles);
            }
        }

        private string _selectedUserRole;

        public string SelectedUserRole
        {
            get { return _selectedUserRole; }
            set 
            { 
                _selectedUserRole = value; 
                NotifyOfPropertyChange(() => SelectedUserRole);
            }
        }

        private string _selectedAvailableRole;

        public string SelectedAvailableRole
        {
            get { return _selectedAvailableRole; }
            set
            {
                _selectedAvailableRole = value;
                NotifyOfPropertyChange(() => SelectedAvailableRole);
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

        private async Task LoadRolesAsync()
        {
            var roles = await _userEndpoint.GetAllRolesAsync();
            foreach (var role in roles)
            {
                if (UserRoles.IndexOf(role.Value) < 0) //not in list
                {
                    AvailableRoles.Add(role.Value);
                }
            }
        }

        public async Task AddSelectedRole()
        {
            await _userEndpoint.AddUserToRole(SelectedUser.UserId, SelectedAvailableRole);

            UserRoles.Add(SelectedAvailableRole);
            AvailableRoles.Remove(SelectedAvailableRole);
        }
        public async Task RemoveSelectedRole()
        {
            // TODO: disallow to remove admin role for current user

            await _userEndpoint.RemoveUserFromRole(SelectedUser.UserId, SelectedUserRole);

            AvailableRoles.Add(SelectedUserRole);
            UserRoles.Remove(SelectedUserRole);
        }
    }
}
