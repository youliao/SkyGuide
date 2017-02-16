using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using BLL;
using DAL;
using System.ComponentModel;

namespace SkyGuide
{
    /// <summary>
    /// PostInfoControl.xaml 的交互逻辑
    /// </summary>
    public partial class PostInfoControl : UserControl
    {
        public PostInfoControl()
        {
            InitializeComponent();
        }
    }

    #region 值转换器

    public class TagGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<TagDetail> tagsGroup = new ObservableCollection<TagDetail>();

            List<TagDetail> list = (List<TagDetail>)value;

            foreach (TagDetail tag in list.Where(a => a.tag_type == "1"))
            {
                tagsGroup.Add(tag);
            }

            foreach (TagDetail tag in list.Where(a => a.tag_type == "3"))
            {
                tagsGroup.Add(tag);
            }

            foreach (TagDetail tag in list.Where(a => a.tag_type == "5"))
            {
                tagsGroup.Add(tag);
            }

            foreach (TagDetail tag in list.Where(a => a.tag_type == "4"))
            {
                tagsGroup.Add(tag);
            }

            foreach (TagDetail tag in list.Where(a => a.tag_type == "0"))
            {
                tagsGroup.Add(tag);
            }

            foreach (TagDetail tag in list.Where(a => a.tag_type == "6"))
            {
                tagsGroup.Add(tag);
            }

            ICollectionView vw = CollectionViewSource.GetDefaultView(tagsGroup);
            vw.GroupDescriptions.Add(new PropertyGroupDescription("tag_type"));

            return tagsGroup;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TagTypeToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value.ToString())
            {
                case "0":
                    return "General";
                case "1":
                    return "Artist";
                case "3":
                    return "Copyright";
                case "4":
                    return "Character";
                case "5":
                    return "Circle";
                case "6":
                    return "Faults";
                default:
                    return "General";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PostDetailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            List<PostDetail> li = new List<PostDetail>();

            li.Add((PostDetail)value);
            return li;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FileSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double fileSize = System.Convert.ToDouble(value);

            if (fileSize < 1024 * 1024)
                return string.Format("{0:###.#} KB ({1:N0} B)", (fileSize / 1024), fileSize);
            else
                return string.Format("{0:###.#} MB ({1:N0} B)", (fileSize / 1024 / 1024), fileSize);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SiteUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PostDetail detail = (PostDetail)value;

            switch(detail.site)
            {
                case Site.Yande:
                    return string.Format("https://yande.re/post/show/{0}", detail.id);
                case Site.Konachan:
                    return string.Format("http://konachan.com/post/show/{0}", detail.id);
                case Site.Danbooru:
                    return string.Format("http://danbooru.donmai.us/posts/{0}", detail.id);
                case Site.Gelbooru:
                    return string.Format("http://gelbooru.com/index.php?page=post&s=view&id={0}", detail.id);
                case Site.Sankaku:
                    return string.Format("https://chan.sankakucomplex.com/post/show/{0}", detail.id);
                default:
                    return null;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TrimmedTextBlockVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;

            FrameworkElement textBlock = (FrameworkElement)value;

            textBlock.Measure(new System.Windows.Size(Double.PositiveInfinity, Double.PositiveInfinity));

            if (((FrameworkElement)value).ActualWidth < ((FrameworkElement)value).DesiredSize.Width)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}
