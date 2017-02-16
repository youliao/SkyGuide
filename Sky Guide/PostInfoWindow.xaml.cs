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
using System.Windows.Shapes;
using BLL;
using DAL;

namespace SkyGuide
{
    /// <summary>
    /// PostInfoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PostInfoWindow : Window
    {
        #region 属性

        /// <summary>
        /// 主窗体所选择的图站
        /// </summary>
        Site[] selectedSite;

        /// <summary>
        /// 图片预览信息列表
        /// </summary>
        List<PostPreviewInfo> postPreviewInfoList;

        /// <summary>
        /// 当前图片在图片预览信息列表的索引
        /// </summary>
        int _imageIndex;
        int imageIndex
        {
            get
            {
                return _imageIndex;
            }
            set
            {
                if (value < 0 || value >= postPreviewInfoList.Count)
                    return;

                List<PostDetail> detailList = BLL.Post.GetPostDetail(postPreviewInfoList[value].file_md5, this.selectedSite);
                this.postInfoControl.tabControl.ItemsSource = detailList;
                this.postInfoControl.tabControl.SelectedIndex = 0;

                this.imageViewerControl.SetImage(detailList[0]);

                //预读下一张图片
                if (value - _imageIndex == 1 && value + 1 < postPreviewInfoList.Count)
                    this.imageViewerControl.SetCache(BLL.Post.GetPostDetail(postPreviewInfoList[value + 1].file_md5, this.selectedSite)[0]);
                //预读上一张图片
                else if (value - _imageIndex == -1 && value - 1 >= 0)
                    this.imageViewerControl.SetCache(BLL.Post.GetPostDetail(postPreviewInfoList[value - 1].file_md5, this.selectedSite)[0]);

                _imageIndex = value;
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 图片信息窗体构造函数
        /// </summary>
        /// <param name="file_md5">图片md5</param>
        /// <param name="selectedSite">主窗体所选图站</param>
        /// <param name="postPreviewInfoList">主窗体预览信息列表</param>
        public PostInfoWindow(string file_md5, Site[] selectedSite, List<PostPreviewInfo> postPreviewInfoList)
        {
            InitializeComponent();
            this.postPreviewInfoList = postPreviewInfoList;
            this.selectedSite = selectedSite;
            imageIndex = postPreviewInfoList.FindIndex(a => a.file_md5 == file_md5);
        }

        /// <summary>
        /// 鼠标滚轮滚动翻页
        /// </summary>
        private void imageViewerControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                imageIndex -= 1;
            else
                imageIndex += 1;
        }

        /// <summary>
        /// 图片窗体关闭后，主窗体页码对应改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PostInfoWindow_Closed(object sender, EventArgs e)
        {
            int pageIndex = this.imageIndex / (this.Owner as MainWindow).searchControl.pageSize + 1;

            if (pageIndex != (this.Owner as MainWindow).searchControl.pageIndex)
                (this.Owner as MainWindow).searchControl.pageIndex = pageIndex;
        }

        #endregion

    }
}
