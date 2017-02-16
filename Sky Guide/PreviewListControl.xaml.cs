using BLL;
using DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SkyGuide
{
    /// <summary>
    /// PreviewListControl.xaml 的交互逻辑
    /// </summary>
    public partial class PreviewListControl : UserControl
    {
        #region 属性

        #region 依赖属性

        /// <summary>
        /// 预览图行、列数
        /// </summary>
        public Size ColumnsRows
        {
            get
            {
                return (Size)GetValue(ColumnsRowsProperty);
            }
            set
            {
                SetValue(ColumnsRowsProperty, value);
            }
        }
        public static readonly DependencyProperty ColumnsRowsProperty = DependencyProperty.Register(
            "ColumnsRows",
            typeof(Size),
            typeof(PreviewListControl),
            new PropertyMetadata(new Size(), null, new CoerceValueCallback(PreviewColumnsRowsChange)));
        /// <summary>
        /// 在值改变前触发
        /// </summary>
        private static object PreviewColumnsRowsChange(DependencyObject d, object baseValue)
        {
            PreviewListControl control = d as PreviewListControl;

            Size newSize = (Size)baseValue;
            control.uniformGrid.Rows = (int)newSize.Height;
            control.uniformGrid.Columns = (int)newSize.Width;

            control.uniformGrid.Children.Clear();

            for (int x = 0; x < (int)newSize.Height * (int)newSize.Width; x++)
            {
                control.uniformGrid.Children.Add(new PreviewBoxControl());
                //添加光标移入事件
                control.uniformGrid.Children[x].MouseEnter += (sender, evt) =>
                {
                    int previewBoxIndex = control.uniformGrid.Children.IndexOf((sender as PreviewBoxControl));

                    if (control.CurrentMode == Mode.Posts)
                    {
                        if (previewBoxIndex < control.pageMD5Array.Length)
                            control.SelectedMD5 = control.pageMD5Array[previewBoxIndex];
                        else
                            control.SelectedMD5 = null;
                    }
                    else
                    {
                        if (previewBoxIndex < control.pageIdSiteArray.Length)
                            control.SelectedIdSite = control.pageIdSiteArray[previewBoxIndex];
                        else
                            control.SelectedIdSite = new KeyValuePair<int, Site>();
                    }
                };
            }

            control.PreviewBoxCount = (int)newSize.Height * (int)newSize.Width;
            return baseValue;
        }

        /// <summary>
        /// 当前显示Mode
        /// </summary>
        public Mode CurrentMode
        {
            get
            {
                return (Mode)GetValue(CurrentModeProperty);
            }
            set
            {
                SetValue(CurrentModeProperty, value);
            }
        }
        public static readonly DependencyProperty CurrentModeProperty = DependencyProperty.Register(
            "CurrentMode",
            typeof(Mode),
            typeof(PreviewListControl),
            new PropertyMetadata(new PropertyChangedCallback(CurrentModePropertyChanged)));
        private static void CurrentModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PreviewListControl control = d as PreviewListControl;
            Mode mode = (Mode)e.NewValue;

            bool visible;
            if (mode == Mode.Posts)
            {
                visible = false;
                control.uniformGrid.ContextMenu = control.Resources["PostMenu"] as ContextMenu;
            }
            else
            {
                visible = true;
                control.uniformGrid.ContextMenu = control.Resources["PoolMenu"] as ContextMenu;
            }

            for (int x = 0; x < control.uniformGrid.Children.Count; x++)
            {
                (control.uniformGrid.Children[x] as PreviewBoxControl).PoolTittleVisible = visible;
            }
        }

        /// <summary>
        /// 预览图数量
        /// </summary>
        public int PreviewBoxCount
        {
            get
            {
                return (int)GetValue(previewBoxCountProperty);
            }
            set
            {
                SetValue(previewBoxCountProperty, value);
            }
        }
        public static readonly DependencyProperty previewBoxCountProperty = DependencyProperty.Register("PreviewBoxCount", typeof(int), typeof(PreviewListControl));

        /// <summary>
        /// Post排序依据
        /// </summary>
        public PostOrderBy PostOrderByColumn
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
        public static readonly DependencyProperty postOrderByColumnProperty = DependencyProperty.Register("PostOrderByColumn", typeof(PostOrderBy), typeof(PreviewListControl));

        /// <summary>
        /// Pool排序依据
        /// </summary>
        public PoolOrderBy PoolOrderByColumn
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
        public static readonly DependencyProperty poolOrderByColumnProperty = DependencyProperty.Register("PoolOrderByColumn", typeof(PoolOrderBy), typeof(PreviewListControl));

        /// <summary>
        /// Post排序模式
        /// </summary>
        public OrderMode PostOrderMode
        {
            get
            {
                return (OrderMode)GetValue(postOrderModeProperty);
            }
            set
            {
                SetValue(postOrderModeProperty, value);
            }
        }
        public static readonly DependencyProperty postOrderModeProperty = DependencyProperty.Register("PostOrderMode", typeof(OrderMode), typeof(PreviewListControl));

        /// <summary>
        /// Pool排序模式
        /// </summary>
        public OrderMode PoolOrderMode
        {
            get
            {
                return (OrderMode)GetValue(poolOrderModeProperty);
            }
            set
            {
                SetValue(poolOrderModeProperty, value);
            }
        }
        public static readonly DependencyProperty poolOrderModeProperty = DependencyProperty.Register("PoolOrderMode", typeof(OrderMode), typeof(PreviewListControl));

        #endregion

        /// <summary>
        /// 本页MD5数组
        /// </summary>
        private string[] _pageMD5Array = new string[] { };
        public string[] pageMD5Array
        {
            get
            {
                return _pageMD5Array;
            }
            set
            {
                int oldIndex = Array.IndexOf(_pageMD5Array, SelectedMD5);
                if (oldIndex != -1 && oldIndex < value.Length)
                    SelectedMD5 = value[oldIndex];

                _pageMD5Array = value;
            }
        }

        /// <summary>
        /// 本页Pool数组
        /// </summary>
        private KeyValuePair<int,Site>[] _pageIdSiteArray = new KeyValuePair<int, Site>[] { };
        private KeyValuePair<int, Site>[] pageIdSiteArray
        {
            get
            {
                return _pageIdSiteArray;
            }
            set
            {
                int oldIndex = Array.IndexOf(_pageIdSiteArray, SelectedIdSite);
                if (oldIndex != -1 && oldIndex < value.Length)
                    SelectedIdSite = value[oldIndex];

                _pageIdSiteArray = value;
            }
        }

        /// <summary>
        /// 选中图片MD5
        /// </summary>
        public string SelectedMD5
        {
            get;
            private set;
        }

        /// <summary>
        /// 选中Pool的id
        /// </summary>
        public KeyValuePair<int, Site> SelectedIdSite { get; private set; }

        #endregion

        #region 公有方法

        public PreviewListControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 显示Post预览图
        /// </summary>
        /// <param name="postPreviewInfoList"></param>
        public void DisplayPreviewList(List<PostPreviewInfo> postPreviewInfoList)
        {
            if (this.uniformGrid.Children.Count == 0)
                return;

            this.pageMD5Array = postPreviewInfoList.Select(a => a.file_md5).ToArray();

            //设置预览图
            for (int x = 0; x < postPreviewInfoList.Count; x++)
            {
                (this.uniformGrid.Children[x] as PreviewBoxControl).PreviewSource = postPreviewInfoList[x].preview;
            }

            //清空多余预览图
            if (postPreviewInfoList.Count < this.PreviewBoxCount)
            {
                for (int x = postPreviewInfoList.Count; x < this.PreviewBoxCount; x++)
                {
                    (this.uniformGrid.Children[x] as PreviewBoxControl).PreviewSource = null;
                }
            }
        }

        /// <summary>
        /// 显示Pool预览图
        /// </summary>
        /// <param name="poolPreviewInfoList"></param>
        public void DisplayPreviewList(List<PoolPreviewInfo> poolPreviewInfoList)
        {
            if (this.uniformGrid.Children.Count == 0)
                return;

            List<KeyValuePair<int, Site>> list = new List<KeyValuePair<int, Site>>();
            foreach(var item in poolPreviewInfoList)
            {
                list.Add(new KeyValuePair<int, Site>(item.id, item.site));
            }
            this.pageIdSiteArray = list.ToArray();

            //设置预览图
            for (int x = 0; x < poolPreviewInfoList.Count; x++)
            {
                (this.uniformGrid.Children[x] as PreviewBoxControl).PreviewSource = poolPreviewInfoList[x].preview;
                (this.uniformGrid.Children[x] as PreviewBoxControl).PoolTittle = poolPreviewInfoList[x].name;
            }

            //清空多余预览图
            if (poolPreviewInfoList.Count < this.PreviewBoxCount)
            {
                for (int x = poolPreviewInfoList.Count; x < this.PreviewBoxCount; x++)
                {
                    (this.uniformGrid.Children[x] as PreviewBoxControl).PreviewSource = null;
                    (this.uniformGrid.Children[x] as PreviewBoxControl).PoolTittle = null;
                }
            }
        }

        /// <summary>
        /// 清空预览图
        /// </summary>
        public void ClearPreview()
        {
            if (this.CurrentMode == Mode.Posts)
            {
                foreach (var previewBox in this.uniformGrid.Children)
                {
                    (previewBox as PreviewBoxControl).PreviewSource = null;
                }
            }
            else
            {
                foreach (var previewBox in this.uniformGrid.Children)
                {
                    (previewBox as PreviewBoxControl).PreviewSource = null;
                    (previewBox as PreviewBoxControl).PoolTittle = null;
                }
            }
        }

        #endregion

        #region 私有方法

        #region 右键菜单

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
        /// 自定义行数输入框加载时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rows_TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Text = this.ColumnsRows.Height.ToString();
        }

        /// <summary>
        /// 自定义列行数输入框加载时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Columns_TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Text = this.ColumnsRows.Width.ToString();
        }

        /// <summary>
        /// 自定义行列点击时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Custom_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            int rowsCount = 0;
            int columnsCount = 0;

            MenuItem parent = (sender as MenuItem);

            if (int.TryParse(((parent.Items[0] as Grid).Children[1] as TextBox).Text, out rowsCount) == false)
                return;
            if (int.TryParse(((parent.Items[1] as Grid).Children[1] as TextBox).Text, out columnsCount) == false)
                return;

            if (rowsCount > 0 && columnsCount > 0)
                this.ColumnsRows = new Size(columnsCount, rowsCount);
        }

        /// <summary>
        /// 预览图显示方式菜单加载时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Display_MenuItem_Loaded(object sender, RoutedEventArgs e)
        {
            MenuItem parent = (sender as MenuItem);

            //勾选设定相同的项目
            foreach (MenuItem item in parent.Items.OfType<MenuItem>().ToArray())
            {
                if (this.ColumnsRows.ToString().Equals(item.Tag))
                {
                    item.IsChecked = true;
                    return;
                }
            }

            //勾选最后一项
             (parent.Items[parent.Items.Count - 1] as MenuItem).IsChecked = true;
        }

        /// <summary>
        /// 预览图显示菜单点击时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Display_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem selectedItem = e.Source as MenuItem;

            if (selectedItem.Tag != null)
            {
                this.ColumnsRows = Size.Parse(selectedItem.Tag.ToString());
            }
        }

        /// <summary>
        /// 排序菜单加载时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Order_MenuItem_Loaded(object sender, RoutedEventArgs e)
        {
            MenuItem parent = (sender as MenuItem);

            if (this.CurrentMode == Mode.Posts)
            {
                //勾选设定相同的项目
                foreach (MenuItem item in parent.Items.OfType<MenuItem>().ToArray())
                {
                    if (this.PostOrderByColumn.ToString().Equals(item.Tag) || this.PostOrderMode.ToString().Equals(item.Tag))
                        item.IsChecked = true;
                }
            }
            else
            {
                //勾选设定相同的项目
                foreach (MenuItem item in parent.Items.OfType<MenuItem>().ToArray())
                {
                    if (this.PoolOrderByColumn.ToString().Equals(item.Tag) || this.PoolOrderMode.ToString().Equals(item.Tag))
                        item.IsChecked = true;
                }
            }
        }

        /// <summary>
        /// 排序依据菜单点击时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrderBy_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(this.CurrentMode == Mode.Posts)
                this.PostOrderByColumn = (PostOrderBy)Enum.Parse(typeof(PostOrderBy), (e.Source as MenuItem).Tag.ToString());
            else
                this.PoolOrderByColumn = (PoolOrderBy)Enum.Parse(typeof(PoolOrderBy), (e.Source as MenuItem).Tag.ToString());
        }

        /// <summary>
        /// 排序模式菜单点击时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrderMode_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.CurrentMode == Mode.Posts)
                this.PostOrderMode = (OrderMode)Enum.Parse(typeof(OrderMode), (e.Source as MenuItem).Tag.ToString());
            else
                this.PoolOrderMode = (OrderMode)Enum.Parse(typeof(OrderMode), (e.Source as MenuItem).Tag.ToString());
        }

        /// <summary>
        /// 原图位置菜单点击时
        /// </summary>
        private void OrginalPath_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fb = new System.Windows.Forms.FolderBrowserDialog();
            fb.ShowNewFolderButton = false;

            if (Directory.Exists(Properties.Settings.Default.ImageFolder))
                fb.SelectedPath = Properties.Settings.Default.ImageFolder;

            if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default.ImageFolder = fb.SelectedPath;
            }
        }

        /// <summary>
        /// 保存进度菜单点击时
        /// </summary>
        private void Save_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow).SaveStatus();
        }

        /// <summary>
        /// 载入进度菜单点击时
        /// </summary>
        private void Load_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow).LoadStatus();
        }

        #endregion

        #endregion


    }
}
