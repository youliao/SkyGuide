using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class Tag
    {
        /// <summary>
        /// 获取搜索输入框推荐标签
        /// </summary>
        /// <param name="incompleteTag">不完整的标签</param>
        /// <param name="site">图站</param>
        /// <returns></returns>
        public static List<TagDetail> GetRecommendedTag(string incompleteTag, Site[] siteArray, int limit = 5)
        {
            //多线程查询结果集
            List<TagDetail> result = new List<TagDetail>();

            //将参数打包
            Dictionary<string, object>[] paramDicArray = new Dictionary<string, object>[siteArray.Length];

            //根据查询图站名称创建线程
            Task<List<TagDetail>>[] tasks = new Task<List<TagDetail>>[siteArray.Length];

            for (int x = 0; x < siteArray.Length; x++)
            {
                paramDicArray[x] = new Dictionary<string, object>();
                paramDicArray[x].Add("incompleteTag", incompleteTag);
                paramDicArray[x].Add("site", siteArray[x]);

                tasks[x] = new Task<List<TagDetail>>(p =>
                {
                    Dictionary<string, object> pDic = (Dictionary<string, object>)p;

                    return GetOneSiteRecommendedTag((string)pDic["incompleteTag"], (Site)pDic["site"], limit);

                }, paramDicArray[x]);
            }
            //开启多线程查询
            foreach (var t in tasks)
                t.Start();

            Task.WaitAll(tasks);

            //合并多线程结果集
            foreach (var t in tasks)
                result.AddRange(t.Result);

            IEnumerable<TagDetail> re = result;

            //去除重复，并将图片数量合并
            if (siteArray.Length > 1)
                re = re.GroupBy(a => a.name).Select(b => new TagDetail() { name = b.First().name, tag_type = b.First().tag_type, post_count = b.Sum(c => c.post_count) });

            //按数量倒序排序
            re = re.OrderByDescending(a => a.post_count);
            //获取前几个数据
            re = re.Take(limit);

            return re.ToList();
        }

        /// <summary>
        /// CRC32转图片标签
        /// </summary>
        /// <param name="multiCRC32">多个图片标签CRC32</param>
        /// <returns></returns>
        internal static List<TagDetail> CRC32ToTagDic(string multiCRC32, Site site)
        {
            List<TagDetail> re = new List<TagDetail>();
            DataTable dt = DAL.Tag.SelectTagsForDetail(multiCRC32, site);

            if (dt == null || dt.Rows.Count == 0)
                return re;

            foreach (DataRow dr in dt.Rows)
            {
                re.Add(new TagDetail()
                {
                    name = dr["name"].ToString(),
                    tag_type = dr["type"].ToString(),
                    post_count = Convert.ToInt32(dr["post_count"])
                });
            }

            return re;
        }

        /// <summary>
        /// 处理输入的标签
        /// </summary>
        /// <param name="inputTags">未经处理的标签</param>
        /// <returns></returns>
        internal static string TreatInputTags(string inputTags)
        {
            if (string.IsNullOrWhiteSpace(inputTags))
                return inputTags;

            List<string> tagList = new List<string>(inputTags.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            string tags = TagToCRC32(tagList.First());
            tagList.RemoveAt(0);

            foreach (string tag in tagList)
            {
                switch (tag.First())
                {
                    case '+':
                        tags += string.Format(" OR {0} ", TagToCRC32(tag.Substring(1)));
                        break;
                    case '-':
                        tags += string.Format(" NOT {0} ", TagToCRC32(tag.Substring(1)));
                        break;
                    default:
                        tags += string.Format(" AND {0} ", TagToCRC32(tag));
                        break;
                }
            }

            return tags;
        }

        /// <summary>
        /// Tag转CRC32
        /// </summary>
        /// <param name="strObj">目标字符串</param>
        /// <returns></returns>
        private static string TagToCRC32(string strObj)
        {
            ulong Crc;
            var Crc32Table = new ulong[256];

            for (int i = 0; i < 256; i++)
            {
                Crc = (ulong)i;
                for (int j = 8; j > 0; j--)
                {
                    if ((Crc & 1) == 1)
                        Crc = (Crc >> 1) ^ 0xEDB88320;
                    else
                        Crc >>= 1;
                }
                Crc32Table[i] = Crc;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(strObj);
            ulong value = 0xffffffff;
            int len = buffer.Length;

            for (int i = 0; i < len; i++)
            {
                value = (value >> 8) ^ Crc32Table[(value & 0xFF) ^ buffer[i]];
            }

            return (value ^ 0xffffffff).ToString("x");
        }

        /// <summary>
        /// 获取单个图站推荐标签
        /// </summary>
        /// <param name="incompleteTag">不完整的标签</param>
        /// <param name="site">图站</param>
        /// <returns></returns>
        private static List<TagDetail> GetOneSiteRecommendedTag(string incompleteTag, Site site, int limit = 5)
        {
            List<TagDetail> re = new List<TagDetail>();
            DataTable dt = DAL.Tag.SelectTagsForRecommend(incompleteTag, site, limit);

            if (dt == null || dt.Rows.Count == 0)
                return re;

            foreach (DataRow dr in dt.Rows)
            {
                re.Add(new TagDetail()
                {
                    name = dr["name"].ToString(),
                    tag_type = dr["type"].ToString(),
                    post_count = Convert.ToInt32(dr["post_count"])
                });
            }

            return re;
        }
    }
}
