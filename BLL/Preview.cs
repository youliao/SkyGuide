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
    public class Preview
    {

        /// <summary>
        /// 获取Post预览图
        /// </summary>
        /// <param name="previewInfoList">预览信息列表</param>
        /// <returns></returns>
        public static List<PostPreviewInfo> GetPreviewImage(List<PostPreviewInfo> previewInfoList)
        {
            //按md5首位分类
            var md5LookUp = previewInfoList.Select(a => a.file_md5).ToLookup(b => b.Substring(0, 1));

            //多线程查询结果集
            IEnumerable<KeyValuePair<string, byte[]>> result = new Dictionary<string, byte[]>();

            //根据查询图站数量创建线程
            Task<Dictionary<string, byte[]>>[] tasks = new Task<Dictionary<string, byte[]>>[md5LookUp.Count];
            int x = 0;
            foreach (var md5Group in md5LookUp)
            {
                tasks[x] = new Task<Dictionary<string, byte[]>>(p =>
                {
                    return GetPreviewImageByGroup((IGrouping<string, string>)p);
                }, md5Group);
                x++;
            }

            //开启多线程查询
            foreach (var t in tasks)
                t.Start();

            Task.WaitAll(tasks);

            //合并多线程结果集
            foreach (var t in tasks)
                result = result.Concat(t.Result);

            return PostPreviewInfo.AddPreview(previewInfoList, result.ToDictionary(k => k.Key, v => v.Value));
        }

        /// <summary>
        /// 获取Pool预览图
        /// </summary>
        /// <param name="previewInfoList">预览信息列表</param>
        /// <returns></returns>
        public static List<PoolPreviewInfo> GetPreviewImage(List<PoolPreviewInfo> previewInfoList)
        {
            //按md5首位分类
            var md5LookUp = previewInfoList.Select(a => a.file_md5).ToLookup(b => b.Substring(0, 1));

            //多线程查询结果集
            IEnumerable<KeyValuePair<string, byte[]>> result = new Dictionary<string, byte[]>();

            //根据查询图站数量创建线程
            Task<Dictionary<string, byte[]>>[] tasks = new Task<Dictionary<string, byte[]>>[md5LookUp.Count];
            int x = 0;
            foreach (var md5Group in md5LookUp)
            {
                tasks[x] = new Task<Dictionary<string, byte[]>>(p =>
                {
                    return GetPreviewImageByGroup((IGrouping<string, string>)p);
                }, md5Group);
                x++;
            }

            //开启多线程查询
            foreach (var t in tasks)
                t.Start();

            Task.WaitAll(tasks);

            //合并多线程结果集
            foreach (var t in tasks)
                result = result.Concat(t.Result);

            return PoolPreviewInfo.AddPreview(previewInfoList, result.ToDictionary(k => k.Key, v => v.Value));
        }

        /// <summary>
        /// 按分组获取预览图
        /// </summary>
        /// <param name="md5Group">按首位分组的md5</param>
        /// <returns></returns>
        private static Dictionary<string, byte[]> GetPreviewImageByGroup(IGrouping<string, string> md5Group)
        {
            Dictionary<string, byte[]> re = new Dictionary<string, byte[]>();

            DataTable dt = DAL.Preview.SelectPreviewImage(md5Group);

            if (dt == null || dt.Rows.Count == 0)
                return re;

            foreach (DataRow row in dt.Rows)
                re.Add(row["file_md5"].ToString(), (byte[])row["preview"]);
                
            return re;
        }
    }
}
