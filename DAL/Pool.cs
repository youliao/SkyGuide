using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Pool
    {
        /// <summary>
        /// 获取Pool预览列表
        /// </summary>
        /// <param name="inputName">用户输入的名称</param>
        /// <param name="site">图站</param>
        /// <param name="column">按此列排序</param>
        /// <param name="ratingArray">分级</param>
        /// <returns></returns>
        public static DataTable SelectPoolPreviewList(string inputName,Site site, PoolOrderBy column, Rating[] ratingArray)
        {
            string selectColumns = string.Format("id,name,file_md5,{0}", column.ToString());
            string ratingIN = string.Join(",", ratingArray.Select(s => string.Format("'{0}'", s.ToString().Substring(0, 1).ToLower())));

            string commandText = string.Format("SELECT {0} FROM pools WHERE name LIKE ('%'||@inputName||'%') AND rating IN ({1})", selectColumns, ratingIN);

            SQLiteParameter param = new SQLiteParameter("@inputName", inputName);
            SQLiteCommand command = SQLiteHelper.CreateCommand(DBConn.GetConn(site), commandText, param);

            return SQLiteHelper.ExecuteDataTable(command);
        }

        /// <summary>
        /// 获取Pool详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="site">图站</param>
        /// <returns></returns>
        public static DataTable SelectPoolDetail(int id, Site site)
        {
            string commandText = "SELECT * FROM pools WHERE id=@id";

            SQLiteParameter param = new SQLiteParameter("@id", id);
            SQLiteCommand command = SQLiteHelper.CreateCommand(DBConn.GetConn(site), commandText, param);

            return SQLiteHelper.ExecuteDataTable(command);
        }

        /// <summary>
        /// 获取Pool中posts预览列表
        /// </summary>
        /// <param name="pool_id">pool的id</param>
        /// <param name="site">图站</param>
        /// <returns></returns>
        public static DataTable SelectPoolPostsPreviewList(int pool_id, Site site)
        {
            string commandText = string.Format("SELECT file_md5,sequence FROM pool_posts WHERE pool_id=@pool_id");

            SQLiteParameter param = new SQLiteParameter("@pool_id", pool_id);
            SQLiteCommand command = SQLiteHelper.CreateCommand(DBConn.GetConn(site), commandText, param);

            return SQLiteHelper.ExecuteDataTable(command);
        }
    }
}
