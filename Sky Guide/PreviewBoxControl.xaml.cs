using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
    public partial class PreviewBoxControl : UserControl
    {
        public BitmapImage PreviewSource
        {
            set
            {
                this.image.Source = value;
            }
        }

        /// <summary>
        /// Pool标题
        /// </summary>
        public string PoolTittle
        {
            set
            {
                this.textBlock.Text = value;              
            }
        }

        /// <summary>
        /// Pool标题可见
        /// </summary>
        public bool PoolTittleVisible
        {
            set
            {
                if (value == true)
                {
                    this.textBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    this.textBlock.Text = string.Empty;
                    this.textBlock.Visibility = Visibility.Collapsed;
                }
            }
        }

        public PreviewBoxControl()
        {
            InitializeComponent();
        }

    }

    public class DoubleTruncateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Truncate((double)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
