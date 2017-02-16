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
using DAL;
using BLL;

namespace SkyGuide
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 保存进度
        /// </summary>
        public void SaveStatus()
        {
            string tag = this.searchControl.TagSearchText;
            string result = this.searchControl.ResultSearchText;
            string site = string.Join(",", this.searchControl.SiteSelected);
            string rating = string.Join(",", this.searchControl.RatingSelected);
            string prefer = string.Join(",", this.searchControl.PreferSelected);
            string orderByColumn = this.searchControl.postOrderByColumn.ToString();
            string orderMode = this.searchControl.postOrderMode.ToString();
            string md5 = string.Join(",", this.previewListControl.pageMD5Array);

            //〇tag,①result,②rating,③site,④prefer,⑤orderByColumn,⑥orderMode,⑦md5
            string statusText = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}", tag, result, rating, site, prefer, orderByColumn, orderMode, md5);
            string base64Text = Convert.ToBase64String(Encoding.Default.GetBytes(statusText));

            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();

            if (System.IO.Directory.Exists(Properties.Settings.Default.StatusFolder))
                sfd.InitialDirectory = Properties.Settings.Default.StatusFolder;
            else
                sfd.InitialDirectory = Environment.CurrentDirectory;
 
            sfd.Filter = "进度文件|*.svs";
            //确认保存
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    System.IO.File.WriteAllText(sfd.FileName, base64Text);
                    Properties.Settings.Default.StatusFolder = System.IO.Path.GetDirectoryName(sfd.FileName);
                }
                catch(Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
        }

        public void LoadStatus()
        {
            
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();

            if (System.IO.Directory.Exists(Properties.Settings.Default.StatusFolder))
                ofd.InitialDirectory = Properties.Settings.Default.StatusFolder;
            else
                ofd.InitialDirectory = Environment.CurrentDirectory;

            ofd.Filter = "进度文件|*.svs";

            //确认打开
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    string base64Text = System.IO.File.ReadAllText(ofd.FileName);
                    string statusText = Encoding.Default.GetString(Convert.FromBase64String(base64Text));

                    //tag
                    string[] infos = statusText.Split('|');

                    //result
                    this.searchControl.tagSearchTextBox.Text = infos[0];
                    if (string.IsNullOrEmpty(infos[1]))
                    {
                        this.searchControl.checkBox.IsChecked = false;
                    }
                    else
                    {
                        this.searchControl.resultSearchTextBox.Text = infos[1];
                        this.searchControl.checkBox.IsChecked = true;
                    }

                    //rating
                    List<Rating> ratingList = new List<Rating>();
                    foreach(string item in infos[2].Split(','))
                    {
                        ratingList.Add((Rating)Enum.Parse(typeof(Rating), item));
                    }
                    this.searchControl.RatingSelected = ratingList.ToArray();

                    //site
                    List<Site> siteList = new List<Site>();
                    foreach (string item in infos[3].Split(','))
                    {
                        siteList.Add((Site)Enum.Parse(typeof(Site), item));
                    }
                    this.searchControl.SiteSelected = siteList.ToArray();

                    //prefer
                    List<Prefer> preferList = new List<Prefer>();
                    foreach (string item in infos[4].Split(','))
                    {
                        preferList.Add((Prefer)Enum.Parse(typeof(Prefer), item));
                    }
                    this.searchControl.PreferSelected = preferList.ToArray();

                    //orderByColumn
                    this.searchControl.postOrderByColumn = (PostOrderBy)Enum.Parse(typeof(PostOrderBy), infos[5]);

                    //orderMode
                    this.searchControl.postOrderMode = (OrderMode)Enum.Parse(typeof(OrderMode), infos[6]);

                    //开始搜索
                    this.searchControl.StartSearch();

                    //确定页码
                    foreach (string item in infos[7].Split(','))
                    {
                        var list = this.searchControl.postPreviewInfoList.Where(a => a.file_md5 == item).ToList();

                        if (list.Count == 0)
                            continue;

                        int index = this.searchControl.postPreviewInfoList.IndexOf(list[0]);

                        this.searchControl.pageIndex = index / this.searchControl.pageSize + 1;

                        break;
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }


            }

        }

        /// <summary>
        /// 关闭时保存设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// 页码设定时
        /// </summary>
        private void searchControl_PageIndexSet(bool isEmpty)
        {
            if (isEmpty)
            {
                previewListControl.ClearPreview();
            }
            else
            {
                if (this.searchControl.ModeSelected == Mode.Posts)
                {
                    //分页
                    List<PostPreviewInfo> thisPagePreview = PostPreviewInfo.Paging(searchControl.postPreviewInfoList, searchControl.pageIndex, searchControl.pageSize);
                    //查询预览图
                    thisPagePreview = BLL.Preview.GetPreviewImage(thisPagePreview);
                    previewListControl.DisplayPreviewList(thisPagePreview);
                }
                else
                {
                    //分页
                    List<PoolPreviewInfo> thisPagePreview = PoolPreviewInfo.Paging(searchControl.poolPreviewInfoList, searchControl.pageIndex, searchControl.pageSize);
                    //查询预览图
                    thisPagePreview = BLL.Preview.GetPreviewImage(thisPagePreview);
                    previewListControl.DisplayPreviewList(thisPagePreview);
                }

            }
        }

        /// <summary>
        /// 鼠标滚轮翻页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void previewListControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                searchControl.pageIndex -= 1;
            else if(e.Delta < 0)
                searchControl.pageIndex += 1;
        }

        /// <summary>
        /// 鼠标左键点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void previewListControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //鼠标不在图片上
            if (e.OriginalSource.GetType() != typeof(Border))
                return;

            if (this.searchControl.ModeSelected == Mode.Posts)
            {
                PostInfoWindow postInfoWindow = new PostInfoWindow(previewListControl.SelectedMD5, searchControl.SiteSelected, searchControl.postPreviewInfoList);
                postInfoWindow.Owner = this;
                postInfoWindow.ShowDialog();
            }
            else
            {
                MainWindow poolWindow = new MainWindow();
                poolWindow.Owner = this;
                poolWindow.searchControl.Visibility = Visibility.Collapsed;
                poolWindow.searchControl.postPreviewInfoList = BLL.Pool.GetOneSitePoolPostsPreview(this.previewListControl.SelectedIdSite.Key, this.previewListControl.SelectedIdSite.Value);
                poolWindow.previewListControl.uniformGrid.ContextMenu = null;
                poolWindow.ShowDialog();

            }
        }
    }
}
