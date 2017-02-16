using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BLL
{
    /// <summary>
    /// Post预览信息
    /// </summary>
    public struct PostPreviewInfo : IComparable<PostPreviewInfo>, IEquatable<PostPreviewInfo>
    {
        public string file_md5 { get; set; }
        public object order_column { get; set; }
        public BitmapImage preview { get; set; }

        public PostPreviewInfo(string file_md5, object order_column, BitmapImage preview = null)
        {
            this.file_md5 = file_md5;
            this.order_column = order_column;
            this.preview = preview;
        }

        public int CompareTo(PostPreviewInfo other)
        {
            Type columnType = order_column.GetType();

            if (Equals(columnType, typeof(int)))
                return System.Convert.ToInt32(this.order_column).CompareTo(other.order_column);
            else if (Equals(columnType, typeof(DateTime)))
                return System.Convert.ToDateTime(this.order_column).CompareTo(other.order_column);
            else
                return this.order_column.ToString().CompareTo(other.order_column);
        }

        /// <summary>
        /// file_md5相等则结构体相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(PostPreviewInfo other)
        {
            return this.file_md5.Equals(other.file_md5);
        }

        /// <summary>
        /// 转换为结构体Preview
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<PostPreviewInfo> Convert(DataTable dt)
        {
            List<PostPreviewInfo> re = new List<PostPreviewInfo>();

            if (dt == null || dt.Rows.Count == 0)
                return re;

            switch (dt.Columns.Count)
            {
                //偏好
                case 1:
                    foreach (DataRow row in dt.Rows)
                        re.Add(new PostPreviewInfo(row[0].ToString(), null));
                    break;
                case 2:
                    foreach (DataRow row in dt.Rows)
                        re.Add(new PostPreviewInfo(row[0].ToString(), row[1]));
                    break;
            }

            return re;
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="allPreviewList">信息总集</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">显示数量</param>
        /// <returns></returns>
        public static List<PostPreviewInfo> Paging(List<PostPreviewInfo> allPreviewList,int pageIndex,int pageSize)
        {
            //异常
            if (pageIndex <= 0 || pageSize <= 0 || allPreviewList.Count < pageSize * (pageIndex - 1))
                return null;

            return allPreviewList.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
        }

        /// <summary>
        /// 增加Post预览图
        /// </summary>
        /// <param name="pagingList"></param>
        /// <param name="previewImageDic"></param>
        /// <returns></returns>
        public static List<PostPreviewInfo> AddPreview(List<PostPreviewInfo> pagingList,Dictionary<string, byte[]> previewImageDic)
        {
            List<PostPreviewInfo> re = new List<PostPreviewInfo>();

            foreach(var previewInfo in pagingList)
            {
                if (previewImageDic.ContainsKey(previewInfo.file_md5))
                {
                    using (MemoryStream ms = new MemoryStream(previewImageDic[previewInfo.file_md5]))
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = ms;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        re.Add(new PostPreviewInfo(previewInfo.file_md5, previewInfo.order_column, bitmapImage));
                    }    
                }
                else
                    re.Add(previewInfo);
            }

            return re;
        }
    }

    /// <summary>
    /// Pool预览信息
    /// </summary>
    public struct PoolPreviewInfo : IComparable<PoolPreviewInfo>, IEquatable<PoolPreviewInfo>
    {
        public int id { get; set; }
        public string name { get; set; }
        public string file_md5 { get; set; }
        public object order_column { get; set; }
        public Site site { get; set; }
        public BitmapImage preview { get; set; }

        public PoolPreviewInfo(int id, string name, string file_md5, object order_column, Site site, BitmapImage preview=null)
        {
            this.id = id;
            this.name = name;
            this.file_md5 = file_md5;
            this.order_column = order_column;
            this.site = site;
            this.preview = preview;
        }

        public int CompareTo(PoolPreviewInfo other)
        {
            Type columnType = order_column.GetType();

            if (Equals(columnType, typeof(int)))
                return System.Convert.ToInt32(this.order_column).CompareTo(other.order_column);
            else if (Equals(columnType, typeof(DateTime)))
                return System.Convert.ToDateTime(this.order_column).CompareTo(other.order_column);
            else
                return this.order_column.ToString().CompareTo(other.order_column);
        }

        /// <summary>
        /// file_md5相等则结构体相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(PoolPreviewInfo other)
        {
            return this.file_md5.Equals(other.file_md5);
        }

        /// <summary>
        /// 转换为结构体Preview
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<PoolPreviewInfo> Convert(DataTable dt,Site site=Site.Yande)
        {
            List<PoolPreviewInfo> re = new List<PoolPreviewInfo>();

            if (dt == null || dt.Rows.Count == 0)
                return re;

            switch (dt.Columns.Count)
            {
                //偏好
                case 1:
                    foreach (DataRow row in dt.Rows)
                        re.Add(new PoolPreviewInfo(0, null, row["file_md5"].ToString(), null, Site.Yande));
                    break;
                case 4:
                    foreach (DataRow row in dt.Rows)
                        re.Add(new PoolPreviewInfo(System.Convert.ToInt32(row["id"]), row["name"].ToString(), row["file_md5"].ToString(), row[3].ToString(),site));
                    break;
            }

            return re;
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="allPreviewList">信息总集</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">显示数量</param>
        /// <returns></returns>
        public static List<PoolPreviewInfo> Paging(List<PoolPreviewInfo> allPreviewList, int pageIndex, int pageSize)
        {
            //异常
            if (pageIndex <= 0 || pageSize <= 0 || allPreviewList.Count < pageSize * (pageIndex - 1))
                return null;

            return allPreviewList.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
        }


        /// <summary>
        /// 增加Pool预览图
        /// </summary>
        /// <param name="pagingList"></param>
        /// <param name="previewImageDic"></param>
        /// <returns></returns>
        public static List<PoolPreviewInfo> AddPreview(List<PoolPreviewInfo> pagingList, Dictionary<string, byte[]> previewImageDic)
        {
            List<PoolPreviewInfo> re = new List<PoolPreviewInfo>();

            foreach (var previewInfo in pagingList)
            {
                if (previewImageDic.ContainsKey(previewInfo.file_md5))
                {
                    using (MemoryStream ms = new MemoryStream(previewImageDic[previewInfo.file_md5]))
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = ms;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        re.Add(new PoolPreviewInfo(previewInfo.id, previewInfo.name, previewInfo.file_md5, previewInfo.order_column, previewInfo.site, bitmapImage));
                    }
                }
                else
                    re.Add(previewInfo);
            }

            return re;
        }
    }

    /// <summary>
    /// Post详情
    /// </summary>
    public struct PostDetail
    {
        public Site site { get; set; }
        public int id { get; set; }
        public DateTime submitted_on { get; set; }
        public string submitted_by { get; set; }
        public Rating rating { get; set; }
        public int score { get; set; }
        public bool corrupted { get; set; }
        public bool has_children { get; set; }
        public int? parent_id { get; set; }
        public string source { get; set; }
        public List<TagDetail> tags { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string image_format { get; set; }
        public string file_md5 { get; set; }
        public int file_size { get; set; }
        public Prefer prefer { get; set; }

        public bool IsEmpty
        {
            get
            {
                return Equals(file_md5, null);
            }
        }
    }

    /// <summary>
    /// Pool详情
    /// </summary>
    public struct PoolDetail
    {
        public Site site { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public Rating rating { get; set; }
        public int post_count { get; set; }

        public bool IsEmpty
        {
            get
            {
                return Equals(name, null);
            }
        }
    }

    /// <summary>
    /// Tag详情
    /// </summary>
    public struct TagDetail
    {
        public string name { get; set; }
        public string tag_type { get; set; }
        public int post_count { get; set; }
    }

}
