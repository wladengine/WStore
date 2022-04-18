using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WStoreWPFUserInterface.ViewModels;

namespace WStoreWPFUserInterface
{
    //this is the class, where we will setup Caliburn.Micro
    public class Bootstraper : BootstrapperBase
    {
        public Bootstraper()
        {
            Initialize();
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
    }
}
