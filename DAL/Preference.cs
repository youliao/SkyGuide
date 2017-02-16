using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    /// <summary>
    /// 偏好
    /// </summary>
    public class Preference
    {
        /// <summary>
        /// 获取偏好MD5列表
        /// </summary>
        /// <param name="mode">模式</param>
        /// <param name="preferenceArray">需获取的偏好</param>
        /// <returns></returns>
        public static DataTable SelectPreferMD5(Mode mode, params Prefer[] selectPreferArray)
        {
            string commandText = string.Format("SELECT file_md5 FROM {0} WHERE prefer IN ({1})", mode.ToString(), string.Join(",", selectPreferArray.Select(s => s.ToString("d"))));
            SQLiteCommand command = SQLiteHelper.CreateCommand(DBConn.prefer,commandText);

            return SQLiteHelper.ExecuteDataTable(command);
        }

        /// <summary>
        /// 获取MD5的偏好
        /// </summary>
        /// <param name="mode">模式</param>
        /// <param name="file_md5"></param>
        /// <returns></returns>
        public static DataTable SelectMD5Prefer(Mode mode, string file_md5)
        {
            string commandText = string.Format("SELECT prefer FROM {0} WHERE file_md5 = @file_md5", mode.ToString());

            SQLiteParameter param = new SQLiteParameter("@file_md5", file_md5);
            SQLiteCommand command = SQLiteHelper.CreateCommand(DBConn.prefer, commandText,param);

            return SQLiteHelper.ExecuteDataTable(command);
        }

        /// <summary>
        /// 更新偏好信息
        /// </summary>
        /// <param name="mode">模式</param>
        /// <param name="file_md5">图片MD5</param>
        /// <param name="prefer"></param>
        /// <returns></returns>
        public static int UpdatePrefer(Mode mode, string file_md5, Prefer prefer)
        {
            string commandText = string.Empty;
            List<SQLiteParameter> paramList = new List<SQLiteParameter>();

            if (prefer == Prefer.Normal)
            {
                commandText = string.Format("DELETE FROM {0} WHERE file_md5 = @file_md5", mode.ToString());
                paramList.Add(new SQLiteParameter("@file_md5", file_md5));
            }
            else
            {
                commandText = string.Format("DELETE FROM {0} WHERE file_md5 = @file_md5;INSERT INTO {0} VALUES (@file_md5,@prefer)", mode.ToString());
                paramList.Add(new SQLiteParameter("@file_md5", file_md5));
                paramList.Add(new SQLiteParameter("@prefer", prefer.ToString("d")));
            }

            SQLiteCommand command = SQLiteHelper.CreateCommand(DBConn.prefer, commandText, paramList.ToArray());

            return SQLiteHelper.ExecuteNonQuery(command);
        }
    }
}
