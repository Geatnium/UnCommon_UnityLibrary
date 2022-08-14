using UnityEngine;

namespace UnCommon
{
    /// <summary>
    /// オブジェクト参照関連の拡張関数クラス
    /// </summary>
    public static class ObjectExtensions
    {
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
    }
}
