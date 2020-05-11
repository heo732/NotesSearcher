using System;
using System.Diagnostics;
using System.Reflection;

using QAHelper.WPF;

namespace QAHelper.ViewModels
{
    public class AboutViewModel : BindableBase
    {
        public AboutViewModel()
        {
            SupportCommand = new DelegateCommand(SupportAction);
        }

        public string SupportEmail => "andriy.buzhak.1@gmail.com";

        public string ToolVersion
        {
            get
            {
                Version v = Assembly.GetExecutingAssembly().GetName().Version;
                return "v" + string.Join(".", v.Major, v.Minor, v.Build);
            }
        }

        public DelegateCommand SupportCommand { get; }

        private void SupportAction()
        {
            Process.Start(new Uri("mailto:" + SupportEmail + "?subject=QA Helper " + ToolVersion + " Support: <Please, specify subject of the issue>&body=<Please, specify description of the issue>").AbsoluteUri);
        }
    }
}