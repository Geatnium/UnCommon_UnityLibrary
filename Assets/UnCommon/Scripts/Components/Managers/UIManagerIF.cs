using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UnCommon
{
    /// <summary>
    /// UIマネージャーのインターフェース
    /// </summary>
    public interface IUIManager : IManager
    {
        /// <summary>
        /// 指定ラベルのUIを生成する。
        /// </summary>
        /// <param name="label">ラベル</param>
        /// <param name="instance">生成されたUIの出力先</param>
        /// <returns>生成に成功したらtrueを返す。</returns>
        bool GetUIInstance(Name label, out Widget instance);

        /// <summary>
        /// 指定ラベルのUIを生成する。
        /// </summary>
        /// <param name="label">ラベル</param>
        /// <param name="instance">生成されたUIの出力先</param>
        /// <returns>生成に成功したらtrueを返す。</returns>
        bool GetUIInstance(Name label, out GameObject instance);

        /// <summary>
        /// 指定ラベルのUIがインスタンスされていたら破棄する
        /// </summary>
        /// <param name="label"></param>
        void DestroyUI(Name label);

        /// <summary>
        /// 指定ラベルのUIがデータベースにあるか調べる
        /// </summary>
        /// <param name="label">ラベル</param>
        /// <returns></returns>
        bool ExistsUIInDatabase(Name label);

        /// <summary>
        /// 指定ラベルのUIがインスタンスされているか調べる
        /// </summary>
        /// <param name="label">ラベル</param>
        /// <returns></returns>
        bool ExistsUIInstance(Name label);

        /// <summary>
        /// フェードイン
        /// </summary>
        /// <param name="color">フェードの色</param>
        /// <param name="duration">フェード時間（sec）</param>
        /// <param name="delay">フェードイン開始までの遅延時間（sec）</param>
        /// <param name="finishedCallback">フェードイン終了時のイベント</param>
        void FadeIn(Color color, float duration = 1.0f, float delay = 1.0f, Action finishedCallback = null);

        /// <summary>
        /// フェードアウト
        /// </summary>
        /// <param name="color">フェードの色</param>
        /// <param name="duration">フェード時間（sec）</param>
        /// <param name="delay">フェードアウト開始までの遅延時間（sec）</param>
        /// <param name="finishedCallback">フェードアウト終了時のイベント</param>
        void FadeOut(Color color, float duration = 1.0f, float delay = 0.0f, Action finishedCallback = null);

        /// <summary>
        /// フェードイン
        /// </summary>
        /// <param name="color">フェードの色</param>
        /// <param name="duration">フェード時間（sec）</param>
        /// <param name="delay">フェードイン開始までの遅延時間（sec）</param>
        UniTask FadeIn(Color color, float duration = 1.0f, float delay = 1.0f);

        /// <summary>
        /// フェードアウト
        /// </summary>
        /// <param name="color">フェードの色</param>
        /// <param name="duration">フェード時間（sec）</param>
        /// <param name="delay">フェードイン開始までの遅延時間（sec）</param>
        UniTask FadeOut(Color color, float duration = 1.0f, float delay = 0.0f);
    }
}