using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class Preference
    {

        /// <summary>
        /// 获取Post偏好
        /// </summary>
        /// <param name="mode">模式</param>
        /// <param name="preferArray">前端所选偏好</param>
        /// <returns></returns>
        internal static List<PostPreviewInfo> GetPostPrefer(Prefer[] preferArray)
        {
            List<Prefer> selected = new List<Prefer>();

            if (preferArray == null || preferArray.Length == 0 || preferArray.Length == 3)
                return null;

            if (preferArray.Contains(Prefer.Normal))
            {
                if (preferArray.Contains(Prefer.Bad) == false)
                    selected.Add(Prefer.Bad);
                if (preferArray.Contains(Prefer.Good) == false)
                    selected.Add(Prefer.Good);
            }
            else
            {
                if (preferArray.Contains(Prefer.Bad))
                    selected.Add(Prefer.Bad);
                if (preferArray.Contains(Prefer.Good))
                    selected.Add(Prefer.Good);
            }

            DataTable dt = DAL.Preference.SelectPreferMD5(Mode.Posts, selected.ToArray());

            return PostPreviewInfo.Convert(dt);
        }

        /// <summary>
        /// 获取Pool偏好
        /// </summary>
        /// <param name="mode">模式</param>
        /// <param name="preferArray">前端所选偏好</param>
        /// <returns></returns>
        internal static List<PoolPreviewInfo> GetPoolPrefer(Prefer[] preferArray)
        {
            List<Prefer> selected = new List<Prefer>();

            if (preferArray == null || preferArray.Length == 0 || preferArray.Length == 3)
                return null;

            if (preferArray.Contains(Prefer.Normal))
            {
                if (preferArray.Contains(Prefer.Bad) == false)
                    selected.Add(Prefer.Bad);
                if (preferArray.Contains(Prefer.Good) == false)
                    selected.Add(Prefer.Good);
            }
            else
            {
                if (preferArray.Contains(Prefer.Bad))
                    selected.Add(Prefer.Bad);
                if (preferArray.Contains(Prefer.Good))
                    selected.Add(Prefer.Good);
            }

            DataTable dt = DAL.Preference.SelectPreferMD5(Mode.Posts, selected.ToArray());

            return PoolPreviewInfo.Convert(dt);
        }

        /// <summary>
        /// 获取MD5对应偏好
        /// </summary>
        /// <param name="mode">模式</param>
        /// <param name="file_md5">MD5</param>
        /// <returns></returns>
        public static Prefer GetMD5Prefer(Mode mode,string file_md5)
        {
            DataTable dt = DAL.Preference.SelectMD5Prefer(mode, file_md5);

            if (dt == null || dt.Rows.Count == 0)
                return Prefer.Normal;

            return (Prefer)Enum.Parse(typeof(Prefer), Convert.ToInt32(dt.Rows[0][0]).ToString());        
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
            return DAL.Preference.UpdatePrefer(mode, file_md5, prefer);
        }
    }
}
