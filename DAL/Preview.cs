using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Preview
    {
        /// <summary>
        /// 获取预览图
        /// </summary>
        /// <param name="md5Group">按首位分组的md5</param>
        /// <returns></returns>
        public static DataTable SelectPreviewImage(IGrouping<string, string> md5Group)
        {
            string md5IN = string.Join(",", md5Group.Select(s => string.Format("'{0}'", s)));
            string commandText = string.Format("SELECT file_md5,preview FROM _{0} WHERE file_md5 IN ({1})", md5Group.Key, md5IN);

            SQLiteCommand command = SQLiteHelper.CreateCommand(DBConn.preview, commandText);

            return SQLiteHelper.ExecuteDataTable(command);
        }
    }
}
