using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using WStoreWPFUserInterface.EventModels;
using WStoreWPFUserInterface.Helpers;
using WStoreWPFUserInterface.Library.Api;

namespace WStoreWPFUserInterface.ViewModels
{
    public class LoginViewModel : Screen
    {
        private string _userName;
        private string _password;

        private IAPIHelper _apiHelper;
        private IEventAggregator _events;

        public LoginViewModel(IAPIHelper apiHelper, IEventAggregator events)
        {
            _apiHelper = apiHelper;
            _events = events;

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["userName"]))
                UserName = ConfigurationManager.AppSettings["userName"];
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["password"]))
                Password = ConfigurationManager.AppSettings["password"];
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

                // get more information about user
                await _apiHelper.GetLoggedInUserInfo(result.Access_Token);

                // raise the event specially in UI thread (for to be sure, that every UI listener can hear this event)
                await _events.PublishOnUIThreadAsync(new LogOnEvent());
            }
            catch (Exception ex)
            {
                ErrorMessage =  ex.Message;
            }
        }
    }
}
