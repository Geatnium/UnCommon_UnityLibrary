using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnCommon
{
    /// <summary>
    /// コレクション関連の拡張関数クラス
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// コレクションに対して要素をユニークに追加する。
        /// </summary>
        /// <typeparam name="T">タイプ</typeparam>
        /// <param name="collection">コレクション</param>
        /// <param name="item">追加したい要素</param>
        /// <returns>追加に成功したらtrue</returns>
        public static bool AddUnique<T>(this ICollection<T> collection, in T item)
        {
            if (collection.Contains(item)) return false;
            collection.Add(item);
            return true;
        }

        /// <summary>
        /// コレクションがNullではなく、要素数も一つ以上あるか
        /// </summary>
        /// <typeparam name="T">タイプ</typeparam>
        /// <param name="collection">コレクション</param>
        /// <returns>コレクションがNullではなく、要素数も一つ以上ある場合true</returns>
        public static bool IsValid<T>(this ICollection<T> collection)
        {
            if (collection == null) return false;
            if (collection.Count == 0) return false;
            return true;
        }

        /// <summary>
        /// コレクションの指定インデックスは有効かどうか
        /// </summary>
        /// <typeparam name="T">タイプ</typeparam>
        /// <param name="collection">コレクション</param>
        /// <param name="index">インデックス</param>
        /// <returns>インデックスの要素がある場合はtrue</returns>
        public static bool IsValidIndex<T>(this ICollection<T> collection, in int index)
        {
            if (!collection.IsValid()) return false;
            if (index >= collection.Count) return false;
            T[] array = new T[collection.Count];
            collection.CopyTo(array, 0);
            return array[index] != null;
        }
    }
}
