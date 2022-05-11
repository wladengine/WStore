using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Caliburn.Micro;
using WStoreWPFUserInterface.EventModels;
using WStoreWPFUserInterface.Library.Models;

namespace WStoreWPFUserInterface.ViewModels
{
    public class ShellViewModel : Conductor<object>, 
        IHandle<LogOnEvent> // we subscribing to handle any LogOnEvent and make some work
    {
        private readonly SalesViewModel _salesVM;

        private readonly IEventAggregator _events;
        private ILoggedInUserModel _loggedInUserModel;

        public ShellViewModel(IEventAggregator events, SalesViewModel salesVM, ILoggedInUserModel loggedInUserModel)
        {
            _salesVM = salesVM;
            _loggedInUserModel = loggedInUserModel;

            _events = events;
            _events.SubscribeOnUIThread(this); // activate the subscription in UI thread
        }

        protected override async void OnViewLoaded(object view)
        {
            await ActivateItemAsync(IoC.Get<LoginViewModel>()); // Use the default IoC container in Caliburn.Micro

            // Wow, whats happened? The view is displayed just without any direct creation of ViewModel!
            // Thats because we created the connection in Bootstrapper.cs, where any ViewModel will be registered in Caliburn.Micro
            // and after any call will be generated automatically.
            // Magic? No, just reflection + dependency injetion + good tool (Caliburn.Micro).
            // It could be good, if you are familiar with theese things :p.
        }

        public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(_salesVM);
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        public bool IsLoggedIn
        {
            get
            {
                return !string.IsNullOrWhiteSpace(_loggedInUserModel.Token);
            }
        }

        public async Task ExitApplication()
        {
            await TryCloseAsync();
        }
        public async Task LogOut()
        {
            _loggedInUserModel.LogOffUser();
            await ActivateItemAsync(IoC.Get<LoginViewModel>());

            NotifyOfPropertyChange(() => IsLoggedIn);
        }
    }
}
