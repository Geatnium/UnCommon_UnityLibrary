using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace UnCommon
{
    /// <summary>
    /// 共通部分の拡張関数クラス
    /// </summary>
    public static class StandardExtensions
    {
        // -----------------------------------------------------------------------------
        // 文字列
        // -----------------------------------------------------------------------------
        #region 文字列

        /// <summary>
        /// Nullか一文字も無いか調べる
        /// </summary>
        /// <param name="s">文字列</param>
        /// <returns>Nullもしくは一文字もなければtrue</returns>
        public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);

        #endregion


        // -----------------------------------------------------------------------------
        // 文字列
        // -----------------------------------------------------------------------------
        #region 参照チェック系

        /// <summary>
        /// オブジェクト参照が有効かどうか
        /// </summary>
        /// <param name="obj">オブジェクト</param>
        /// <returns>有効であればtrue</returns>
        public static bool IsValid(this Object obj) => obj != null;

        /// <summary>
        /// 変数の中身が有効化どうか
        /// </summary>
        /// <param name="obj">オブジェクト</param>
        /// <returns>有効であればtrue</returns>
        public static bool IsValid(this object obj) => obj != null;

        #endregion


        // -----------------------------------------------------------------------------
        // 文字列
        // -----------------------------------------------------------------------------
        #region 配列・リスト操作系

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

        #endregion
    }
}
