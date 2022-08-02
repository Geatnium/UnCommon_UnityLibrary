using System.Diagnostics;

namespace UnCommon
{
    /// <summary>
    /// デバッグログ関連のユーティリティクラス
    /// </summary>
    public static class DebugLogger
    {
        /// <summary>
        /// ログを出力する（リリースビルドでは出力されない）
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        [Conditional("DEBUG")]
        public static void Log(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        /// <summary>
        /// エラーログを出力する（リリースビルドでは出力されない）
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        [Conditional("DEBUG")]
        public static void LogError(object message)
        {
            UnityEngine.Debug.LogError(message);
        }

        /// <summary>
        /// ワーニングログを出力する（リリースビルドでは出力されない）
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        [Conditional("DEBUG")]
        public static void LogWarning(object message)
        {
            UnityEngine.Debug.LogWarning(message);
        }
    }

}