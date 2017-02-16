using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Tag
    {
        /// <summary>
        /// 为图片详情查询某图片的标签
        /// </summary>
        /// <param name="multiCRC32">多个CRC32标签</param>
        /// <returns></returns>
        public static DataTable SelectTagsForDetail(string multiCRC32,Site site)
        {
            string tagCRC32IN = string.Join(",", multiCRC32.Split(' ').Select(s => string.Format("'{0}'",s)));

            string commandText = string.Format("SELECT name,type,post_count FROM tags WHERE crc32 IN ({0}) ORDER BY type,name", tagCRC32IN);

            SQLiteCommand command = SQLiteHelper.CreateCommand(DBConn.GetConn(site),commandText);

            return SQLiteHelper.ExecuteDataTable(command);
        }

        /// <summary>
        /// 为推荐查询匹配图片标签
        /// </summary>
        /// <param name="incompleteTag">不完整的标签</param>
        /// <param name="limit">返回数量</param>
        /// <returns></returns>
        public static DataTable SelectTagsForRecommend(string incompleteTag, Site site, int limit = 5)
        {
            //搜索 （标签*） 和 （*_标签*）
            string commandText = @"SELECT name,type,post_count FROM tags WHERE name LIKE (@incompleteTag||'%') OR name LIKE ('%\_'||@incompleteTag||'%') ESCAPE '\' ORDER BY post_count DESC LIMIT @limit";
            SQLiteParameter[] paramArray = {
                new SQLiteParameter("@incompleteTag", incompleteTag),
                new SQLiteParameter("@limit", limit),
            }; 

            SQLiteCommand command = SQLiteHelper.CreateCommand(DBConn.GetConn(site), commandText, paramArray);

            return SQLiteHelper.ExecuteDataTable(command);
        }
    }
}
