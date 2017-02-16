using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    internal class DBConn
    {
        public static string yande { get; private set; }
        public static string konachan { get; private set; }
        public static string danbooru { get; private set; }
        public static string gelbooru { get; private set; }
        public static string sankaku { get; private set; }
        /// <summary>
        /// 偏好
        /// </summary>
        public static string prefer { get; private set; }
        /// <summary>
        /// 预览图
        /// </summary>
        public static string preview { get; private set; }

        static DBConn()
        {
            string dbPath = Path.Combine(Environment.CurrentDirectory, "db");

            yande = string.Format("Data Source={0};Version=3;Read Only=True;", Path.Combine(dbPath, "yande.s3db"));
            konachan = string.Format("Data Source={0};Version=3;Read Only=True;", Path.Combine(dbPath, "konachan.s3db"));
            danbooru = string.Format("Data Source={0};Version=3;Read Only=True;", Path.Combine(dbPath, "danbooru.s3db"));
            gelbooru = string.Format("Data Source={0};Version=3;Read Only=True;", Path.Combine(dbPath, "gelbooru.s3db"));
            sankaku = string.Format("Data Source={0};Version=3;Read Only=True;", Path.Combine(dbPath, "sankaku.s3db"));
            prefer = string.Format("Data Source={0};Version=3;Read Only=False;", Path.Combine(dbPath, "moe_ex.s3db"));
            preview = string.Format("Data Source={0};Version=3;Read Only=True;", Path.Combine(dbPath, "moe_previews.s3db"));
        }

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <param name="site">图站</param>
        /// <returns></returns>
        public static string GetConn(Site site)
        {
            switch (site)
            {
                case Site.Yande:
                    return yande;
                case Site.Konachan:
                    return konachan;
                case Site.Danbooru:
                    return danbooru;
                case Site.Gelbooru:
                    return gelbooru;
                case Site.Sankaku:
                    return sankaku;
                default:
                    return null;
            }
        }
    }
}
