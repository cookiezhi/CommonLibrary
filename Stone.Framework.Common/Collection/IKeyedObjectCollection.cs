using System.Collections.Generic;

namespace Stone.Framework.Common.Collection
{
    public interface IKeyedObjectCollection<in TKey, TItem> : ICollection<TItem> where TItem : IKeyedObject<TKey>
    {
        /// <summary>
        /// 获取或设置指定索引处的元素
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        TItem this[int index] { get; }

        /// <summary>
        /// 获取或设置与指定键值的元素。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TItem this[TKey key] { get; }

        /// <summary>
        /// 通过键获取值。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TItem GetItemByKey(TKey key);

        /// <summary>
        /// 判断是否包含键
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Contains(TKey key);
    }
}