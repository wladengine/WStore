using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using WStoreWPFUserInterface.Helpers;

namespace WStoreWPFUserInterface.ViewModels
{
    public class LoginViewModel : Screen
    {
        private string _userName;
        private string _password;

        private IAPIHelper _apiHelper;

        public LoginViewModel(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public string UserName
        {
            get { return _userName; }
            set 
            { 
                _userName = value; 
                NotifyOfPropertyChange(() => UserName);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }
        
        public string Password
        {
            get { return _password; }
            set 
            { 
                _password = value; 
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }

        public bool CanLogIn //this property allows us to enable/disable button named LogIn
        {
            get
            {
                bool output = false;

                if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                    output = true;

                return output;
            }
        }

        private string _errorMessage;
        
        public bool IsErrorVisible
        {
            get 
            {
                bool output = false;

                if (ErrorMessage?.Length > 0)
                {
                    output = true;
                }

                return output;
            }
        }
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set 
            { 
                _errorMessage = value;
                NotifyOfPropertyChange(() => ErrorMessage);
                NotifyOfPropertyChange(() => IsErrorVisible);//dont forget to notify dependent properties
            }
        }



        public async Task LogIn()
        {
            try
            {
                var result = await _apiHelper.AuthenticateAsync(UserName, Password);
                ErrorMessage = String.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage =  ex.Message;
            }
        }
    }
}
