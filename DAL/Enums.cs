using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    /// <summary>
    /// 图站
    /// </summary>
    public enum Site
    {
        Yande,
        Konachan,
        Danbooru,
        Gelbooru,
        Sankaku
    }

    /// <summary>
    /// 分级
    /// </summary>
    public enum Rating
    {
        Safe,
        Questionable,
        Explicit
    }

    /// <summary>
    /// Post排序依据
    /// </summary>
    public enum PostOrderBy
    {
        submitted_on,
        score,
        width,
        height,
        file_size
    }

    public enum PoolOrderBy
    {
        created_at,
        name,
        updated_at,
        post_count
    }

    /// <summary>
    /// 排序模式
    /// </summary>
    public enum OrderMode
    {
        ASC,
        DESC
    }

    /// <summary>
    /// 偏好
    /// </summary>
    public enum Prefer
    {
        Good = 1,
        Normal = -1,
        Bad = 0
    }

    /// <summary>
    /// 模式
    /// </summary>
    public enum Mode
    {
        Posts,
        Pools
    }
}
