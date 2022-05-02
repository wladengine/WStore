using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WStoreWPFUserInterface.Helpers;
using WStoreWPFUserInterface.Library.Api;
using WStoreWPFUserInterface.Library.Helpers;
using WStoreWPFUserInterface.Library.Models;
using WStoreWPFUserInterface.ViewModels;

namespace WStoreWPFUserInterface
{
    //this is the class, where we will setup Caliburn.Micro
    public class Bootstraper : BootstrapperBase
    {
        private SimpleContainer _container = new SimpleContainer();

        public Bootstraper()
        {
            Initialize();

            // some hack for visibility of passwordbox text changing
            // https://stackoverflow.com/questions/30631522/caliburn-micro-support-for-passwordbox
            ConventionManager.AddElementConvention<PasswordBox>(
                Helpers.PasswordBoxHelper.BoundPasswordProperty,
                "Password",
                "PasswordChanged");
        }

        protected override void Configure()
        {
            //when everyone asks for the container, we will return the instance of out _container
            _container.Instance(_container)
                .PerRequest<IProductEndpoint, ProductEndpoint>(); // DI will create a new instance of implementing class at every request (not a singleton)

            //it's kind of little bit meta
            // container holds an instance of itself to pass out, when people ask for SimpleContainer
            // the reason for that is because we may want to get this container in order to manipulate something or change something
            // or get information out of besides from our constructor

            //now we create a pair of important singletones for simplification our work and feel a power of Caliber.Micro
            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>() //EventAggregator is a helpful thing to aggregate every event in application
                                                                //one central clearinghouse looks a good thing to connect every module by "event bus"
                .Singleton<IAPIHelper, APIHelper>()
                .Singleton<IConfigHelper, ConfigHelper>()
                .Singleton<ILoggedInUserModel, LoggedInUserModel>();  // we can also just register a singleton for future use


            // use an reflection to automatize the registration of every new View and ViewModel
            GetType().Assembly.GetTypes() //for every type in our entire application
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => _container.RegisterPerRequest(viewModelType, viewModelType.ToString(), viewModelType));
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            //reassign the default window when application starts
            //notice, than we using the wiring of ShellViewModel, not the ShellView - just do not forget about naming conventions in WPF and MVVM
            DisplayRootViewFor<ShellViewModel>();
            
            //HINT: we need to not forget to describe the bootstraper resource in App.xaml in <Application.Resources> section
            //for example:
            //<Application.Resources>
            //    <ResourceDictionary>
            //        <ResourceDictionary.MergedDictionaries>
            //            <ResourceDictionary>
            //                <local:Bootstraper x:Key="Bootstraper" />
            //            </ ResourceDictionary>
            //        </ ResourceDictionary.MergedDictionaries>
            //    </ ResourceDictionary>
            //</ Application.Resources>
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
            //that means, that a Caliburn.Micro (and it's dependency injection container) will get information about asked instances from the container
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
