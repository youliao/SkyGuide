using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Post
    {
        /// <summary>
        /// 获取post预览列表
        /// </summary>
        /// <param name="customSQL">自定义SQL</param>
        /// <param name="treatedTags">经处理过的标签</param>
        /// <param name="site">图站</param>
        /// <param name="column">按此列排序</param>
        /// <param name="ratingArray">分级</param>
        /// <returns></returns>
        public static DataTable SelectPostPreviewList(string customSQL, string treatedTags, Site site, PostOrderBy column, Rating[] ratingArray)
        {
            string selectColumns = string.Format("file_md5,{0}", column.ToString());
            string ratingIN = string.Join(",", ratingArray.Select(s => string.Format("'{0}'", s.ToString().Substring(0, 1).ToLower())));
            string tagsMATCH = string.IsNullOrWhiteSpace(treatedTags) ? string.Empty : string.Format(" AND id IN (SELECT docid FROM posts_fts4 WHERE tags MATCH '{0}')", treatedTags);
         
          
            if(string.IsNullOrWhiteSpace(customSQL)==false)
                customSQL = string.Format(" AND ({0})", string.Join(" AND ", customSQL.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)));

            string commandText = string.Format("SELECT {0} FROM posts WHERE rating IN ({1}){2}{3}", selectColumns, ratingIN, tagsMATCH, customSQL);

            SQLiteCommand command = SQLiteHelper.CreateCommand(DBConn.GetConn(site), commandText);

            try
            {
                DataTable dt = SQLiteHelper.ExecuteDataTable(command);
                return dt;
            }
            catch
            {
                return null;
            }        
        }

        /// <summary>
        /// 获取post详情
        /// </summary>
        /// <param name="file_md5"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        public static DataTable SelectPostDetail(string file_md5,Site site)
        {
            string commandText = "SELECT * FROM posts WHERE file_md5 = @file_md5";

            SQLiteParameter param = new SQLiteParameter("@file_md5", file_md5);
            SQLiteCommand command = SQLiteHelper.CreateCommand(DBConn.GetConn(site), commandText, param);

            return SQLiteHelper.ExecuteDataTable(command);
        }
    }
}
