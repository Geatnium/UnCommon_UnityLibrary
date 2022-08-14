
namespace UnCommon
{
    /// <summary>
    /// 文字列の拡張関数クラス
    /// </summary>
    public static class StringExtensions
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
    }
}
