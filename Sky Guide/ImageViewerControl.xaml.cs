using BLL;
using DAL;
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

namespace SkyGuide
{
    /// <summary>
    /// ImageViewer.xaml 的交互逻辑
    /// </summary>
    public partial class ImageViewerControl : UserControl
    {
        #region 属性

        /// <summary>
        /// 图片文件夹
        /// </summary>
        public string ImageFolder
        {
            get
            {
                return (string)GetValue(ImageFolderProperty);
            }
            set
            {
                SetValue(ImageFolderProperty, value);
            }
        }
        public static readonly DependencyProperty ImageFolderProperty = DependencyProperty.Register("ImageFolder", typeof(string), typeof(ImageViewerControl));

        string _imagePath;
        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImagePath
        {
            get
            {
                return _imagePath;
            }
            private set
            {
                _imagePath = value;
            }
        }

        KeyValuePair<string,BitmapImage> Cache = new KeyValuePair<string, BitmapImage>(string.Empty,null);

        /// <summary>
        /// 图片详情
        /// </summary>
        public PostDetail Detail { get; private set; }

        /// <summary>
        /// 移动图片参照位置
        /// </summary>
        private Point RefPoint { get; set; }

        #endregion

        #region 方法

        public ImageViewerControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设定图片
        /// </summary>
        /// <param name="detail"></param>
        public void SetImage(PostDetail detail)
        {
            this.Detail = detail;

            //默认为程序目录
            if (string.IsNullOrWhiteSpace(this.ImageFolder))
                this.ImageFolder = Environment.CurrentDirectory;

            string imgPath = GetImagePath(detail);

            //图片不存在
            if (System.IO.File.Exists(imgPath) == false)
            {
                MessageBox.Show(string.Format("{0}\n图片不存在！", imgPath));
                return;
            }

            this.ImagePath = imgPath;

            this.Dispatcher.BeginInvoke(new Action<string>((s) =>
            {
                //重置图片显示效果
                this.image.Margin = new Thickness();
                this.image.Height = double.NaN;
                this.image.Width = double.NaN;

                try
                {
                    //有缓存
                    if (this.Cache.Key == detail.file_md5)
                        this.image.Source = Cache.Value;
                    else
                        this.image.Source = new BitmapImage(new Uri(s));

                    GC.Collect();
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }

            }), System.Windows.Threading.DispatcherPriority.Background, imgPath);

        }

        /// <summary>
        /// 设定缓存
        /// </summary>
        /// <param name="detail"></param>
        public void SetCache(PostDetail detail)
        {
            string imgPath = GetImagePath(detail);

            //图片不存在
            if (System.IO.File.Exists(imgPath) == false)
                return;

            Task<BitmapImage> task = new Task<BitmapImage>(p =>
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.UriSource = new Uri((string)p);
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }, imgPath);

            task.ContinueWith(t =>
            {
                this.Cache = new KeyValuePair<string, BitmapImage>(detail.file_md5,task.Result);
            }
            );
             
            task.Start();
        }

        /// <summary>
        /// 获取图片路径
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        private string GetImagePath(PostDetail detail)
        {
            //默认为程序目录
            if (string.IsNullOrWhiteSpace(this.ImageFolder))
                return string.Empty;

            return System.IO.Path.Combine(
                this.ImageFolder,
                detail.file_md5.Substring(0, 2),
                detail.file_md5.Substring(2, 2),
                detail.file_md5 + "." + detail.image_format);

        }

        /// <summary>
        /// 在图片内点击鼠标左键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void image_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.RefPoint = e.GetPosition(this.image);
            this.image.Cursor = Cursors.Hand;
        }

        /// <summary>
        /// 移动图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void image_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.image.Cursor != Cursors.Hand)
                return;
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                this.image.Cursor = Cursors.Arrow;
                return;
            }

            //移动限制，不超出图片边缘
            double limitX = this.image.ActualWidth - (this.image.Parent as Grid).ActualWidth;
            double limitY = this.image.ActualHeight - (this.image.Parent as Grid).ActualHeight;

            if (limitX <= 0 && limitY <= 0)
                return;

            Point newPoint = e.GetPosition(this.image);
            Thickness newMargin = this.image.Margin;

            double deltaX = limitX > 0 ? (newPoint.X - RefPoint.X) : 0;
            double deltaY = limitY > 0 ? (newPoint.Y - RefPoint.Y) : 0;

            //上拉
            if (deltaY > 0)
            {
                //小于极限值，生效
                if (Math.Abs(newMargin.Bottom) < limitY)
                {
                    newMargin.Bottom -= Math.Abs(deltaY);

                    //若超过极限值，重置为极限值
                    if (Math.Abs(newMargin.Bottom) > limitY)
                        newMargin.Bottom = -limitY;
                }
            }
            //下拉
            else if (deltaY < 0)
            {
                //小于极限值，上拉生效
                if (Math.Abs(newMargin.Top) < limitY)
                {
                    newMargin.Top -= Math.Abs(deltaY);

                    //若超过极限值，重置为极限值
                    if (Math.Abs(newMargin.Top) > limitY)
                        newMargin.Top = -limitY;
                }
            }

            //右拉
            if (deltaX > 0)
            {
                //小于极限值，生效
                if (Math.Abs(newMargin.Right) < limitX)
                {
                    newMargin.Right -= Math.Abs(deltaX);

                    //若超过极限值，重置为极限值
                    if (Math.Abs(newMargin.Right) > limitX)
                        newMargin.Right = -limitX;
                }
            }
            //左拉
            else if (deltaX < 0)
            {
                //小于极限值，生效
                if (Math.Abs(newMargin.Left) < limitX)
                {
                    newMargin.Left -= Math.Abs(deltaX);

                    //若超过极限值，重置为极限值
                    if (Math.Abs(newMargin.Left) > limitX)
                        newMargin.Left = -limitX;
                }
            }

            if (newMargin.Bottom < newMargin.Top)
            {
                newMargin.Bottom -= newMargin.Top;
                newMargin.Top = 0;
            }
            else
            {
                newMargin.Top -= newMargin.Bottom;
                newMargin.Bottom = 0;
            }

            if (newMargin.Left < newMargin.Right)
            {
                newMargin.Left -= newMargin.Right;
                newMargin.Right = 0;
            }
            else
            {
                newMargin.Right -= newMargin.Left;
                newMargin.Left = 0;
            }

            this.image.Margin = newMargin;

        }

        /// <summary>
        /// 图框大小改变时
        /// </summary>
        private void image_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //首次打开
            if (e.PreviousSize.Height == 0 || e.PreviousSize.Width == 0)
                return;

            double newTop = this.image.Margin.Top;
            double newBottom = this.image.Margin.Bottom;

            double newLeft = this.image.Margin.Left;
            double newRight = this.image.Margin.Right;

            if (e.HeightChanged)
            {
                if (this.image.ActualHeight <= (this.image.Parent as Grid).ActualHeight)
                {
                    newTop = 0;
                    newBottom = 0;
                }
                else
                {
                    newTop *= e.NewSize.Height / e.PreviousSize.Height;
                    newBottom *= e.NewSize.Height / e.PreviousSize.Height;

                    double limitY = this.image.ActualHeight - (this.image.Parent as Grid).ActualHeight;

                    newTop = Math.Abs(newTop) > limitY ? -limitY : newTop;
                    newBottom = Math.Abs(newBottom) > limitY ? -limitY : newBottom;
                }
            }

            if (e.WidthChanged)
            {
                if (this.image.ActualWidth <= (this.image.Parent as Grid).ActualWidth)
                {
                    newLeft = 0;
                    newRight = 0;
                }
                else
                {
                    newLeft *= e.NewSize.Width / e.PreviousSize.Width;
                    newRight *= e.NewSize.Width / e.PreviousSize.Width;

                    double limitX = this.image.ActualWidth - (this.image.Parent as Grid).ActualWidth;

                    newLeft = Math.Abs(newLeft) > limitX ? -limitX : newLeft;
                    newRight = Math.Abs(newRight) > limitX ? -limitX : newRight;
                }
            }

            this.image.Margin = new Thickness(newLeft, newTop, newRight, newBottom);
        }

        /// <summary>
        /// 图框容器变化时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double limitX = this.image.ActualWidth - (this.image.Parent as Grid).ActualWidth;
            double limitY = this.image.ActualHeight - (this.image.Parent as Grid).ActualHeight;

            double newTop = this.image.Margin.Top;
            double newBottom = this.image.Margin.Bottom;
            double newLeft = this.image.Margin.Left;
            double newRight = this.image.Margin.Right;

            newTop = Math.Abs(newTop) > limitY ? -limitY : newTop;
            newBottom = Math.Abs(newBottom) > limitY ? -limitY : newBottom;
            newLeft = Math.Abs(newLeft) > limitX ? -limitX : newLeft;
            newRight = Math.Abs(newRight) > limitX ? -limitX : newRight;

            this.image.Margin = new Thickness(newLeft, newTop, newRight, newBottom);
        }

        /// <summary>
        /// 右键菜单单选模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_Single(object sender, RoutedEventArgs e)
        {
            MenuItem parent = (sender as MenuItem);

            int clickedIndex = parent.Items.IndexOf(e.Source);

            int preSeparatorIndex = 0;
            int nextSeparatorIndex = 0;

            Separator[] separatorArray = parent.Items.OfType<Separator>().ToArray();

            foreach (var separator in separatorArray)
            {
                int separatorIndex = parent.Items.IndexOf(separator);

                if (separatorIndex < clickedIndex)
                    preSeparatorIndex = separatorIndex;
                else
                    nextSeparatorIndex = separatorIndex;
            }

            for (int x = preSeparatorIndex; x < (nextSeparatorIndex == 0 ? parent.Items.Count : nextSeparatorIndex); x++)
            {
                if (parent.Items[x].GetType() == typeof(MenuItem))
                    (parent.Items[x] as MenuItem).IsChecked = false;
            }

            (e.Source as MenuItem).IsChecked = true;
        }

        /// <summary>
        /// 偏好菜单加载时
        /// </summary>
        private void Prefer_MenuItem_Loaded(object sender, RoutedEventArgs e)
        {
            MenuItem parent = (sender as MenuItem);

            foreach (MenuItem item in parent.Items.OfType<MenuItem>().ToArray())
            {
                if (this.Detail.prefer.ToString().Equals(item.Header))
                    item.IsChecked = true;
                else
                    item.IsChecked = false;
            }
        }

        /// <summary>
        /// 偏好菜单点击时
        /// </summary>
        private void Prefer_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem selectedItem = e.Source as MenuItem;

            Prefer prefer = (Prefer)Enum.Parse(typeof(Prefer), selectedItem.Header.ToString());

            PostDetail temp = this.Detail;
            temp.prefer = prefer;
            this.Detail = temp;

            List<PostDetail> list = new List<PostDetail>();
            foreach(var pd in ((this.Parent as Grid).Parent as PostInfoWindow).postInfoControl.tabControl.ItemsSource.Cast<PostDetail>())
            {
                temp = pd;
                temp.prefer = prefer;
                list.Add(temp);
            }

            int selectedIndex = ((this.Parent as Grid).Parent as PostInfoWindow).postInfoControl.tabControl.SelectedIndex;
            ((this.Parent as Grid).Parent as PostInfoWindow).postInfoControl.tabControl.ItemsSource = list;
            ((this.Parent as Grid).Parent as PostInfoWindow).postInfoControl.tabControl.SelectedIndex = selectedIndex;

            BLL.Preference.UpdatePrefer(Mode.Posts, this.Detail.file_md5, prefer);
        }

        /// <summary>
        /// 点击放大
        /// </summary>
        private void Upscale_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //放大1.5倍
            this.image.Height = this.image.ActualHeight * 1.5;
            this.image.Width = this.image.ActualWidth * 1.5;
        }

        /// <summary>
        /// 点击缩小
        /// </summary>
        private void Downscale_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //缩小1.5倍
            this.image.Height = this.image.ActualHeight / 1.5;
            this.image.Width = this.image.ActualWidth / 1.5;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        private void Open_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", this.ImagePath);
        }

        /// <summary>
        /// 原始大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrginalSize_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.image.Height = this.Detail.height;
            this.image.Width = this.Detail.width;
        }

        #endregion

    }
}
