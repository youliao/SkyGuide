using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class Pool
    {
        /// <summary>
        /// 获取Pool预览图
        /// </summary>
        /// <param name="inputName">用户输入的名称</param>
        /// <param name="ratingArray">分级</param>
        /// <param name="preferArray">偏好</param>
        /// <param name="column">按此列排序</param>
        /// <param name="orderMode">排序模式</param>
        /// <param name="siteArray">图站</param>
        /// <returns></returns>
        public static List<PoolPreviewInfo> GetPoolPreview(string inputName, Rating[] ratingArray, Prefer[] preferArray, PoolOrderBy column, OrderMode orderMode, Site[] siteArray)
        {
            //多线程查询结果集
            List<PoolPreviewInfo> result = new List<PoolPreviewInfo>();

            //将参数打包
            Dictionary<string, object>[] paramDicArray = new Dictionary<string, object>[siteArray.Length];

            //根据查询图站数量创建线程
            Task<List<PoolPreviewInfo>>[] tasks = new Task<List<PoolPreviewInfo>>[siteArray.Length];
            for (int x = 0; x < siteArray.Length; x++)
            {
                paramDicArray[x] = new Dictionary<string, object>();
                paramDicArray[x].Add("inputName", inputName);
                paramDicArray[x].Add("ratingArray", ratingArray);
                paramDicArray[x].Add("column", column);
                paramDicArray[x].Add("site", siteArray[x]);

                tasks[x] = new Task<List<PoolPreviewInfo>>(p =>
                {
                    Dictionary<string, object> pDic = (Dictionary<string, object>)p;

                    return GetOneSitePoolPreview((string)pDic["inputName"], (Site)pDic["site"], (PoolOrderBy)pDic["column"], (Rating[])pDic["ratingArray"]);
                }, paramDicArray[x]);
            }

            //开启多线程查询
            foreach (var t in tasks)
                t.Start();

            Task.WaitAll(tasks);

            //合并多线程结果集
            foreach (var t in tasks)
                result.AddRange(t.Result);

            IEnumerable<PoolPreviewInfo> temp;

            //偏好
            List <PoolPreviewInfo> preferList = Preference.GetPoolPrefer(preferArray);
            if (preferList == null || preferList.Count ==0)
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

            //排序
            if (orderMode == OrderMode.ASC)
                temp = temp.OrderBy(c => c.order_column);
            else
                temp = temp.OrderByDescending(c => c.order_column);

            return temp.ToList();

        }

        /// <summary>
        /// 获取单个图站Pool详情
        /// </summary>
        /// <param name="file_md5"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        public static PoolDetail GetOneSitePoolDetail(int id, Site site)
        {
            DataTable dt = DAL.Pool.SelectPoolDetail(id, site);

            if (dt == null || dt.Rows.Count == 0)
                return new PoolDetail();

            return new PoolDetail()
            {
                site = site,
                id = Convert.ToInt32(dt.Rows[0]["id"]),
                name = dt.Rows[0]["name"].ToString(),
                description = dt.Rows[0]["description"].ToString(),
                created_at = Convert.ToDateTime(dt.Rows[0]["created_at"]),
                updated_at = Convert.ToDateTime(dt.Rows[0]["updated_at"]),
                rating = (Rating)Enum.Parse(typeof(Rating), Enum.GetNames(typeof(Rating)).Where(a => a.Substring(0, 1).ToLower() == dt.Rows[0]["rating"].ToString()).First()),
                post_count = Convert.ToInt32(dt.Rows[0]["post_count"]),
            };
        }

        /// <summary>
        /// 获取单个图站PoolPosts信息
        /// </summary>
        /// <param name="pool_id"></param>
        /// <param name="orderMode"></param>
        /// <param name="preferArray"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        public static List<PostPreviewInfo> GetOneSitePoolPostsPreview(int pool_id,Site site)
        {
            DataTable dt = DAL.Pool.SelectPoolPostsPreviewList(pool_id, site);

            IEnumerable<PostPreviewInfo> temp = PostPreviewInfo.Convert(dt);

            //排序
            temp = temp.OrderBy(c => c.order_column);

            return temp.ToList();
        }

        /// <summary>
        /// 获取单个图站Pool预览信息
        /// </summary>
        /// <param name="inputName">用户输入的名称</param>
        /// <param name="site">图站</param>
        /// <param name="column">按此列排序</param>
        /// <param name="ratingArray">分级</param>
        /// <returns></returns>
        private static List<PoolPreviewInfo> GetOneSitePoolPreview(string inputName, Site site, PoolOrderBy column, Rating[] ratingArray)
        {
            DataTable dt = DAL.Pool.SelectPoolPreviewList(inputName, site, column, ratingArray);

            return PoolPreviewInfo.Convert(dt, site);
        }
    }
}
