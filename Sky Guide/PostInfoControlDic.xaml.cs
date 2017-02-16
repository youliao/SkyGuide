using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SkyGuide
{
    /// <summary>
    /// PreviewBoxControl.xaml 的交互逻辑
    /// </summary>
    public partial class PostInfoControlDic
    {
        public PostInfoControlDic()
        {
            InitializeComponent();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = (Hyperlink)e.OriginalSource;
            try
            {
                Process.Start(link.NavigateUri.AbsoluteUri);
            }
            catch
            { }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string tag = (sender as TextBlock).Text;

            Process.Start(Process.GetCurrentProcess().MainModule.FileName, tag);
        }
    }
}
