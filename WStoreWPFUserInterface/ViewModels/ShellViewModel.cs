using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;

namespace WStoreWPFUserInterface.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        private LoginViewModel _loginVM;
        public ShellViewModel(LoginViewModel loginVM)
        {
            _loginVM = loginVM;
            var t = ActivateItemAsync(_loginVM); 
            t.Wait();
            // Wow, whats happened? The view is displayed just without any direct creation of ViewModel!
            // Thats because we created the connection in Bootstrapper.cs, where any ViewModel will be registered in Caliburn.Micro
            // and after any call will be generated automatically.
            // Magic? No, just reflection + dependency injetion + good tool.
            // It could be good, if you are familiar with theese things :p.
        }
    }
}
