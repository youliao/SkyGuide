using System;
using System.Collections.Generic;
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

namespace Sky_Guide
{
    /// <summary>
    /// PagingControl.xaml 的交互逻辑
    /// </summary>
    public partial class PagingControl : UserControl
    {
        private int _pageIndex;
        public int pageIndex
        {
            get
            {
                return _pageIndex;
            }
            private set
            {
                //限制不能超过总页数
                if (value >= 1 && value <= this.pageCount)
                {
                    _pageIndex = value;
                    this.comboBox.SelectedIndex = value - 1;
                    //执行页码设定事件
                    PageIndexOnSet();
                }           
            }
        }

        private int _previewCount;
        public int previewCount
        {
            get
            {
                return _previewCount;
            }
            set
            {
                _previewCount = value;

                if (value % pageSize == 0)
                    pageCount = value / pageSize;
                else
                    pageCount = value / pageSize + 1;
            }
        }

        private int _pageSize;
        public int pageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value;

                if (previewCount % value == 0)
                    pageCount = previewCount / value;
                else
                    pageCount = previewCount / value + 1;
            }
        }

        private int _pageCount;
        public int pageCount
        {
            get
            {
                return _pageCount;
            }
            private set
            {
                _pageCount = value;

                List<string> pageNumber = new List<string>();
                for(int x=1;x<= value;x++)
                {
                    pageNumber.Add(x.ToString());
                }
                this.comboBox.ItemsSource = pageNumber;

                if (value > 0)
                    this.pageIndex = 1;
            }
        }

        //事件委托
        public delegate void ChangedEventHandler();

        /// <summary>
        /// 页码设定时触发
        /// </summary>
        public event ChangedEventHandler PageIndexSet;

        protected virtual void PageIndexOnSet()
        {
            PageIndexSet?.Invoke();
        }

        public PagingControl()
        {
            InitializeComponent();
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                this.pageIndex = Convert.ToInt32(e.AddedItems[0]);
        }
    }

    public class PagingColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int pageIndex = System.Convert.ToInt32(value);

            switch (pageIndex % 2)
            {
                case 0:
                    return "#6F88DA";
                case 1:
                    return "#BFC2F7";
                default:
                    return "#FFFFFF";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
