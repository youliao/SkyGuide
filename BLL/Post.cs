using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace BLL
{
    public class Post
    {

        /// <summary>
        /// 获取Post预览图
        /// </summary>
        /// <param name="customSQL">自定义SQL</param>
        /// <param name="inputTags">未经处理的标签</param>
        /// <param name="ratingArray">分级</param>
        /// <param name="preferArray">偏好</param>
        /// <param name="column">按此列排序</param>
        /// <param name="orderMode">排序模式</param>
        /// <param name="siteArray">图站</param>
        /// <returns></returns>
        public static List<PostPreviewInfo> GetPostPreviewInfo(string customSQL, string inputTags, Rating[] ratingArray, Prefer[] preferArray, PostOrderBy column, OrderMode orderMode, Site[] siteArray)
        {
            //多线程查询结果集
            List<PostPreviewInfo> result = new List<PostPreviewInfo>();

            //将参数打包
            Dictionary<string, object>[] paramDicArray = new Dictionary<string, object>[siteArray.Length];

            //根据查询图站数量创建线程
            Task<List<PostPreviewInfo>>[] tasks = new Task<List<PostPreviewInfo>>[siteArray.Length];
            for (int x = 0; x < siteArray.Length; x++)
            {
                paramDicArray[x] = new Dictionary<string, object>();
                paramDicArray[x].Add("customSQL", customSQL);
                paramDicArray[x].Add("inputTags", inputTags);
                paramDicArray[x].Add("ratingArray", ratingArray);
                paramDicArray[x].Add("column", column);
                paramDicArray[x].Add("site", siteArray[x]);

                tasks[x] = new Task<List<PostPreviewInfo>>(p =>
                {
                    Dictionary<string, object> pDic = (Dictionary<string, object>)p;

                    return GetOneSitePostPreview((string)pDic["customSQL"], (string)pDic["inputTags"], (Site)pDic["site"], (PostOrderBy)pDic["column"], (Rating[])pDic["ratingArray"]);
                }, paramDicArray[x]);
            }

            //开启多线程查询
            foreach (var t in tasks)
                t.Start();
            
            Task.WaitAll(tasks);

            //合并多线程结果集
            foreach (var t in tasks)
                result.AddRange(t.Result);

            IEnumerable<PostPreviewInfo> temp = result;

            //偏好
            List<PostPreviewInfo> preferList = Preference.GetPostPrefer(preferArray);
            if (preferList == null)
            {
                temp = result;
            }
            else
            {
                //差集
                if (preferArray.Contains(Prefer.Normal))
                    temp = result.Except(preferList);
                //交集
                else
                    temp = result.Intersect(preferList);
            }

            //去除重复
            if (siteArray.Length > 1)
            {
                //分数相加
                if (column == PostOrderBy.score)
                    temp = temp.GroupBy(a => a.file_md5).Select(b => new PostPreviewInfo() { file_md5 = b.First().file_md5, order_column = b.Sum(c => Convert.ToInt32(c.order_column)) });
                //发布日期选最早
                else if (column == PostOrderBy.submitted_on)
                    temp = temp.GroupBy(a => a.file_md5).Select(b => b.Min());
                else
                    temp = temp.GroupBy(a => a.file_md5).Select(b => b.First());
            }

            //排序
            if (orderMode == OrderMode.ASC)
                temp = temp.OrderBy(c => c.order_column);
            else
                temp = temp.OrderByDescending(c => c.order_column);

            return temp.ToList();
        }

        /// <summary>
        /// 获取图片详情
        /// </summary>
        /// <param name="file_md5">MD5</param>
        /// <param name="siteArray">图站</param>
        /// <returns></returns>
        public static List<PostDetail> GetPostDetail(string file_md5, Site[] siteArray)
        {
            //多线程查询结果集
            List<PostDetail> result = new List<PostDetail>();

            //将参数打包
            Dictionary<string, object>[] paramDicArray = new Dictionary<string, object>[siteArray.Length];

            //根据查询图站名称创建线程
            Task<PostDetail>[] tasks = new Task<PostDetail>[siteArray.Length];

            for (int x = 0; x < siteArray.Length; x++)
            {
                paramDicArray[x] = new Dictionary<string, object>();
                paramDicArray[x].Add("file_md5", file_md5);
                paramDicArray[x].Add("site", siteArray[x]);

                tasks[x] = new Task<PostDetail>(p =>
                {
                    Dictionary<string, object> pDic = (Dictionary<string, object>)p;

                    return GetOneSitePostDetail((string)pDic["file_md5"], (Site)pDic["site"]);
                }, paramDicArray[x]);
            }
            //开启多线程查询
            foreach (var t in tasks)
                t.Start();

            Task.WaitAll(tasks);

            //合并多线程结果集
            foreach (var t in tasks)
            {
                if (string.IsNullOrEmpty(t.Result.file_md5))
                    continue;

                result.Add(t.Result);
            }

            UpdatePrefer(ref result);
            return result;
        }

        /// <summary>
        /// 获取单个图站Post预览信息
        /// </summary>
        /// <param name="customSQL">自定义SQL</param>
        /// <param name="inputTags">未经处理的标签</param>
        /// <param name="column">按此列排序</param>
        /// <param name="site">图站</param>
        /// <param name="ratingArray">分级</param>
        /// <returns></returns>
        private static List<PostPreviewInfo> GetOneSitePostPreview(string customSQL, string inputTags, Site site, PostOrderBy column, Rating[] ratingArray)
        {
            string treatedTags = Tag.TreatInputTags(inputTags);

            DataTable dt = DAL.Post.SelectPostPreviewList(customSQL, treatedTags, site, column, ratingArray);

            return PostPreviewInfo.Convert(dt);
        }

        /// <summary>
        /// 获取单个图站图片详情
        /// </summary>
        /// <param name="file_md5"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        private static PostDetail GetOneSitePostDetail(string file_md5, Site site)
        {
            DataTable dt = DAL.Post.SelectPostDetail(file_md5, site);

            if (dt == null || dt.Rows.Count == 0)
                return new PostDetail();

            return new PostDetail()
            {
                site = site,
                id = Convert.ToInt32(dt.Rows[0]["id"]),
                submitted_on = Convert.ToDateTime(dt.Rows[0]["submitted_on"]),
                submitted_by = dt.Rows[0]["submitted_by"].ToString(),
                rating = (Rating)Enum.Parse(typeof(Rating), Enum.GetNames(typeof(Rating)).Where(a => a.Substring(0, 1).ToLower() == dt.Rows[0]["rating"].ToString()).First()),
                score = Convert.ToInt32(dt.Rows[0]["score"]),
                corrupted = Convert.ToBoolean(dt.Rows[0]["corrupted"]),
                has_children = Convert.ToBoolean(dt.Rows[0]["has_children"]),
                parent_id = dt.Rows[0]["parent_id"] == DBNull.Value ? null : (int?)(dt.Rows[0]["parent_id"]),
                source = dt.Rows[0]["source"].ToString(),
                tags = Tag.CRC32ToTagDic(dt.Rows[0]["tags"].ToString(), site),
                width = Convert.ToInt32(dt.Rows[0]["width"]),
                height = Convert.ToInt32(dt.Rows[0]["height"]),          
                image_format = dt.Rows[0]["image_format"].ToString(),
                file_size = Convert.ToInt32(dt.Rows[0]["file_size"]),
                file_md5 = dt.Rows[0]["file_md5"].ToString(),
                prefer = Prefer.Normal
            };
        }

        private static void UpdatePrefer(ref List<PostDetail> list)
        {
            Prefer prefer = Preference.GetMD5Prefer(Mode.Posts,list[0].file_md5);

            for(int x=0;x<list.Count;x++)
            {
                PostDetail pd = list[x];
                pd.prefer = prefer;
                list[x] = pd;
            }
        }
    }
}
