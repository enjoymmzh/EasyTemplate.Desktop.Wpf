using System;

namespace EasyTemplate.Desktop.Wpf.Models;

// <summary>
/// 基类Id
/// </summary>
public abstract class EntityBaseId
{
    /// <summary>
    /// Id
    /// </summary>
    public virtual int Id { get; set; }
}

/// <summary>
/// 框架实体基类
/// </summary>
public abstract class EntityBase : EntityBaseId
{
    /// <summary>
    /// 创建时间
    /// </summary>
    public virtual DateTime? CreateTime { get; set; }
    /// <summary>
    /// 创建者Id
    /// </summary>
    public virtual long? CreateUserId { get; set; }
    /// <summary>
    /// 更新时间
    /// </summary>
    public virtual DateTime? UpdateTime { get; set; }
    /// <summary>
    /// 修改者Id
    /// </summary>
    public virtual long? UpdateUserId { get; set; }
    /// <summary>
    /// 软删除,1:删除，0:不删除
    /// </summary>
    public virtual bool IsDelete { get; set; } 
}

/// <summary>
/// 通用获取列表参数
/// </summary>
public abstract class QueryBase
{
    /// <summary>
    /// 分页页码
    /// </summary>
    public int pi { get; set; } = 1;
    /// <summary>
    /// 分页数量
    /// </summary>
    public int ps { get; set; } = 10;
}
