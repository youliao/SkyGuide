using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SkyGuide
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var comException = e.Exception as System.Runtime.InteropServices.COMException;

            if (comException != null && comException.ErrorCode == -2147221040)
                e.Handled = true;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow mainWinodw = new MainWindow();
            if(e.Args.Length>0)
            {
                mainWinodw.searchControl.tagSearchTextBox.Text = string.Join(" ", e.Args) + " ";
                mainWinodw.searchControl.StartSearch();
            }
            mainWinodw.Show();
        }
    }
}
