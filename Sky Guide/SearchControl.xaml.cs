using System;
using System.Collections.Generic;
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
using System.Windows.Media.Animation;
using System.Globalization;

namespace SkyGuide
{
    /// <summary>
    /// TextListBox.xaml 的交互逻辑
    /// </summary>
    public partial class SearchControl : UserControl
    {
        #region 属性

        #region 依赖属性

        /// <summary>
        /// Post排序依据
        /// </summary>
        public PostOrderBy postOrderByColumn
        {
            get
            {
                return (PostOrderBy)GetValue(postOrderByColumnProperty);
            }
            set
            {
                SetValue(postOrderByColumnProperty, value);
            }
        }
        public static readonly DependencyProperty postOrderByColumnProperty = DependencyProperty.Register(
            "postOrderByColumn",
            typeof(PostOrderBy),
            typeof(SearchControl),
            new PropertyMetadata(new PropertyChangedCallback((d, e) =>
            {
                SearchControl control = (SearchControl)d;

                if (e.NewValue.Equals(e.OldValue))
                    return;
                if (control.postPreviewInfoList == null)
                    return;

                control.postPreviewInfoList = BLL.Post.GetPostPreviewInfo(control.ResultSearchText, control.TagSearchText, control.RatingSelected, control.PreferSelected, control.postOrderByColumn, control.postOrderMode, control.SiteSelected);
            })));

        /// <summary>
        /// Pool排序依据
        /// </summary>
        public PoolOrderBy poolOrderByColumn
        {
            get
            {
                return (PoolOrderBy)GetValue(poolOrderByColumnProperty);
            }
            set
            {
                SetValue(poolOrderByColumnProperty, value);
            }
        }
        public static readonly DependencyProperty poolOrderByColumnProperty = DependencyProperty.Register(
            "poolOrderByColumn", 
            typeof(PoolOrderBy),
            typeof(SearchControl),
            new PropertyMetadata(new PropertyChangedCallback((d, e) =>
            {
                SearchControl control = (SearchControl)d;

                if (e.NewValue.Equals(e.OldValue))
                    return;
                if (control.poolPreviewInfoList == null)
                    return;

                control.poolPreviewInfoList = BLL.Pool.GetPoolPreview(control.TagSearchText, control.RatingSelected, control.PreferSelected, control.poolOrderByColumn, control.poolOrderMode, control.SiteSelected);
            })));

        /// <summary>
        /// Post排序模式
        /// </summary>
        public OrderMode postOrderMode
        {
            get
            {
                return (OrderMode)GetValue(postOrderModeModeProperty);
            }
            set
            {
                SetValue(postOrderModeModeProperty, value);
            }
        }
        public static readonly DependencyProperty postOrderModeModeProperty = DependencyProperty.Register(
            "postOrderMode",
            typeof(OrderMode),
            typeof(SearchControl),
            new PropertyMetadata(new PropertyChangedCallback((d, e) =>
            {
                SearchControl control = (SearchControl)d;

                if (e.NewValue.Equals(e.OldValue))
                    return;
                if (control.postPreviewInfoList == null)
                    return;

                control.postPreviewInfoList.Reverse();
                control.pageIndex = 1;
            })));

        /// <summary>
        /// Pool排序模式
        /// </summary>
        public OrderMode poolOrderMode
        {
            get
            {
                return (OrderMode)GetValue(poolOrderModeModeProperty);
            }
            set
            {
                SetValue(poolOrderModeModeProperty, value);
            }
        }
        public static readonly DependencyProperty poolOrderModeModeProperty = DependencyProperty.Register(
            "poolOrderMode", 
            typeof(OrderMode),
            typeof(SearchControl),
            new PropertyMetadata(new PropertyChangedCallback((d, e) =>
            {
                SearchControl control = (SearchControl)d;

                if (e.NewValue.Equals(e.OldValue))
                    return;
                if (control.poolPreviewInfoList == null)
                    return;

                control.poolPreviewInfoList.Reverse();
                control.pageIndex = 1;
            })));

        /// <summary>
        /// 单页预览图数量
        /// </summary>
        public int pageSize
        {
            get
            {
                return (int)GetValue(pageSizeProperty);
            }
            set
            {
                SetValue(pageSizeProperty, value);
            }
        }
        public static readonly DependencyProperty pageSizeProperty = DependencyProperty.Register(
            "pageSize",
            typeof(int),
            typeof(SearchControl),
            new PropertyMetadata(new PropertyChangedCallback(pageSizePropertyChanged)));
        private static void pageSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchControl control = d as SearchControl;
            int size = Convert.ToInt32(e.NewValue);

            if (control.PreviewCount % size == 0)
                control.pageCount = control.PreviewCount / size;
            else
                control.pageCount = control.PreviewCount / size + 1;
        }

        /// <summary>
        /// 所选模式
        /// </summary>
        public Mode ModeSelected
        {
            get
            {
                return (Mode)GetValue(ModeSelectedProperty);
            }
            set
            {
                SetValue(ModeSelectedProperty, value);
            }
        }
        public static readonly DependencyProperty ModeSelectedProperty = DependencyProperty.Register(
            "ModeSelected", 
            typeof(Mode), 
            typeof(SearchControl),
            new PropertyMetadata(new PropertyChangedCallback(ModeSelectedPropertyChanged)));
        private static void ModeSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchControl control = d as SearchControl;
            Mode mode = (Mode)e.NewValue;

            foreach (ListBoxItem item in control.modeListBox.Items)
            {
                if (item.IsSelected == false && mode == (Mode)Enum.Parse(typeof(Mode), (item.Content as CheckBox).Content.ToString(), true))
                    control.modeListBox.SelectedItem = item;
            }
        }
        /// <summary>
        ///所选分级
        /// </summary>
        public Rating[] RatingSelected
        {
            get
            {
                return (Rating[])GetValue(RatingSelectedProperty);
            }
            set
            {
                SetValue(RatingSelectedProperty, value);
            }
        }
        public static readonly DependencyProperty RatingSelectedProperty = DependencyProperty.Register("RatingSelected", typeof(Rating[]),
            typeof(SearchControl),
            new PropertyMetadata(new PropertyChangedCallback(RatingSelectedPropertyChanged)));
        private static void RatingSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchControl control = d as SearchControl;
            Rating[] ratingArray = (Rating[])e.NewValue;

            foreach (ListBoxItem item in control.ratingListBox.Items)
            {
                if (item.IsSelected == true && ratingArray.Contains((Rating)Enum.Parse(typeof(Rating), (item.Content as CheckBox).Content.ToString(), true)) == false)
                    control.ratingListBox.SelectedItems.Remove(item);
                else if (item.IsSelected == false && ratingArray.Contains((Rating)Enum.Parse(typeof(Rating), (item.Content as CheckBox).Content.ToString(), true)) == true)
                    control.ratingListBox.SelectedItems.Add(item);
            }
        }

        /// <summary>
        ///所选图站
        /// </summary>
        public Site[] SiteSelected
        {
            get
            {
                return (Site[])GetValue(SiteSelectedProperty);
            }
            set
            {
                SetValue(SiteSelectedProperty, value);
            }
        }
        public static readonly DependencyProperty SiteSelectedProperty = DependencyProperty.Register("SiteSelected", typeof(Site[]),
            typeof(SearchControl),
            new PropertyMetadata(new PropertyChangedCallback(SiteSelectedPropertyChanged)));
        private static void SiteSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchControl control = d as SearchControl;
            Site[] siteArray = (Site[])e.NewValue;

            foreach (ListBoxItem item in control.siteListBox.Items)
            {
                if(item.IsSelected == true && siteArray.Contains((Site)Enum.Parse(typeof(Site), (item.Content as CheckBox).Content.ToString(), true)) == false)
                    control.siteListBox.SelectedItems.Remove(item);
                else if(item.IsSelected ==false && siteArray.Contains((Site)Enum.Parse(typeof(Site), (item.Content as CheckBox).Content.ToString(), true)) == true)
                    control.siteListBox.SelectedItems.Add(item);
            }
        }

        /// <summary>
        ///所选偏好
        /// </summary>
        public Prefer[] PreferSelected
        {
            get
            {
                return (Prefer[])GetValue(PreferSelectedProperty);
            }
            set
            {
                SetValue(PreferSelectedProperty, value);
            }
        }
        public static readonly DependencyProperty PreferSelectedProperty = DependencyProperty.Register("PreferSelected", typeof(Prefer[]),
            typeof(SearchControl),
            new PropertyMetadata(new PropertyChangedCallback(PreferSelectedPropertyChanged)));
        private static void PreferSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchControl control = d as SearchControl;
            Prefer[] preferArray = (Prefer[])e.NewValue;

            foreach (ListBoxItem item in control.preferListBox.Items)
            {
                if (item.IsSelected == true && preferArray.Contains((Prefer)Enum.Parse(typeof(Prefer), (item.Content as CheckBox).Content.ToString(), true)) == false)
                    control.preferListBox.SelectedItems.Remove(item);
                else if (item.IsSelected == false && preferArray.Contains((Prefer)Enum.Parse(typeof(Prefer), (item.Content as CheckBox).Content.ToString(), true)) == true)
                    control.preferListBox.SelectedItems.Add(item);
            }
        }

        #endregion

        private bool _disableTagSearch;
        /// <summary>
        /// 关闭tag提示
        /// </summary>
        private bool disableTagSearch
        {
            get
            {
                return _disableTagSearch;
            }
            set
            {
                if (value == _disableTagSearch)
                    return;

                if (value == true)
                {
                    this.tagSearchTextBox.TextChanged -= new TextChangedEventHandler(this.SearchTextBox_TextChanged);
                    this.tagListBox.Visibility = Visibility.Collapsed;
                }
                else
                    this.tagSearchTextBox.TextChanged += new TextChangedEventHandler(this.SearchTextBox_TextChanged);

                _disableTagSearch = value;
            }
        }

        private bool _disableResultSearch;
        /// <summary>
        /// 关闭在结果中搜索
        /// </summary>
        private bool disableResultSearch
        {
            get
            {
                return _disableResultSearch;
            }
            set
            {
                if (value == _disableResultSearch)
                    return;

                if (value == true)
                {
                    this.checkBox.IsChecked = false;
                    this.checkBox.IsEnabled = false;          
                }
                else
                {
                    this.checkBox.IsEnabled = true;
                }

                _disableResultSearch = value;
            }
        }

        List<PostPreviewInfo> _postPreviewInfoList;
        /// <summary>
        /// Post预览图信息列表
        /// </summary>
        public List<PostPreviewInfo> postPreviewInfoList
        {
            get
            {
                return _postPreviewInfoList;
            }
            set
            {
                _postPreviewInfoList = value;

                if (value == null)
                    return;

                this.PreviewCount = value.Count;

                if (pageSize == 0)
                    return;

                if (value.Count % pageSize == 0)
                    pageCount = value.Count / pageSize;
                else
                    pageCount = value.Count / pageSize + 1;
            }
        }

        List<PoolPreviewInfo> _poolPreviewInfoList;
        /// <summary>
        /// Pool预览图信息列表
        /// </summary>
        public List<PoolPreviewInfo> poolPreviewInfoList
        {
            get
            {
                return _poolPreviewInfoList;
            }
            private set
            {
                _poolPreviewInfoList = value;

                if (value == null)
                    return;

                this.PreviewCount = value.Count;

                if (pageSize == 0)
                    return;

                if (value.Count % pageSize == 0)
                    pageCount = value.Count / pageSize;
                else
                    pageCount = value.Count / pageSize + 1;
            }
        }

        /// <summary>
        /// 搜索结果预览图总数
        /// </summary>
        public int PreviewCount { get; private set; }

        /// <summary>
        /// Tag搜索文本
        /// </summary>
        public string TagSearchText
        {
            get
            {
                return tagSearchTextBox.Text;
            }
        }

        /// <summary>
        /// 在结果中搜索文本
        /// </summary>
        public string ResultSearchText
        {
            get
            {
                return this.resultSearchTextBox.IsVisible ? resultSearchTextBox.Text : string.Empty;
            }
        }

        private int _pageIndex;
        /// <summary>
        /// 所选页码
        /// </summary>
        public int pageIndex
        {
            get
            {
                return _pageIndex;
            }
            set
            {
                if (value == -1)
                {
                    PageIndexSet?.Invoke(true);
                }

                //限制不能超过总页数
                if (value >= 1 && value <= this.pageCount)
                {
                    _pageIndex = value;
                    this.comboBox.SelectedIndex = value - 1;
                    //执行页码设定事件
                    PageIndexSet?.Invoke(false);
                }
            }
        }

        private int _pageCount;
        /// <summary>
        /// 总页数
        /// </summary>
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
                for (int x = 1; x <= value; x++)
                {
                    pageNumber.Add(x.ToString());
                }
                this.comboBox.ItemsSource = pageNumber;

                if (value > 0)
                    this.pageIndex = 1;
                else
                    this.pageIndex = -1;
            }
        }

        public delegate void ChangedEventHandler(bool isEmpty);
        /// <summary>
        /// 页码设定时触发
        /// </summary>
        public event ChangedEventHandler PageIndexSet;

        #endregion

        #region 方法

        #region 公有方法

        public SearchControl()
        {
            InitializeComponent();
        }

        public void StartSearch()
        {
            if (this.ModeSelected == Mode.Posts)
                this.postPreviewInfoList = BLL.Post.GetPostPreviewInfo(this.ResultSearchText, this.TagSearchText, this.RatingSelected, this.PreferSelected, this.postOrderByColumn, this.postOrderMode, this.SiteSelected);
            else
                this.poolPreviewInfoList = BLL.Pool.GetPoolPreview(this.TagSearchText, this.RatingSelected, this.PreferSelected, this.poolOrderByColumn, this.poolOrderMode, this.SiteSelected);

            (Window.GetWindow(this) as MainWindow).Title = this.TagSearchText;
        }
        #endregion

        #region 私有方法

        /// <summary>
        /// 修改排序模式
        /// </summary>
        /// <param name="mode"></param>
        private void ReversePostPreviewInfoList(OrderMode mode)
        {
            if (mode == this.postOrderMode)
                return;
            if (this.postPreviewInfoList == null)
                return;

            postPreviewInfoList.Reverse();
        }

        /// <summary>
        /// ListBox淡出效果
        /// </summary>
        private void ListBoxFadeEffect()
        {
            //动画开始前，锁定输入框和列表框
            tagSearchTextBox.IsReadOnly = true;
            tagListBox.IsEnabled = false;

            DoubleAnimation da = new DoubleAnimation() { From = 1, To = 0, Duration = TimeSpan.FromMilliseconds(300) };
            Storyboard.SetTarget(da, this.tagListBox);
            Storyboard.SetTargetProperty(da, new PropertyPath("(ListBox.Opacity)"));

            Storyboard fade = new Storyboard();
            fade.Children.Add(da);
            fade.FillBehavior = FillBehavior.Stop;

            //动画结束后不重置为初始状态 
            fade.Completed += (o, s) =>
            {
                this.tagListBox.Opacity = 0;
                this.tagListBox.Visibility = Visibility.Collapsed;
                //解锁输入框和列表框
                tagSearchTextBox.IsReadOnly = false;
                tagListBox.IsEnabled = true;
                //输入框重置为-1
                tagListBox.SelectedIndex = -1;
                //输入光标移至最后
                this.tagSearchTextBox.Focus();
                this.tagSearchTextBox.SelectionStart = this.tagSearchTextBox.Text.Length;
            };

            fade.Begin();
        }

        /// <summary>
        /// tag推荐框选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TagListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            TagDetail selectedValue = (TagDetail)e.AddedItems[0];

            //选择的tag添加到最后一个空格末尾
            if (this.tagSearchTextBox.Text.Contains(' '))
            {
                int lastSpaceIndex = this.tagSearchTextBox.Text.LastIndexOf(' ');
                //最后一个空格后是否存在+或-
                if (this.tagSearchTextBox.Text.IndexOf(" +", lastSpaceIndex) != -1 || this.tagSearchTextBox.Text.IndexOf(" -", lastSpaceIndex) != -1)
                    this.tagSearchTextBox.Text = string.Format("{0}{1} ", this.tagSearchTextBox.Text.Substring(0, lastSpaceIndex + 2), selectedValue.name);
                else
                    this.tagSearchTextBox.Text = string.Format("{0}{1} ", this.tagSearchTextBox.Text.Substring(0, lastSpaceIndex + 1), selectedValue.name);
            }
            else
                this.tagSearchTextBox.Text = string.Format("{0} ", selectedValue.name);

            //淡出效果
            ListBoxFadeEffect();
        }

        /// <summary>
        /// tag输入框输入时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string incompleteTag = string.Empty;

            if (this.tagSearchTextBox.Text.Contains(' '))
                incompleteTag = this.tagSearchTextBox.Text.Substring(this.tagSearchTextBox.Text.LastIndexOf(' ')).TrimStart(' ', '+', '-');
            else
                incompleteTag = this.tagSearchTextBox.Text;

            if (string.IsNullOrWhiteSpace(incompleteTag))
            {
                this.tagListBox.Opacity = 0;
                return;
            }

            List<TagDetail> tagList = BLL.Tag.GetRecommendedTag(incompleteTag, this.SiteSelected, 5);

            if (tagList.Count == 0)
            {
                this.tagListBox.Opacity = 0;
            }
            else
            {
                this.tagListBox.ItemsSource = tagList;
                this.tagListBox.Opacity = 1;
                this.tagListBox.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// tag输入框按下Enter键触发搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.TagSearchText))
                return;
            if (e.Key != Key.Enter)
                return;

            StartSearch();
        }

        /// <summary>
        /// 页码选择改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;
            if (this.pageIndex == Convert.ToInt32(e.AddedItems[0]))
                return;

            this.pageIndex = Convert.ToInt32(e.AddedItems[0]);
        }

        /// <summary>
        /// 鼠标在页码上时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_MouseEnter(object sender, MouseEventArgs e)
        {
            if (comboBox.HasItems)
                this.comboBox.Focus();
        }

        private void RatingCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            List<Rating> ratingList = new List<Rating>(this.RatingSelected);
            Rating selected = (Rating)Enum.Parse(typeof(Rating), (sender as CheckBox).Content.ToString());

            if (ratingList.Contains(selected) == false)
                ratingList.Add(selected);

            this.RatingSelected = ratingList.ToArray();
        }

        private void RatingCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            List<Rating> ratingList = new List<Rating>(this.RatingSelected);
            Rating selected = (Rating)Enum.Parse(typeof(Rating), (sender as CheckBox).Content.ToString());

            if (ratingList.Contains(selected))
                ratingList.Remove(selected);

            this.RatingSelected = ratingList.ToArray();
        }

        private void SiteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            List<Site> siteList = new List<Site>(this.SiteSelected);
            Site selected = (Site)Enum.Parse(typeof(Site), (sender as CheckBox).Content.ToString());

            if (siteList.Contains(selected) == false)
                siteList.Add(selected);

            this.SiteSelected = siteList.ToArray();
        }

        private void SiteCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            List<Site> siteList = new List<Site>(this.SiteSelected);
            Site selected = (Site)Enum.Parse(typeof(Site), (sender as CheckBox).Content.ToString());

            if (siteList.Contains(selected))
                siteList.Remove(selected);

            this.SiteSelected = siteList.ToArray();
        }

        private void PreferCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            List<Prefer> preferList = new List<Prefer>(this.PreferSelected);
            Prefer selected = (Prefer)Enum.Parse(typeof(Prefer), (sender as CheckBox).Content.ToString());

            if (preferList.Contains(selected) == false)
                preferList.Add(selected);

            this.PreferSelected = preferList.ToArray();
        }

        private void PreferCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            List<Prefer> preferList = new List<Prefer>(this.PreferSelected);
            Prefer selected = (Prefer)Enum.Parse(typeof(Prefer), (sender as CheckBox).Content.ToString());

            if (preferList.Contains(selected))
                preferList.Remove(selected);

            this.PreferSelected = preferList.ToArray();
        }

        private void ModeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.IsInitialized == false)
                return;

            this.ModeSelected = (Mode)Enum.Parse(typeof(Mode), (sender as CheckBox).Content.ToString());

            if (ModeSelected == Mode.Pools)
            {
                this.disableTagSearch = true;
                this.disableResultSearch = true;
                this.poolPreviewInfoList = new List<PoolPreviewInfo>();

                (this.siteListBox.ItemContainerGenerator.ContainerFromIndex(1) as ListBoxItem).IsEnabled = false;
                (this.siteListBox.ItemContainerGenerator.ContainerFromIndex(1) as ListBoxItem).IsSelected = false;

                (this.siteListBox.ItemContainerGenerator.ContainerFromIndex(3) as ListBoxItem).IsEnabled = false;
                (this.siteListBox.ItemContainerGenerator.ContainerFromIndex(3) as ListBoxItem).IsSelected = false;

                (this.siteListBox.ItemContainerGenerator.ContainerFromIndex(4) as ListBoxItem).IsEnabled = false;
                (this.siteListBox.ItemContainerGenerator.ContainerFromIndex(4) as ListBoxItem).IsSelected = false;
            }
            else
            {
                this.disableTagSearch = false;
                this.disableResultSearch = false;
                this.postPreviewInfoList = new List<PostPreviewInfo>();

                (this.siteListBox.ItemContainerGenerator.ContainerFromIndex(1) as ListBoxItem).IsEnabled = true;

                (this.siteListBox.ItemContainerGenerator.ContainerFromIndex(3) as ListBoxItem).IsEnabled = true;

                (this.siteListBox.ItemContainerGenerator.ContainerFromIndex(4) as ListBoxItem).IsEnabled = true;
            }
        }

        #endregion

        #endregion

    }

    #region 值转换器

    /// <summary>
    /// Tag的Post总数
    /// </summary>
    public class PostCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int postCount = 0;

            if (int.TryParse(value.ToString(), out postCount) == false)
                return 0;

            if (postCount > 10000)
                return string.Format("{0}k", postCount / 1000);
            if (postCount > 1000)
                return string.Format("{0:F1}k", (float)postCount / 1000);

            return postCount;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Tag的颜色
    /// </summary>
    public class TagColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string tag_type = value.ToString();

            switch (tag_type)
            {
                case "0":
                    return "#EE8887";
                case "1":
                    return "#CCCC00";
                case "3":
                    return "#DD00DD";
                case "4":
                    return "#00AA00";
                case "5":
                    return "#00BBBB";
                case "6":
                    return "#DC2020";
                default:
                    return "#FFFFFF";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// 页码颜色
    /// </summary>
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

    #endregion
}
